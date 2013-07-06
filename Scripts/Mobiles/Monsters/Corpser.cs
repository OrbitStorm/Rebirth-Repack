using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a corpser's corpse" )]
	public class Corpser : BaseCreature
	{
		[Constructable]
		public Corpser() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 8;
			Name = "a corpser";
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 26, 45 );
			SetStam( 26, 50 );
			SetInt( 26, 40 );
			SetMana( 0 );
			Karma = -125;

			CantWalk = true;

			BaseSoundID = 352;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );
			SetSkill( SkillName.Parry, 15.1, 25 );
			SetSkill( SkillName.Wrestling, 45.1, 60 );

			VirtualArmor = 9;
			SetDamage( 3, 6 );

			PackGold( 15, 55 );
		}

		public Corpser( Serial serial ) : base( serial )
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

