using System;
using System.Collections; using System.Collections.Generic;
using System.Text;
using Server;
using Server.Misc;
using Server.Network;
using Server.Gumps;

namespace Server.Accounting
{
	public class AccountManagement
	{
		public static string GetEMail( Account a )
		{
			return a.GetTag( "EMail" );
		}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( Event_Login );

			Server.Commands.CommandSystem.Register( "Account", AccessLevel.Player, new Server.Commands.CommandEventHandler( Command_Account ) );
			Server.Commands.CommandSystem.Register( "Auth", AccessLevel.Player, new Server.Commands.CommandEventHandler( Command_Register ) );
		}

		[Usage( "Account" )]
		[Description( "Allows a player to manage their account settings." )]
		public static void Command_Account( Server.Commands.CommandEventArgs e )
		{
			Mobile m = e.Mobile;

			if ( AutoRestart.Restarting )
			{
				m.SendMessage( "Accounts cannot be managed during server wars." );
				return;
			}

			Account a = m.Account as Account;

			if ( a == null ) // Sanity
				return;

			AuthKey ak = GetKey( a );

			string email = GetEMail( a );
			if ( email == null && ( ak == null || ak.Expired ) )
			{
				m.SendMessage( "There is no e-mail linked to your account. Please register first." );
				m.CloseGump( typeof( LinkAddressGump ) );
				m.SendGump( new LinkAddressGump( m, a ) );
				return;
			}


			if ( ak != null && !ak.Expired)
			{
				m.SendMessage( "You already have a request pending. Please finalize your current request before making another." );
				return;
			}

			m.CloseGump( typeof( AccountManagementGump ) );
			m.SendGump( new AccountManagementGump( m, a ) );
		}

		[Usage( "Auth <key>" )]
		[Description( "Finalizes an account management request." )]
		public static void Command_Register( Server.Commands.CommandEventArgs e )
		{
			Mobile m = e.Mobile;

			if ( AutoRestart.Restarting )
			{
				m.SendMessage( "Validation requests expire during server wars. Please re-request after server restart." );
				return;
			}

			string key = e.ArgString;

			if ( key == null || key == "" )
			{
				m.SendMessage( "You must enter a valid key." );
				return;
			}

			Account a = m.Account as Account;

			if ( a == null ) // Sanity
				return;

			AuthKey ak = GetKey( a );

			if ( ak == null )
			{
				m.SendMessage( "You have no validations pending." );
				return;
			}
			else if ( ak.Expired )
			{
				m.SendMessage( "That validation key has expired." );
				return;
			}
			else if ( ak.Key != key )
			{
				m.SendMessage( "That validation key is incorrect." );
				return;
			}

			switch ( ak.Type )
			{
				case AuthType.Register:
					string email = ak.State as string;
					a.SetTag( "EMail", email );
					m.SendMessage( "Your account has been registered.", email );
					break;
				case AuthType.EMail:
					a.RemoveTag( "EMail" );
					m.SendMessage( "Your e-mail address has been reset." );
					m.SendMessage( "You will be prompted to re-register on your next login." );
					break;
				case AuthType.Password:
					string pw = ak.State as string;
					a.SetPassword( pw );
					m.SendMessage( "Your password has been changed." );
					break;
				default:
					m.SendMessage( "ERROR: Invalid AuthType, please page an administrator for assistance." );
					break;
			}

			RemoveKey( a );
		}

		public static void Event_Login( LoginEventArgs e )
		{
			Mobile m = e.Mobile;
			Account a = m.Account as Account;

			if ( a == null ) // Sanity
				return;

			AuthKey ak = GetKey( a );
			string email = GetEMail( a );

			if ( email == null && ( ak == null || ak.Expired ) )
			{
				m.SendMessage( "Please link a valid e-mail address to this account." );
				m.SendMessage( "This e-mail address will only be used for account management." );
				m.CloseGump( typeof( LinkAddressGump ) );
				m.SendGump( new LinkAddressGump( m, a ) );
			}
			
			//m.SendMessage( "This account is owned by '{0}'.", a.EMail );
		}

		private static Hashtable m_AuthKeys = new Hashtable();

		public static AuthKey GetKey( Account a )
		{
			AuthKey ak = m_AuthKeys[a] as AuthKey;
			return ak;
		}

		public static AuthKey MakeKey( Account a, AuthType t, object state )
		{
			AuthKey ak = new AuthKey();
			ak.Type = t;
			ak.State = state;
			m_AuthKeys[a] = ak;
			return ak;
		}

		public static void RemoveKey( Account a )
		{
			m_AuthKeys[a] = null;
		}
	}

	public class AuthKey
	{
		public static TimeSpan KeyExpire = TimeSpan.FromHours( 2.0 );
		
		private static char[] m_Characters = new char[]
		{
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
			'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
			'2', '3', '4', '5', '6', '7', '8', '9',
		};

		private DateTime m_Expiration;
		private string m_Key;
		private AuthType m_AuthType;
		private object m_State;

		public DateTime Expiration { get { return m_Expiration; } }
		public bool Expired { get { return DateTime.Now > m_Expiration; } }
		public string Key { get { return m_Key; } }
		public AuthType Type { get { return m_AuthType; } set { m_AuthType = value; } }
		public object State { get { return m_State; } set { m_State = value; } }

		public AuthKey() : this( 10 )
		{
		}

		public AuthKey( int length )
		{
			m_Key = Generate( length );
			m_Expiration = DateTime.Now + KeyExpire;
		}

		public static string Generate( int length )
		{
			StringBuilder sb = new StringBuilder( length );
			
			for ( int i = 0; i < length; i++ )
				sb.Append( m_Characters[Utility.Random(m_Characters.Length)] );
			
			return sb.ToString();
		}
	}

	public enum AuthType
	{
		Register,
		EMail,
		Password,
	}
}

