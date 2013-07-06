using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a deer corpse" )]
	public class Hind : BaseCreature
	{
		[Constructable]
		public Hind() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 237;
			Name = "a hind";
			SetStr( 21, 51 );
			SetHits( 31, 49 );
			SetDex( 47, 77 );
			SetStam( 41, 53 );
			SetInt( 17, 47 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 40;
			SetSkill( SkillName.Tactics, 19.2, 31 );
			SetSkill( SkillName.Wrestling, 26.2, 38 );
			SetSkill( SkillName.MagicResist, 15.2, 27 );
			SetSkill( SkillName.Parry, 22.7, 34.5 );

			VirtualArmor = 8;
			SetDamage( 4, 11 );
		}

		public override int GetAttackSound()
		{
			return 130;
		}

		public override int GetHurtSound()
		{
			return 131;
		}

		public override int GetDeathSound()
		{
			return 132;
		}

		public override int Meat{ get{ return 5; } }
		public override int Hides{ get{ return 8; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Hind( Serial serial ) : base( serial )
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

