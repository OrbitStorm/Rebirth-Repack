using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class Orc : BaseCreature
	{
		[Constructable]
		public Orc() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 17;
			Name = NameList.RandomName( "orc" );
			SetStr( 96, 120 );
			SetHits( 96, 120 );
			SetDex( 81, 105 );
			SetStam( 91, 115 );
			SetInt( 36, 60 );
			SetMana( 71, 95 );
			Karma = -125;

			BaseSoundID = 432;
			SetSkill( SkillName.Tactics, 55.1, 80 );
			SetSkill( SkillName.MagicResist, 50.1, 75 );
			SetSkill( SkillName.Parry, 50.1, 75 );
			SetSkill( SkillName.Magery, 50.1, 75 );
			SetSkill( SkillName.Wrestling, 50.1, 70 );

			VirtualArmor = 14;
			SetDamage( 3, 9 );

			Item item = null;
			item = new ThighBoots();
			AddItem( item );
			PackGold( 15, 75 );
			LootPack.Meager.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public Orc( Serial serial ) : base( serial )
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

