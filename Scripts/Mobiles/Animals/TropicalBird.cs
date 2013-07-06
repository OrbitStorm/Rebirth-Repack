using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a bird corpse" )]
	public class TropicalBird : BaseCreature
	{
		[Constructable]
		public TropicalBird() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 6;
			Name = "a tropical bird";
			Hue = Utility.RandomBirdHue();
			SetStr( 1, 4 );
			SetHits( 3, 6 );
			SetDex( 26, 35 );
			SetStam( 50, 100 );
			SetInt( 1, 4 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 10;
			BaseSoundID = 191;
			SetSkill( SkillName.Tactics, 5.1, 10 );
			SetSkill( SkillName.Wrestling, 5.1, 10 );
			SetSkill( SkillName.MagicResist, 5.1, 10 );
			SetSkill( SkillName.Parry, 15.1, 25 );

			VirtualArmor = 1;
			SetDamage( 1 );
		}

		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Meat{ get{ return 1; } }
		public override int Feathers{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public TropicalBird( Serial serial ) : base( serial )
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

