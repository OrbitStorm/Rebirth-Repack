using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a polar bear corpse" )]
	public class PolarBear : BaseCreature
	{
		[Constructable]
		public PolarBear() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 213;
			Name = "a polar bear";
			Hue = 2301;
			SetStr( 116, 140 );
			SetHits( 116, 140 );
			SetDex( 81, 105 );
			SetStam( 81, 105 );
			SetInt( 26, 50 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 50;
			BaseSoundID = 95;
			SetSkill( SkillName.Tactics, 70.1, 100 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 70.1, 85 );
			SetSkill( SkillName.Wrestling, 60.1, 90 );

			VirtualArmor = 9;
			SetDamage( 5, 14 );
		}

		public override int Meat{ get{ return 2; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public override int GenerateFurs(Corpse c)
		{
			Item i = new LightFur();
			i.Amount = 2;
			c.DropItem( i );
			return 2;
		}

		public PolarBear( Serial serial ) : base( serial )
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

