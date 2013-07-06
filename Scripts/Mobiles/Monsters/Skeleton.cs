using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class Skeleton : BaseCreature
	{
		[Constructable]
		public Skeleton() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = Utility.RandomList( 50,56 );
			Name = "a skeleton";
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 16, 40 );
			SetMana( 0 );
			Karma = -125;

			BaseSoundID = 451;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 55 );

			VirtualArmor = 8;
			SetDamage( 2, 8 );

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
			LootPack.Meager.Generate( this );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Skeleton( Serial serial ) : base( serial )
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

