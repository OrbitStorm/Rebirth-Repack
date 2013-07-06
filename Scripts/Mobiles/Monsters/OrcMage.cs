using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a glowing corpse" )]
	[TypeAlias( "Server.Mobiles.OrcishMage" )]
	public class OrcishMage : BaseCreature
	{
		[Constructable]
		public OrcishMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 17;
			Name = "An Orcish mage";
			Hue = Utility.RandomGreenHue();
			SetStr( 96, 130 );
			SetHits( 111, 145 );
			SetDex( 91, 115 );
			SetStam( 101, 135 );
			SetInt( 61, 85 );
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

			LootPack.Rich.Generate( this );
			if ( Utility.RandomBool() )
				LootPack.Average.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public OrcishMage( Serial serial ) : base( serial )
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

