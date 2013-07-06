using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a headless corpse" )]
	public class HeadlessOne : BaseCreature
	{
		[Constructable]
		public HeadlessOne() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 31;
			Name = "a headless one";
			Hue = Utility.RandomSkinHue();
			SetStr( 26, 50 );
			SetHits( 26, 50 );
			SetDex( 36, 55 );
			SetStam( 36, 55 );
			SetInt( 16, 30 );
			SetMana( 16, 30 );
			Karma = -125;

			BaseSoundID = 407;
			SetSkill( SkillName.Tactics, 25.1, 40 );
			SetSkill( SkillName.Wrestling, 25.1, 40 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );
			SetSkill( SkillName.Parry, 35.1, 45 );

			VirtualArmor = 9;
			SetDamage( 3, 12 );

			if ( Utility.RandomBool() )
				LootPack.Poor.Generate( this );
			else
				LootPack.Meager.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public HeadlessOne( Serial serial ) : base( serial )
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

