using System;
using Server.Network;
using Server.Gumps;

namespace Server.Menus.Questions
{
	public class StuckMenu : Gump
	{
		private static Point3D[] m_Locations = new Point3D[]
			{
				new Point3D( 1522, 1757, 28 ), // Britain
				new Point3D( 2005, 2754, 30 ), // Trinsic
				new Point3D( 2973,  891,  0 ), // Vesper
				new Point3D( 2498,  392,  0 ), // Minoc
				new Point3D(  490, 1166,  0 ), // Yew
				//new Point3D( 2230, 1159,  0 ), // Cove
				//new Point3D( 5720, 3109, -1 ), // Papua
				//new Point3D( 5216, 4033, 37 )  // Delucia
			};

		public static bool IsInSecondAgeArea( Mobile m )
		{
			// Must be redone with a specific external support
			// in order to consider dungeons too

			return ( m.X >= 5120 && m.Y >= 2304 );
		}

		private Mobile m_Mobile, m_Sender;
		private bool m_MarkUse;

		private Timer m_Timer;

		public StuckMenu( Mobile beholder, Mobile beheld, bool markUse ) : base( 150, 50 )
		{
			m_Sender = beholder;
			m_Mobile = beheld;
			m_MarkUse = markUse;
			Closable = false; 
			Dragable = false; 

			AddPage( 0 );

			AddBackground( 0, 0, 270, 320, 2600 );

			AddHtmlLocalized( 50, 25, 170, 40, 1011027, false, false ); //Chose a town:

			if ( beholder == null || beholder.AccessLevel == AccessLevel.Player )
			{
				AddButton( 50, 60, 208, 209, 0xFF, GumpButtonType.Reply, 0 );
				AddHtml( 75, 60, 335, 40, "Random Mainland City", false, false );
			}
			else
			{
				AddButton( 50, 60, 208, 209, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 60, 335, 40, 1011028, false, false ); // Britain

				AddButton( 50, 95, 208, 209, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 95, 335, 40, 1011029, false, false ); // Trinsic

				AddButton( 50, 130, 208, 209, 3, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 130, 335, 40, 1011030, false, false ); // Vesper

				AddButton( 50, 165, 208, 209, 4, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 165, 335, 40, 1011031, false, false ); // Minoc

				AddButton( 50, 200, 208, 209, 5, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 200, 335, 40, 1011032, false, false ); // Yew

				AddButton( 50, 235, 208, 209, 6, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 235, 335, 40, 1011033, false, false ); // Cove
			}

			AddButton( 55, 268, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 90, 270, 320, 40, 1011012, false, false ); // CANCEL
		}

		public void BeginClose()
		{
			StopClose();

			m_Timer = new CloseTimer( m_Mobile );
			m_Timer.Start();

			m_Mobile.Frozen = true;
		}

		public void StopClose()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Mobile.Frozen = false;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			StopClose();

			try
			{
				if ( info.ButtonID == 0 )
				{
					if ( m_Mobile == m_Sender )
						m_Mobile.SendLocalizedMessage( 1010588 ); // You choose not to go to any city.
				}
				else if ( info.ButtonID == 0xFF )
				{
					Teleport( Utility.Random( m_Locations.Length ) );
				}
				else 
				{
					Teleport( info.ButtonID - 1 );
				}
			}
			catch
			{
				m_Mobile.SendMessage( "Error.  Try again." );
			}
		}

		private void Teleport( int index )
		{
			if ( m_MarkUse ) 
			{
				m_Mobile.SendLocalizedMessage( 1010589 ); // You will be teleported within the next two minutes.

				new TeleportTimer( m_Mobile, m_Locations[index], TimeSpan.FromSeconds( 10.0 + (Utility.RandomDouble() * 110.0) ) ).Start();

				m_Mobile.UsedStuckMenu();
			}
			else
			{
				new TeleportTimer( m_Mobile, m_Locations[index], TimeSpan.Zero ).Start();
			}
		}

		private class CloseTimer : Timer
		{
			private Mobile m_Mobile;
			private DateTime m_End;

			public CloseTimer( Mobile m ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_End = DateTime.Now + TimeSpan.FromMinutes( 3.0 );
			}

			protected override void OnTick()
			{
				if ( m_Mobile.NetState == null || DateTime.Now > m_End )
				{
					m_Mobile.Frozen = false;
					m_Mobile.CloseGump( typeof( StuckMenu ) );

					Stop();
				}
				else
				{
					m_Mobile.Frozen = true;
				}
			} 
		} 

		private class TeleportTimer : Timer
		{
			private Mobile m_Mobile;
			private Point3D m_Location;
			private DateTime m_End;
			private DateTime m_NextMessage;

			public TeleportTimer( Mobile m, Point3D loc, TimeSpan delay ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
			{
				Priority = TimerPriority.TwoFiftyMS;

				m_Mobile = m;
				m_Location = loc;
				m_End = DateTime.Now + delay;
				m_NextMessage = DateTime.Now + TimeSpan.FromSeconds( 5.0 );
			}

			protected override void OnTick()
			{
				m_Mobile.RevealingAction();
				if ( DateTime.Now > m_End && !( m_Mobile.Alive && Server.SkillHandlers.Hiding.CheckCombat( m_Mobile, 12 ) ) )
				{
					m_Mobile.Frozen = false;
					m_Mobile.Location = m_Location;

					Stop();
				}
				else
				{
					m_Mobile.Frozen = true;
				}
			}
		}
	}
}