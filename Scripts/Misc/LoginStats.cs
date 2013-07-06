using System;
using Server.Network;
using Server.Accounting;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Misc
{
	public class LoginStats
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Login( LoginEventArgs args )
		{
            int userCount = NetState.Instances.Count;
			int itemCount = World.Items.Count;
			int mobileCount = World.Mobiles.Count;
			int spellCount = Spells.SpellRegistry.Count;

			Mobile m = args.Mobile;

			if ( m.AccessLevel > AccessLevel.Player )
			{
					m.SendAsciiMessage( "Welcome, {0}! There {1} currently {2} user{3} online, with {4} item{5} and {6} mobile{7} in the world.",
					args.Mobile.Name,
					userCount == 1 ? "is" : "are",
					userCount, userCount == 1 ? "" : "s",
					itemCount, itemCount == 1 ? "" : "s",
					mobileCount, mobileCount == 1 ? "" : "s" );
			}
			else
			{
				m.SendAsciiMessage( "There {0} currently {1} user{2} online.", 
					userCount == 1 ? "is" : "are",
					userCount, 
					userCount != 1 ? "s" : "" );
			}
		}
	}
}
