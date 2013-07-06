using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a raven corpse" )]
	public class Raven : BaseCreature
	{
		[Constructable]
		public Raven() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 6;
			Name = "a raven";
			Hue = 2305;
			SetStr( 11, 17 );
			SetHits( 5, 15 );
			SetDex( 26, 35 );
			SetStam( 50, 100 );
			SetInt( 6, 10 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 19;
			BaseSoundID = 209;
			SetSkill( SkillName.Tactics, 8.6, 17 );
			SetSkill( SkillName.Wrestling, 8.6, 17 );
			SetSkill( SkillName.MagicResist, 5.1, 10 );
			SetSkill( SkillName.Parry, 10.2, 35.1 );

			VirtualArmor = 3;
			SetDamage( 1 );
		}

		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Meat{ get{ return 1; } }
		public override int Feathers{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Raven( Serial serial ) : base( serial )
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

