using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an air elemental corpse" )]
	public class AirElemental : BaseCreature
	{
		[Constructable]
		public AirElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 13;
			Name = "an air elemental";
			SetStr( 116, 135 );
			SetDex( 56, 65 );
			SetInt( 61, 75 );
			Karma = -125;

			BaseSoundID = 263;
			SetSkill( SkillName.Tactics, 60.1, 80 );
			SetSkill( SkillName.MagicResist, 60.1, 75 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Magery, 60.1, 75 );
			SetSkill( SkillName.Wrestling, 60.1, 80 );

			VirtualArmor = 19;
			SetDamage( 5, 13 );

			LootPack.Rich.Generate( this );
			PackGold( 15, 100 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public AirElemental( Serial serial ) : base( serial )
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

