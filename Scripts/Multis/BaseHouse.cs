using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis.Deeds;
using Server.Regions;
using Server.Network;
using Server.Targeting;
using Server.Accounting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;

namespace Server.Multis
{
	public abstract class BaseHouse : BaseMulti
	{
		private bool m_Public;
		private HouseRegion m_Region;
		private HouseSign m_Sign;
		private TrashBarrel m_Trash;
		private ArrayList m_Doors;
		private ArrayList m_Addons;
		private Mobile m_Owner;
		private int m_Price;
		private int m_Visits;
		private DateTime m_BuiltOn, m_LastTraded;
		private uint m_KeyValue;

		public virtual bool IsAosRules{ get{ return Core.AOS; } }

		public static BaseHouse FindHouseAt( Mobile m )
		{
			if ( m == null || m.Deleted )
				return null;

			return FindHouseAt( m.Location, m.Map, 16 );
		}

		public static BaseHouse FindHouseAt( Item item )
		{
			if ( item == null || item.Deleted )
				return null;

			return FindHouseAt( item.GetWorldLocation(), item.Map, item.ItemData.Height );
		}

		public static BaseHouse FindHouseAt( Point3D loc, Map map, int height )
		{
			if ( map == null || map == Map.Internal )
				return null;

			Sector sector = map.GetSector( loc );

			for ( int i = 0; i < sector.Multis.Count; ++i )
			{
				BaseHouse house = sector.Multis[i] as BaseHouse;

				if ( house != null && house.IsInside( loc, height ) )
					return house;
			}

			return null;
		}

		public bool IsInside( Mobile m )
		{
			if ( m == null || m.Deleted || m.Map != this.Map )
				return false;

			return IsInside( m.Location, 16 );
		}

		public bool IsInside( Item item )
		{
			if ( item == null || item.Deleted || item.Map != this.Map )
				return false;

			return IsInside( item.Location, item.ItemData.Height );
		}

		public virtual bool IsInside( Point3D p, int height )
		{
			if ( Deleted )
				return false;

			MultiComponentList mcl = Components;

			int x = p.X - (X + mcl.Min.X);
			int y = p.Y - (Y + mcl.Min.Y);

			if ( x < 0 || x >= mcl.Width || y < 0 || y >= mcl.Height )
				return false;

			StaticTile[] tiles = mcl.Tiles[x][y];

			for ( int j = 0; j < tiles.Length; ++j )
			{
				StaticTile tile = tiles[j];
				int id = tile.ID & 0x3FFF;
				ItemData data = TileData.ItemTable[id];

				// Slanted roofs do not count; they overhang blocking south and east sides of the multi
				if ( (data.Flags & TileFlag.Roof) != 0 )
					continue;

				// Signs and signposts are not considered part of the multi
				if ( (id >= 0xB95 && id <= 0xC0E) || (id >= 0xC43 && id <= 0xC44) )
					continue;

				int tileZ = tile.Z + this.Z;

				if ( p.Z == tileZ || (p.Z + height) > tileZ )
					return true;
			}

			return false;
		}

		public BaseHouse( int multiID, Mobile owner, int MaxLockDown, int MaxSecure ) : base( multiID )
		{
			m_BuiltOn = DateTime.Now;
			m_LastTraded = DateTime.MinValue;

			m_Doors = new ArrayList();
			m_Addons = new ArrayList();
			m_Region = new HouseRegion( this );

			m_Owner = owner;

			UpdateRegionArea();

			Movable = false;
		}

		public BaseHouse( Serial serial ) : base( serial )
		{
		}

		public override void OnMapChange()
		{
			//m_Region.Map = this.Map;

			if ( m_Sign != null && !m_Sign.Deleted )
				m_Sign.Map = this.Map;

			if ( m_Trash != null && !m_Trash.Deleted )
				m_Trash.Map = this.Map;

			if ( m_Doors != null )
			{
				for(int i=0;i<m_Doors.Count;i++)
					((Item)m_Doors[i]).Map = this.Map;
			}

			if ( m_Addons != null )
			{
				for(int i=0;i<m_Addons.Count;i++)
					((Item)m_Addons[i]).Map = this.Map;
			}
		}

		public virtual void ChangeSignType( int itemID )
		{
			if ( m_Sign != null )
				m_Sign.ItemID = itemID;
		}

		public abstract Rectangle2D[] Area{ get; }

