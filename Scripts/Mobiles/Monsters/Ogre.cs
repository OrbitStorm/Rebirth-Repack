using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an ogre corpse" )]
	public class Ogre : BaseCreature
	{
		[Constructable]
		public Ogre() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 1;
			Name = "an ogre";
			SetStr( 166, 195 );
			SetHits( 166, 195 );
			SetDex( 46, 65 );
			SetStam( 46, 65 );
			SetInt( 46, 70 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 427;
			SetSkill( SkillName.Tactics, 60.1, 70 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 70.1, 80 );

			VirtualArmor = Utility.RandomMinMax( 5, 15 );
			SetDamage( 12 );

			LootPack.Rich.Generate( this );
			LootPack.Meager.Generate( this );
			PackItem( new Club() );
		}

		public override int Meat{ get{ return 2; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Ogre( Serial serial ) : base( serial )
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

