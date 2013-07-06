using System;
using Server;
using Server.Items;
using Server.Guilds;

namespace Server.Misc
{
	public class Keywords
	{
		public static void Initialize()
		{
			// Register our speech handler
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

		public static void EventSink_Speech( SpeechEventArgs args )
		{
			Mobile from = args.Mobile;
			int[] keywords = args.Keywords;

			for ( int i = 0; i < keywords.Length; ++i )
			{
				switch ( keywords[i] )
				{
					case 0x002A: // *i resign from my guild*
					{
						if ( from.Guild is Guild && from.Guild != null )
							((Guild)from.Guild).RemoveMember( from );

						break;
					}
					/*case 0x0032: // "*i must consider my sins*
					{
						if ( from.Kills == 0 )
							from.SendLocalizedMessage( 502122 );
						else if ( from.Kills <= 4 )
							from.SendLocalizedMessage( 502125 );
						else if ( from.Kills >= 5 )
							from.SendLocalizedMessage( 502123 );
						break;
					}*/
				}
			}
			
			string lower = args.Speech.ToLower();
			if ( lower.IndexOf( "view updates" ) != -1 )
			{
				Server.Gumps.UpdatesGump.ShowUpdates( from );
			}
		}
	}
}
