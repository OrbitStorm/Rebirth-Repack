using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a harpy corpse" )]
	public class Harpy : BaseCreature
	{
		[Constructable]
		public Harpy() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 30;
			Name = "a harpy";
			SetStr( 96, 120 );
			SetHits( 96, 120 );
			SetDex( 86, 110 );
			SetStam( 86, 110 );
			SetInt( 51, 75 );
			SetMana( 51, 75 );
			Karma = -125;

			BaseSoundID = 402;
			SetSkill( SkillName.Tactics, 70.1, 100 );
			SetSkill( SkillName.MagicResist, 50.1, 65 );
			SetSkill( SkillName.Parry, 75.1, 90 );
			SetSkill( SkillName.Wrestling, 60.1, 90 );

			VirtualArmor = 14;
			SetDamage( 3, 9 );

			PackGold( 15, 50 );
			LootPack.Meager.Generate( this );
		}

		public override int Meat{ get{ return 4; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Feathers{ get{ return 50; } }

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Harpy( Serial serial ) : base( serial )
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

