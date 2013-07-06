using System;
using System.Collections; using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Mail;
using Server;
using Server.Accounting;
using Server.Misc;
using Server.Network;

namespace Server.Gumps
{
	public class LinkAddressGump : Gump
	{
		private Mobile m_Mobile;
		private Account m_Account;
		
		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		private const int LabelColor32 = 0xFFFFFF;
		private const int LabelHue = 0x480;

		public LinkAddressGump( Mobile m, Account a ) : base( 20, 30 )
		{
			m_Mobile = m;
			m_Account = a;
			
			AddPage( 0 );

			AddBlackAlpha( 10, 120, 360, 120 );
			AddHtml( 10, 125, 400, 20, Color( Center( "Register Address" ), LabelColor32 ), false, false );

			AddLabel( 20, 150, LabelHue, "E-Mail Address:" );
			AddTextField( 130, 150, 230, 20, 0 );

			AddLabel( 20, 180, LabelHue, "Confirm E-Mail:" );
			AddTextField( 130, 180, 230, 20, 1 );

			AddButtonLabeled( 80, 210, 1, "Submit" );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID != 1 )
				return;

			TextRelay emailEntry = info.GetTextEntry( 0 );
			TextRelay confirmEntry = info.GetTextEntry( 1 );

			string email = ( emailEntry == null ? null : emailEntry.Text.Trim() );
			string confirm = ( confirmEntry == null ? null : confirmEntry.Text.Trim() );

