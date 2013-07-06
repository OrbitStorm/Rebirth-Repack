using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcishLord : BaseCreature
	{
		[Constructable]
		public OrcishLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 7;
			Name = "An Orcish Lord";
			SetStr( 100, 270 );
			SetHits( 111, 145 );
			SetDex( 94, 190 );
			SetStam( 101, 135 );
			SetInt( 64, 160 );
			SetMana( 86, 110 );
			Karma = -125;

			BaseSoundID = 432;
			SetSkill( SkillName.Tactics, 75.1, 90 );
			SetSkill( SkillName.MagicResist, 70.1, 85 );
			SetSkill( SkillName.Parry, 60.1, 85 );
			SetSkill( SkillName.Magery, 70.2, 95 );
			SetSkill( SkillName.Swords, 60.1, 85 );

			VirtualArmor = 15;
			SetDamage( 2, 16 );

			if ( Utility.Random( 4 ) < 3 )
				LootPack.Rich.Generate( this );
			else
				LootPack.FilthyRich.Generate( this );
			PackItem( new ThighBoots() );
			PackItem( new OrcHelm() );
			if ( Utility.RandomBool() )
				PackItem( new RingmailChest() );
			if ( Utility.RandomBool() )
				PackItem( Loot.RandomGem() );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public OrcishLord( Serial serial ) : base( serial )
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

