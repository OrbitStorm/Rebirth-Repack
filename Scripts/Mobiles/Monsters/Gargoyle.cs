using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a gargoyle corpse" )]
	public class Gargoyle : BaseCreature
	{
		[Constructable]
		public Gargoyle() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 4;
			Name = "a gargoyle";
			SetStr( 146, 175 );
			SetHits( 146, 175 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 81, 105 );
			SetMana( 81, 105 );
			Karma = -125;

			BaseSoundID = 372;
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.MagicResist, 70.1, 85 );
			SetSkill( SkillName.Parry, 35.1, 45 );
			SetSkill( SkillName.Magery, 70.1, 85 );
			SetSkill( SkillName.Wrestling, 40.1, 80 );

			VirtualArmor = 16;
			SetDamage( 3, 18 );

			LootPack.FilthyRich.Generate( this );
			LootPack.HighScrolls.Generate( this );
			PackGem();
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public override int Meat{ get{ return 1; } }

		public Gargoyle( Serial serial ) : base( serial )
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

