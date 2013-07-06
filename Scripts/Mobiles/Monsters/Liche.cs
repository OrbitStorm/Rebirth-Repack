using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a lich's corpse" )]
	public class Lich : BaseCreature
	{
		[Constructable]
		public Lich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 24;
			Name = "a lich";
			SetStr( 106, 135 );
			SetHits( 106, 135 );
			SetDex( 66, 85 );
			SetStam( 66, 85 );
			SetInt( 176, 205 );
			SetMana( 276, 305 );
			Karma = -125;

			BaseSoundID = 412;
			SetSkill( SkillName.Tactics, 70.1, 90 );
			SetSkill( SkillName.MagicResist, 70.1, 90 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Magery, 70.1, 80 );

			VirtualArmor = 25;
			SetDamage( 15, 25 );

			if ( Utility.RandomBool() )
				PackGem();
			if ( Utility.Random( 4 ) == 0 )
				LootPack.Average.Generate( this );
			else
				LootPack.Rich.Generate( this );
			PackGold( 15, 55 );
			LootPack.MedScrolls.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Lich( Serial serial ) : base( serial )
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

