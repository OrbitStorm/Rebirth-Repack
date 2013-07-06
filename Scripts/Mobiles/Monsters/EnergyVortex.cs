using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an energy vortex corpse" )]
	public class EnergyVortex : BaseCreature
	{
		[Constructable]
		public EnergyVortex() : base( AIType.AI_Melee, FightMode.Smartest,  11, 1, 0.25, 0.3 )
		{
			if ( Utility.Random( 100 ) == 17 )
				Body = 0xDC;
			else
				Body = 0xD;

			Hue = 20;
			Name = "an energy vortex";
			SetStr( 100 );
			SetHits( 900 );
			SetDex( 200 );
			SetStam( 0 );
			SetInt( 100 );
			SetMana( 0 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Parry, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );

			SetDamage( 15, 25 );
		}

		public override TimeSpan ReaquireDelay{ get{ return TimeSpan.FromSeconds( 0.1 ); } }

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Utility.RandomDouble() < 0.15 ? Poison.Deadly : Poison.Greater; } }
		public override bool IsHouseSummonable{ get{ return true; } }

		public override bool Commandable{ get{ return false; } }

		public override bool AlwaysMurderer { get { return true; } }

		public EnergyVortex( Serial serial ) : base( serial )
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

