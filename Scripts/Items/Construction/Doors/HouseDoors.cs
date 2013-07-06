using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Gumps;

namespace Server.Items
{
	public class MetalHouseDoor : BaseHouseDoor
	{
		[Constructable]
		public MetalHouseDoor( DoorFacing facing ) : base( facing, 0x675 + (2 * (int)facing), 0x676 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset( facing ) )
		{
		}

		public MetalHouseDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer ) // Default Serialize method
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader ) // Default Deserialize method
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DarkWoodHouseDoor : BaseHouseDoor
	{
		[Constructable]
		public DarkWoodHouseDoor( DoorFacing facing ) : base( facing, 0x6A5 + (2 * (int)facing), 0x6A6 + (2 * (int)facing), 0xEA, 0xF1, BaseDoor.GetOffset( facing ) )
		{
		}

		public DarkWoodHouseDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer ) // Default Serialize method
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader ) // Default Deserialize method
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GenericHouseDoor : BaseHouseDoor
	{
		[Constructable]
		public GenericHouseDoor( DoorFacing facing, int baseItemID, int openedSound, int closedSound ) : base( facing, baseItemID + (2 * (int)facing), baseItemID + 1 + (2 * (int)facing), openedSound, closedSound, BaseDoor.GetOffset( facing ) )
		{
		}

		public GenericHouseDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer ) // Default Serialize method
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader ) // Default Deserialize method
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public abstract class BaseHouseDoor : BaseDoor
	{
		private DoorFacing m_Facing;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public DoorFacing Facing
		{
			get{ return m_Facing; }
			set{ m_Facing = value; }
		}

		public BaseHouseDoor( DoorFacing facing, int closedID, int openedID, int openedSound, int closedSound, Point3D offset ) : base( closedID, openedID, openedSound, closedSound, offset )
		{
			m_Facing = facing;
		}

		public BaseHouse FindHouse()
		{
			Point3D loc;

			if ( Open )
				loc = new Point3D( X - Offset.X, Y - Offset.Y, Z - Offset.Z );
			else
				loc = this.Location;

			BaseHouse house = BaseHouse.FindHouseAt( loc, Map, 20 );
			if ( house == null || ( this.KeyValue != 0 && house.KeyValue != this.KeyValue ) )
			{
				Regions.HouseRegion hr = Region.Find( loc, this.Map ) as Regions.HouseRegion;
				if ( hr != null && hr.House != null && ( this.KeyValue == 0 || hr.House.KeyValue == this.KeyValue ) && hr.House.IsInside( loc, 20 ) )
					house = hr.House;
				else
					house = null;
			}
			return house;
		}

		public override void OnOpened( Mobile from )
		{
			BaseHouse house = FindHouse();

			if ( house == null )
			{
				house = BaseHouse.FindHouseAt( from );
				if ( house != null && this.KeyValue != 0 && house.KeyValue != this.KeyValue )
					house = null;
			}

			if( house != null )
			{
				house.Visits++;
				if ( house.Sign != null && house.IsOwner( from ) && from.AccessLevel == AccessLevel.Player )
					house.Sign.RefreshHouse( from );
			}
			else
			{
				from.SendAsciiMessage( "There seems to be some problem with this house door.  It's house could not be found.  Contact a Game Master." );
			}
		}

		public override bool UseLocks()
		{
			BaseHouse house = FindHouse();

			return ( house == null || !house.IsAosRules );
		}

		public BaseHouseDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Facing );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Facing = (DoorFacing)reader.ReadInt();
					break;
				}
			}
		}

		public override bool IsInside( Mobile from )
		{
			int x,y,w,h;

			const int r = 2;
			const int bs = r*2+1;
			const int ss = r+1;

			switch ( m_Facing )
			{
				case DoorFacing.WestCW:
				case DoorFacing.EastCCW: x = -r; y = -r; w = bs; h = ss; break;

				case DoorFacing.EastCW: 
				case DoorFacing.WestCCW: x = -r; y = 0; w = bs; h = ss; break;

				case DoorFacing.SouthCW:
				case DoorFacing.NorthCCW: x = -r; y = -r; w = ss; h = bs; break;

				case DoorFacing.NorthCW:
				case DoorFacing.SouthCCW: x = 0; y = -r; w = ss; h = bs; break;

				default: return false;
			}

			int rx = from.X - X;
			int ry = from.Y - Y;
			int az = Math.Abs( from.Z - Z );

			return ( rx >= x && rx < (x+w) && ry >= y && ry < (y+h) && az <= 4 );
		}
	}
}
