using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class HousePlacementEntry
	{
		private Type m_Type;
		private int m_Description;
		private int m_Storage;
		private int m_Cost;
		private int m_MultiID;
		private Point3D m_Offset;

		public Type Type{ get{ return m_Type; } }

		public int Description{ get{ return m_Description; } }
		public int Storage{ get{ return m_Storage; } }
		public int Cost{ get{ return m_Cost; } }

		public int MultiID{ get{ return m_MultiID; } }
		public Point3D Offset{ get{ return m_Offset; } }

		public HousePlacementEntry( Type type, int description, int storage, int lockdowns, int cost, int xOffset, int yOffset, int zOffset, int multiID )
		{
			m_Type = type;
			m_Description = description;
			m_Storage = storage;
			m_Cost = cost;

			m_Offset = new Point3D( xOffset, yOffset, zOffset );

			m_MultiID = multiID;
		}

		public BaseHouse ConstructHouse( Mobile from )
		{
			try
			{
				object[] args;
				if ( m_Type == typeof( SmallOldHouse ) || m_Type == typeof( SmallShop ) || m_Type == typeof( TwoStoryHouse ) )
					args = new object[2]{ from, m_MultiID };
				else
					args = new object[1]{ from };

				return Activator.CreateInstance( m_Type, args ) as BaseHouse;
			}
			catch
			{
			}

			return null;
		}

		public bool OnPlacement( Mobile from, Point3D p )
		{
			if ( !from.CheckAlive() )
				return false;

			ArrayList toMove;
			Point3D center = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );
			HousePlacementResult res = HousePlacement.Check( from, m_MultiID, center, out toMove );

			switch ( res )
			{
				case HousePlacementResult.Valid:
				{
					from.SendLocalizedMessage( 1011576 ); // This is a valid location.
					BaseHouse house = ConstructHouse( from );

					if ( house == null )
						return false;

					house.Price = m_Cost;

					if ( Banker.Withdraw( from, m_Cost ) )
					{
						from.SendLocalizedMessage( 1060398, m_Cost.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
					}
					else
					{
						house.RemoveKeys( from );
						house.Delete();
						from.SendLocalizedMessage( 1060646 ); // You do not have the funds available in your bank box to purchase this house.  Try placing a smaller house, or adding gold or checks to your bank box.
						return false;
					}

					house.MoveToWorld( center, from.Map );

					for ( int i = 0; i < toMove.Count; ++i )
					{
						object o = toMove[i];

						if ( o is Mobile )
							((Mobile)o).Location = house.BanLocation;
						else if ( o is Item )
							((Item)o).Location = house.BanLocation;
					}
								

					return true;
				}
				case HousePlacementResult.BadItem:
				case HousePlacementResult.BadLand:
				case HousePlacementResult.BadStatic:
				case HousePlacementResult.BadRegionHidden:
				case HousePlacementResult.NoSurface:
				{
					from.SendLocalizedMessage( 1043287 ); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
					break;
				}
				case HousePlacementResult.BadRegion:
				{
					from.SendLocalizedMessage( 501265 ); // Housing cannot be created in this area.
					break;
				}
			}

			return false;
		}

		private static Hashtable m_Table;

		static HousePlacementEntry()
		{
			m_Table = new Hashtable();

			FillTable( m_ClassicHouses );
			FillTable( m_TwoStoryFoundations );
			FillTable( m_ThreeStoryFoundations );
		}

		public static HousePlacementEntry Find( BaseHouse house )
		{
			object obj = m_Table[house.GetType()];

			if ( obj is HousePlacementEntry )
			{
				return ((HousePlacementEntry)obj);
			}
			else if ( obj is ArrayList )
			{
				ArrayList list = (ArrayList)obj;

				for ( int i = 0; i < list.Count; ++i )
				{
					HousePlacementEntry e = (HousePlacementEntry)list[i];

					if ( e.m_MultiID == (house.ItemID & 0x3FFF) )
						return e;
				}
			}
			else if ( obj is Hashtable )
			{
				Hashtable table = (Hashtable)obj;

				obj = table[house.ItemID & 0x3FFF];

				if ( obj is HousePlacementEntry )
					return (HousePlacementEntry)obj;
			}

			return null;
		}

		private static void FillTable( HousePlacementEntry[] entries )
		{
			for ( int i = 0; i < entries.Length; ++i )
			{
				HousePlacementEntry e = (HousePlacementEntry)entries[i];

				object obj = m_Table[e.m_Type];

				if ( obj == null )
				{
					m_Table[e.m_Type] = e;
				}
				else if ( obj is HousePlacementEntry )
				{
					ArrayList list = new ArrayList();

					list.Add( obj );
					list.Add( e );

					m_Table[e.m_Type] = list;
				}
				else if ( obj is ArrayList )
				{
					ArrayList list = (ArrayList)obj;

					if ( list.Count == 8 )
					{
						Hashtable table = new Hashtable();

						for ( int j = 0; j < list.Count; ++j )
							table[((HousePlacementEntry)list[j]).m_MultiID] = list[j];

						table[e.m_MultiID] = e;

						m_Table[e.m_Type] = table;
					}
					else
					{
						list.Add( e );
					}
				}
				else if ( obj is Hashtable )
				{
					((Hashtable)obj)[e.m_MultiID] = e;
				}
			}
		}
		
		private static HousePlacementEntry[] m_ClassicHouses = new HousePlacementEntry[]
			{
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011303,	425,	212,	37000,		0,	4,	0,	0x0064	),
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011304,	425,	212,	37000,		0,	4,	0,	0x0066	),
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011305,	425,	212,	36750,		0,	4,	0,	0x0068	),
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011306,	425,	212,	35250,		0,	4,	0,	0x006A	),
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011307,	425,	212,	36750,		0,	4,	0,	0x006C	),
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011308,	425,	212,	36750,		0,	4,	0,	0x006E	),
				new HousePlacementEntry( typeof( SmallShop ),			1011321,	425,	212,	50500,	   -1,	4,	0,	0x00A0	),
				new HousePlacementEntry( typeof( SmallShop ),			1011322,	425,	212,	52500,		0,	4,	0,	0x00A2	),
				new HousePlacementEntry( typeof( SmallTower ),			1011317,	580,	290,	73500,		3,	4,	0,	0x0098	),
				new HousePlacementEntry( typeof( TwoStoryVilla ),		1011319,	1100,	550,	113750,		3,	6,	0,	0x009E	),
				new HousePlacementEntry( typeof( SandStonePatio ),		1011320,	850,	425,	76500,	   -1,	4,	0,	0x009C	),
				new HousePlacementEntry( typeof( LogCabin ),			1011318,	1100,	550,	81750,		1,	6,	0,	0x009A	),
				new HousePlacementEntry( typeof( GuildHouse ),			1011309,	1370,	685,	131500,	   -1,	7,	0,	0x0074	),
				new HousePlacementEntry( typeof( TwoStoryHouse ),		1011310,	1370,	685,	162750,	   -3,	7,	0,	0x0076	),
				new HousePlacementEntry( typeof( TwoStoryHouse ),		1011311,	1370,	685,	162000,	   -3,	7,	0,	0x0078	),
				new HousePlacementEntry( typeof( LargePatioHouse ),		1011315,	1370,	685,	129250,	   -4,	7,	0,	0x008C	),
				new HousePlacementEntry( typeof( LargeMarbleHouse ),	1011316,	1370,	685,	160500,	   -4,	7,	0,	0x0096	),
				new HousePlacementEntry( typeof( Tower ),				1011312,	2119,	1059,	366500,		0,	7,	0,	0x007A	),
				new HousePlacementEntry( typeof( Keep ),				1011313,	2625,	1312,	572750,		0, 11,	0,	0x007C	),
				new HousePlacementEntry( typeof( Castle ),				1011314,	4076,	2038,	865250,		0, 16,	0,	0x007E	)
			};

		public static HousePlacementEntry[] ClassicHouses{ get{ return m_ClassicHouses; } }

		private static HousePlacementEntry[] m_TwoStoryFoundations = new HousePlacementEntry[]{};
		public static HousePlacementEntry[] TwoStoryFoundations{ get{ return m_TwoStoryFoundations; } }

		private static HousePlacementEntry[] m_ThreeStoryFoundations = new HousePlacementEntry[]{};
		public static HousePlacementEntry[] ThreeStoryFoundations{ get{ return m_ThreeStoryFoundations; } }
	}
}