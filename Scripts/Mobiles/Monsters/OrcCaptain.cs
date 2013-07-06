using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	[TypeAlias( "Server.Mobiles.OrcishCaptain" )]
	public class OrcCaptain : BaseCreature
	{
		[Constructable]
		public OrcCaptain() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 7;
			Name = NameList.RandomName( "orc" );
			SetStr( 111, 145 );
			SetHits( 111, 145 );
			SetDex( 101, 135 );
			SetStam( 101, 135 );
			SetInt( 86, 110 );
			SetMana( 86, 110 );
			Karma = -125;

			BaseSoundID = 432;
			SetSkill( SkillName.Tactics, 85.1, 100 );
			SetSkill( SkillName.MagicResist, 70.1, 85 );
			SetSkill( SkillName.Parry, 70.1, 95 );
			SetSkill( SkillName.Magery, 60.1, 85 );
			SetSkill( SkillName.Swords, 70.1, 95 );

			VirtualArmor = 17;
			SetDamage( 2, 18 );

			LootPack.Average.Generate( this );
			PackItem( new ThighBoots() );
			if ( Utility.RandomBool() )
				PackItem( new RingmailChest() );
			if ( Utility.RandomBool() )
				PackItem( Loot.RandomGem() );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public OrcCaptain( Serial serial ) : base( serial )
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

