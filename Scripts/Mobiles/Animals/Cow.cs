using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a cow corpse" )]
	public class Cow : BaseCreature
	{
		[Constructable]
		public Cow() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = Utility.RandomList( 216,231 );
			Name = "a cow";
			SetStr( 42, 76 );
			SetHits( 45, 85 );
			SetDex( 26, 45 );
			SetStam( 32, 56 );
			SetInt( 2, 10 );
			SetMana( 0 );

			switch ( Utility.Random( 4 ) )
			{
				//case 1:
				//	Hue = 442; // brown? (kind of red/orange)
				//	break;
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
			MinTameSkill = 30;
			BaseSoundID = 120;
			SetSkill( SkillName.Tactics, 27.6, 45 );
			SetSkill( SkillName.Wrestling, 27.6, 45 );
			SetSkill( SkillName.MagicResist, 17.6, 25 );
			SetSkill( SkillName.Parry, 22.6, 35 );

			VirtualArmor = 9;
			SetDamage( 6, 12 );
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
				from.NextActionTime = DateTime.Now + TimeSpan.FromSeconds( 0.5 );
			}
		}

		public override int Meat{ get{ return 8; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Cow( Serial serial ) : base( serial )
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

