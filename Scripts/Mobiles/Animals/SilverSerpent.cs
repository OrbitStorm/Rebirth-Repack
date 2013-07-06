using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a silver serpent corpse" )]
	public class SilverSerpent : BaseCreature
	{
		[Constructable]
		public SilverSerpent() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 52;
			Name = "a silver serpent";
			SetStr( 61, 80 );
			SetHits( 26, 45 );
			SetDex( 51, 65 );
			SetStam( 46, 55 );
			SetInt( 11, 20 );
			SetMana( 26, 40 );

			Tamable = true;
			MinTameSkill = 60;
			BaseSoundID = 219;
			SetSkill( SkillName.Tactics, 40.1, 45 );
			SetSkill( SkillName.MagicResist, 25.1, 40 );
			SetSkill( SkillName.Parry, 45.1, 60 );
			SetSkill( SkillName.Wrestling, 40.1, 55 );

			VirtualArmor = 9;
			SetDamage( 1, 8 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }

		public SilverSerpent( Serial serial ) : base( serial )
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

