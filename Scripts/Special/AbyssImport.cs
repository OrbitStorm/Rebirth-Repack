using System;
using System.IO;
using System.Text;
using System.Collections; using System.Collections.Generic;
using Server.Mobiles;
using Server.Accounting;
using Server.Items;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Prompts;

namespace Server
{
	public class AbyssMergeTicket : BaseItem
	{
		private PlayerMobile m_Owner;
		private int m_Stats;
		private double m_Skills;

		[CommandProperty( AccessLevel.GameMaster )]
		public PlayerMobile Owner { get { return m_Owner; } set { m_Owner = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double Skills { get { return m_Skills; } set { m_Skills = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Stats { get { return m_Stats; } set { m_Stats = value; } }

		[Constructable]
		public AbyssMergeTicket() : this( null, 0, 0 )
		{
		}

		public AbyssMergeTicket( PlayerMobile owner, int stats, double skills ) : base( 0x14F0 )
		{
			if ( owner != null )
				Name = String.Format( "Skill & Stat Chooser for {0}", owner.Name );
			else
				Name = "Skill & Stat Chooser";

			Hue = 0x47E;
			LootType = LootType.Newbied;

			m_Owner = owner;
			m_Stats = (int)(stats * 0.75);
			m_Skills = skills * 0.75;
		}

		public AbyssMergeTicket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Owner );
			writer.Write( m_Skills );
			writer.Write( m_Stats );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Owner = reader.ReadMobile() as PlayerMobile;
			m_Skills = reader.ReadDouble();
			m_Stats = reader.ReadInt();
		}

		private static string m_WarningMsg = 
			"<u>Welcome to UOGamers: Rebirth!</u><br>" +
			"In the pages that follow, you will be given the chance to redistribute 75% of your Abyss Reborn stats and skills however you like.<br>" +
			"Before doing this, you should read the guides available on the forums at www.uorebirth.com to make sure you understand the ruleset.<br>" +
			"Once you have used this item, you will not be able to alter this character again.<br>" +
			"<br>" +
			"Proceed?";

		public override void OnDoubleClick(Mobile from)
		{
			if ( from == m_Owner )
			{
				from.SendGump( new WarningGump( 1060635, 30720, m_WarningMsg, 0xFFC000, 420, 400, new WarningGumpCallback( WarningResponse ), null ) );
			}
			else
			{
				from.SendAsciiMessage( "You are not the original owner of this, and you may not use it." );
			}
		}

		private void WarningResponse( Mobile from, bool okay, object state )
		{
			if ( okay )
			{
				from.CloseGump( typeof( SevenSkillsGump ) );
				from.SendGump( new SevenSkillsGump( this ) );
			}
		}
	}

	public class SevenSkillsGump : Gump
	{
		AbyssMergeTicket m_Sender;
		int m_Page;
		double m_Skills;
		Hashtable m_Table;

		public SevenSkillsGump( AbyssMergeTicket sender ) : this( sender, 0, sender.Skills, new Hashtable() )
		{
		}

		public SevenSkillsGump( AbyssMergeTicket sender, int page, double skills, Hashtable table ) : base( 50, 50 )
		{
			if ( page < 0 )
				page = 0;

			m_Page = page;
			m_Sender = sender;
			m_Skills = skills;
			m_Table = table;

			Closable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);

			AddBackground(10, 10, 225, 425, 9380);
			AddLabel(15, 15, 1152, String.Format( "Choose a skill ({0} points left)", m_Skills ) );

			for ( int i = 0; i < 8; i++ )
			{
				int curSkill = i + ( page * 8 );

				if ( curSkill >= SkillInfo.Table.Length || SkillInfo.Table[curSkill] == null || curSkill >= (int)SkillName.Meditation )
					break;

				AddButton( 40, 55 + ( 45 * i ), 208, 209, curSkill+1, GumpButtonType.Reply, 0 );
				AddLabel(70, 55 + ( 45 * i ) , 0, SkillInfo.Table[curSkill].Name );
			}

			//AddButton(91, 411, 247, 248, 0xFD, GumpButtonType.Reply, 0);
			//Okay Button ->  # 1

			if ( ( Core.AOS ? SkillInfo.Table.Length : (int)SkillName.RemoveTrap+1 ) - ( page * 8 + 8 ) > 0 )
			{
				AddButton(190, 412, 4005, 4007, 0xFE, GumpButtonType.Reply, 0);
				//Forward button -> #2
			}

			if ( page > 0 )
			{
				AddButton(29, 412, 4014, 4016, 0xFF, GumpButtonType.Reply, 0);
				//Back Button -> #3
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Sender == null || m_Sender.Deleted )
				return;

			PlayerMobile m = sender.Mobile as PlayerMobile;
			int sk = info.ButtonID-1;
			if ( info.ButtonID == 0xFE )
			{
				m.SendGump( new SevenSkillsGump( m_Sender, ++m_Page, m_Skills, m_Table ) );
			}
			else if ( info.ButtonID == 0xFF )
			{
				m.SendGump( new SevenSkillsGump( m_Sender, --m_Page, m_Skills, m_Table ) );
			}
			else if ( info.ButtonID == 0 || sk < 0 || sk >= (int)SkillName.Meditation )
			{
				m.SendAsciiMessage( "Canceled." );
				return;
			}
			else
			{
				m.Prompt = new SkillValuePrompt( (SkillName)sk, m_Table, m_Sender, m_Skills );
				m.SendAsciiMessage( "Enter a value for {0} (Maximum of 75.0):", sk );
			}
		}
	}

