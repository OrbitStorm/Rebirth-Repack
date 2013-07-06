using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a drake corpse" )]
	public class Drake : BaseCreature
	{
		[Constructable]
		public Drake() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = Utility.RandomList( 60,61 );
			Name = "a drake";
			SetStr( 201, 230 );
			SetHits( 501, 900 );
			SetDex( 133, 152 );
			SetStam( 43, 62 );
			SetInt( 101, 140 );
			SetMana( 86, 205 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 100;
			BaseSoundID = 362;
			SetSkill( SkillName.Tactics, 65.1, 90 );
			SetSkill( SkillName.MagicResist, 65.1, 80 );
			SetSkill( SkillName.Parry, 65.1, 80 );
			SetSkill( SkillName.Wrestling, 65.1, 80 );

			VirtualArmor = 28;
			SetDamage( 4, 24 );

			if ( Utility.Random( 4 ) < 3 )
				LootPack.Average.Generate( this );
			else
				LootPack.Rich.Generate( this );
			LootPack.Rich.Generate( this );

			for(int i=0;i<1+Utility.Random( 4 );i++)
				PackGem();
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 20; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Drake( Serial serial ) : base( serial )
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

