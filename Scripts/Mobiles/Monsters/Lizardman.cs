using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a lizardman corpse" )]
	public class Lizardman : BaseCreature
	{
		[Constructable]
		public Lizardman() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = Utility.RandomList( 33,35,36 );
			Name = NameList.RandomName( "lizardman" );
			SetStr( 96, 120 );
			SetHits( 86, 110 );
			SetDex( 86, 105 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 417;
			SetSkill( SkillName.Tactics, 55.1, 80 );
			SetSkill( SkillName.MagicResist, 35.1, 60 );
			SetSkill( SkillName.Parry, 55.1, 75 );
			SetSkill( SkillName.Wrestling, 50.1, 70 );

			VirtualArmor = 14;
			SetDamage( 3, 9 );

			PackGold( 15, 75 );
			if ( BodyValue == 35 )
				PackItem( new ShortSpear() );
			else if ( BodyValue == 36 )
				PackItem( new Mace() );
			else
				PackGem();
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Lizardman; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public Lizardman( Serial serial ) : base( serial )
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

