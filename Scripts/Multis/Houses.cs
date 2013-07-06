using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class Tent : BaseHouse
	{
		public class TentChest : WoodenChest
		{
			private Tent m_Tent;

			public TentChest( Tent owner )
			{
				m_Tent = owner;
				KeyValue = m_Tent.KeyValue;
				Locked = true;
				LockLevel = MaxLockLevel = 0;
				Movable = false;
			}

			public TentChest( Serial serial ) : base( serial )
			{
			}

			public override void OnDoubleClick( Mobile from )
			{
				if ( m_Tent != null && m_Tent.IsOwner( from ) && from.InRange( this, 5 ) && from.AccessLevel == AccessLevel.Player )
				{
					m_Tent.Visits++;
					if ( m_Tent.Sign != null )
						m_Tent.Sign.RefreshHouse( from );
				}

				base.OnDoubleClick( from );
			}

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );

				if ( m_Tent != null && m_Tent.Sign != null )
					BaseItem.LabelTo( this, from, true, m_Tent.Sign.GetDecayString() );
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int)0 );
				writer.Write( m_Tent );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();
				m_Tent = reader.ReadItem() as Tent;

				if ( m_Tent == null )
					Delete();
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete ();
				if ( m_Tent != null && !m_Tent.Deleted )
					m_Tent.Delete();
			}
		}

		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D(-3,-3,7,7) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override int DefaultPrice{ get{ return 43800; } }
		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[0]; } }
		public override KeyType KeyType { get { return KeyType.Iron; } }

		public Tent( Mobile owner, int id ) : base( id, owner, 1, 0 )
		{
			BanLocation = new Point3D( 0, 5, 0 );
			SetSign( 0, 0, -4 );
			if ( this.Sign != null )
				this.Sign.Visible = false;
			CreateKeys( owner );
			AddAddon( new TentChest( this ), 0, -2, 0 );
		}

		public Tent( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed()
		{
			if ( (ItemID&0x3FFF) == 0x70 || (ItemID&0x3FFF) == 0x71 )
				return new BlueTentDeed();
			else
				return new GreenTentDeed();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallOldHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D(-3,-3,7,7 ), new Rectangle2D( -1, 4, 3, 1 ) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override int DefaultPrice{ get{ return 43800; } }
		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[0]; } }

		public SmallOldHouse( Mobile owner, int id ) : base( id, owner, 125, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( 0, 3, 7, keyValue );

			SetSign( 2, 4, 5 );

			BanLocation = new Point3D( 2, 4, 0 );
		}

		public SmallOldHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			switch ( ItemID )
			{
				case 0x64: return new StonePlasterHouseDeed();
				case 0x66: return new FieldStoneHouseDeed();
				case 0x68: return new SmallBrickHouseDeed();
				case 0x6A: return new WoodHouseDeed();
				case 0x6C: return new WoodPlasterHouseDeed(); 
				case 0x6E: 
				default: return new ThatchedRoofCottageDeed();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallForgeHouse : SmallOldHouse
	{
		public SmallForgeHouse( Mobile owner, int id ) : base( owner, id )
		{
			AddAddon( new Forge(), 0, 0, 7 );
			Anvil a = new Anvil();
			a.ItemID = 0xFB0;
			AddAddon( a, 1, 0, 7 );
		}

		public SmallForgeHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			return new SmallForgeHouseDeed();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallTrainingHouse : SmallOldHouse
	{
		public SmallTrainingHouse( Mobile owner, int id ) : base( owner, id )
		{
			AddAddon( new TrainingDummy(), -2,  1, 7 );
			AddAddon( new TrainingDummy( 0x1070 ),  1, -2, 7 );
		}

		public SmallTrainingHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			return new SmallTrainingHouseDeed();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallPickpocketHouse : SmallOldHouse
	{
		public SmallPickpocketHouse( Mobile owner, int id ) : base( owner, id )
		{
			AddAddon( new PickpocketDip( 0x1EC3 ), -2,  1, 7 );
			AddAddon( new PickpocketDip( 0x1EC0 ),  1, -2, 7 );
		}

		public SmallPickpocketHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			return new SmallPickpocketHouseDeed();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallTailorHouse : SmallOldHouse
	{
		public SmallTailorHouse( Mobile owner, int id ) : base( owner, id )
		{
			AddAddon( new SpinningwheelEastAddon(), -1, -2, 7 );
			AddAddon( new LoomSouthAddon(), 1, -2, 7 );
		}

		public SmallTailorHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			return new SmallTailorHouseDeed();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallBakeryHouse : SmallOldHouse
	{
		public SmallBakeryHouse( Mobile owner, int id ) : base( owner, id )
		{
			AddAddon( new FlourMillSouthAddon(), -2, -2, 7 );
			AddAddon( new StoneFireplaceSouthAddon(), 0, -2, 7 );
		}

		public SmallBakeryHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			return new SmallBakeryHouseDeed();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class GuildHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 14, 14 ), new Rectangle2D( -2, 7, 4, 1 ) };

		public override int DefaultPrice{ get{ return 144500; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[20]; } }
		public override int ConvertOffsetX{ get{ return -1; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public GuildHouse( Mobile owner ) : base( 0x74, owner, 225, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( -1, 6, 7, keyValue );

			SetSign( 4, 8, 16 );

			BanLocation = new Point3D( 4, 8, 0 );

			AddSouthDoor( -3, -1, 7 );
			AddSouthDoor(  3, -1, 7 );
		}

		public GuildHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new BrickHouseDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class TwoStoryHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, 0, 14, 7 ), new Rectangle2D( -7, -7, 9, 7 ), new Rectangle2D( -4, 7, 4, 1 ) };

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public override int DefaultPrice{ get{ return 192400; } }

		public TwoStoryHouse( Mobile owner, int id ) : base( id, owner, 325, 0 )
		{
			uint keyValue = CreateKeys( owner );
			
			SetSign( 2, 8, 16 );
			BanLocation = new Point3D( 2, 8, 0 );

			//AddSouthDoors( -3, 6, 7, keyValue );

			BaseDoor westDoor = MakeDoor( true, DoorFacing.WestCW );
			westDoor.Locked = true;
			westDoor.KeyValue = keyValue;
			AddDoor( westDoor, -3, 6, 7 );

			BaseDoor eastDoor = MakeDoor( true, DoorFacing.EastCCW );
			eastDoor.Locked = true;
			eastDoor.KeyValue = keyValue;
			AddDoor( eastDoor, -2, 6, 7 );

			westDoor.Link = eastDoor;
			eastDoor.Link = westDoor;

			AddSouthDoor( -3, 0, 7, keyValue );
			AddSouthDoor( id == 0x76 ? -2 : -3, 0, 27, keyValue );
		}

		public TwoStoryHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{ 
			switch( ItemID )
			{
				case 0x76: return new TwoStoryWoodPlasterHouseDeed();
				case 0x78:
				default: return new TwoStoryStonePlasterHouseDeed();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Tower : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 16, 14 ), new Rectangle2D( -1, 7, 4, 2 ), new Rectangle2D( -11, 0, 4, 7 ), new Rectangle2D( 9, 0, 4, 7 ) };

		public override int DefaultPrice{ get{ return 433200; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[37]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public Tower( Mobile owner ) : base( 0x7A, owner, 450, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, 0, 6, 6, keyValue );

			SetSign( 5, 8, 16 );

			BanLocation = new Point3D( 5, 8, 0 );

			AddSouthDoor( false, 3, -2, 6 );
			AddEastDoor( false, 1, 4, 26 );
			AddEastDoor( false, 1, 4, 46 );
		}

		public Tower( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new TowerDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Keep : BaseHouse//warning: ODD shape!
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -11, -11, 7, 8 ), new Rectangle2D( -11, 5, 7, 8 ), new Rectangle2D( 6, -11, 7, 8 ), new Rectangle2D( 6, 5, 7, 8 ), new Rectangle2D( -9, -3, 5, 8 ), new Rectangle2D( 6, -3, 5, 8 ), new Rectangle2D( -4, -9, 10, 20 ), new Rectangle2D( -1, 11, 4, 1 ) };

		public override int DefaultPrice{ get{ return 665200; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public Keep( Mobile owner ) : base( 0x7C, owner, 650, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, 0, 10, 6, keyValue );
			
			SetSign( 5, 12, 16 );

			BanLocation = new Point3D( 5, 13, 0 );
		}

		public Keep( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new KeepDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Castle : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{  new Rectangle2D( -15, -15, 31, 31 ), new Rectangle2D( -1, 16, 4, 1 ) };

		public override int DefaultPrice{ get{ return 1022800; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public Castle( Mobile owner ) : base( 0x7E, owner, 750, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, 0, 15, 6, keyValue );
			
			SetSign( 5, 17, 16 );

			BanLocation = new Point3D( 5, 17, 0 );

			AddSouthDoors( false, 0, 11, 6, true );
			AddSouthDoors( false, 0, 5, 6, false );
			AddSouthDoors( false, -1, -11, 6, false );
		}

		public Castle( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new CastleDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class LargePatioHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 15, 14 ), new Rectangle2D( -5, 7, 4, 1 ) };

		public override int DefaultPrice{ get{ return 152800; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[29]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public LargePatioHouse( Mobile owner ) : base( 0x8C, owner, 325, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( -4, 6, 7, keyValue );
			
			SetSign( 1, 8, 16 );

			BanLocation = new Point3D( 1, 8, 0 );

			AddEastDoor( 1, 4, 7, keyValue );
			AddEastDoor( 1, -4, 7, keyValue );
			AddSouthDoor( 4, -1, 7, keyValue );
		}

		public LargePatioHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new LargePatioDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class LargeForgeHouse : LargePatioHouse
	{
		public override HouseDeed GetDeed()
		{
			return new LargeForgeHouseDeed();
		}

		public LargeForgeHouse( Mobile owner ) : base( owner )
		{
			Item i;

			i = new Forge();
			i.ItemID = 0x197A;
			AddAddon( i, 3, 3, 7 );
			
			i = new Forge();
			i.ItemID = 0x197E;
			AddAddon( i, 4, 3, 7 );

			i = new Forge();
			i.ItemID = 0x1982;
			AddAddon( i, 5, 3, 7 );

			i = new Anvil();
			i.ItemID = 0xFB0;
			AddAddon( i, 4, 1, 7 );
		}

		public LargeForgeHouse( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class LargeMarbleHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 15, 14 ), new Rectangle2D( -6, 7, 6, 1 ) };

		public override int DefaultPrice{ get{ return 192000; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[29]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public LargeMarbleHouse( Mobile owner ) : base( 0x96, owner, 325, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, -4, 3, 4, keyValue );

			SetSign( 1, 8, 11 );

			BanLocation = new Point3D( 1, 8, 0 );
		}

		public LargeMarbleHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new LargeMarbleDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallTower : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -3, -3, 8, 7 ), new Rectangle2D( 2, 4, 3, 1 ) };

		public override int DefaultPrice{ get{ return 88500; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[6]; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public SmallTower( Mobile owner ) : base( 0x98, owner, 175, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( false, 3, 3, 6, keyValue );

			SetSign( 1, 4, 5 );

			BanLocation = new Point3D( 1, 4, 0 );
		}

		public SmallTower( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new SmallTowerDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class LogCabin : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -3, -6, 8, 13 ) };

		public override int DefaultPrice{ get{ return 97800; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[12]; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public LogCabin( Mobile owner ) : base( 0x9A, owner, 425, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( 1, 4, 8, keyValue );
			
			SetSign( 5, 8, 20 );

			BanLocation = new Point3D( 5, 8, 0 );

			AddSouthDoor( 1, 0, 29 );
		}

		public LogCabin( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new LogCabinDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SandStonePatio : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -5, -4, 12, 8 ), new Rectangle2D( -2, 4, 3, 1 ) };

		public override int DefaultPrice{ get{ return 90900; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[35]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public SandStonePatio( Mobile owner ) : base( 0x9C, owner, 400, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( -1, 3, 6, keyValue );
			
			SetSign( 4, 6, 24 );

			BanLocation = new Point3D( 4, 6, 0 );
		}

		public SandStonePatio( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new SandstonePatioDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class TwoStoryVilla : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -5, -5, 11, 11 ), new Rectangle2D( 2, 6, 4, 1 ) };

		public override int DefaultPrice{ get{ return 136500; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[31]; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }

		public TwoStoryVilla( Mobile owner ) : base( 0x9E, owner, 425, 0 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( 3, 1, 5, keyValue );
			
			SetSign( 3, 8, 24 );

			BanLocation = new Point3D( 3, 8, 0 );

			AddEastDoor( 1, 0, 25 );
			AddSouthDoor( -3, -1, 25 );
		}

		public TwoStoryVilla( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new VillaDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SmallShop : BaseHouse
	{
		public override Rectangle2D[] Area{ get{ return ( ItemID == 0xA2 ? AreaArray1 : AreaArray2 ); } }

		public override int DefaultPrice{ get{ return 63000; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[0]; } }

		public static Rectangle2D[] AreaArray1 = new Rectangle2D[]{ new Rectangle2D(-3,-3,7,7), new Rectangle2D( -1, 4, 4, 1 ) };
		public static Rectangle2D[] AreaArray2 = new Rectangle2D[]{ new Rectangle2D(-3,-3,7,7), new Rectangle2D( -2, 4, 3, 1 ) };

		public SmallShop( Mobile owner, int id ) : base( id, owner, 100, 0 )
		{
			uint keyValue = CreateKeys( owner );

			BaseDoor door = MakeDoor( false, DoorFacing.EastCW );

			door.Locked = true;
			door.KeyValue = keyValue;

			if ( door is BaseHouseDoor )
				((BaseHouseDoor)door).Facing = DoorFacing.EastCCW;

			AddDoor( door, -2, 0, id == 0xA2 ? 24 : 27 );

			//AddSouthDoor( false, -2, 0, 27 - (id == 0xA2 ? 3 : 0), keyValue );
			
			SetSign( 3, 4, 7 - (id == 0xA2 ? 2 : 0) );
			BanLocation = new Point3D( 3, 4, 0 );
		}

		public SmallShop( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{ 
			switch ( ItemID )
			{
				case 0xA0: return new StoneWorkshopDeed(); 
				case 0xA2:
				default: return new MarbleWorkshopDeed();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
