using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a llama corpse" )]
	public class Llama : BaseCreature
	{
		[Constructable]
		public Llama() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 220;
			Name = "a llama";
			SetStr( 21, 49 );
			SetHits( 21, 49 );
			SetDex( 36, 55 );
			SetStam( 36, 55 );
			SetInt( 16, 30 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 50;
			BaseSoundID = 181;
			SetSkill( SkillName.Tactics, 19.2, 29 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );
			SetSkill( SkillName.Parry, 35.1, 45 );
			SetSkill( SkillName.Wrestling, 19.2, 29 );

			VirtualArmor = 8;
			SetDamage( 2, 6 );
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
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public override int GenerateFurs(Corpse c)
		{
			c.DropItem( new LightFur() );
			return 1;
		}

		public Llama( Serial serial ) : base( serial )
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

