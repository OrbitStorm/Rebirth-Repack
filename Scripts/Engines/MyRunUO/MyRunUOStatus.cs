using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Engines.MyRunUO
{
	public class MyRunUOStatus
	{
		public static void Initialize()
		{
			if ( Config.Enabled )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( 20.0 ), Config.StatusUpdateInterval, new TimerCallback( Begin ) );
				Commands.CommandSystem.Register( "UpdateWebStatus", AccessLevel.Administrator, new Server.Commands.CommandEventHandler( UpdateWebStatus_OnCommand ) );
			}
		}

		[Usage( "UpdateWebStatus" )]
		[Description( "Starts the process of updating the MyRunUO online status database." )]
		public static void UpdateWebStatus_OnCommand( Server.Commands.CommandEventArgs e )
		{
			if ( m_Command == null || m_Command.HasCompleted )
			{
				Begin();
				e.Mobile.SendAsciiMessage( "Web status update process has been started." );
			}
			else
			{
				e.Mobile.SendAsciiMessage( "Web status database is already being updated." );
			}
		}

		private static DatabaseCommandQueue m_Command;

		public static void Begin()
		{
			if ( m_Command != null && !m_Command.HasCompleted )
				return;

			DateTime start = DateTime.Now;
			Console.WriteLine( "MyRunUO: Updating status database" );

			try
			{
				m_Command = new DatabaseCommandQueue( "MyRunUO: Status database updated in {0:F1} seconds", "MyRunUO Status Database Thread" );

				m_Command.Enqueue( String.Format( "DELETE FROM {0}", Config.StatusTable ) );

				List<NetState> online = NetState.Instances;

				for ( int i = 0; i < online.Count; ++i )
				{
					NetState ns = (NetState)online[i];
					Mobile mob = ns.Mobile;

					if ( mob != null )
						m_Command.Enqueue( String.Format( "INSERT INTO {1} VALUES ({0})", mob.Serial.Value.ToString(), Config.StatusTable ) );
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( "MyRunUO: Error updating status database" );
				Console.WriteLine( e );
			}

			if ( m_Command != null )
				m_Command.Enqueue( null );
		}
	}
}