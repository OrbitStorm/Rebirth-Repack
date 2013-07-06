using System;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
	public class TasteID
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.TasteID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 502807 ); // What would you like to taste?

			return TimeSpan.FromSeconds( 10.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Food )
				{
					if ( from.CheckTargetSkill( SkillName.TasteID, targeted, 0, 100 ) )
					{
						if ( ((Food)targeted).Poison != null )
							from.SendLocalizedMessage( 1038284 ); // It appears to have poison smeared on it
						else
							from.SendAsciiMessage( "You notice nothing unusual." );
					}
					else
					{
						from.SendLocalizedMessage( 502823 ); // You cannot discern anything about this substance
					}
				}
				else
				{
					// TODO: Potion support ?
					from.SendLocalizedMessage( 502820 ); // That's not something you can taste
				}
			}
		}
	}
}