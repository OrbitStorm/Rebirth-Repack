using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a blade spirit corpse" )]
	public class BladeSpirit : BaseCreature
	{
		[Constructable]
		public BladeSpirit() : base( AIType.AI_Melee, FightMode.Fastest,  11, 1, 0.25, 0.3 )
		{
			Body = 574;
			Name = "a blade spirit";

			SetStr( 75 );
			SetHits( 200 );

			SetDex( 75 );
			SetStam( 0 );
			
			SetInt( 100 );
			SetMana( 0 );

			SetSkill( SkillName.Parry, 25, 47.5 );
			SetSkill( SkillName.MagicResist, 25, 47.5 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetDamage( 5, 10 );
		}

		public override int GetAttackSound() { return 0x23B; } // katana

		public override TimeSpan ReaquireDelay{ get{ return TimeSpan.FromSeconds( 0.5 ); } }

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }
		public override bool IsHouseSummonable{ get{ return true; } }

		public override bool Commandable{ get{ return false; } }

		public override bool AlwaysMurderer { get { return true; } }

		public BladeSpirit( Serial serial ) : base( serial )
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

