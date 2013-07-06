using System;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
	public class RemoveTrap
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.RemoveTrap].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.SendAsciiMessage( "That skill has been disabled for historical accuracy." );
			return TimeSpan.Zero;

			/*m.Target = new InternalTarget();

			m.SendLocalizedMessage( 502368 ); // Wich trap will you attempt to disarm?

			return TimeSpan.FromSeconds( 10.0 ); // 10 second delay before beign able to re-use a skill
			*/
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile )
				{
					from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate
				}
				else if ( targeted is TrapableContainer )
				{
					TrapableContainer targ = (TrapableContainer)targeted;

					from.Direction = from.GetDirectionTo( targ );

					if ( !targ.Trapped )
					{
						from.SendLocalizedMessage( 502373 ); // That doesn't appear to be trapped
						return;
					}

					from.PlaySound( 0x241 );
					
					if ( from.CheckTargetSkill( SkillName.RemoveTrap, targ, targ.TrapPower, targ.TrapPower + 30 ) )
					{
						targ.Trapped = false;
						from.SendLocalizedMessage( 502377 ); // You successfully render the trap harmless
					}
					else
					{
						from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
					}
				}
				else
				{
					from.SendLocalizedMessage( 502373 ); // That does'nt appear to be trapped
				}
			}
		}
	}
}