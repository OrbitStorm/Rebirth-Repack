using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a ratman corpse" )]
	public class Ratman : BaseCreature
	{
		[Constructable]
		public Ratman() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = Utility.RandomList( 42,44,45 );
			Name = NameList.RandomName( "ratman" );
			SetStr( 96, 120 );
			SetHits( 96, 115 );
			SetDex( 81, 100 );
			SetStam( 81, 100 );
			SetInt( 36, 60 );
			SetMana( 36, 60 );
			Karma = -125;

			BaseSoundID = 437;
			SetSkill( SkillName.Tactics, 50.1, 75 );
			SetSkill( SkillName.MagicResist, 35.1, 60 );
			SetSkill( SkillName.Parry, 50.1, 70 );
			SetSkill( SkillName.Wrestling, 50.1, 75 );

			VirtualArmor = 14;
			SetDamage( 3, 6 );

			PackGold( 15, 50 );
			LootPack.Meager.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Ratman; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public Ratman( Serial serial ) : base( serial )
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

