using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a magpie corpse" )]
	public class Magpie : BaseCreature
	{
		[Constructable]
		public Magpie() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 6;
			Name = "a magpie";
			Hue = 2305;
			SetStr( 11, 17 );
			SetHits( 5, 15 );
			SetDex( 26, 35 );
			SetStam( 50, 100 );
			SetInt( 6, 10 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 15;
			BaseSoundID = 148;
			SetSkill( SkillName.Tactics, 9.2, 17 );
			SetSkill( SkillName.MagicResist, 5.1, 10 );
			SetSkill( SkillName.Parry, 25.1, 35 );
			SetSkill( SkillName.Wrestling, 9.2, 17 );

			VirtualArmor = 1;
			SetDamage( 1 );
		}

		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Meat{ get{ return 1; } }
		public override int Feathers{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Magpie( Serial serial ) : base( serial )
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

