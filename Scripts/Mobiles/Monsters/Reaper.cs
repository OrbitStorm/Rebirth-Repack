using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a reaper corpse" )]
	public class Reaper : BaseCreature
	{
		[Constructable]
		public Reaper() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 47;
			Name = "a reaper";
			SetStr( 66, 80 );
			SetHits( 146, 160 );
			SetDex( 66, 75 );
			SetStam( 0 );
			SetInt( 36, 50 );
			SetMana( 50, 150 );
			Karma = -125;

			CantWalk = true;

			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 35.1, 50 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Magery, 40.1, 50 );
			SetSkill( SkillName.Wrestling, 50.1, 60 );

			VirtualArmor = 16;
			SetDamage( 5, 15 );

			Item item = null;
			item = new Log( Utility.RandomMinMax( 1, 10 ) );
			PackItem( item );

			LootPack.Average.Generate( this );
		}

		public Reaper( Serial serial ) : base( serial )
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

