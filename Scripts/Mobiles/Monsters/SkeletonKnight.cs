using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class SkeletalKnight : BaseCreature
	{
		[Constructable]
		public SkeletalKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 57;
			Name = "a skeletal knight";
			SetStr( 96, 150 );
			SetHits( 96, 150 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 451;
			SetSkill( SkillName.Tactics, 85.1, 100 );
			SetSkill( SkillName.MagicResist, 65.1, 80 );
			SetSkill( SkillName.Parry, 85.1, 95 );
			SetSkill( SkillName.Wrestling, 85.1, 95 );

			VirtualArmor = 18;
			SetDamage( 2, 20 );

			PackItem( new PlateChest() );
			LootPack.Rich.Generate( this );

			Item item = null;
			switch ( Utility.Random( 10 ) )
			{
				case 0:
				{
					item = new BoneChest();
					break;
				}
				case 1:
				{
					item = new BoneLegs();
					break;
				}
				case 2:
				{
					item = new BoneArms();
					break;
				}
				case 3:
				{
					item = new BoneGloves();
					break;
				}
				case 4:
				{
					item = new BoneHelm();
					break;
				}
			}
			PackItem( item );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public SkeletalKnight( Serial serial ) : base( serial )
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
	public class BoneKnight : BaseCreature
	{
		[Constructable]
		public BoneKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 57;
			Name = "a bone knight";
								
			SetStr( 96, 150 );
			SetHits( 96, 150 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 451;
			SetSkill( SkillName.Tactics, 85.1, 100 );
			SetSkill( SkillName.MagicResist, 65.1, 80 );
			SetSkill( SkillName.Parry, 85.1, 95 );
			SetSkill( SkillName.Wrestling, 85.1, 95 );

			VirtualArmor = 18;
			SetDamage( 2, 20 );

			Item item = null;
			item = new PlateChest();
			PackItem( item );
			LootPack.Rich.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public BoneKnight( Serial serial ) : base( serial )
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

