using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a giant spider corpse" )]
	public class GiantSpider : BaseCreature
	{
		[Constructable]
		public GiantSpider() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 28;
			Name = "a giant spider";
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 36, 60 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 70;
			BaseSoundID = 387;
			SetSkill( SkillName.Tactics, 35.1, 50 );
			SetSkill( SkillName.Wrestling, 50.1, 65 );
			SetSkill( SkillName.MagicResist, 25.1, 40 );
			SetSkill( SkillName.Parry, 35.1, 45 );

			VirtualArmor = 8;
			SetDamage( 3, 15 );

			Item item = null;
			item = new SpidersSilk( Utility.RandomMinMax( 1, 3 ) );
			PackItem( item );

			PackGold( 5, 25 );
			if ( Utility.RandomBool() )
				PackGem();
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }


		public override bool AlwaysMurderer{ get{ return true; } }

		public GiantSpider( Serial serial ) : base( serial )
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

