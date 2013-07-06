using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a cat corpse" )]
	public class Cat : BaseCreature
	{
		[Constructable]
		public Cat() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 201;
			Name = "a cat";
			Hue = Utility.RandomAnimalHue();
			SetStr( 10, 18 );
			SetHits( 11, 17 );
			SetDex( 26, 45 );
			SetStam( 40, 70 );
			SetInt( 6, 30 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 20;
			BaseSoundID = 105;
			SetSkill( SkillName.Tactics, 9.1, 18 );
			SetSkill( SkillName.Wrestling, 9.1, 19 );
			SetSkill( SkillName.MagicResist, 15.1, 30 );
			SetSkill( SkillName.Parry, 22.6, 45 );

			VirtualArmor = 4;
			SetDamage( 1 );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Feline; } }


		public Cat( Serial serial ) : base( serial )
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

