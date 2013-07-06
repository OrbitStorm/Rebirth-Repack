using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a hell hound corpse" )]
	public class HellHound : BaseCreature
	{
		[Constructable]
		public HellHound() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 225;
			Name = "a hell hound";
			if ( Utility.RandomBool() )
				Hue = Utility.RandomRedHue();
			else
				Hue = Utility.RandomMinMax( 34, 39 );
			SetStr( 96, 120 );
			SetHits( 96, 120 );
			SetDex( 81, 105 );
			SetStam( 91, 115 );
			SetInt( 36, 60 );
			SetMana( 71, 95 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 90;
			BaseSoundID = 229;
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.Wrestling, 60.1, 80 );
			SetSkill( SkillName.MagicResist, 57.6, 75 );
			SetSkill( SkillName.Parry, 62.6, 75 );

			VirtualArmor = 11;
			SetDamage( 6, 22 );

			Item item = null;
			item = new SulfurousAsh( Utility.RandomMinMax( 1, 3 ) );
			PackItem( item );
			item = new SulfurousAsh( Utility.RandomMinMax( 1, 3 ) );
			PackItem( item );
			item = new SulfurousAsh( Utility.RandomMinMax( 1, 3 ) );
			PackItem( item );

			PackGold( 15, 50 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public override bool HasBreath{ get{ return true; } } // fire breath enabled

		public HellHound( Serial serial ) : base( serial )
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

