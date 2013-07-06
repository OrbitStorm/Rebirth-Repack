using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class Dragon : BaseCreature
	{
		[Constructable]
		public Dragon() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.35, 0.8 )
		{
			Body = Utility.RandomList( 12,59 );
			Name = "a dragon";
			SetStr( 296, 325 );
			SetHits( 701, 1100 );
			SetDex( 86, 105 );
			SetStam( 86, 95 );
			SetInt( 136, 175 );
			SetMana( 251, 350 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 99;
			BaseSoundID = 362;
			SetSkill( SkillName.Tactics, 97.6, 100 );
			SetSkill( SkillName.MagicResist, 99.1, 100 );
			SetSkill( SkillName.Parry, 55.1, 95 );
			SetSkill( SkillName.Wrestling, 90.1, 92.5 );

			VirtualArmor = 30;
			SetDamage( 9, 29 );

			if ( Utility.Random( 4 ) < 3 )
				LootPack.Rich.Generate( this );
			else
				LootPack.FilthyRich.Generate( this );
			if ( Utility.Random( 4 ) < 3 )
				LootPack.Rich.Generate( this );
			else
				LootPack.FilthyRich.Generate( this );
			PackGold( 15, 55 );

			for(int i=0;i<3+Utility.Random( 4 );i++)
				PackGem();
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 30; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public Dragon( Serial serial ) : base( serial )
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