        public virtual void UpdateRegionArea()
        {
            if (m_Region != null)
                m_Region.Unregister();

            if (this.Map != null)
            {
                m_Region = new HouseRegion(this);
                m_Region.Register();
            }
            else
            {
                m_Region = null;
            }
        }

		public override void OnLocationChange( Point3D oldLocation )
		{
			int x = base.Location.X - oldLocation.X;
			int y = base.Location.Y - oldLocation.Y;
			int z = base.Location.Z - oldLocation.Z;

			if ( m_Sign != null && !m_Sign.Deleted )
				m_Sign.Location = new Point3D( m_Sign.X + x, m_Sign.Y + y, m_Sign.Z + z );

			if ( m_Trash != null && !m_Trash.Deleted )
				m_Trash.Location = new Point3D( m_Trash.X + x, m_Trash.Y + y, m_Trash.Z + z );

			UpdateRegionArea();

			m_Region.GoLocation = new Point3D( m_Region.GoLocation.X + x, m_Region.GoLocation.Y + y, m_Region.GoLocation.Z + z );

			if ( m_Doors != null )
			{
				for(int i=0;i<m_Doors.Count;i++)
				{
					Item item = (Item)m_Doors[i];
					if ( item != null && !item.Deleted )
						item.Location = new Point3D( item.X + x, item.Y + y, item.Z + z );
				}
			}

			if ( m_Addons != null )
			{
				for(int i=0;i<m_Addons.Count;i++)
				{
					Item item = (Item)m_Addons[i];
					if ( item != null && !item.Deleted )
						item.Location = new Point3D( item.X + x, item.Y + y, item.Z + z );
				}
			}
		}

		public BaseDoor AddEastDoor( int x, int y, int z )
		{
			return AddEastDoor( true, x, y, z );
		}

		public BaseDoor AddEastDoor( bool wood, int x, int y, int z )
		{
			BaseDoor door = MakeDoor( wood, DoorFacing.SouthCW );

			AddDoor( door, x, y, z );

			return door;
		}

		public BaseDoor AddSouthDoor( int x, int y, int z )
		{
			return AddSouthDoor( true, x, y, z );
		}

		public BaseDoor AddSouthDoor( bool wood, int x, int y, int z )
		{
			BaseDoor door = MakeDoor( wood, DoorFacing.WestCW );

			AddDoor( door, x, y, z );

			return door;
		}

		public BaseDoor AddEastDoor( int x, int y, int z, uint k )
		{
			return AddEastDoor( true, x, y, z, k );
		}

		public BaseDoor AddEastDoor( bool wood, int x, int y, int z, uint k )
		{
			BaseDoor door = MakeDoor( wood, DoorFacing.SouthCW );

			door.Locked = true;
			door.KeyValue = k;

			AddDoor( door, x, y, z );

			return door;
		}

		public BaseDoor AddSouthDoor( int x, int y, int z, uint k )
		{
			return AddSouthDoor( true, x, y, z, k );
		}

		public BaseDoor AddSouthDoor( bool wood, int x, int y, int z, uint k )
		{
			BaseDoor door = MakeDoor( wood, DoorFacing.WestCW );

			door.Locked = true;
			door.KeyValue = k;

			AddDoor( door, x, y, z );

			return door;
		}

		public BaseDoor[] AddSouthDoors( int x, int y, int z, uint k )
		{
			return AddSouthDoors( true, x, y, z, k );
		}

		public BaseDoor[] AddSouthDoors( bool wood, int x, int y, int z, uint k )
		{
			BaseDoor westDoor = MakeDoor( wood, DoorFacing.WestCW );
			BaseDoor eastDoor = MakeDoor( wood, DoorFacing.EastCCW );

			westDoor.Locked = true;
			eastDoor.Locked = true;

			westDoor.KeyValue = k;
			eastDoor.KeyValue = k;

			westDoor.Link = eastDoor;
			eastDoor.Link = westDoor;

			AddDoor( westDoor, x, y, z );
			AddDoor( eastDoor, x + 1, y, z );

			return new BaseDoor[2]{ westDoor, eastDoor };
		}

		public uint KeyValue
		{
			get { return m_KeyValue; }
			set { m_KeyValue = value; }
		}

		public virtual KeyType KeyType { get { return KeyType.Gold; } }
		public uint CreateKeys( Mobile m )
		{
			m_KeyValue = Key.RandomValue();

			if ( !IsAosRules )
			{
				m.BankBox.DropItem( new Key( this.KeyType, m_KeyValue ) );
				m.AddToBackpack( new Key( this.KeyType, m_KeyValue ) );
			}

			return m_KeyValue;
		}

