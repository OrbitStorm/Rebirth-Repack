using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;


namespace Server.Mobiles
{
	public class ForestSpawner1 : Spawner
	{
		[Constructable]
		public ForestSpawner1()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromMinutes( 45 );

			Count = 25;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "Hind" );
			CreaturesName.Add( "GreatHart" );
			CreaturesName.Add( "Boar" );
			CreaturesName.Add( "BlackBear" );
			CreaturesName.Add( "Cow" );
			CreaturesName.Add( "Chicken" );
			CreaturesName.Add( "DireWolf" );
			CreaturesName.Add( "Cat" );
			CreaturesName.Add( "Dog" );
			CreaturesName.Add( "JackRabbit" );
			CreaturesName.Add( "Sheep" );
			CreaturesName.Add( "Snake" );
			CreaturesName.Add( "GiantRat" );
		}

		#region Serialization
		public ForestSpawner1( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class ForestSpawner2 : Spawner
	{
		[Constructable]
		public ForestSpawner2()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromMinutes( 45 );

			Count = 25;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "TimberWolf" );
			CreaturesName.Add( "Goat" );
			CreaturesName.Add( "BrownBear" );
			CreaturesName.Add( "Bull" );
			CreaturesName.Add( "HellHound" );
			CreaturesName.Add( "Cat" );
			CreaturesName.Add( "Goat" );
			CreaturesName.Add( "Rabbit" );
			CreaturesName.Add( "Sheep" );
			CreaturesName.Add( "GiantRat" );
			CreaturesName.Add( "Pig" );
			CreaturesName.Add( "Llama" );
			CreaturesName.Add( "Mongbat" );
		}

		#region Serialization
		public ForestSpawner2( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class ForestSpawner3 : Spawner
	{
		[Constructable]
		public ForestSpawner3()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromMinutes( 45 );

			Count = 25;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "Cougar" );
			CreaturesName.Add( "Snake" );
			CreaturesName.Add( "TimberWolf" );
			CreaturesName.Add( "JackRabbit" );
			CreaturesName.Add( "StrongMongbat" );
			CreaturesName.Add( "MountainGoat" );
			CreaturesName.Add( "Goat" );
			CreaturesName.Add( "GreyWolf" );
			CreaturesName.Add( "GrizzlyBear" );
			CreaturesName.Add( "GiantRat" );
			CreaturesName.Add( "MountainGoat" );
			CreaturesName.Add( "Wisp" );
			CreaturesName.Add( "Gypsy" );
		}

		#region Serialization
		public ForestSpawner3( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class ForestBaddieSpawn : Spawner
	{
		[Constructable]
		public ForestBaddieSpawn()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromMinutes( 45 );

			Count = 10;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "Orc" );
			CreaturesName.Add( "Ettin" );
			CreaturesName.Add( "Troll" );
			CreaturesName.Add( "Mongbat" );
			CreaturesName.Add( "StrongMongbat" );
			CreaturesName.Add( "AirElemental" );
			CreaturesName.Add( "Ratman" );
			CreaturesName.Add( "Slime" );
			CreaturesName.Add( "Reaper" );
			CreaturesName.Add( "Ogre" );
			CreaturesName.Add( "Corpser" );
			CreaturesName.Add( "Harpy" );
		}

		#region Serialization
		public ForestBaddieSpawn( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class JungleSpawner1 : Spawner
	{
		[Constructable]
		public JungleSpawner1()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromMinutes( 45 );

			Count = 25;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "Alligator" );
			CreaturesName.Add( "Alligator" );
			CreaturesName.Add( "Snake" );
			CreaturesName.Add( "Snake" );
			CreaturesName.Add( "TropicalBird" );
			CreaturesName.Add( "TropicalBird" );
			CreaturesName.Add( "Panther" );
			CreaturesName.Add( "Gorilla" );
			CreaturesName.Add( "Boar" );
			CreaturesName.Add( "SilverSerpent" );
			CreaturesName.Add( "StrongMongbat" );
			CreaturesName.Add( "GiantRat" );
			CreaturesName.Add( "SewerRat" );
		}

		#region Serialization
		public JungleSpawner1( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class IceSpawner1 : Spawner
	{
		[Constructable]
		public IceSpawner1()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromMinutes( 45 );

			Count = 25;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "WhiteWolf" );
			CreaturesName.Add( "WhiteWolf" );
			CreaturesName.Add( "SnowLeopard" );
			CreaturesName.Add( "PolarBear" );
			CreaturesName.Add( "PolarBear" );
			CreaturesName.Add( "Walrus" );
			CreaturesName.Add( "MountainGoat" );
			CreaturesName.Add( "GrizzlyBear" );
			CreaturesName.Add( "BlackBear" );
			CreaturesName.Add( "TimberWolf" );
			CreaturesName.Add( "Cougar" );
			CreaturesName.Add( "StrongMongbat" );
			CreaturesName.Add( "SnowLeopard" );
		}

		#region Serialization
		public IceSpawner1( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class BirdSpawner : Spawner
	{
		[Constructable]
		public BirdSpawner()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromMinutes( 30 );

			Count = 20;

			CreaturesName.Capacity = 5;
			CreaturesName.Add( "Eagle" );
			CreaturesName.Add( "Magpie" );
			CreaturesName.Add( "Bird" );
			CreaturesName.Add( "Raven" );
			CreaturesName.Add( "Crow" );
		}

		#region Serialization
		public BirdSpawner( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class TownSpawner1 : Spawner
	{
		[Constructable]
		public TownSpawner1()
		{
			SpawnRange = 40;
			HomeRange = 15;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 10 );
			MaxDelay = TimeSpan.FromMinutes( 30 );

			Count = 25;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "Cat" );
			CreaturesName.Add( "Dog" );
			CreaturesName.Add( "SewerRat" );
			CreaturesName.Add( "Magpie" );
			CreaturesName.Add( "Raven" );
			CreaturesName.Add( "Bird" );
			CreaturesName.Add( "Crow" );
			CreaturesName.Add( "Crow" );
			CreaturesName.Add( "Crow" );
			CreaturesName.Add( "HireMage" );
			CreaturesName.Add( "HireFighter" );
			CreaturesName.Add( "HireThief" );
			CreaturesName.Add( "Beggar" );
		}

		#region Serialization
		public TownSpawner1( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class OrcGroup : Spawner
	{
		[Constructable]
		public OrcGroup()
		{
			SpawnRange = 50;
			HomeRange = 20;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 10 );
			MaxDelay = TimeSpan.FromMinutes( 60 );

			Count = 10;

			CreaturesName.Capacity = 13;
			CreaturesName.Add( "Orc" );
			CreaturesName.Add( "Orc" );
			CreaturesName.Add( "Orc" );
			CreaturesName.Add( "Orc" );
			CreaturesName.Add( "StrongMongbat" );
			CreaturesName.Add( "OrcCaptain" );
			CreaturesName.Add( "OrcCaptain" );
			CreaturesName.Add( "OrcishMage" );
			CreaturesName.Add( "OrcishLord" );
			CreaturesName.Add( "Ettin" );
			CreaturesName.Add( "Troll" );
			CreaturesName.Add( "Ogre" );
			CreaturesName.Add( "OgreLord" );
		}

		#region Serialization
		public OrcGroup( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class SeaSpawner1 : Spawner
	{
		[Constructable]
		public SeaSpawner1()
		{
			SpawnRange = 250;
			HomeRange = 25;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 10 );
			MaxDelay = TimeSpan.FromMinutes( 30 );

			Count = 20;

			CreaturesName.Add( "SeaSerpent" );
			CreaturesName.Add( "Dolphin" );
		}

		private static bool HasWaterAt( Map map, IPoint2D p )
		{
			LandTile landTile = map.Tiles.GetLandTile( p.X, p.Y );
			StaticTile[] tiles = map.Tiles.GetStaticTiles( p.X, p.Y, true );

			bool hasWater = false;

			if ( landTile.Z == -5 && ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)) )
				hasWater = true;
			int landZ = 0, landAvg = 0, landTop = 0;

			map.GetAverageZ( p.X, p.Y, ref landZ, ref landAvg, ref landTop );

			//if ( !landTile.Ignored && top > landZ && landTop > z )
			//	return false;

			for ( int i = 0; i < tiles.Length; ++i )
			{
				StaticTile tile = tiles[i];
				bool isWater = ( tile.ID >= 0x5796 && tile.ID <= 0x57B2 );

				if ( tile.Z == -5 && isWater )
					hasWater = true;
				else if ( tile.Z >= -5 && !isWater )
					return false;
			}

			return hasWater;
		}

		public override Point3D GetSpawnPosition()
		{
			Map map = Map;

			if ( map == null )
				return Location;

			for ( int i = 0; i < 25; i++ )
			{
				int range = SpawnRange <= 0 ? HomeRange : SpawnRange;
				Point2D point = new Point2D( Location.X + (Utility.Random( (range * 2) + 1 ) - range), Location.Y + (Utility.Random( (range * 2) + 1 ) - range) );

				if ( HasWaterAt( this.Map, point ) )
					return new Point3D( point, -5 );
			}

			return this.Location;
		}

		#region Serialization
		public SeaSpawner1( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class LizardmanLair : Spawner
	{
		[Constructable]
		public LizardmanLair()
		{
			HomeRange = 15;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 5;

			CreaturesName.Add( "Lizardman" );
		}

		#region Serialization
		public LizardmanLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class SpiderLair : Spawner
	{
		[Constructable]
		public SpiderLair()
		{
			HomeRange = 10;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 5;

			CreaturesName.Add( "GiantSpider" );
			CreaturesName.Add( "GiantSpider" );
			CreaturesName.Add( "GiantSpider" );
			CreaturesName.Add( "Scorpion" );
			CreaturesName.Add( "Snake" );
		}

		#region Serialization
		public SpiderLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class ZombieLair : Spawner
	{
		[Constructable]
		public ZombieLair()
		{
			HomeRange = 10;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 5;

			CreaturesName.Add( "Zombie" );
			CreaturesName.Add( "Zombie" );
			CreaturesName.Add( "Zombie" );
			CreaturesName.Add( "Ghoul" );
			CreaturesName.Add( "HeadlessOne" );
		}

		#region Serialization
		public ZombieLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class DaemonLair : Spawner
	{
		[Constructable]
		public DaemonLair()
		{
			HomeRange = 10;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 4;

			CreaturesName.Add( "Daemon" );
			CreaturesName.Add( "Daemon" );
			CreaturesName.Add( "Daemon" );
			CreaturesName.Add( "Daemon" );
			CreaturesName.Add( "Daemon" );
			CreaturesName.Add( "Balron" );
		}

		#region Serialization
		public DaemonLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class GazerLair : Spawner
	{
		[Constructable]
		public GazerLair()
		{
			HomeRange = 10;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 4;

			CreaturesName.Add( "Gazer" );
			CreaturesName.Add( "Gazer" );
			CreaturesName.Add( "Gazer" );
			CreaturesName.Add( "ElderGazer" );
		}

		#region Serialization
		public GazerLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class LichLair : Spawner
	{
		[Constructable]
		public LichLair()
		{
			HomeRange = 10;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 4;

			CreaturesName.Add( "Lich" );
			CreaturesName.Add( "Lich" );
			CreaturesName.Add( "Lich" );
			CreaturesName.Add( "SkeletalKnight" );
			CreaturesName.Add( "BoneMagi" );
			CreaturesName.Add( "LichLord" );
		}

		#region Serialization
		public LichLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class RatmanLair : Spawner
	{
		[Constructable]
		public RatmanLair()
		{
			HomeRange = 10;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 6;

			CreaturesName.Add( "Ratman" );
			CreaturesName.Add( "Ratman" );
			CreaturesName.Add( "Ratman" );
			CreaturesName.Add( "Ratman" );
			CreaturesName.Add( "StrongMongbat" );
			CreaturesName.Add( "Slime" );
			CreaturesName.Add( "Snake" );
		}

		#region Serialization
		public RatmanLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class UndeadLair : Spawner
	{
		[Constructable]
		public UndeadLair()
		{
			HomeRange = 10;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 8;

			CreaturesName.Add( "HeadlessOne" );
			CreaturesName.Add( "HeadlessOne" );
			CreaturesName.Add( "Zombie" );
			CreaturesName.Add( "Zombie" );
			CreaturesName.Add( "Ghoul" );
			CreaturesName.Add( "Ghoul" );
			CreaturesName.Add( "Skeleton" );
			CreaturesName.Add( "Skeleton" );
			CreaturesName.Add( "SkeletalKnight" );
			CreaturesName.Add( "BoneMagi" );
			CreaturesName.Add( "Lich" );
		}

		#region Serialization
		public UndeadLair( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class EscortSpawner : Spawner
	{
		[Constructable]
		public EscortSpawner()
		{
			HomeRange = 15;
			SpawnRange = 100;
			RelativeHomeRange = true;

			Count = 10;

			MinDelay = TimeSpan.FromMinutes( 30 );
			MaxDelay = TimeSpan.FromHours( 5 );

			CreaturesName.Add( "EscortableMage" );
			CreaturesName.Add( "BrideGroom" );
			CreaturesName.Add( "Noble" );
			CreaturesName.Add( "Peasant" );
			CreaturesName.Add( "Messenger" );
			CreaturesName.Add( "SeekerOfAdventure" );
		}

		#region Serialization
		public EscortSpawner( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}

	public class DungeonMisc : Spawner
	{
		[Constructable]
		public DungeonMisc()
		{
			HomeRange = 15;
			SpawnRange = 50;
			RelativeHomeRange = true;
			MinDelay = TimeSpan.FromMinutes( 15 );
			MaxDelay = TimeSpan.FromHours( 1.5 );

			Count = 15;

			CreaturesName.Add( "Slime" );
			CreaturesName.Add( "StrongMongbat" );
			CreaturesName.Add( "Mongbat" );
			CreaturesName.Add( "HeadlessOne" );
			CreaturesName.Add( "Snake" );
		}

		#region Serialization
		public DungeonMisc( Serial s ) : base(s) { }
		public override void Serialize( GenericWriter w ) { base.Serialize( w ); w.Write((int)0); }
		public override void Deserialize( GenericReader r ) { base.Deserialize( r ); r.ReadInt(); }
		#endregion
	}
}
