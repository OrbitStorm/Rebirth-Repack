using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an ettin corpse" )]
	public class Ettin : BaseCreature
	{
		[Constructable]
		public Ettin() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 18;
			Name = "an ettin";
			SetStr( 136, 165 );
			SetHits( 136, 165 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 31, 55 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 367;
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.MagicResist, 40.1, 55 );
			SetSkill( SkillName.Parry, 50.1, 60 );
			SetSkill( SkillName.Wrestling, 50.1, 60 );

			VirtualArmor = 19;
			SetDamage( 4, 20 );

			LootPack.Rich.Generate( this );
			PackGold( 70, 80 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Ettin( Serial serial ) : base( serial )
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

