using System;

namespace Server.SkillHandlers
{
	public class Stealth
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Stealth].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.SendAsciiMessage( "That skill has been disabled for historical accuracy." );
			return TimeSpan.Zero;

			/*if ( !m.Hidden )
			{
				m.SendLocalizedMessage( 502725 ); // You must hide first
			}
			else if ( m.Skills[SkillName.Hiding].Base < 80.0 )
			{
				m.SendLocalizedMessage( 502726 ); // You are not hidden well enough.  Become better at hiding.
			}
			else if ( m.CheckSkill( SkillName.Stealth, 0.0, 120.0 ) )
			{
				int steps = (int)(m.Skills[SkillName.Stealth].Value / (Core.AOS ? 5.0 : 10.0));

				if ( steps < 1 )
					steps = 1;

				m.AllowedStealthSteps = steps;

				m.SendLocalizedMessage( 502730 ); // You begin to move quietly.

				return TimeSpan.FromSeconds( 10.0 );
			}
			else
			{
				m.SendLocalizedMessage( 502731 ); // You fail in your attempt to move unnoticed.
				m.RevealingAction();
			}

			return TimeSpan.FromSeconds( 10.0 );*/
		}
	}
}