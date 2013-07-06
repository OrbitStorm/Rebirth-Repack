using System;
using System.Collections; using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.SkillHandlers
{
	public class AnimalTaming
	{
		private static Hashtable m_BeingTamed = new Hashtable();

		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.AnimalTaming].Callback = new SkillUseCallback( OnUse );
		}

		private static bool m_DisableMessage;

		public static bool DisableMessage
		{
			get{ return m_DisableMessage; }
			set{ m_DisableMessage = value; }
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();

			m.Target = new InternalTarget();
			m.RevealingAction();

			if ( !m_DisableMessage )
				m.SendLocalizedMessage( 502789 ); // Tame which animal?

			return TimeSpan.FromHours( 1.0 );
		}

		public static bool CheckMastery( Mobile tamer, BaseCreature creature )
		{
			return creature.Owners.Contains( tamer );
		}

		public static bool MustBeSubdued( BaseCreature bc )
		{
			return bc.SubdueBeforeTame && (bc.Hits > (bc.HitsMax / 10));
		}

		public static void Scale( BaseCreature bc, double scalar, bool scaleStats )
		{
			if ( scaleStats )
			{
				if ( bc.RawStr > 0 )
					bc.RawStr = (int)Math.Max( 1, bc.RawStr * scalar );

				if ( bc.RawDex > 0 )
					bc.RawDex = (int)Math.Max( 1, bc.RawDex * scalar );

				if ( bc.HitsMaxSeed > 0 )
				{
					bc.HitsMaxSeed = (int)Math.Max( 1, bc.HitsMaxSeed * scalar );
					bc.Hits = bc.Hits;
				}

				if ( bc.StamMaxSeed > 0 )
				{
					bc.StamMaxSeed = (int)Math.Max( 1, bc.StamMaxSeed * scalar );
					bc.Stam = bc.Stam;
				}
			}

			for ( int i = 0; i < bc.Skills.Length; ++i )
				bc.Skills[i].Base *= scalar;
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 6, false, TargetFlags.None )
			{
			}

			protected override void OnTargetFinish( Mobile from )
			{
				from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 10.0 );
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				from.RevealingAction();

				if ( targeted is Mobile )
				{
					if ( targeted is BaseCreature )
					{
						BaseCreature creature = (BaseCreature)targeted;

						if ( !creature.Tamable || creature.MinTameSkill > 150 )
						{
							from.SendLocalizedMessage( 502469 ); // That being can not be tamed.
						}
						else if ( creature.Controled )
						{
							from.SendLocalizedMessage( 502467 ); // That animal looks tame already.
						}
						else if ( from.Female ? !creature.AllowFemaleTamer : !creature.AllowMaleTamer )
						{
							from.SendLocalizedMessage( 502801 ); // You can't tame that!
						}
						/*else if ( from.Followers + creature.ControlSlots > from.FollowersMax )
						{
							from.SendLocalizedMessage( 1049611 ); // You have too many followers to tame that creature.
						}*/
						/*else if ( creature.Owners.Count >= BaseCreature.MaxOwners && !creature.Owners.Contains( from ) )
						{
							from.SendLocalizedMessage( 1005615 ); // This animal has had too many owners and is too upset for you to tame.
						}*/
						else if ( MustBeSubdued( creature ) )
						{
							from.SendLocalizedMessage( 1054025 ); // You must subdue this creature before you can tame it!
						}
						else if ( CheckMastery( from, creature ) || from.Skills[SkillName.AnimalTaming].Value >= creature.MinTameSkill*1.2 - 25.0 )
						{
							if ( m_BeingTamed.Contains( targeted ) )
							{
								from.SendLocalizedMessage( 502802 ); // Someone else is already taming this.
							}
							else
							{
								m_BeingTamed[targeted] = from;

								// Face the creature
								from.Direction = from.GetDirectionTo( creature );

								from.LocalOverheadMessage( MessageType.Emote, 0x59, 1010597 ); // You start to tame the creature.
								from.NonlocalOverheadMessage( MessageType.Emote, 0x59, 1010598 ); // *begins taming a creature.*

								new InternalTimer( from, creature, Utility.Random( 3, 2 ) ).Start();
							}
						}
						else
						{
							from.SendLocalizedMessage( 502806 ); // You have no chance of taming this creature.
						}
					}
					else
					{
						from.SendLocalizedMessage( 502469 ); // That being can not be tamed.
					}
				}
				else
				{
					from.SendLocalizedMessage( 502801 ); // You can't tame that!
				}
			}

			private class InternalTimer : Timer
			{
				private Mobile m_Tamer;
				private BaseCreature m_Creature;
				private int m_MaxCount;
				private int m_Count;
				private bool m_Paralyzed;

				public InternalTimer( Mobile tamer, BaseCreature creature, int count ) : base( TimeSpan.FromSeconds( 3.0 ), TimeSpan.FromSeconds( 3.0 ), count )
				{
					m_Tamer = tamer;
					m_Creature = creature;
					m_MaxCount = count;
					m_Paralyzed = creature.Paralyzed;
					Priority = TimerPriority.TwoFiftyMS;
				}

				protected override void OnTick()
				{
					m_Count++;

					// m_Tamer.NextSkillTime = DateTime.Now;
					if ( !m_Tamer.InRange( m_Creature, 6 ) )
					{
						m_BeingTamed.Remove( m_Creature );
						m_Tamer.SendLocalizedMessage( 502795 ); // You are too far away to continue taming.
						Stop();
					}
					else if ( !m_Tamer.CheckAlive() )
					{
						m_BeingTamed.Remove( m_Creature );
						m_Tamer.SendLocalizedMessage( 502796 ); // You are dead, and cannot continue taming.
						Stop();
					}
					else if ( !m_Tamer.CanSee( m_Creature ) || !m_Tamer.InLOS( m_Creature ) )
					{
						m_BeingTamed.Remove( m_Creature );
						m_Tamer.SendLocalizedMessage( 502800 ); // You can't see that.
						Stop();
					}
					else if ( !m_Creature.Tamable )
					{
						m_BeingTamed.Remove( m_Creature );
						m_Tamer.SendLocalizedMessage( 502469 ); // That being can not be tamed.
						Stop();
					}
					else if ( m_Creature.Controled )
					{
						m_BeingTamed.Remove( m_Creature );
						m_Tamer.SendLocalizedMessage( 502804 ); // That animal looks tame already.
						Stop();
					}
					else if ( m_Creature.Owners.Count >= BaseCreature.MaxOwners && !m_Creature.Owners.Contains( m_Tamer ) )
					{
						m_BeingTamed.Remove( m_Creature );
						m_Tamer.SendLocalizedMessage( 1005615 ); // This animal has had too many owners and is too upset for you to tame.
						Stop();
					}
					else if ( m_Creature.SubdueBeforeTame && (m_Creature.Hits >= (m_Creature.HitsMax / 5)) )
					{
						m_BeingTamed.Remove( m_Creature );
						m_Tamer.SendLocalizedMessage( 1054025 ); // You must subdue this creature before you can tame it!
						Stop();
					}
					else if ( m_Count < m_MaxCount )
					{
						m_Tamer.RevealingAction();
						m_Tamer.PublicOverheadMessage( MessageType.Regular, 0x3B2, Utility.Random( 502790, 4 ) );
						m_Tamer.Direction = m_Tamer.GetDirectionTo( m_Creature );

						if ( m_Creature.Paralyzed )
							m_Paralyzed = true;
					}
					else
					{
						m_Tamer.RevealingAction();
						m_BeingTamed.Remove( m_Creature );

						if ( m_Creature.Paralyzed )
							m_Paralyzed = true;

						bool alreadyOwned = m_Creature.Owners.Contains( m_Tamer );

						double skill = m_Creature.MinTameSkill*1.2;

						if ( skill > 0 && CheckMastery( m_Tamer, m_Creature ) )
							skill = 0; // 50% at 0.0?

						bool success;
						if ( alreadyOwned )
							success = m_Tamer.CheckTargetSkill( SkillName.AnimalTaming, m_Creature, -25.0, 25.0 );
						else
							success = m_Tamer.CheckTargetSkill( SkillName.AnimalTaming, m_Creature, skill - 25.0, skill + 25.0 );

						if ( success )
						{
							if ( m_Creature.Owners.Count == 0 && m_Creature.SubdueBeforeTame )
								Scale( m_Creature, 0.50, true ); // Creatures which must be subdued take an additional 50% loss of skills and stats

							if ( alreadyOwned )
							{
								m_Tamer.SendLocalizedMessage( 502797 ); // That wasn't even challenging.
							}
							else
							{
								m_Creature.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 502799, m_Tamer.NetState ); // It seems to accept you as master.
								m_Creature.Owners.Add( m_Tamer );
							}

							m_Creature.SetControlMaster( m_Tamer );
							m_Creature.IsBonded = false;

							int val = 2 + (int)(( ( m_Tamer.Skills[ SkillName.AnimalTaming ].Value - skill ) / 50.0 + 0.5 ) * 9.0);
							if ( val >= (int)PetLoyalty.WonderfullyHappy )
								m_Creature.Loyalty = PetLoyalty.WonderfullyHappy;
							else if ( val <= (int)PetLoyalty.ExtremelyUnhappy  )
								m_Creature.Loyalty = PetLoyalty.ExtremelyUnhappy;
							else
								m_Creature.Loyalty = (PetLoyalty)val;
							
							if ( alreadyOwned && m_Creature.Loyalty > PetLoyalty.SomewhatContent )
								m_Creature.Loyalty = PetLoyalty.SomewhatContent;

							m_Creature.OnTamed( m_Tamer );
						}
						else
						{
							m_Tamer.SendLocalizedMessage( 502798 ); // You fail to tame the creature.
						}
					}
				}
			}
		}
	}
}