		public BaseDoor[] AddSouthDoors( int x, int y, int z )
		{
			return AddSouthDoors( true, x, y, z, false );
		}

		public BaseDoor[] AddSouthDoors( bool wood, int x, int y, int z, bool inv )
		{
			BaseDoor westDoor = MakeDoor( wood, inv ? DoorFacing.WestCCW : DoorFacing.WestCW );
			BaseDoor eastDoor = MakeDoor( wood, inv ? DoorFacing.EastCW : DoorFacing.EastCCW );

			westDoor.Link = eastDoor;
			eastDoor.Link = westDoor;

			AddDoor( westDoor, x, y, z );
			AddDoor( eastDoor, x + 1, y, z );

			return new BaseDoor[2]{ westDoor, eastDoor };
		}

		public BaseDoor MakeDoor( bool wood, DoorFacing facing )
		{
			if ( wood )
				return new DarkWoodHouseDoor( facing );
			else
				return new MetalHouseDoor( facing );
		}

		public void AddDoor( BaseDoor door, int xoff, int yoff, int zoff )
		{
			door.MoveToWorld( new Point3D( xoff+this.X, yoff+this.Y, zoff+this.Z ), this.Map );
			m_Doors.Add( door );
		}

		public void AddTrashBarrel( Mobile from )
		{
			for ( int i = 0; m_Doors != null && i < m_Doors.Count; ++i )
			{
				BaseDoor door = m_Doors[i] as BaseDoor;
				Point3D p = door.Location;

				if ( door.Open )
					p = new Point3D( p.X - door.Offset.X, p.Y - door.Offset.Y, p.Z - door.Offset.Z );

				if ( (from.Z + 16) >= p.Z && (p.Z + 16) >= from.Z )
				{
					if ( from.InRange( p, 1 ) )
					{
						from.SendLocalizedMessage( 502120 ); // You cannot place a trash barrel near a door or near steps.
						return;
					}
				}
			}

			if ( m_Trash == null || m_Trash.Deleted )
			{
				m_Trash = new TrashBarrel();

				m_Trash.Movable = false;
				m_Trash.MoveToWorld( from.Location, from.Map );

				from.SendLocalizedMessage( 502121 ); /* You have a new trash barrel.
													  * Three minutes after you put something in the barrel, the trash will be emptied.
													  * Be forewarned, this is permanent! */
			}
			else
			{
				m_Trash.MoveToWorld( from.Location, from.Map );
			}
		}

		public void SetSign( int xoff, int yoff, int zoff )
		{
			m_Sign = new HouseSign( this );
			m_Sign.MoveToWorld( new Point3D( this.X + xoff, this.Y + yoff, this.Z + zoff ), this.Map );
		}

		public override bool Decays
		{
			get
			{
				return false;
			}
		}

		public PlayerVendor FindPlayerVendor()
		{
			Region r = m_Region;

			if ( r == null )
				return null;

			List<Mobile> list = r.GetMobiles();

			for ( int i = 0; i < list.Count; ++i )
			{
				PlayerVendor pv = list[i] as PlayerVendor;

				if ( pv != null && Contains( pv ) )
					return pv;
			}

			return null;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int)m_KeyValue );
			writer.Write( (int) m_Visits );
			writer.Write( (int) m_Price );

			writer.Write( m_BuiltOn );
			writer.Write( m_LastTraded );

			writer.Write( m_Public );

			writer.Write( BanLocation );

			writer.Write( m_Owner );

			writer.Write( m_Sign );
			writer.Write( m_Trash );

			writer.WriteItemList( m_Doors, true );
			writer.WriteItemList( m_Addons, true );

			if ( m_Sign != null && !m_Sign.Deleted && m_Sign.HouseDecayDate < DateTime.Now )
				Timer.DelayCall( TimeSpan.FromSeconds( Utility.Random( 60 ) + 60 ), new TimerCallback( m_Sign.CheckDecay ) );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Region = new HouseRegion( this );

