using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a cougar corpse" )]
	public class Cougar : BaseCreature
	{
		[Constructable]
		public Cougar() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 214;
			Name = "a cougar";
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 66, 85 );
			SetStam( 66, 85 );
			SetInt( 26, 50 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 55;
			BaseSoundID = 115;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.Wrestling, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 15.1, 30 );
			SetSkill( SkillName.Parry, 55.1, 65 );

			VirtualArmor = 8;
			SetDamage( 2, 12 );
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Feline; } }

		public Cougar( Serial serial ) : base( serial )
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

