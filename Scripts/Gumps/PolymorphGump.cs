using System;
using Server;
using Server.Network;
using Server.Targets;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Menus.ItemLists;

namespace Server.Gumps
{
	public class PolymorphMenu : ItemListMenu
	{
		private class PolyEntry : ItemListEntry
		{
			private int m_Body;

			public PolyEntry( string name, int itemid, int body ) : base( name, itemid )
			{
				m_Body = body;
			}

			public int Body { get { return m_Body; } }
		}

		private static PolyEntry[] m_Menu = new PolyEntry[]
		{
			new PolyEntry( "Bear, Black",	8399, 0xD3 ),
			new PolyEntry( "Bear, Grizzly", 8411, 0xD4 ),
			new PolyEntry( "Bear, Polar",	8417, 0xD5 ),
			new PolyEntry( "Chicken",		8401, 0xD0 ),
			new PolyEntry( "Daemon",		8403, 0x09 ),
			new PolyEntry( "Dog",			8405, 0xD9 ),
			new PolyEntry( "Ettin",			8408, 0x02 ),
			new PolyEntry( "Gargoyle",		8409, 0x04 ),
			new PolyEntry( "Gorilla",		8437, 0x1D ),
			new PolyEntry( "Human, Male",	8397, 0x190 ),
			new PolyEntry( "Human, Female", 8398, 0x191 ),
			new PolyEntry( "Lizard Man",	8414, 0x21 ),
			new PolyEntry( "Ogre",			8415, 0x01 ),
			new PolyEntry( "Orc",			8416, 0x11 ),
			new PolyEntry( "Panther",		8473, 0xD6 ),
			new PolyEntry( "Slime",			8424, 0x33 ),
			new PolyEntry( "Troll",			8425, 0x36 ),
			new PolyEntry( "Wolf",			8426, 0xE1 ),
		};

		private Item m_Scroll;
		public PolymorphMenu( Item scroll ) : base( "Choose a creature", m_Menu )
		{
			m_Scroll = scroll;
		}

		public override void OnResponse(NetState state, int index)
		{
			if ( index >= m_Menu.Length || index < 0 )
				return;

			new PolymorphSpell( state.Mobile, m_Scroll, m_Menu[index].Body ).Cast();
		}
	}

	/*public class PolymorphGump : Gump
	{
		private class PolymorphEntry
		{
			private int m_Art, m_Body, m_Num;

			public PolymorphEntry( int Art, int Body, int LocNum )
			{
				m_Art = Art;
				m_Body = Body;
				m_Num = LocNum;
			}

			public int ArtID { get { return m_Art; } }
			public int BodyID { get { return m_Body; } }
			public int LocNumber{ get { return m_Num; } }
		}

		private class PolymorphCategory
		{
			private int m_Num;
			private PolymorphEntry[] m_Entries;

			public PolymorphCategory( int num, PolymorphEntry[] entries )
			{
				m_Num = num;
				m_Entries = entries;
			}

			public PolymorphEntry[] Entries{ get { return m_Entries; } }
			public int LocNumber{ get { return m_Num; } }
		}

		private static PolymorphCategory[] Categories = new PolymorphCategory[]
		{
			new PolymorphCategory( 1015235, new PolymorphEntry[] //Animals
			{
				new PolymorphEntry( 8401, 0xD0, 1015236 ),//Chicken
				new PolymorphEntry( 8405, 0xD9, 1015237 ),//Dog
				new PolymorphEntry( 8426, 0xE1, 1015238 ),//Wolf
				new PolymorphEntry( 8473, 0xD6, 1015239 ),//Panther
				new PolymorphEntry( 8437, 0x1D, 1015240 ),//Gorilla
				new PolymorphEntry( 8399, 0xD3, 1015241 ),//Black Bear
				new PolymorphEntry( 8411, 0xD4, 1015242 ),//Grizzly Bear
				new PolymorphEntry( 8417, 0xD5, 1015243 ),//Polar Bear
				new PolymorphEntry( 8397, 0x190, 1015244 )//Human Male
			} ),

			new PolymorphCategory( 1015245, new PolymorphEntry[] //Monsters
			{
				new PolymorphEntry( 8424, 0x33, 1015246 ),//Slime
				new PolymorphEntry( 8416, 0x11, 1015247 ),//Orc
				new PolymorphEntry( 8414, 0x21, 1015248 ),//Lizard Man
				new PolymorphEntry( 8409, 0x04, 1015249 ),//Gargoyle
				new PolymorphEntry( 8415, 0x01, 1015250 ),//Orge
				new PolymorphEntry( 8425, 0x36, 1015251 ),//Troll
				new PolymorphEntry( 8408, 0x02, 1015252 ),//Ettin
				new PolymorphEntry( 8403, 0x09, 1015253 ),//Daemon
				new PolymorphEntry( 8398, 0x191, 1015254 ),//Human Female
			} )
		};

		private Mobile m_Caster;
		private Item m_Scroll;

		public PolymorphGump( Mobile caster, Item scroll ) : base( 50, 50 )
		{
			m_Caster = caster;
			m_Scroll = scroll;

			int x,y;
			AddPage( 0 );
			AddBackground( 0, 0, 585, 393, 5054 );
			AddBackground( 195, 36, 387, 275, 3000 );
			AddHtmlLocalized( 0, 0, 510, 18, 1015234, false, false ); // <center>Polymorph Selection Menu</center>
			AddHtmlLocalized( 60, 355, 150, 18, 1011036, false, false ); // OKAY
			AddButton( 25, 355, 4005, 4007, 1, GumpButtonType.Reply, 1 );
			AddHtmlLocalized( 320, 355, 150, 18, 1011012, false, false ); // CANCEL
			AddButton( 285, 355, 4005, 4007, 0, GumpButtonType.Reply, 2 );

			y = 35;
			for ( int i=0;i<Categories.Length;i++ )
			{
				PolymorphCategory cat = (PolymorphCategory)Categories[i];
				AddHtmlLocalized( 5, y, 150, 25, cat.LocNumber, true, false );
				AddButton( 155, y, 4005, 4007, 0, GumpButtonType.Page, i+1 );
				y += 25;
			}

			for ( int i=0;i<Categories.Length;i++ )
			{
				PolymorphCategory cat = (PolymorphCategory)Categories[i];
				AddPage( i+1 );

				for ( int c=0;c<cat.Entries.Length;c++ )
				{
					PolymorphEntry entry = (PolymorphEntry)cat.Entries[c];
					x = 198 + (c%3)*129;
					y = 38 + (c/3)*67;

					AddHtmlLocalized( x, y, 100, 18, entry.LocNumber, false, false );
					AddItem( x+20, y+25, entry.ArtID );
					AddRadio( x, y+20, 210, 211, false, (c<<8) + i );
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID == 1 && info.Switches.Length > 0 )
			{
				int cnum = info.Switches[0];
				int cat = cnum%256;
				int ent = cnum>>8;

				if ( cat < Categories.Length )
				{
					if ( ent < Categories[cat].Entries.Length )
					{
						Spell spell = new PolymorphSpell( m_Caster, m_Scroll, Categories[cat].Entries[ent].BodyID );
						spell.Cast();
					}
				}
			}
		}
	}*/
}
