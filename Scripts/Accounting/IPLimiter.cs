using System;
using System.Net;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Accounting;

namespace Server.Misc
{
	public class IPLimiter
	{
		public static void Configure()
		{
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Login( LoginEventArgs args )
		{
			//HWVerify( args.Mobile );
		}

		public static bool HWVerify( Mobile m )
		{
			NetState ourState = m.NetState;
			if ( ourState == null )
				return false;
			if ( m.AccessLevel >= AccessLevel.GameMaster )
				return true;
			
			IPAddress ourAddress = ourState.Address;

			List<NetState> netStates = NetState.Instances;

			int count = 0;

			ArrayList infos = new ArrayList( 1 );
			for ( int i = 0; i < netStates.Count; ++i )
			{
				NetState ns = (NetState)netStates[i];

				if ( ourAddress.Equals( ns.Address ) )
				{
					++count;
					Account a = ns.Account as Account;
					if ( a == null )
						continue;
					if ( a.AccessLevel >= AccessLevel.GameMaster || ( ns.Mobile != null && ns.Mobile.AccessLevel >= AccessLevel.GameMaster ) )
						return true;

					bool valid;
					HardwareInfo hw = a.HardwareInfo;
					if ( hw != null && !( hw.DXMajor == 0 && hw.DXMinor == 0 && hw.CpuModel == 0 && hw.CpuClockSpeed == 0 ) )
					{
						valid = count <= infos.Count+1;
						for(int c=0;c<infos.Count;c++)
						{
							if ( ((HardwareInfo)infos[c]).Equals( hw ) )
							{
								valid = false;
								break;
							}
						}

						if ( valid )
							infos.Add( hw );
					}
					else
					{
						valid = count == 1;
					}

					if ( !valid )
					{
						m.SendAsciiMessage( 0x25, "Your connection could not be verified to be unique from the other connections from this IP address." );
						m.SendAsciiMessage( 0x25, "Are already connected to this server with at least 1 client from this computer!" );
						ourState.Send( new PopupMessage( PMMessage.CharInWorld ) );
						ourState.Dispose();
						return false;
					}
				}
			}

			return true;
		}

		public static bool Enabled = true;
		public static bool SocketBlock = false; // true to block at connection, false to block at login request

		public const int MaxAddresses = 3;

		public static bool Verify( NetState ns )
		{
			string tag;
			int max = MaxAddresses;
			if ( !Enabled )
				return true;

			if ( ns.Account != null )
			{
				tag = ((Account)ns.Account).GetTag( "MaxConn" );
				if ( tag != null && tag != "" )
				{
					try
					{
						max = Convert.ToInt32( tag );
					}
					catch
					{
						max = MaxAddresses;
					}
				}
				if ( ((Account)ns.Account).AccessLevel > AccessLevel.Player )
					max = int.MaxValue;
			}

			IPAddress ourAddress = ns.Address;
			List<NetState> netStates = NetState.Instances;

			int count = 1;
			for ( int i = 0; i < netStates.Count; ++i )
			{
				NetState compState = (NetState)netStates[i];

				if ( compState != ns && ourAddress.Equals( compState.Address ) )
				{
					if ( compState.Account != null )
					{
						tag = ((Account)compState.Account).GetTag( "MaxConn" );
						if ( tag != null && tag != "" )
						{
							try
							{
								max = Convert.ToInt32( tag );
							}
							catch
							{
							}
						}

						if ( ((Account)compState.Account).AccessLevel >= AccessLevel.GameMaster )
							max = int.MaxValue;
					}

					++count;

					if ( count > max )
						return false;
				}
			}

			return count <= max;
		}
	}
}
