using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class SkeletalMage : BaseCreature
	{
		[Constructable]
		public SkeletalMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 50;
			Name = "a skeletal mage";
			Hue = Utility.RandomRedHue();
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 86, 110 );
			SetMana( 71, 120 );
			Karma = -125;

			BaseSoundID = 451;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 55 );
			SetSkill( SkillName.Magery, 60.1, 70 );

			VirtualArmor = 19;
			SetDamage( 2, 8 );

			if ( Utility.RandomBool() )
				LootPack.Average.Generate( this );
			else
				LootPack.Poor.Generate( this );
			LootPack.HighScrolls.Generate( this );
			LootPack.HighScrolls.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public SkeletalMage( Serial serial ) : base( serial )
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

	[CorpseName( "a skeletal corpse" )]
	public class BoneMagi : BaseCreature
	{
		[Constructable]
		public BoneMagi() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 50;
			Name = "a bone magi";
			Hue = Utility.RandomRedHue();
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 86, 110 );
			SetMana( 71, 120 );
			Karma = -125;

			BaseSoundID = 451;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 55 );
			SetSkill( SkillName.Magery, 60.1, 70 );

			VirtualArmor = 19;
			SetDamage( 2, 8 );

			if ( Utility.RandomBool() )
				LootPack.Rich.Generate( this );
			else
				LootPack.Poor.Generate( this );
			LootPack.HighScrolls.Generate( this );
			LootPack.HighScrolls.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public BoneMagi( Serial serial ) : base( serial )
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

