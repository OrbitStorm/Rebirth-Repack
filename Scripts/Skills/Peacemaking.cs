using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.SkillHandlers
{
	public class Peacemaking
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Peacemaking].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();
			BaseInstrument.PickInstrument( m, new InstrumentPickedCallback( OnPickedInstrument ) );
			return TimeSpan.FromSeconds( 10.0 ); 
		}

		public static void OnPickedInstrument( Mobile from, BaseInstrument instrument )
		{
			from.RevealingAction();
			if ( !BaseInstrument.CheckMusicianship( from ) )
			{
				from.SendLocalizedMessage( 500612 ); // You play poorly, and there is no effect.
				instrument.PlayInstrumentBadly( from );
				instrument.ConsumeUse( from );
			}
			else if ( !from.CheckSkill( SkillName.Peacemaking, 0.0, 100.0 ) )
			{
				from.SendLocalizedMessage( 500613 ); // You attempt to calm everyone, but fail.
				instrument.PlayInstrumentBadly( from );
				instrument.ConsumeUse( from );
			}
			else
			{
				instrument.PlayInstrumentWell( from );
				instrument.ConsumeUse( from );

				Map map = from.Map;

				if ( map != null )
				{
					int range = BaseInstrument.GetBardRange( from, SkillName.Peacemaking );

					bool calmed = false;

					foreach ( Mobile m in from.GetMobilesInRange( range ) )
					{
						BaseCreature bc = m as BaseCreature;
						if ( ( bc != null && bc.Uncalmable ) || m == from )
							continue;

						calmed = true;

						m.SendLocalizedMessage( 500616 ); // You hear lovely music, and forget to continue battling!
						m.Combatant = null;
						if ( !m.Player )
							m.Warmode = false;

						if ( bc != null && !bc.BardPacified )
							bc.Pacify( from, DateTime.Now + TimeSpan.FromSeconds( 2.5 ) );
					}

					if ( !calmed )
						from.SendLocalizedMessage( 1049648 ); // You play hypnotic music, but there is nothing in range for you to calm.
					else
						from.SendLocalizedMessage( 500615 ); // You play your hypnotic music, stopping the battle.
				}
			}
		}
	}
}
