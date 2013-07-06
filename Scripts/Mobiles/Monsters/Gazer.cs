using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a gazer corpse" )]
	public class Gazer : BaseCreature
	{
		[Constructable]
		public Gazer() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 22;
			Name = "a gazer";
			SetStr( 96, 125 );
			SetHits( 86, 115 );
			SetDex( 86, 105 );
			SetStam( 46, 65 );
			SetInt( 41, 65 );
			SetMana( 41, 55 );
			Karma = -125;

			BaseSoundID = 377;
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.MagicResist, 50.1, 65 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Magery, 50.1, 65 );
			SetSkill( SkillName.Wrestling, 50.1, 70 );

			VirtualArmor = 19;
			SetDamage( 3, 12 );

			if ( Utility.RandomBool() )
				LootPack.Rich.Generate( this );
			else
				LootPack.FilthyRich.Generate( this );
			PackGold( 15, 75 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Gazer( Serial serial ) : base( serial )
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