			switch ( version )
			{
				case 0:
				{
					m_KeyValue = reader.ReadUInt();
					m_Visits = reader.ReadInt();
					m_Price = reader.ReadInt();
					m_BuiltOn = reader.ReadDateTime();
					m_LastTraded = reader.ReadDateTime();
					m_Public = reader.ReadBool();
					m_Region.GoLocation = reader.ReadPoint3D();
					if ( version < 8 )
						m_Price = DefaultPrice;

					m_Owner = reader.ReadMobile();

					UpdateRegionArea();

					Region.AddRegion( m_Region );

					m_Sign = reader.ReadItem() as HouseSign;
					m_Trash = reader.ReadItem() as TrashBarrel;

					m_Doors = reader.ReadItemList();
					m_Addons = reader.ReadItemList();
					
					if ( (Map == null || Map == Map.Internal) && Location == Point3D.Zero )
						Delete();
					break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get
			{
				return m_Owner;
			}
			set
			{
				m_Owner = value;

				if ( m_Sign != null )
					m_Sign.InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Visits
		{
			get{ return m_Visits; }
			set{ m_Visits = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D BanLocation
		{
			get
			{
				return m_Region.GoLocation;
			}
			set
			{
				m_Region.GoLocation = new Point3D( m_Region.GoLocation.X + value.X, m_Region.GoLocation.Y + value.Y, m_Region.GoLocation.Z + value.Z );
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D RelativeBanLocation
        {
            get
            {
                if (m_Region == null)
                    return Point3D.Zero;
                return new Point3D(this.Location.X - m_Region.GoLocation.X,
                    this.Location.Y - m_Region.GoLocation.Y,
                    this.Location.Z - m_Region.GoLocation.Z);
            }
            set
            {
                if (m_Region != null)
                    m_Region.GoLocation = new Point3D(this.X + value.X, this.Y + value.Y, this.Z + value.Z);
            }
        }

		public Region Region{ get{ return m_Region; } }
		public ArrayList Doors{ get{ return m_Doors; } }
		public ArrayList Addons{ get{ return m_Addons; } }
		public HouseSign Sign{ get{ return m_Sign; } set{ m_Sign = value; } }

		public DateTime BuiltOn
		{
			get{ return m_BuiltOn; }
			set{ m_BuiltOn = value; }
		}

		public DateTime LastTraded
		{
			get{ return m_LastTraded; }
			set{ m_LastTraded = value; }
		}

		public override void OnDelete()
		{
			new FixColumnTimer( this ).Start();

			base.OnDelete();
		}

		private class FixColumnTimer : Timer
		{
			private Map m_Map;
			private int m_StartX, m_StartY, m_EndX, m_EndY;

			public FixColumnTimer( BaseMulti multi ) : base( TimeSpan.Zero )
			{
				m_Map = multi.Map;

				MultiComponentList mcl = multi.Components;

				m_StartX = multi.X + mcl.Min.X;
				m_StartY = multi.Y + mcl.Min.Y;
				m_EndX = multi.X + mcl.Max.X;
				m_EndY = multi.Y + mcl.Max.Y;
			}

			protected override void OnTick()
			{
				if ( m_Map == null )
					return;

				for ( int x = m_StartX; x <= m_EndX; ++x )
					for ( int y = m_StartY; y <= m_EndY; ++y )
						m_Map.FixColumn( x, y );
			}
		}

		public void OnDecayed()
		{
			Map map = this.Map;

			if ( map == null )
				return;

			MultiComponentList mcl = Components;
			IPooledEnumerable eable = map.GetObjectsInBounds( new Rectangle2D( X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height ) );

			ArrayList del = new ArrayList();
			foreach ( object o in eable )
			{
				if ( o is Guildstone )
				{
					if ( Contains( (Item)o ) )
						del.Add( o );
				}
				else if ( o is StrongBox )
				{
					del.Add( o );
				}
				else if ( o is PlayerVendor )
				{
					if ( Contains( (Mobile)o ) )
						del.Add( o );
				}
				else if ( o is BaseHouse )
				{
					((BaseHouse)o).Z = this.Z;
				}
			}
			eable.Free();

			for(int i=0;i<del.Count;i++)
			{
				object o = del[i];
				if ( o is PlayerVendor )
				{
					PlayerVendor v = (PlayerVendor)o;
					v.Say( 503235 ); // I regret nothing!postal
					v.Blessed = false;
					v.Kill();
					v.Delete();
				}
				else if ( o is Item )
				{
					((Item)o).Delete();
				}
			}
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

            if (m_Region != null)
            {
                m_Region.Unregister();
                m_Region = null;
            }

			if ( m_Sign != null )
				m_Sign.Delete();

			if ( m_Trash != null )
				m_Trash.Delete();

			if ( m_Doors != null )
			{
				for ( int i = 0; i < m_Doors.Count; ++i )
				{
					Item item = (Item)m_Doors[i];

					if ( item != null && !item.Deleted )
						item.Delete();
				}

				m_Doors.Clear();
			}

			if ( m_Addons != null )
			{
				for(int i=0;i<m_Addons.Count;i++)
				{
					Item addon = (Item)m_Addons[i];
					if ( addon is Tent.TentChest && !addon.Deleted && addon.Map != Map.Internal && addon.Location != Point3D.Zero )
					{
						for(int j=0;j<addon.Items.Count;j++)
							((Item)addon.Items[j]).MoveToWorld( addon.Location, addon.Map );
					}

					if ( addon != null && !addon.Deleted )
						addon.Delete();
				}

				m_Addons.Clear();
			}
		}

		public void AddAddon( Item item, int xoff, int yoff, int zoff )
		{
			item.MoveToWorld( new Point3D( xoff+this.X, yoff+this.Y, zoff+this.Z ), this.Map );
			m_Addons.Add( item );
		}

		public void RemoveAddon( Item item )
		{
			m_Addons.Remove( item );
		}

		public bool IsOwner( Mobile m )
		{
			if ( m == null )
				return false; 

			if ( m.AccessLevel >= AccessLevel.GameMaster ) // || m == m_Owner
				return true;

			if ( m.Backpack != null )
			{
				Item[] keys = m.Backpack.FindItemsByType( typeof( Key ), true );
				for(int i=0;i<keys.Length;i++)
				{
					if ( ((Key)keys[i]).KeyValue == m_KeyValue )
						return true;
				}
			}
			return false;
		}

		public void RemoveKeys( Mobile m )
		{
			if ( m_Doors != null )
			{
				uint keyValue = 0;

				for ( int i = 0; keyValue == 0 && i < m_Doors.Count; ++i )
				{
					BaseDoor door = m_Doors[i] as BaseDoor;

					if ( door != null )
						keyValue = door.KeyValue;
				}

				Key.RemoveKeys( m, keyValue );
			}
		}

		public void ChangeLocks( Mobile m )
		{
			uint keyValue = CreateKeys( m );

			if ( m_Doors != null )
			{
				for ( int i = 0; i < m_Doors.Count; ++i )
				{
					BaseDoor door = m_Doors[i] as BaseDoor;

					if ( door != null )
						door.KeyValue = keyValue;
				}
			}
		}

		public void RemoveLocks()
		{
			if ( m_Doors != null )
			{
				for (int i=0;i<m_Doors.Count;++i)
				{
					BaseDoor door = m_Doors[i] as BaseDoor;

					if ( door != null )
					{
						door.KeyValue = 0;
						door.Locked = false;
					}
				}
			}
		}

		public virtual HousePlacementEntry ConvertEntry{ get{ return null; } }
		public virtual int ConvertOffsetX{ get{ return 0; } }
		public virtual int ConvertOffsetY{ get{ return 0; } }
		public virtual int ConvertOffsetZ{ get{ return 0; } }

		public virtual int DefaultPrice{ get{ return 0; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Price{ get{ return m_Price; } set{ m_Price = value; } }

		public virtual HouseDeed GetDeed()
		{
			return null;
		}

		public virtual Guildstone FindGuildstone()
		{
			Map map = this.Map;

			if ( map == null )
				return null;

			MultiComponentList mcl = Components;
			IPooledEnumerable eable = map.GetItemsInBounds( new Rectangle2D( X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height ) );

			foreach ( Item item in eable )
			{
				if ( item is Guildstone && Contains( item ) )
				{
					eable.Free();
					return (Guildstone)item;
				}
			}

			eable.Free();
			return null;
		}
	}
}

namespace Server.Prompts
{
	public class HouseRenamePrompt : Prompt
	{
		private BaseHouse m_House;

		public HouseRenamePrompt( BaseHouse house )
		{
			m_House = house;
		}

		public override void OnCancel(Mobile from)
		{
			from.SendAsciiMessage( "House rename request canceled." );
		}

		public override void OnResponse( Mobile from, string text )
		{
			if ( m_House.IsOwner( from ) )
			{
				if ( m_House.Sign != null )
					m_House.Sign.Name = text;

				from.SendAsciiMessage( "Sign changed to : {0}", text );
			}
			else
			{
				from.SendAsciiMessage( "You do not own this house." );
			}
		}
	}
}

