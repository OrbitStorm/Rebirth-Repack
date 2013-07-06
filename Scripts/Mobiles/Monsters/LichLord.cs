using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class LichLord : BaseCreature
	{
		[Constructable]
		public LichLord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 24;
			Name = "a lich lord";
			SetStr( 416, 505 );
			SetHits( 416, 505 );
			SetDex( 96, 115 );
			SetStam( 96, 115 );
			SetInt( 566, 655 );
			SetMana( 566, 655 );
			Karma = -125;

			BaseSoundID = 412;
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.MagicResist, 90.1, 100 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Magery, 90.1, 100 );
			SetSkill( SkillName.Wrestling, 60.1, 80 );

			VirtualArmor = 26;
			SetDamage( 6, 18 );

			LootPack.Rich.Generate( this );
			LootPack.Rich.Generate( this );
			PackGold( 15, 100 );
			LootPack.HighScrolls.Generate( this );
			LootPack.MedScrolls.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		//public override bool AutoDispel{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public LichLord( Serial serial ) : base( serial )
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

