using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a fire elemental corpse" )]
	public class FireElemental : BaseCreature
	{
		[Constructable]
		public FireElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 15;
			Name = "a fire elemental";
			SetStr( 116, 135 );
			SetHits( 116, 135 );
			SetDex( 56, 65 );
			SetStam( 56, 65 );
			SetInt( 61, 75 );
			SetMana( 61, 75 );
			Karma = -125;

			BaseSoundID = 273;
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 60.1, 75 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Magery, 60.1, 75 );
			SetSkill( SkillName.Wrestling, 70.1, 100 );

			VirtualArmor = 19;
			SetDamage( 4, 12 );

			LootPack.FilthyRich.Generate( this );
			PackGold( 15, 100 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public FireElemental( Serial serial ) : base( serial )
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

