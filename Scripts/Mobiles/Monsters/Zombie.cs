using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a rotting corpse" )]
	public class Zombie : BaseCreature
	{
		[Constructable]
		public Zombie() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 3;
			Name = "a zombie";
			SetStr( 46, 70 );
			SetHits( 46, 70 );
			SetDex( 31, 50 );
			SetStam( 31, 50 );
			SetInt( 26, 40 );
			SetMana( 26, 40 );
			Karma = -125;

			BaseSoundID = 471;
			SetSkill( SkillName.Tactics, 35.1, 50 );
			SetSkill( SkillName.MagicResist, 15.1, 40 );
			SetSkill( SkillName.Parry, 20.1, 30 );
			SetSkill( SkillName.Wrestling, 35.1, 50 );

			VirtualArmor = 9;
			SetDamage( 2, 8 );

			Item item = null;
			LootPack.Poor.Generate( this );
			if ( Utility.RandomBool() )
				PackGold( 5, 25 );
			if ( Utility.RandomBool() )
				PackGem();
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

		public Zombie( Serial serial ) : base( serial )
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

