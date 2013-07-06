using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections; using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Web.Mail;

namespace Server.SMTP
{
	public class SmtpDirect
	{
		private enum SmtpResponse
		{
			ConnectSuccess = 220,
			GenericSuccess = 250,
			DataSuccess = 354,
			QuitSuccess = 221
		}

		private const string SmtpServer = "24.234.117.168";

		private static IPEndPoint m_SmtpServer;
		private static string m_HostName;

		public static void Send( MailMessage message )
		{
			ThreadPool.QueueUserWorkItem( new WaitCallback( SendThread ), message );
		}

		private static void SendThread( object state )
		{
			MailMessage message = (MailMessage)state;

			Console.WriteLine( "Network: Sending email to {0}...", message.To );

			bool success = false;

			try
			{
				success = UnsafeSend( message );
			}
			catch
			{
			}

			if ( success )
				Console.WriteLine( "Network: Email sent to {0}", message.To );
			else
				Console.WriteLine( "Network: Couldn't send email to {0}", message.To );
		}

		private static bool UnsafeSend( MailMessage message )
		{
			if ( m_SmtpServer == null )
				m_SmtpServer = new IPEndPoint( Dns.Resolve( SmtpServer ).AddressList[0], 25 );

			if ( m_HostName == null )
				m_HostName = Dns.GetHostName();

			Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

			try
			{
				socket.Connect( m_SmtpServer );
				if ( !CheckResponse( socket, SmtpResponse.ConnectSuccess ) )
					return false;

				Send( socket, "HELO {0}\r\n", m_HostName );
				if ( !CheckResponse( socket, SmtpResponse.GenericSuccess ) )
					return false;

				Send( socket, "MAIL From:{0}\r\n", message.From );
				if ( !CheckResponse( socket, SmtpResponse.GenericSuccess ) )
					return false;

				if ( !SendRecipientList( socket, message.To ) )
					return false;

				if ( !SendRecipientList( socket, message.Cc ) )
					return false;

				Send( socket, "DATA\r\n" );
				if ( !CheckResponse( socket, SmtpResponse.DataSuccess ) )
					return false;

				Send( socket, CompileMessageString( message ) );

				if ( !CheckResponse( socket, SmtpResponse.GenericSuccess ) )
					return false;

				Send( socket, "QUIT\r\n" );
				if ( !CheckResponse( socket, SmtpResponse.QuitSuccess ) )
					return false;
			}
			catch
			{
				Shutdown( socket );
				return false;
			}

			Shutdown( socket );
			return true;
		}

		private static bool SendRecipientList( Socket socket, string list )
		{
			if ( list == null )
				return true;

			string[] split = list.Split( ';' );

			for ( int i = 0; i < split.Length; ++i )
			{
				Send( socket, "RCPT TO:{0}\r\n", split[i] );

				if ( !CheckResponse( socket, SmtpResponse.GenericSuccess ) )
					return false;
			}

			return true;
		}

		private static void AppendRecipientList( StringBuilder sb, string type, string list )
		{
			if ( list == null )
				return;

			string[] split = list.Split( ';' );

			if ( split.Length == 0 )
				return;

			sb.Append( type ).Append( ": " );

			for ( int i = 0; i < split.Length; ++i )
			{
				if ( i > 0 )
					sb.Append( ',' );

				sb.Append( split[i] );
			}

			sb.Append( "\r\n" );
		}

		private static string CompileMessageString( MailMessage message )
		{
			StringBuilder sb = new StringBuilder();

			sb.Append( "From: " ).Append( message.From ).Append( "\r\n" );

			AppendRecipientList( sb, "To", message.To );
			AppendRecipientList( sb, "Cc", message.Cc );

			sb.Append( "Date: " ).Append( DateTime.Now.ToString( "ddd, d M y H:m:s z" ) ).Append( "\r\n" );
			sb.Append( "Subject: " ).Append( message.Subject ).Append( "\r\n" );
			sb.Append( "X-Mailer: RunUO Smtp\r\n" );

			sb.Append( "\r\n" );

			sb.Append( message.Body );

			if ( !message.Body.EndsWith( "\r\n" ) )
				sb.Append( "\r\n" );

			sb.Append( ".\r\n\r\n\r\n" );

			return sb.ToString();
		}

		private static byte[] m_SendBuffer;

		private static void Send( Socket socket, string message )
		{
			if ( m_SendBuffer == null || m_SendBuffer.Length < message.Length )
				m_SendBuffer = new byte[message.Length];

			int byteCount = Encoding.ASCII.GetBytes( message, 0, message.Length, m_SendBuffer, 0 );

			socket.Send( m_SendBuffer, 0, byteCount, SocketFlags.None );
		}

		private static void Send( Socket socket, string format, params object[] args )
		{
			Send( socket, String.Format( format, args ) );
		}

		private static void Send( Socket socket, string format, object arg )
		{
			Send( socket, String.Format( format, arg ) );
		}

		private static void Send( Socket socket, string format, object arg1, object arg2 )
		{
			Send( socket, String.Format( format, arg1, arg2 ) );
		}

		private static void Shutdown( Socket socket )
		{
			try{ socket.Shutdown( SocketShutdown.Both ); }
			catch{}

			try{ socket.Close(); }
			catch{}
		}

		private static byte[] m_Buffer = new byte[1024];

		private static bool CheckResponse( Socket socket, SmtpResponse response )
		{
			int byteCount = socket.Receive( m_Buffer, 0, m_Buffer.Length, SocketFlags.None );

			if ( byteCount <= 2 )
			{
				Shutdown( socket );
				return false;
			}

			int resp = (int)response;

			byte dig1 = (byte)( '0' + ((resp / 100) % 10) );
			byte dig2 = (byte)( '0' + ((resp /  10) % 10) );
			byte dig3 = (byte)( '0' + ((resp /   1) % 10) );

			bool valid = ( m_Buffer[0] == dig1 && m_Buffer[1] == dig2 && m_Buffer[2] == dig3 );

			if ( !valid )
				Shutdown( socket );

			return valid;
		}
	}
} 
