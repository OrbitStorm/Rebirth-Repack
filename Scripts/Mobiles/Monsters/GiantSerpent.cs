using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a giant serpent corpse" )]
	public class GiantSerpent : BaseCreature
	{
		[Constructable]
		public GiantSerpent() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 21;
			Name = "a giant serpent";
			Hue = Utility.RandomSnakeHue();
			SetStr( 186, 215 );
			SetHits( 186, 215 );
			SetDex( 56, 80 );
			SetStam( 56, 75 );
			SetInt( 66, 85 );
			SetMana( 66, 90 );

			BaseSoundID = 219;
			SetSkill( SkillName.Tactics, 65.1, 70 );
			SetSkill( SkillName.MagicResist, 25.1, 40 );
			SetSkill( SkillName.Parry, 45.1, 60 );
			SetSkill( SkillName.Wrestling, 60.1, 80 );

			VirtualArmor = 16;
			SetDamage( 5, 19 );

			if ( Utility.Random( 3 ) == 0 )
				PackItem( Utility.Random( 10 ) == 0 ? (Item)new DeadlyPoisonPotion() : (Item)new GreaterPoisonPotion() );
			PackGold( 20, 40 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

		public override int Meat{ get{ return 4; } }
		public override int Hides{ get{ return 15; } }

		public GiantSerpent( Serial serial ) : base( serial )
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

