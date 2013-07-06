using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an earth elemental corpse" )]
	public class EarthElemental : BaseCreature
	{
		[Constructable]
		public EarthElemental() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 14;
			Name = "an earth elemental";
			SetStr( 116, 135 );
			SetDex( 56, 65 );
			SetInt( 61, 75 );
			Karma = -125;

			BaseSoundID = 268;
			SetSkill( SkillName.Tactics, 60.1, 100 );
			SetSkill( SkillName.Wrestling, 40.1, 80 );
			SetSkill( SkillName.MagicResist, 30.1, 75 );
			SetSkill( SkillName.Parry, 40.2, 65 );

			VirtualArmor = 15;
			SetDamage( 3, 18 );

			LootPack.Rich.Generate( this );
			PackGold( 40, 60 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public override void OnDeath(Container c)
		{
			base.OnDeath (c);
			if ( c!= null && Utility.RandomBool() )
				c.DropItem( new Brazier() );
		}

		public EarthElemental( Serial serial ) : base( serial )
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

