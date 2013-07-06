using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a bull corpse" )]
	public class Bull : BaseCreature
	{
		[Constructable]
		public Bull() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = Utility.RandomList( 232,233 );
			Name = "a bull";
			SetStr( 77, 111 );
			SetHits( 77, 111 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 47, 75 );
			SetMana( 0 );

			switch ( Utility.Random( 4 ) )
			{
				case 1:
					Hue = 443; // brown
					break;
				case 2:
					Hue = 2305; // black
					break;
				case 3:
					Hue = 2301; // white
					break;
				case 0:
				default:
					Hue = 0;
					break;
			}

			Tamable = true;
			MinTameSkill = 80;
			BaseSoundID = 120;
			SetSkill( SkillName.Tactics, 67.6, 85 );
			SetSkill( SkillName.Wrestling, 40.1, 57.5 );
			SetSkill( SkillName.MagicResist, 17.6, 25 );
			SetSkill( SkillName.Parry, 42.6, 55 );

			VirtualArmor = 14;
			SetDamage( 4, 9 );
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( from.InRange( this.Location, 3 ) )
			{
				int rnd = Utility.Random( 100 );
				if ( rnd < 5 )
					Animate( 8, 3, 1, true, false, 0 );
				
				if ( rnd < 20 )
					PlaySound( 121 );
				else if ( rnd < 40 )
					PlaySound( 120 );
				from.NextActionTime = DateTime.Now + TimeSpan.FromSeconds( 1 );
			}
		}

		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 9; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bull; } }

		public Bull( Serial serial ) : base( serial )
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

