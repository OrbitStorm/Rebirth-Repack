using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Accounting;

namespace Server.Engines.Help
{
	public class ContainedMenu : QuestionMenu
	{
		private Mobile m_From;

		public ContainedMenu( Mobile from ) : base( "You already have an open help request. We will have someone assist you as soon as possible.  What would you like to do?", new string[]{ "Leave my old help request like it is.", "Remove my help request from the queue." } )
		{
			m_From = from;
		}

		public override void OnCancel( NetState state )
		{
			m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
		}

		public override void OnResponse( NetState state, int index )
		{
			if ( index == 0 )
			{
				m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
			}
			else if ( index == 1 )
			{
				PageEntry entry = PageQueue.GetEntry( m_From );

				if ( entry != null && entry.Handler == null )
				{
					m_From.SendLocalizedMessage( 1005307, "", 0x35 ); // Removed help request.
					PageQueue.Remove( entry );
				}
				else
				{
					m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
				}
			}
		}
	}

	public class HelpMenu : QuestionMenu
	{
		public static void Initialize()
		{
			EventSink.HelpRequest += new HelpRequestEventHandler( EventSink_HelpRequest );
		}

		private static void EventSink_HelpRequest( HelpRequestEventArgs e )
		{
			if ( !PageQueue.CheckAllowedToPage( e.Mobile ) )
				return;
			if ( StuckMenu.IsInSecondAgeArea( e.Mobile ) )
				return;

			if ( PageQueue.Contains( e.Mobile ) )
				e.Mobile.SendMenu( new ContainedMenu( e.Mobile ) );
			else
				e.Mobile.SendMenu( new HelpMenu() );//e.Mobile.SendGump( new HelpGump( e.Mobile ) );
		}

		private static bool IsYoung( Mobile m )
		{
			Server.Accounting.Account acct = m.Account as Account;

			return ( acct != null && (DateTime.Now - acct.Created) < TimeSpan.FromHours( 24.0 ) );
		}

		public static bool CheckCombat( Mobile m )
		{
			for ( int i = 0; i < m.Aggressed.Count; ++i )
			{
				AggressorInfo info = (AggressorInfo)m.Aggressed[i];

				if ( DateTime.Now - info.LastCombatTime < TimeSpan.FromSeconds( 30.0 ) )
					return true;
			}

			return false;
		}

		private static string[] m_RootMenu = new string[]
		{
			"VISIT UORebirth.com: Select this to open your browser to our web page where you can find FAQs, Playguides, account management, news, and other information.",
			"CHARACTER IS PHYSICALLY STUCK: Choose this if you are totally unable to move.  Do not use this if you are lost.",
			"ANOTHER PLAYER IS HARRASSING ME: Choose this if another player is verbally harrassing you.  Before choosing this option attempt to leave the area, and ask the player to stop.",
			"BUG REPORT: Select this option if you believe you have found a game bug, or if you have witnessed another player exploiting a game bug. Before choosing this, be sure to check the playguide to see if you are simply mistaken.",
			"OTHER: Choose this if your problem does not fall into another category but you are ABSOLUTELY SURE that you need a Game Master to assist you.",
		};

		public HelpMenu() : base( "Please remember Game Masters are only available to help you as a LAST RESORT.  If you have tried all other avenues and failed, choose from the following options:", m_RootMenu  )
		{
		}

		public override void OnCancel(NetState state)
		{
			//state.Mobile.SendLocalizedMessage( 501235, "", 0x35 ); // Help request aborted.
		}

		public override void OnResponse( NetState state, int index )
		{
			Mobile from = state.Mobile;

			PageType type = (PageType)(-1);
			switch ( index )
			{
				case 0:
					from.LaunchBrowser( "http://www.uorebirth.com/" );
					break;
				case 1:
				{
					if ( from.Region is Server.Regions.Jail )
					{
						from.SendLocalizedMessage( 1041530, "", 0x35 ); // You'll need a better jailbreak plan then that!
					}
					else if ( from.CanUseStuckMenu() && from.Region.CanUseStuckMenu( from ) && !CheckCombat( from ) && !from.Frozen && !from.Criminal )
					{
						StuckMenu menu = new StuckMenu( from, from, true );
						menu.BeginClose();
						from.SendGump( menu );
					}
					else
					{
						type = PageType.Stuck;
					}
					break;
				}
				case 2:
					type = PageType.Harassment;
					break;
				case 3:
					type = PageType.Bug;
					break;
				case 4:
					type = PageType.Other;
					break;
			}
			
			if ( type != (PageType)(-1) && PageQueue.CheckAllowedToPage( from ) )
			{
				from.Prompt = new PagePrompt( type );
				from.SendAsciiMessage( "Enter a description of your problem, or press (Esc) to cancel:" );
			}
			//from.SendGump( new PagePromptGump( from, type ) );
		}
	}
}
