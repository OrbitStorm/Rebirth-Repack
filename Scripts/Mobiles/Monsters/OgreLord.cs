using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an ogre lord's corpse" )]
	public class OgreLord : BaseCreature
	{
		[Constructable]
		public OgreLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 1;
			Name = "an ogre lord";
			SetStr( 267, 445 );
			SetHits( 666, 755 );
			SetDex( 66, 75 );
			SetStam( 86, 175 );
			SetInt( 46, 70 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 427;
			SetSkill( SkillName.Tactics, 90.1, 100 );
			SetSkill( SkillName.MagicResist, 65.1, 80 );
			SetSkill( SkillName.Parry, 75.1, 85 );
			SetSkill( SkillName.Wrestling, 90.1, 100 );

			VirtualArmor = 25;
			SetDamage( 10, 40 );

			LootPack.FilthyRich.Generate( this );
			LootPack.FilthyRich.Generate( this );
			PackGold( 15, 100 );
			PackItem( new Club() );
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int Meat{ get{ return 2; } }

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool Unprovokable{ get{ return true; } }

		public OgreLord( Serial serial ) : base( serial )
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

