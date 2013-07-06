using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a gorilla corpse" )]
	public class Gorilla : BaseCreature
	{
		[Constructable]
		public Gorilla() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 29;
			Name = "a gorilla";
			SetStr( 53, 95 );
			SetHits( 53, 95 );
			SetDex( 36, 55 );
			SetStam( 36, 65 );
			SetInt( 36, 60 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 5;
			BaseSoundID = 158;
			SetSkill( SkillName.Wrestling, 43.3, 58 );
			SetSkill( SkillName.Tactics, 43.3, 58 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 43.1, 53 );

			VirtualArmor = 14;
			SetDamage( 2, 12 );
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Gorilla( Serial serial ) : base( serial )
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

