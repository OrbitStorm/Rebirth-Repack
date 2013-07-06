using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a snake corpse" )]
	public class Snake : BaseCreature
	{
		[Constructable]
		public Snake() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 52;
			Name = "a snake";
			Hue = Utility.RandomSnakeHue();
			SetStr( 22, 34 );
			SetHits( 22, 34 );
			SetDex( 16, 25 );
			SetStam( 16, 25 );
			SetInt( 6, 10 );
			SetMana( 0 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 70;
			BaseSoundID = 219;
			SetSkill( SkillName.Tactics, 19.3, 34 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );
			SetSkill( SkillName.Parry, 15.1, 25 );
			SetSkill( SkillName.Wrestling, 19.3, 34 );

			VirtualArmor = 8;
			SetDamage( 1, 4 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override Poison HitPoison{ get{ return Poison.Lesser; } }

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Eggs; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Snake( Serial serial ) : base( serial )
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

