using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a ghoulish corpse" )]
	public class Ghost : BaseCreature
	{
		[Constructable]
		public Ghost() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 26;
			switch ( Utility.Random( 3 ) )
			{
				case 0: Name = "a spectre"; break;
				case 1: Name = "a shade"; break;
				case 2: Name = "a ghoul"; break;
			}
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 36, 60 );
			Karma = -125;

			BaseSoundID = 382;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 35.1, 50 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 55 );

			VirtualArmor = 14;
			SetDamage( 6, 12 );

			if ( Utility.RandomBool() )
				PackGem();
			LootPack.Average.Generate( this );
			LootPack.LowScrolls.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Ghost( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[CorpseName( "a ghoulish corpse" )]
	public class Ghoul : BaseCreature
	{
		[Constructable]
		public Ghoul() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 26;
			switch ( Utility.Random( 3 ) )
			{
				case 0: Name = "a spectre"; break;
				case 1: Name = "a shade"; break;
				case 2: Name = "a ghoul"; break;
			}
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 36, 60 );
			Karma = -125;

			BaseSoundID = 382;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 35.1, 50 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 55 );

			VirtualArmor = 14;
			SetDamage( 6, 12 );

			if ( Utility.RandomBool() )
				PackGem();
			LootPack.Average.Generate( this );
			LootPack.LowScrolls.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Ghoul( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[CorpseName( "a ghoulish corpse" )]
	public class Spectre : BaseCreature
	{
		[Constructable]
		public Spectre() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 26;
			switch ( Utility.Random( 3 ) )
			{
				case 0: Name = "a spectre"; break;
				case 1: Name = "a shade"; break;
				case 2: Name = "a ghoul"; break;
			}
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 36, 60 );
			Karma = -125;

			BaseSoundID = 382;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 35.1, 50 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 55 );

			VirtualArmor = 14;
			SetDamage( 6, 12 );

			if ( Utility.RandomBool() )
				PackGem();
			LootPack.Average.Generate( this );
			LootPack.LowScrolls.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Spectre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[CorpseName( "a ghoulish corpse" )]
	public class Shade : BaseCreature
	{
		[Constructable]
		public Shade() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 26;
			switch ( Utility.Random( 3 ) )
			{
				case 0: Name = "a spectre"; break;
				case 1: Name = "a shade"; break;
				case 2: Name = "a ghoul"; break;
			}
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 36, 60 );
			Karma = -125;

			BaseSoundID = 382;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 35.1, 50 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 55 );

			VirtualArmor = 14;
			SetDamage( 6, 12 );

			if ( Utility.RandomBool() )
				PackGem();
			LootPack.Average.Generate( this );
			LootPack.LowScrolls.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Shade( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}

