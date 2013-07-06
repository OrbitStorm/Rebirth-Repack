using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a reindeer corpse" )]
	public class Reindeer : BaseCreature
	{
		[Constructable]
		public Reindeer() : base( AIType.AI_Melee, FightMode.Closest, 15, 7, 0.4, 0.75 )
		{
			Body = 234;
			Name = Utility.RandomBool() ? "Prancer" : "Dancer";
			
			SetStr( 9000 );
			SetHits( 41, 71 );
			SetDex( 9000 );
			SetStam( 10 );
			SetInt( 9000 );
			SetMana( 0 );

			SetSkill( SkillName.Wrestling, 90.1, 100 );
			SetSkill( SkillName.Fencing, 90.1, 100 );
			SetSkill( SkillName.Macing, 90.1, 100 );
			SetSkill( SkillName.Swords, 90.1, 100 );
			SetSkill( SkillName.DetectHidden, 90.1, 100 );
			SetSkill( SkillName.Archery, 90.1, 100 );
			SetSkill( SkillName.Parry, 90.1, 100 );
			SetSkill( SkillName.Tactics, 90.1, 100 );
			SetSkill( SkillName.MagicResist, 90.1, 100 );

			VirtualArmor = 32;
			SetDamage( 4, 10 );
		}

		public override int GetAttackSound() 
		{ 
			return 0x82; 
		} 

		public override int GetHurtSound() 
		{ 
			return 0x83; 
		} 

		public override int GetDeathSound() 
		{ 
			return 0x84; 
		} 

		public override bool Unprovokable { get { return true; } }

		public Reindeer( Serial serial ) : base( serial )
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

