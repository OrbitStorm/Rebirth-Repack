using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a mongbat corpse" )]
	public class StrongMongbat : BaseCreature
	{
		[Constructable]
		public StrongMongbat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 39;
			Name = "a mongbat";
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 61, 80 );
			SetStam( 61, 80 );
			SetInt( 26, 50 );
			SetMana( 26, 50 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 80;
			BaseSoundID = 422;
			SetSkill( SkillName.Tactics, 35.1, 50 );
			SetSkill( SkillName.MagicResist, 15.1, 30 );
			SetSkill( SkillName.Parry, 50.1, 60 );
			SetSkill( SkillName.Wrestling, 20.1, 35 );

			VirtualArmor = 10;
			SetDamage( 3, 9 );

			PackGold( 5, 25 );
			if ( Utility.RandomBool() )
				PackGem();
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public StrongMongbat( Serial serial ) : base( serial )
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