	public class SkillValuePrompt : Prompt
	{
		private SkillName m_Sk;
		private Hashtable m_Table;
		private AbyssMergeTicket m_Sender;
		private double m_Skills;

		public SkillValuePrompt( SkillName sk, Hashtable table, AbyssMergeTicket sender, double skills )
		{
			m_Sk = sk;
			m_Table = table;
			m_Sender = sender;
			m_Skills = skills;
		}

		public override void OnCancel(Mobile from)
		{
			from.CloseGump( typeof( SevenSkillsGump ) );
			from.SendGump( new SevenSkillsGump( m_Sender, 0, m_Skills, m_Table ) );
		}

		public override void OnResponse(Mobile from, string text)
		{
			double val = Utility.ToDouble( text.Trim() );

			double max = 75;

			if ( m_Table[m_Sk] is double )
				m_Skills += ((double)m_Table[m_Sk]);

			if ( m_Skills < 75 )
				max = m_Skills;

			if ( val > max )
				val = max;
			else if ( val < 0 )
				val = 0;

			if ( val > 60 && ( m_Sk == SkillName.Blacksmith || m_Sk == SkillName.Lockpicking || m_Sk == SkillName.Alchemy || m_Sk == SkillName.Provocation ) )
			{
				from.SendAsciiMessage( "Due to popularity, this skill is limited to 60.0" );

				val = 60;
			}

			m_Skills -= val;

			m_Table[m_Sk] = val;

			from.CloseGump( typeof( SevenSkillsGump ) );
			if ( m_Skills > 1 )
				from.SendGump( new SevenSkillsGump( m_Sender, 0, m_Skills, m_Table ) );
			else
				from.SendGump( new StatBallGump( m_Sender, m_Table ) );

			from.SendAsciiMessage( "This selection will be applied when you are done choosing." );
		}
	}

