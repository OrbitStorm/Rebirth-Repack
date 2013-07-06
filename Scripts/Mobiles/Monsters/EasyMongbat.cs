using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a mongbat corpse" )]
	public class Mongbat : BaseCreature
	{
		[Constructable]
		public Mongbat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 39;
			Name = "a mongbat";
			SetStr( 6, 10 );
			SetHits( 4, 8 );
			SetDex( 26, 38 );
			SetStam( 40, 70 );
			SetInt( 6, 14 );
			SetMana( 0 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 5;
			BaseSoundID = 422;
			SetSkill( SkillName.Tactics, 5.1, 10 );
			SetSkill( SkillName.MagicResist, 5.1, 14 );
			SetSkill( SkillName.Parry, 25.1, 38 );
			SetSkill( SkillName.Wrestling, 5.1, 10 );

			VirtualArmor = Utility.RandomMinMax( 2, 10 );
			SetDamage( 1, 2 );

			if ( Utility.Random( 3 ) == 0 )
				PackGem();
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Mongbat( Serial serial ) : base( serial )
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

