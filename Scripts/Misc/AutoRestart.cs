using System;
using System.IO;
using System.Diagnostics;
using Server;

namespace Server.Misc
{
	public class AutoRestart : Timer
	{
		private static bool Enabled = true; // is the script enabled?

		private static TimeSpan RestartTime = TimeSpan.FromSeconds( 1.0 ); // 0:00:01am (server time) to restart
		private static TimeSpan RestartDelay = TimeSpan.FromMinutes( 15.25 ); // how long the server should remain active before restart (period of 'server wars')

		private static TimeSpan WarningDelay = TimeSpan.FromMinutes( 3 ); // at what interval should the shutdown message be displayed?

		private static bool m_Restarting;
		private static DateTime m_RestartTime;

		public static bool Restarting
		{
			get{ return m_Restarting; }
		}

		public static void Initialize()
		{
			if ( Enabled )
			{
				Commands.CommandSystem.Register( "Restart", AccessLevel.Administrator, new Server.Commands.CommandEventHandler( Restart_OnCommand ) );
				new AutoRestart().Start();
			}
		}

		public static void Restart_OnCommand( Server.Commands.CommandEventArgs e )
		{
			if ( m_Restarting )
			{
				e.Mobile.SendAsciiMessage( "The server is already restarting." );
			}
			else
			{
				e.Mobile.SendAsciiMessage( "You have initiated server shutdown." );
				Enabled = true;
				m_RestartTime = DateTime.Now;
			}
		}

		public AutoRestart() : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{
			Priority = TimerPriority.FiveSeconds;

			m_RestartTime = DateTime.Now.Date + RestartTime;

			if ( m_RestartTime < DateTime.Now )
				m_RestartTime += TimeSpan.FromDays( 1.0 );
		}

		private void Warning_Callback()
		{
			//World.Broadcast( 0x22, true, "The server is going down shortly." );
			Scripts.Commands.CommandHandlers.SystemMessage( "The server is going down shortly." );
		}

		private void Restart_Callback()
		{
			Scripts.Commands.CommandHandlers.SystemMessage( "The server is going down shortly." );

			while ( AsyncWriter.ThreadCount > 0 /*|| Accounting.Accounts.ThreadRunning*/ )
				System.Threading.Thread.Sleep( 100 );

			System.Threading.Thread.Sleep( 5000 );
			Process.Start( Core.ExePath );
			Core.Process.Kill();
		}

		protected override void OnTick()
		{
			if ( m_Restarting || !Enabled )
				return;

			if ( DateTime.Now < m_RestartTime )
				return;

			if ( WarningDelay > TimeSpan.Zero )
			{
				Warning_Callback();
				Timer.DelayCall( WarningDelay, WarningDelay, new TimerCallback( Warning_Callback ) );
			}

            Misc.AutoSave.Save();

			m_Restarting = true;
			Timer.DelayCall( RestartDelay, new TimerCallback( Restart_Callback ) );
		}
	}
}
