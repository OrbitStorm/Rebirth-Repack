using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a goat corpse" )]
	public class Goat : BaseCreature
	{
		[Constructable]
		public Goat() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 209;
			Name = "a goat";
			SetStr( 21, 29 );
			SetDex( 37, 47 );
			SetStam( 40, 50 );
			SetInt( 7, 15 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 30;
			BaseSoundID = 153;
			SetSkill( SkillName.Tactics, 19.2, 29 );
			SetSkill( SkillName.Wrestling, 20.1, 30 );
			SetSkill( SkillName.MagicResist, 15.2, 25 );
			SetSkill( SkillName.Parry, 25.2, 37 );

			VirtualArmor = 5;
			SetDamage( 3, 5 );
		}

		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 8; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay | FoodType.FruitsAndVegies; } }

		public Goat( Serial serial ) : base( serial )
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

