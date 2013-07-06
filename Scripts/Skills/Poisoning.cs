using System;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
	public class Poisoning
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Poisoning].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new InternalTargetPoison();

			m.SendLocalizedMessage( 502137 ); // Select the poison you wish to use

			return TimeSpan.FromSeconds( 10.0 ); // 10 second delay before beign able to re-use a skill
		}

		private class InternalTargetPoison : Target
		{
			public InternalTargetPoison() :  base ( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is BasePoisonPotion )
				{
					from.SendLocalizedMessage( 502142 ); // To what do you wish to apply the poison?
					from.Target = new InternalTarget( (BasePoisonPotion)targeted );
				}
				else // Not a Poison Potion
				{
					from.SendLocalizedMessage( 502139 ); // That is not a poison potion.
				}
			}

			private class InternalTarget : Target
			{
				private BasePoisonPotion m_Potion;

				public InternalTarget( BasePoisonPotion potion ) :  base ( 2, false, TargetFlags.None )
				{
					m_Potion = potion;
				}

				protected override void OnTarget( Mobile from, object targeted )
				{
					if ( m_Potion.Deleted )
						return;
					if ( targeted is Food || ( targeted is BaseWeapon && ( ((BaseWeapon)targeted).Type == WeaponType.Slashing || ((BaseWeapon)targeted).Type == WeaponType.Piercing ) ) )
					{
						new InternalTimer( from, (Item)targeted, m_Potion ).Start();

						from.PlaySound( 0x4F );
						m_Potion.Delete();

						from.AddToBackpack( new Bottle() );
					}
					else
					{
						from.SendAsciiMessage( "You cannot poison that! You can only poison bladed weapons, piercing weapons, or food." );
					}
				}

				private class InternalTimer : Timer
				{
					private Mobile m_From;
					private Item m_Target;
					private int m_Poison;
					private double m_MinSkill, m_MaxSkill;

					public InternalTimer( Mobile from, Item target, BasePoisonPotion potion ) : base( TimeSpan.FromSeconds( 2.0 ) )
					{
						m_From = from;
						m_Target = target;
						m_Poison = potion.Poison.Level;
						m_MinSkill = potion.MinPoisoningSkill;
						m_MaxSkill = potion.MaxPoisoningSkill;
						Priority = TimerPriority.TwoFiftyMS;
					}

					protected override void OnTick()
					{
						if ( !( m_Target is BaseItem ) )
						{
							m_From.SendAsciiMessage( "Internal poisoning error." );
							return;
						}

						if ( m_From.CheckTargetSkill( SkillName.Poisoning, m_Target, 0, 100 ) )
						{
							if ( m_From.Skills.Poisoning.Value < Utility.RandomDouble()*100.0 )
							{
								m_From.SendAsciiMessage( "You apply a dose of poison to {0}.", ((BaseItem)m_Target).BuildSingleClick() );
								if ( m_Poison > 0 )
									m_Poison--;
							}
							else
							{
								m_From.SendAsciiMessage( "You apply a strong dose of poison to {0}.", ((BaseItem)m_Target).BuildSingleClick() );
								if ( m_Target is Food && m_Poison < 4 )
									m_Poison++;
							}

							if ( m_Target is Food )
							{
								((Food)m_Target).Poison = Poison.GetPoison( m_Poison );
							}
							else if ( m_Target is BaseWeapon )
							{
								((BaseWeapon)m_Target).Poison = Poison.GetPoison( m_Poison );
								((BaseWeapon)m_Target).PoisonCharges = 20 - ((m_Poison+1) * 2);
								((BaseWeapon)m_Target).PoisonChance = ( m_From.Skills.Poisoning.Value / 4.0 ) / 100.0;
							}

							Misc.Titles.AlterNotoriety( m_From, -1, NotoCap.Dishonorable );
						}
						else // Failed
						{
							m_From.SendAsciiMessage( "You fail apply a dose of poison to {0}.", ((BaseItem)m_Target).BuildSingleClick() );
						}
					}
				}
			}
		}
	}
}