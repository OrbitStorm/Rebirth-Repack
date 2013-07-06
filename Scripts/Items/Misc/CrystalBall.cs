using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class CrystalBall : BaseItem
	{
		private int m_TextHue;
		private string m_String;

		[Constructable]
		public CrystalBall() : base( 0xE2D )
		{
		}

		public override bool HandlesOnSpeech { get { return true; } }

		public override void OnSpeech(SpeechEventArgs e)
		{
			base.OnSpeech (e);

			if ( !e.Mobile.Hidden || e.Mobile.AccessLevel <= AccessLevel.Player )
			{
				m_TextHue = e.Hue;
				m_String = String.Format( "{0} said {1}", e.Mobile.Name, e.Speech );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_TextHue != 0 && m_String != null && m_String != "" && from.InRange( this.GetWorldLocation(), 3 ) )
				from.Send( new AsciiMessage( Serial, ItemID, MessageType.Regular, m_TextHue, 3, "", m_String ) );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string String 
		{ 
			get { return m_String; }
			set { m_String = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TextHue
		{ 
			get { return m_TextHue; }
			set { m_TextHue = value; }
		}

		public CrystalBall( Serial s ) : base( s )
		{
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int ver = reader.ReadInt();
			switch ( ver )
			{
				case 0:
				{
					m_TextHue = reader.ReadInt();
					m_String = reader.ReadString();
					break;
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( (int)0 );
			writer.Write( (int)m_TextHue );
			writer.Write( m_String == null ? "" : m_String );
		}
	}
}

