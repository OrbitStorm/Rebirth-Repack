using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.SkillHandlers
{
	public class Discordance
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Discordance].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();

			BaseInstrument.PickInstrument( m, new InstrumentPickedCallback( OnPickedInstrument ) );

			return TimeSpan.FromSeconds( 10.0 ); // Cannot use another skill for 1 second
		}

		public static void OnPickedInstrument( Mobile from, BaseInstrument instrument )
		{
			from.RevealingAction();
			from.SendAsciiMessage( 0x3B2, "Whom do you wish to entice?" );//from.SendLocalizedMessage( 1049541 ); // Choose the target for your song of discordance.
			from.Target = new DiscordanceTarget( from, instrument );
		}

		private class DiscordanceInfo
		{
			public Mobile m_From;
			public Mobile m_Creature;
			public DateTime m_EndTime;
			public double m_Scalar;
			public ArrayList m_Mods;

			public DiscordanceInfo( Mobile from, Mobile creature, TimeSpan duration, double scalar, ArrayList mods )
			{
				m_From = from;
				m_Creature = creature;
				m_EndTime = DateTime.Now + duration;
				m_Scalar = scalar;
				m_Mods = mods;

				Apply();
			}

			public void Apply()
			{
				for ( int i = 0; i < m_Mods.Count; ++i )
				{
					object mod = m_Mods[i];

					if ( mod is ResistanceMod )
						m_Creature.AddResistanceMod( (ResistanceMod) mod );
					else if ( mod is StatMod )
						m_Creature.AddStatMod( (StatMod) mod );
					else if ( mod is SkillMod )
						m_Creature.AddSkillMod( (SkillMod) mod );
				}
			}

			public void Clear()
			{
				for ( int i = 0; i < m_Mods.Count; ++i )
				{
					object mod = m_Mods[i];

					if ( mod is ResistanceMod )
						m_Creature.RemoveResistanceMod( (ResistanceMod) mod );
					else if ( mod is StatMod )
						m_Creature.RemoveStatMod( ((StatMod) mod).Name );
					else if ( mod is SkillMod )
						m_Creature.RemoveSkillMod( (SkillMod) mod );
				}
			}
		}

		private static Hashtable m_Table = new Hashtable();

		public static bool GetScalar( Mobile targ, ref double scalar )
		{
			DiscordanceInfo info = m_Table[targ] as DiscordanceInfo;

			if ( info == null )
				return false;

			scalar = info.m_Scalar;
			return true;
		}

		private static void ProcessDiscordance( object state )
		{
			DiscordanceInfo info = (DiscordanceInfo)state;
			Mobile from = info.m_From;
			Mobile targ = info.m_Creature;

			if ( DateTime.Now >= info.m_EndTime || targ.Deleted || from.Map != targ.Map || targ.GetDistanceToSqrt( from ) > 16 )
			{
				info.Clear();
				m_Table.Remove( targ );
			}
			else
			{
				targ.FixedEffect( 0x376A, 1, 32 );
			}
		}

		public class DiscordanceTarget : Target
		{
			private BaseInstrument m_Instrument;

			public DiscordanceTarget( Mobile from, BaseInstrument inst ) : base( BaseInstrument.GetBardRange( from, SkillName.Discordance ), false, TargetFlags.Harmful )
			{
				m_Instrument = inst;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				from.RevealingAction();

				if ( target is Mobile )
				{
					Mobile targ = (Mobile)target;

					if ( targ is BaseCreature && ((BaseCreature)targ).BardImmune )
					{
						from.SendAsciiMessage( "You cannot entice that!" );
					}
					else if ( targ == from )
					{
						from.SendAsciiMessage( "You cannot entice yourself!" );
					}
					else
					{
						if ( !BaseInstrument.CheckMusicianship( from ) )
						{
							from.SayTo( from, 500612 ); // You play poorly, and there is no effect.
							m_Instrument.PlayInstrumentBadly( from );
							m_Instrument.ConsumeUse( from );
						}
						else if ( from.CheckTargetSkill( SkillName.Discordance, target, 0, 100 ) )
						{
							m_Instrument.PlayInstrumentWell( from );
							m_Instrument.ConsumeUse( from );
							
							if ( targ.Player )
							{
								targ.SayTo( targ, "You hear lovely music, and are drawn towards it..." );
								targ.SayTo( from, "You might have better luck with sweet words." );
								return;
							}

							if ( targ.Body.IsHuman )
								targ.Say( "What am I hearing?" );

							if ( targ is BaseGuard || targ is BaseVendor || targ is WanderingHealer || targ is Banker || targ is TownCrier || targ is BaseShieldGuard )
							{
								targ.Say( "Oh, but I cannot wander too far from my work!" );
								targ.SayTo( from, true, "They look too dedicated to their job to be lured away." );
							}
							else 
							{
								from.SayTo( from, true, "You play your hypnotic music, luring them near." );
								if ( targ is BaseCreature )
									((BaseCreature)targ).TargetLocation = new Point2D( from.Location );
							}
						}
						else
						{
							targ.SayTo( targ, true, "You hear lovely music, and for a moment are drawn towards it..." );
							targ.SayTo( from, true, "Your music fails to attract them." );
							m_Instrument.PlayInstrumentBadly( from );
							m_Instrument.ConsumeUse( from );
						}
					}
				}
				else
				{
					m_Instrument.PlayInstrumentBadly( from );
				}
			}
		}
	}
}
