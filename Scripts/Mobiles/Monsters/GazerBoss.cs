using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an elder gazer corpse" )]
	public class ElderGazer : BaseCreature
	{
		[Constructable]
		public ElderGazer() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 22;
			Name = "an elder gazer";
			SetStr( 96, 125 );
			SetHits( 186, 215 );
			SetDex( 86, 105 );
			SetStam( 46, 65 );
			SetInt( 91, 185 );
			SetMana( 191, 285 );
			Karma = -125;

			BaseSoundID = 377;
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 85.1, 100 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Magery, 90.1, 100 );
			SetSkill( SkillName.Wrestling, 80.1, 100 );

			VirtualArmor = 25;
			SetDamage( 3, 24 );

			LootPack.FilthyRich.Generate( this );
			LootPack.FilthyRich.Generate( this );
			PackGold( 55, 105 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public override bool Unprovokable{ get{ return true; } }

		public ElderGazer( Serial serial ) : base( serial )
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

