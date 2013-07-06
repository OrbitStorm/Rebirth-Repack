using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a blood elemental corpse" )]
	public class BloodElemental : BaseCreature
	{
		[Constructable]
		public BloodElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.25, 0.55 )
		{
			Body = 16;

			Hue = Utility.RandomMinMax( 33, 38 );

			Name = "a blood elemental";
			SetStr( 326, 415 );
			SetHits( 326, 415 );
			SetDex( 66, 85 );
			SetStam( 66, 85 );
			SetInt( 91, 215 );
			SetMana( 91, 215 );
			Karma = -125;

			//CanSwim = true;

			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 80.1, 95 );
			SetSkill( SkillName.Parry, 85.1, 95 );
			SetSkill( SkillName.Magery, 85.1, 100 );
			SetSkill( SkillName.Wrestling, 80.1, 100 );

			VirtualArmor = 30;
			SetDamage( 4, 28 );

			LootPack.FilthyRich.Generate( this );
			if ( Utility.RandomBool() )
				LootPack.FilthyRich.Generate( this );
			else
				LootPack.Rich.Generate( this );
			PackGold( 75, 105 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override bool Unprovokable{ get{ return true; } }

		public BloodElemental( Serial serial ) : base( serial )
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

