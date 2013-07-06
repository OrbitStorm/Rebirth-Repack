using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.SkillHandlers
{
	public class Provocation
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Provocation].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();

			BaseInstrument.PickInstrument( m, new InstrumentPickedCallback( OnPickedInstrument ) );

			return TimeSpan.FromSeconds( 10.0 ); // Cannot use another skill for 10 seconds
		}

		public static void OnPickedInstrument( Mobile from, BaseInstrument instrument )
		{
			from.RevealingAction();
			from.SendLocalizedMessage( 501587 ); // Whom do you wish to incite?
			from.Target = new InternalFirstTarget( from, instrument );
		}

		private class InternalFirstTarget : Target
		{
			private BaseInstrument m_Instrument;

			public InternalFirstTarget( Mobile from, BaseInstrument instrument ) : base( BaseInstrument.GetBardRange( from, SkillName.Provocation ), false, TargetFlags.None )
			{
				m_Instrument = instrument;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				from.RevealingAction();

				if ( targeted is BaseCreature && from.CanBeHarmful( (Mobile)targeted, true ) )
				{
					BaseCreature creature = (BaseCreature)targeted;

					if ( creature.Controled )
					{
						from.SendLocalizedMessage( 501590 ); // They are too loyal to their master to be provoked.
					}
					else
					{
						from.RevealingAction();
						from.Target = new InternalSecondTarget( from, m_Instrument, creature );
					}
				}
			}
		}

		private class InternalSecondTarget : Target
		{
			private BaseCreature m_Creature;
			private BaseInstrument m_Instrument;

			public InternalSecondTarget( Mobile from, BaseInstrument instrument, BaseCreature creature ) : base( BaseInstrument.GetBardRange( from, SkillName.Provocation ), false, TargetFlags.None )
			{
				m_Instrument = instrument;
				m_Creature = creature;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				from.RevealingAction();

				if ( targeted is Mobile )
				{
					Mobile targ = (Mobile)targeted;
					BaseCreature creature = targ as BaseCreature;

					if ( m_Creature.Unprovokable || ( creature != null && creature.Unprovokable ) )
					{
						from.SendLocalizedMessage( 1049446 ); // You have no chance of provoking those creatures.
					}
					else if ( m_Creature.Map != targ.Map || !m_Creature.InRange( targ, BaseInstrument.GetBardRange( from, SkillName.Provocation ) ) )
					{
						from.SendLocalizedMessage( 1049450 ); // The creatures you are trying to provoke are too far away from each other for your music to have an effect.
					}
					else if ( m_Creature != targ )
					{
						if ( from == targ )
						{
							from.SendAsciiMessage( "Maybe you should just attack it." );
							return;
						}

						if ( from.CanBeHarmful( m_Creature, true ) && from.CanBeHarmful( targ, true ) )
						{
							// provoking a guard, or provoking to attack a positive creature is bad
							if ( m_Creature is BaseGuard || m_Creature is BaseShieldGuard || targ is BaseGuard || targ is BaseShieldGuard )
							{
								from.SendLocalizedMessage( 1049446 ); // You have no chance of provoking those creatures.
								Server.Misc.Titles.AlterNotoriety( from, -40 );
								return;
							}
							else if ( Notoriety.Compute( from, targ ) == Notoriety.Innocent ) 
							{
								Misc.Titles.AlterNotoriety( from, -10 );
							}

							if ( !BaseInstrument.CheckMusicianship( from ) )
							{
								from.SendLocalizedMessage( 500612 ); // You play poorly, and there is no effect.
								m_Instrument.PlayInstrumentBadly( from );
								m_Instrument.ConsumeUse( from );
							}
							else
							{
								from.DoHarmful( m_Creature );
								from.DoHarmful( targ );

								if ( !from.CheckTargetSkill( SkillName.Provocation, targ, 0, 110 ) )//diff-25.0, diff+25.0 ) )
								{
									from.SendLocalizedMessage( 501599 ); // Your music fails to incite enough anger.
									m_Instrument.PlayInstrumentBadly( from );
									m_Instrument.ConsumeUse( from );
								}
								else
								{
									from.SendLocalizedMessage( 501602 ); // Your music succeeds, as you start a fight.
									m_Instrument.PlayInstrumentWell( from );
									m_Instrument.ConsumeUse( from );
									m_Creature.Provoke( from, targ, true );
								}
							}
						}
					}
					else
					{
						from.SendLocalizedMessage( 501593 ); // You can't tell someone to attack themselves!
					}
				}
			}
		}
	}
}
