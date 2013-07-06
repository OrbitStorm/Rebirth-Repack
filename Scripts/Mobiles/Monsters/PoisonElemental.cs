using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a poison elemental corpse" )]
	public class PoisonElemental : BaseCreature
	{
		[Constructable]
		public PoisonElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.8 )
		{
			Body = 13;
			Name = "a poison elemental";
			SetStr( 326, 415 );
			SetHits( 426, 515 );
			SetDex( 166, 185 );
			SetStam( 166, 185 );
			SetInt( 91, 165 );
			SetMana( 271, 395 );
			Karma = -125;

			Hue = Utility.RandomMinMax( 61, 79 );

			BaseSoundID = 263;
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 80.1, 95 );
			SetSkill( SkillName.Parry, 75.1, 85 );
			SetSkill( SkillName.Magery, 90.1, 100 );
			SetSkill( SkillName.Wrestling, 70.1, 90 );

			VirtualArmor = 35;
			SetDamage( 5, 25 );

			LootPack.FilthyRich.Generate( this );
			LootPack.FilthyRich.Generate( this );
			LootPack.FilthyRich.Generate( this );
			PackGold( 15, 100 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override TimeSpan ReaquireDelay{ get{ return TimeSpan.FromSeconds( 0.5 ); } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override double HitPoisonChance{ get{ return 0.9; } }

		public override bool Unprovokable{ get{ return true; } }

		public PoisonElemental( Serial serial ) : base( serial )
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

