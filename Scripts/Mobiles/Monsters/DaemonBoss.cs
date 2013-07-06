using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a balron corpse" )]
	public class Balron : BaseCreature
	{
		[Constructable]
		public Balron() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5 )
		{
			Body = 10;
			Name = NameList.RandomName( "balron" );
			SetStr( 386, 585 );
			SetHits( 401, 800 );
			SetDex( 77, 155 );
			SetStam( 156, 255 );
			SetInt( 226, 525 );
			SetMana( 451, 550 );
			Karma = -125;

			Hue = Utility.RandomMinMax( 1106, 1110 );

			BaseSoundID = 357;
			SetSkill( SkillName.Tactics, 90.1, 100 );
			SetSkill( SkillName.MagicResist, 90.1, 100 );
			SetSkill( SkillName.Parry, 90.1, 100 );
			SetSkill( SkillName.Magery, 90.1, 100 );
			SetSkill( SkillName.Wrestling, 90.1, 100 );

			VirtualArmor = Utility.RandomMinMax( 18, 33 );
			SetDamage( 30 );

			PackItem( LootPack.BalronSword.Construct() );
			PackGold( 55, 110 );
			LootPack.FilthyRich.Generate( this );
			LootPack.FilthyRich.Generate( this );
			LootPack.HighScrolls.Generate( this );
			LootPack.HighScrolls.Generate( this );
		}

		public override TimeSpan ReaquireDelay{ get{ return TimeSpan.Zero; } }

		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int Meat{ get{ return 1; } }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public override bool Unprovokable{ get{ return true; } }

		public Balron( Serial serial ) : base( serial )
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

