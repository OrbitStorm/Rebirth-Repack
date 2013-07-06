using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a troll corpse" )]
	public class Troll : BaseCreature
	{
		[Constructable]
		public Troll() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = Utility.RandomList( 54,53 );
			Name = "a troll";
			SetStr( 176, 205 );
			SetHits( 176, 205 );
			SetDex( 46, 65 );
			SetStam( 46, 65 );
			SetInt( 46, 70 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 461;
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 45.1, 60 );
			SetSkill( SkillName.Wrestling, 50.1, 70 );

			VirtualArmor = 20;
			SetDamage( 5, 17 );

			LootPack.Rich.Generate( this );
			PackGold( 40, 80 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Troll( Serial serial ) : base( serial )
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

