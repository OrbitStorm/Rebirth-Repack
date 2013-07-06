using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a jack rabbit corpse" )]
	public class JackRabbit : BaseCreature
	{
		[Constructable]
		public JackRabbit() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 205;
			Name = "a jack rabbit";
			Hue = 443;
			SetStr( 6, 10 );
			SetHits( 4, 8 );
			SetDex( 26, 38 );
			SetStam( 40, 70 );
			SetInt( 6, 14 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 5;
			BaseSoundID = 199;
			SetSkill( SkillName.Tactics, 5.1, 10 );
			SetSkill( SkillName.MagicResist, 5.1, 14 );
			SetSkill( SkillName.Parry, 25.1, 38 );
			SetSkill( SkillName.Wrestling, 5.1, 10 );

			VirtualArmor = 4;
			SetDamage( 1, 2 );
		}

		public override int GetAngerSound()
		{
			return -1;
		}

		public override int GetIdleSound()
		{
			return -1;
		}

		public override int GenerateFurs(Corpse c)
		{
			c.DropItem( new LightFur() );
			return 1;
		}

		public JackRabbit( Serial serial ) : base( serial )
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

