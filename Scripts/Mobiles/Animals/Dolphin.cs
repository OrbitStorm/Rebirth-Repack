using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a dolphin corpse" )]
	public class Dolphin : BaseCreature
	{
		[Constructable]
		public Dolphin() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 151;
			Name = "a dolphin";
			SetStr( 21, 49 );
			SetHits( 21, 49 );
			SetDex( 66, 85 );
			SetStam( 90, 140 );
			SetInt( 16, 30 );
			SetMana( 0 );

			CanSwim = true;
			CantWalk = true;

			BaseSoundID = 138;
			SetSkill( SkillName.Tactics, 19.2, 29 );
			SetSkill( SkillName.Wrestling, 19.2, 29 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );
			SetSkill( SkillName.Parry, 65.1, 75 );

			VirtualArmor = 8;
			SetDamage( 3, 6 );
		}

		public override bool AlwaysAttackable
		{
			get
			{
				return true;
			}
		}


		public Dolphin( Serial serial ) : base( serial )
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