	public class StatBallGump : Gump
	{
		AbyssMergeTicket m_Sender;
		Hashtable m_Table;

		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
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

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, 0xFFFFFF ), false, false );
		}

		public StatBallGump( AbyssMergeTicket sender, Hashtable table ) : base( 50, 50 )
		{
			m_Sender = sender;
			m_Table = table;

			Closable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);

			AddBlackAlpha( 10, 120, 275, 150 );
			AddHtml( 10, 125, 275, 20, Color( Center( String.Format( "Distribute {0} Stat Points (max 70 each)", m_Sender.Stats ) ), 0xFFFFFF ), false, false );

			AddLabel( 73, 15, 1152, "" );
			AddLabel( 20, 150, 0x480, "Strength:" );
			AddTextField( 150, 150, 40, 20, 0 );

			AddLabel( 20, 180, 0x480, "Dexterity:" );
			AddTextField( 150, 180, 40, 20, 1 );

			AddLabel( 20, 210, 0x480, "Intelligence:" );
			AddTextField( 150, 210, 40, 20, 2 );

			AddButtonLabeled( 75, 240, 1, "Submit" );
		}

		int Str, Dex, Int;

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Sender == null || m_Sender.Deleted || info.ButtonID != 1 )
				return;

			Mobile m = sender.Mobile;

			m.CloseGump( typeof( StatBallGump ) );

			TextRelay strEntry = info.GetTextEntry( 0 );
			TextRelay dexEntry = info.GetTextEntry( 1 );
			TextRelay intEntry = info.GetTextEntry( 2 );

			try
			{
				Str = Int32.Parse( strEntry == null ? null : strEntry.Text.Trim() );
				Dex = Int32.Parse( dexEntry == null ? null : dexEntry.Text.Trim() );
				Int = Int32.Parse( intEntry == null ? null : intEntry.Text.Trim() );
			}
			catch
			{
				m.SendMessage( "You must complete the form with numeric values before clicking submit." );
				m.SendGump( new StatBallGump( m_Sender, m_Table ) );
				return;
			}
				
			if ( (Str+Dex+Int) != m_Sender.Stats && m_Sender.Stats <= 225 )
			{
				m.SendMessage( "Your stats must total {0}.", m_Sender.Stats );
				m.SendGump( new StatBallGump( m_Sender, m_Table ) );
			}
			else if ( Str > 70 || Dex > 70 || Int > 70 )
			{
				m.SendMessage( "You cannot exceed 70 in any stat." );
				m.SendGump( new StatBallGump( m_Sender, m_Table ) );
			}
			else if ( Str < 10 || Dex < 10 || Int < 10 )
			{
				m.SendMessage( "You must have more than 10 in each stat." );
				m.SendGump( new StatBallGump( m_Sender, m_Table ) );
			}
			else
			{
				StringBuilder sb = new StringBuilder( "<u>Character summery:</u><br>" );
				sb.AppendFormat( "Str: {0}<br>", Str );
				sb.AppendFormat( "Dex: {0}<br>", Dex );
				sb.AppendFormat( "Int: {0}<br>", Int );
				sb.Append( "<br>" );
				foreach ( DictionaryEntry de in m_Table )
					sb.AppendFormat( "{0}: {1}<br>", de.Key, de.Value );
				sb.Append( "<br>Are you sure you want to do this? (This cannot be undone.)" );
				m.SendGump( new WarningGump( 1060635, 30720, sb.ToString(), 0xFFC000, 420, 400, new WarningGumpCallback( ConfirmResponse ), null ) );
			}
		}

		private void ConfirmResponse( Mobile m, bool ok, object state )
		{
			if ( !ok )
			{
				m.SendAsciiMessage( "Canceled." );
				return;
			}

			if ( m == m_Sender.Owner && !m_Sender.Deleted )
			{
				m_Sender.Delete();

				using ( StreamWriter txt = new StreamWriter( Path.Combine( Core.BaseDirectory, "Abyss.log" ), true ) )
				{
					txt.AutoFlush = true;

					txt.WriteLine( "{0} ({1:X})", m.Name, m.Serial.Value );
					txt.WriteLine( "Stats={0}/{1}/{2}", Str, Dex, Int );

					m.RawStr = Str;
					m.RawDex = Dex;
					m.RawInt = Int;

					for(int i=0;i<SkillInfo.Table.Length;i++)
					{
						SkillName sk = (SkillName)i;

						if ( m_Table[sk] is double )
						{
							double val = (double)m_Table[sk];
							txt.WriteLine( "{0}={1}", sk, val );
							m.Skills[sk].Base = val;
						}
						else
						{
							m.Skills[sk].Base = 0;
						}
					}
				}

				m.SendAsciiMessage( "Enjoy UOGamers: Rebirth!" );
			}
		}
	}
}
