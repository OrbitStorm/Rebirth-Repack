using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a wisp corpse" )]
	public class Wisp : BaseCreature
	{
		[Constructable]
		public Wisp() : base( AIType.AI_Mage, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 58;
			Name = "a wisp";
			SetStr( 196, 225 );
			SetHits( 196, 225 );
			SetDex( 196, 225 );
			SetStam( 96, 125 );
			SetInt( 196, 225 );
			SetMana( 196, 225 );

			BaseSoundID = 466;
			SetSkill( SkillName.Tactics, 80 );
			SetSkill( SkillName.MagicResist, 80 );
			SetSkill( SkillName.Parry, 80 );
			SetSkill( SkillName.Magery, 80 );
			SetSkill( SkillName.Wrestling, 80 );

			VirtualArmor = 20;
			SetDamage( 15, 20 );

			Container cont = new Bag();
			PackItem( cont );

			LootPack.FilthyRich.Generate( this, cont );
			LootPack.FilthyRich.Generate( this, cont );
			LootPack.Rich.Generate( this, cont );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Wisp; } }
		public override bool AlwaysAttackable
		{
			get
			{
				return true;
			}
		}

		public Wisp( Serial serial ) : base( serial )
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

