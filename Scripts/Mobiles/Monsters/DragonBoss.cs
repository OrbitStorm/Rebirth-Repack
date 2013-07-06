using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class AncientWyrm : BaseCreature
	{
		[Constructable]
		public AncientWyrm() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.35, 0.8 )
		{
			Body = Utility.RandomList( 12,59 );
			Name = "an ancient wyrm";
			SetStr( 496, 585 );
			SetHits( 701, 1100 );
			SetDex( 86, 175 );
			SetStam( 86, 175 );
			SetInt( 436, 525 );
			SetMana( 251, 550 );
			Karma = -125;
			
			switch ( Utility.Random( 3 ) )
			{
				case 1:
					Hue = Utility.RandomMinMax( 1105, 1110 );
					break;
				
				case 2:
					Hue = Utility.RandomMinMax( 34, 39 );
					break;
				
				case 0:
				default:
					Hue = 0;
					break;
			}

			BaseSoundID = 362;
			SetSkill( SkillName.Tactics, 97.6, 100 );
			SetSkill( SkillName.MagicResist, 99.1, 100 );
			SetSkill( SkillName.Parry, 55.1, 95 );
			SetSkill( SkillName.Wrestling, 90.1, 92.5 );
			SetSkill( SkillName.Magery, 72.5, 92.5 );

			VirtualArmor = 30;
			SetDamage( 11, 41 );

			LootPack.FilthyRich.Generate( this );
			LootPack.FilthyRich.Generate( this );
			LootPack.FilthyRich.Generate( this );
			PackGold( 15, 100 );

			for(int i=0;i<3+Utility.Random( 4 );i++)
				PackGem();
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override int Hides{ get{ return 40; } }
		public override int Meat{ get{ return 19; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override bool Unprovokable{ get{ return true; } }

		public AncientWyrm( Serial serial ) : base( serial )
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

