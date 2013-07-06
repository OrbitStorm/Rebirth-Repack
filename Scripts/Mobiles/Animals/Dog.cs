using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a dog corpse" )]
	public class Dog : BaseCreature
	{
		[Constructable]
		public Dog() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 217;
			Name = "dog";
			Hue = Utility.RandomAnimalHue();
			SetStr( 27, 37 );
			SetHits( 28, 37 );
			SetDex( 28, 43 );
			SetStam( 31, 49 );
			SetInt( 29, 37 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 3;
			BaseSoundID = 133;
			SetSkill( SkillName.Tactics, 19.2, 31 );
			SetSkill( SkillName.Wrestling, 19.2, 31 );
			SetSkill( SkillName.MagicResist, 22.1, 47 );
			SetSkill( SkillName.Parry, 28.1, 53 );

			VirtualArmor = 6;
			SetDamage( 4, 7 );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }


		public Dog( Serial serial ) : base( serial )
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

