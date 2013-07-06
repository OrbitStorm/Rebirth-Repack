using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a sheep corpse" )]
	public class Sheep : BaseCreature, ICarvable
	{
		private DateTime m_NextWoolTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime NextWoolTime
		{
			get{ return m_NextWoolTime; }
			set{ m_NextWoolTime = value; Body = ( DateTime.Now >= m_NextWoolTime ) ? 0xCF : 0xDF; }
		}

		public void Carve( Mobile from, Item item )
		{
			if ( DateTime.Now < m_NextWoolTime )
			{
				// This sheep is not yet ready to be shorn.
				PrivateOverheadMessage( MessageType.Regular, 0x3B2, 500449, from.NetState );
				return;
			}

			from.SendLocalizedMessage( 500452 ); // You place the gathered wool into your backpack.
			from.AddToBackpack( new Wool( Map == Map.Felucca ? 2 : 1 ) );

			NextWoolTime = DateTime.Now + TimeSpan.FromHours( 3.0 ); // TODO: Proper time delay
		}

		public override void OnThink()
		{
			base.OnThink();
			Body = ( DateTime.Now >= m_NextWoolTime ) ? 0xCF : 0xDF;
		}

		[Constructable]
		public Sheep() : base( AIType.AI_Animal, FightMode.Agressor, 10, 1, 0.40, 0.8 )
		{
			Body = 207;
			Name = "a sheep";
			SetStr( 21, 49 );
			SetHits( 21, 49 );
			SetDex( 36, 45 );
			SetStam( 36, 55 );
			SetInt( 16, 20 );
			SetMana( 0 );

			if ( Utility.Random( 10 ) == 0 )
				Hue = 2305; // black

			Tamable = true;
			MinTameSkill = 30;
			BaseSoundID = 214;
			SetSkill( SkillName.Tactics, 9.2, 19 );
			SetSkill( SkillName.Wrestling, 9.2, 19 );
			SetSkill( SkillName.MagicResist, 5.1, 10 );
			SetSkill( SkillName.Parry, 15.1, 25 );

			VirtualArmor = Utility.RandomMinMax( 1, 3 );
			SetDamage( 1, 3 );
		}

		public override int Meat{ get{ return 3; } }
		public override MeatType MeatType{ get{ return MeatType.LambLeg; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public override int Wool{ get{ return (Body == 0xCF ? 2 : 0); } }

		public Sheep( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );

			writer.WriteDeltaTime( m_NextWoolTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					NextWoolTime = reader.ReadDeltaTime();
					break;
				}
			}
		}
	}
}