			if ( email == null || email.Length == 0 )
			{
				m_Mobile.SendMessage( "Registration cancelled." );
			}
			else if ( email != confirm )
			{
				m_Mobile.SendMessage( "You must confirm your e-mail address entry. Both fields must match exactly. Try again." );
				m_Mobile.SendGump( new LinkAddressGump( m_Mobile, m_Account ) );
			}
			/*else if ( !Email..IsValid( email ) )
			{
				m_Mobile.SendMessage( "You have specified an invalid e-mail address. Verify the address and try again." );
				m_Mobile.SendGump( new LinkAddressGump( m_Mobile, m_Account ) );
			}*/
			else
			{
				try
				{
					AuthKey ak = AccountManagement.MakeKey( m_Account, AuthType.Register, email );
					m_Mobile.SendMessage( "An e-mail has been dispatched to {0} with detailed instructions on how to finalize your registration.", email );
					m_Mobile.SendMessage( "Your registration request will expire in {0} hours.", AuthKey.KeyExpire.Hours );

					//MailMessage mm = new MailMessage( "UOGamers Account Manager <noreply@runuo.com>", email );
					MailMessage mm = new MailMessage();
					mm.From = "UOGamers Account Manager <noreply@runuo.com>";
					mm.To = email;
					mm.Subject = "UOGamers Account Management";
					mm.Body = String.Format(
						"{0},\n\tThank you for registering this e-mail address with your UOGamers account. This will allow you to change your password (among other things) securely without Game Master assistance. To finalize your registration, you must enter the following string (while in game) exactly as it appears.\n\n[auth {1}\n\nThis key will expire at {2}. If you have any questions, comments, suggestions, or require assistance, please do not hesitate to page or visit our forums at http://www.uogamers.com/forum\n\n\tThank you,\n\t\tThe UOGamers Administration Team\n\t\thttp://www.uogamers.com\n\n\nThis message is not spam. This registration request was initiated by {3}. If you feel you received this message in error, please disregard it.", m_Mobile.Name, ak.Key, ak.Expiration, state.ToString() );
				
					Email.AsyncSend( mm );
				}
				catch
				{
					m_Mobile.SendMessage( "There was an error, please try again in a few hours." );
				}
			}
		}
	}

	public class AccountManagementGump : Gump
	{
		private Mobile m_Mobile;
		private Account m_Account;
		
		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		private const int LabelColor32 = 0xFFFFFF;
		private const int LabelHue = 0x480;

		public AccountManagementGump( Mobile m, Account a ) : base( 20, 30 )
		{
			m_Mobile = m;
			m_Account = a;
			
			AddPage( 0 );

			AddBlackAlpha( 10, 120, 360, 150 );
			AddHtml( 10, 125, 400, 20, Color( Center( "Account Management" ), LabelColor32 ), false, false );

			AddButtonLabeled( 20, 150, 1, "Reset Account E-Mail" );
			AddButtonLabeled( 20, 180, 2, "Change Password" );
			AddButtonLabeled( 20, 210, 3, "View Access List" );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile m = m_Mobile;
			int buttonID = info.ButtonID;

			switch ( buttonID )
			{
				case 1:
					m.CloseGump( typeof( ResetEMailGump ) );
					m.SendGump( new ResetEMailGump( m_Mobile, m_Account ) );
					break;
				case 2:
					m.CloseGump( typeof( ChangePasswordGump ) );
					m.SendGump( new ChangePasswordGump( m_Mobile, m_Account ) );
					break;
				case 3:
					m.CloseGump( typeof( ViewAccessListGump ) );
					m.SendGump( new ViewAccessListGump( m_Mobile, m_Account ) );
					break;
				default:
					break;
			}
		}
	}

	public class ResetEMailGump : Gump
	{
		private Mobile m_Mobile;
		private Account m_Account;
		
		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		private const int LabelColor32 = 0xFFFFFF;
		private const int LabelHue = 0x480;

		public ResetEMailGump( Mobile m, Account a ) : base( 20, 30 )
		{
			m_Mobile = m;
			m_Account = a;
			
			AddPage( 0 );

			AddBlackAlpha( 10, 120, 360, 150 );
			AddHtml( 10, 125, 400, 20, Color( Center( "Reset E-Mail Address" ), LabelColor32 ), false, false );

			AddHtml( 20, 150, 400, 20, Color( "This will reset your accounts email address,", LabelColor32 ), false, false );
			AddHtml( 20, 175, 400, 20, Color( "allowing you to re-register with a new one.", LabelColor32 ), false, false );

			AddButtonLabeled( 50, 210, 1, "Submit Request" );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID != 1 )
				return;

			string email = AccountManagement.GetEMail( m_Account );
			if ( email == null )
				email = "-null-";

			AuthKey ak = AccountManagement.MakeKey( m_Account, AuthType.EMail, null );
			m_Mobile.SendMessage( "An e-mail has been dispatched to {0} with detailed instructions on how to finalize your request.", email );
			m_Mobile.SendMessage( "Your request will expire in {0} hours.", AuthKey.KeyExpire.Hours );

			//MailMessage mm = new MailMessage( "UOGamers Account Manager <noreply@runuo.com>", email );
			MailMessage mm = new MailMessage();
			mm.From = "UOGamers Account Manager <noreply@runuo.com>";
			mm.To = email;
			mm.Subject = "UOGamers Account Management";
			mm.Body = String.Format(
				"{0},\n\tYou have requested to release this e-mail address from your account. To finalize this request, you must enter the following string (while in game) exactly as it appears.\n\n[auth {1}\n\nThis key will expire at {2}. If you have any questions, comments, suggestions, or require assistance, please do not hesitate to page or visit our forums at http://www.uogamers.com/forum\n\n\tThank you,\n\t\tThe UOGamers Administration Team\n\t\thttp://www.uogamers.com\n\n\nThis message is not spam. This request was initiated by {3}. If you feel you received this message in error, please disregard it.", m_Mobile.Name, ak.Key, ak.Expiration, state.ToString() );
				
			Email.AsyncSend( mm );
		}
	}

	public class ChangePasswordGump : Gump
	{
		private Mobile m_Mobile;
		private Account m_Account;
		
		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		private const int LabelColor32 = 0xFFFFFF;
		private const int LabelHue = 0x480;

		public ChangePasswordGump( Mobile m, Account a ) : base( 20, 30 )
		{
			m_Mobile = m;
			m_Account = a;
			
			AddPage( 0 );

			AddBlackAlpha( 10, 120, 360, 150 );
			AddHtml( 10, 125, 400, 20, Color( Center( "Change Password" ), LabelColor32 ), false, false );

			AddLabel( 20, 150, LabelHue, "New Password:" );
			AddTextField( 200, 150, 160, 20, 0 );

			AddLabel( 20, 180, LabelHue, "Confirm Password:" );
			AddTextField( 200, 180, 160, 20, 1 );

			AddButtonLabeled( 50, 210, 1, "Submit" );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID != 1 )
				return;

			TextRelay pwEntry = info.GetTextEntry( 0 );
			TextRelay confirmEntry = info.GetTextEntry( 1 );

			string pw = ( pwEntry == null ? null : pwEntry.Text.Trim() );
			string confirm = ( confirmEntry == null ? null : confirmEntry.Text.Trim() );

			if ( pw == null || pw.Length == 0 )
			{
				m_Mobile.SendMessage( "Password change cancelled." );
			}
			else if ( pw != confirm )
			{
				m_Mobile.SendMessage( "You must confirm your password entry. Both fields must match exactly. Try again." );
				m_Mobile.SendGump( new ChangePasswordGump( m_Mobile, m_Account ) );
			}
			else
			{
				string email = AccountManagement.GetEMail( m_Account );
				if ( email == null )
					email = "-null-";

				AuthKey ak = AccountManagement.MakeKey( m_Account, AuthType.Password, pw );
				m_Mobile.SendMessage( "An e-mail has been dispatched to {0} with detailed instructions on how to finalize your request.", email );
				m_Mobile.SendMessage( "Your request will expire in {0} hours.", AuthKey.KeyExpire.Hours );

				//MailMessage mm = new MailMessage( "UOGamers Account Manager <noreply@runuo.com>", email );
				MailMessage mm = new MailMessage();
				mm.From = "UOGamers Account Manager <noreply@runuo.com>";
				mm.To = email;
				mm.Subject = "UOGamers Account Management";
				mm.Body = String.Format(
					"{0},\n\tYou have requested to change the password for account '{1}' to '{2}'. To finalize your request, you must enter the following string (while in game) exactly as it appears.\n\n[auth {3}\n\nThis key will expire at {4}. If you have any questions, comments, suggestions, or require assistance, please do not hesitate to page or visit our forums at http://www.uogamers.com/forum\n\n\tThank you,\n\t\tThe UOGamers Administration Team\n\t\thttp://www.uogamers.com\n\n\nThis message is not spam. This request was initiated by {5}. If you feel you received this message in error, please disregard it.", m_Mobile.Name, m_Account, pw, ak.Key, ak.Expiration, state.ToString() );
				
				Email.AsyncSend( mm );
			}
		}
	}

	public class ViewAccessListGump : Gump
	{
		private Mobile m_Mobile;
		private Account m_Account;
		
		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		private const int LabelColor32 = 0xFFFFFF;
		private const int LabelHue = 0x480;

		public ViewAccessListGump( Mobile m, Account a ) : base( 20, 30 )
		{
			m_Mobile = m;
			m_Account = a;
			
			AddPage( 0 );

			AddBlackAlpha( 10, 120, 360, 150 );
			AddHtml( 10, 125, 400, 20, Color( Center( "View Access List" ), LabelColor32 ), false, false );

			AddHtml( 20, 150, 400, 20, Color( "This will e-mail you a listing of all IP", LabelColor32 ), false, false );
			AddHtml( 20, 175, 400, 20, Color( "addresses that have accessed this account.", LabelColor32 ), false, false );

			AddButtonLabeled( 50, 210, 1, "Submit Request" );
		}

		public static Hashtable LogRequested = new Hashtable();

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID != 1 )
				return;

			if ( LogRequested[m_Account] != null )
			{
				m_Mobile.SendMessage( "Sorry, you may only use this option once per day." );
				return;
			}

			string email = AccountManagement.GetEMail( m_Account );
			if ( email == null )
				email = "-null-";

			m_Mobile.SendMessage( "An e-mail has been dispatched to {0} with a listing of all IP addresses that have accessed this account.", email );
			
			StringBuilder sb = new StringBuilder( m_Account.LoginIPs.Length );
			for ( int i = 0; i < m_Account.LoginIPs.Length; i++ )
			{
				IPAddress ip = m_Account.LoginIPs[i];
				sb.Append( String.Format("- {0}\n", ip.ToString()) );
			}

			//MailMessage mm = new MailMessage( "UOGamers Account Manager <noreply@runuo.com>", email );
			MailMessage mm = new MailMessage();
			mm.From = "UOGamers Account Manager <noreply@runuo.com>";
			mm.To = email;
			mm.Subject = "UOGamers Account Management";
			mm.Body = String.Format(
				"{0},\n\tAs you requested, the following is a listing of all IP addresses that have accessed your account.\n\n{1}\n\nIf you have any questions, comments, suggestions, or require assistance, please do not hesitate to page or visit our forums at http://www.uogamers.com/forum\n\n\tThank you,\n\t\tThe UOGamers Administration Team\n\t\thttp://www.uogamers.com\n\n\nThis message is not spam. This request was initiated by {2}. If you feel you received this message in error, please disregard it.", m_Mobile.Name, sb.ToString(), state.ToString() );
				
			Email.AsyncSend( mm );

			LogRequested[m_Account] = 1;
		}
	}
}