using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a sea serpent corpse" )]
	public class SeaSerpent : BaseCreature
	{
		[Constructable]
		public SeaSerpent() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 150;
			Name = "a sea serpent";
			Hue = Utility.RandomBlueHue();
			SetStr( 168, 225 );
			SetHits( 168, 225 );
			SetDex( 58, 85 );
			SetStam( 58, 85 );
			SetInt( 53, 95 );
			SetMana( 53, 95 );
			Karma = -125;

			CanSwim = true;
			CantWalk = true;

			BaseSoundID = 446;
			SetSkill( SkillName.Tactics, 60.1, 70 );
			SetSkill( SkillName.MagicResist, 60.1, 75 );
			SetSkill( SkillName.Wrestling, 60.1, 70 );
			SetSkill( SkillName.Parry, 65.1, 75 );

			VirtualArmor = 15;
			SetDamage( 5, 15 );

			LootPack.FilthyRich.Generate( this );
		}

		public override int Hides{ get{ return 10; } }

		public SeaSerpent( Serial serial ) : base( serial )
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

