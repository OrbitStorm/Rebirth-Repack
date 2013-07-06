using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a hare corpse" )]
	public class Rabbit : BaseCreature
	{
		[Constructable]
		public Rabbit() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 205;
			Name = "a rabbit";
			SetStr( 6, 10 );
			SetHits( 4, 8 );
			SetDex( 26, 38 );
			SetStam( 40, 70 );
			SetInt( 6, 14 );
			SetMana( 0 );

			if ( Utility.RandomBool() )
				Hue = Utility.RandomAnimalHue();

			Tamable = true;
			MinTameSkill = 5;
			SetSkill( SkillName.Tactics, 5.1, 10 );
			SetSkill( SkillName.Wrestling, 5.1, 10 );
			SetSkill( SkillName.MagicResist, 5.1, 14 );
			SetSkill( SkillName.Parry, 25.1, 38 );

			VirtualArmor = Utility.RandomMinMax( 2, 7 );
			SetDamage( 1, 2 );
		}

		public override int GetAngerSound()
		{
			return -1;
		}

		public override int GetIdleSound()
		{
			return -1;
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies; } }

		public override int GenerateFurs(Corpse c)
		{
			c.DropItem( new DarkFur() );
			return 1;
		}

		public Rabbit( Serial serial ) : base( serial )
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

