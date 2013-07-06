using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a rat corpse" )]
	public class Rat : BaseCreature
	{
		[Constructable]
		public Rat() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 238;
			Name = "a rat";
			Hue = 443;
			SetStr( 11, 19 );
			SetHits( 11, 19 );
			SetDex( 36, 45 );
			SetStam( 36, 45 );
			SetInt( 6, 10 );
			SetMana( 0 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 20;
			BaseSoundID = 204;
			SetSkill( SkillName.Tactics, 5.1, 10 );
			SetSkill( SkillName.MagicResist, 5.1, 10 );
			SetSkill( SkillName.Wrestling, 5.1, 10 );
			SetSkill( SkillName.Parry, 35.1, 45 );

			VirtualArmor = Utility.RandomMinMax( 1, 3 );
			SetDamage( 1, 3 );
		}

		public override bool AlwaysAttackable{ get{ return true; } }

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Eggs | FoodType.FruitsAndVegies; } }

		public Rat( Serial serial ) : base( serial )
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

	[CorpseName( "a sewer rat corpse" )]
	public class SewerRat : BaseCreature
	{
		[Constructable]
		public SewerRat() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 238;
			Name = "a sewer rat";
			Hue = 443;
			SetStr( 11, 19 );
			SetHits( 11, 19 );
			SetDex( 36, 45 );
			SetStam( 36, 45 );
			SetInt( 6, 10 );
			SetMana( 0 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 20;
			BaseSoundID = 204;
			SetSkill( SkillName.Tactics, 5.1, 10 );
			SetSkill( SkillName.MagicResist, 5.1, 10 );
			SetSkill( SkillName.Wrestling, 5.1, 10 );
			SetSkill( SkillName.Parry, 35.1, 45 );

			VirtualArmor = Utility.RandomMinMax( 1, 3 );
			SetDamage( 1, 3 );
		}

		public override bool AlwaysAttackable{ get{ return true; } }

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Eggs | FoodType.FruitsAndVegies; } }

		public SewerRat( Serial serial ) : base( serial )
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

