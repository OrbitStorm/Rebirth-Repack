using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a daemon corpse" )]
	public class Daemon : BaseCreature
	{
		[Constructable]
		public Daemon() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5 )
		{
			Body = 9;
			Name = NameList.RandomName( "daemon" );
			SetStr( 266, 285 );
			SetHits( 166, 185 );
			SetDex( 266, 275 );
			SetStam( 66, 75 );
			SetInt( 291, 305 );
			SetMana( 91, 105 );
			Karma = -125;

			BaseSoundID = 357;
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Parry, 100.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			VirtualArmor = Utility.RandomMinMax( 3, 24 );
			SetDamage( 30 );

			LootPack.FilthyRich.Generate( this );
			LootPack.Rich.Generate( this );
			PackGold( 15, 100 );
			LootPack.MedScrolls.Generate( this );
			LootPack.HighScrolls.Generate( this );
		}

		public override TimeSpan ReaquireDelay{ get{ return TimeSpan.FromSeconds( 1.0 ); } }

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int Meat{ get{ return 1; } }

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Daemon( Serial serial ) : base( serial )
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

	[CorpseName( "a daemon corpse" )]
	public class SummonedDaemon : BaseCreature
	{
		[Constructable]
		public SummonedDaemon() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 9;
			Name = NameList.RandomName( "daemon" );
			SetStr( 166, 185 );
			SetDex( 66, 75 );
			SetInt( 91, 105 );

			Karma = -125;

			BaseSoundID = 357;
			SetSkill( SkillName.Tactics, 70.1, 80.0 );
			SetSkill( SkillName.MagicResist, 70.1, 80.0 );
			SetSkill( SkillName.Parry, 65.1, 75.0 );
			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			VirtualArmor = Utility.RandomMinMax( 3, 24 );
			SetDamage( 30 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public SummonedDaemon( Serial serial ) : base( serial )
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

