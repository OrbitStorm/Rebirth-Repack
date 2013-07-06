using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a horse corpse" )]
	[TypeAlias( "Server.Mobiles.BrownHorse", "Server.Mobiles.DirtyHorse", "Server.Mobiles.GrayHorse", "Server.Mobiles.TanHorse" )]
	public class Horse : BaseMount
	{
		private static int[] m_IDs = new int[]
			{
				0xC8, 0x3E9F,
				0xE2, 0x3EA0,
				0xE4, 0x3EA1,
				0xCC, 0x3EA2
			};

		[Constructable]
		public Horse() : this( "a horse" )
		{
		}

		[Constructable]
		public Horse( string name ) : base( name, 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Agressor, 10, 1, 0.25, 0.55 )
		{
			int random = Utility.Random( 4 );

			Body = m_IDs[random * 2];
			ItemID = m_IDs[random * 2 + 1];
			BaseSoundID = 0xA8;
			SetStr( 44, 120 );
			SetHits( 44, 120 );
			SetDex( 36, 55 );
			SetStam( 36, 55 );
			SetInt( 6, 10 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 65;
			BaseSoundID = 168;
			SetSkill( SkillName.Tactics, 29.3, 44 );
			SetSkill( SkillName.Wrestling, 29.3, 44 );
			SetSkill( SkillName.MagicResist, 25.1, 30 );
			SetSkill( SkillName.Parry, 35.1, 45 );

			VirtualArmor = 9;
			SetDamage( 4, 12 );
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }


		public Horse( Serial serial ) : base( serial )
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

	public class HauntedHorse : Horse
	{
		[Constructable]
		public HauntedHorse() : base()
		{
			// nightmare values
			switch ( Utility.Random( 3 ) )
			{
				case 0:
				{
					BodyValue = 116;
					ItemID = 16039;
					break;
				}
				case 1:
				{
					BodyValue = 178;
					ItemID = 16041;
					break;
				}
				case 2:
				{
					BodyValue = 179;
					ItemID = 16055;
					break;
				}
			}
		}

		public HauntedHorse( Serial serial ) : base( serial )
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

			if ( this.Controled )
				new HauntTimer( this, this.ControlMaster ).Start();
		}

		public override void OnTamed(Mobile owner)
		{
			base.OnTamed (owner);

			new HauntTimer( this, owner ).Start();
		}

		public override bool OnBeforeDeath()
		{
			SummonWraith( null );
			return false;
		}

		public void SummonWraith( Mobile target )
		{
			Ghoul w = new Ghoul();
			w.Name = "a wraith";
			w.Hue = 0x4FFF;
			w.SetSkill( SkillName.Magery, 75, 100 );
			w.SetMana( 100 );
			w.AI = AIType.AI_Mage;

			if ( this.Rider != null )
			{
				if ( this.Rider.Map == Map.Internal )
				{
					w.MoveToWorld( Rider.LogoutLocation, Rider.LogoutMap );
				}
				else
				{
					w.MoveToWorld( Rider.Location, Rider.Map );
					target = Rider;
				}
			}
			else
			{
				w.MoveToWorld( this.Location, this.Map );
			}

			if ( target != null && w.CanBeHarmful( target ) && w.InRange( target, 15 ) )
				w.Attack( target );
			else
				w.NextReaquireTime = DateTime.Now;

			w.PlaySound( w.GetAngerSound() );
			w.PlaySound( this.GetDeathSound() );
			Delete();
		}

		private class HauntTimer : Timer
		{
			private HauntedHorse m_Mobile;
			private Mobile m_Owner;
			private int m_Stage;
			public HauntTimer( HauntedHorse hh, Mobile owner ) : base( TimeSpan.FromMinutes( 10.0 + Utility.Random( 6 ) ), TimeSpan.FromSeconds( 5.0 ), 2 )
			{
				Priority = TimerPriority.OneSecond;
				m_Mobile = hh;
				m_Owner = owner;
				m_Stage = 0;
			}

			protected override void OnTick()
			{
				m_Stage++;
				if ( m_Stage == 1 )
				{
					if ( m_Mobile.Rider != null )
					{
						m_Owner = m_Mobile.Rider;
						m_Mobile.Rider = null; // dismount
					}
					if ( m_Mobile.Controled )
					{
						m_Mobile.BondingBegin = m_Mobile.OwnerAbandonTime = DateTime.MinValue;
						m_Mobile.ControlTarget = null;
						m_Mobile.Say( 1043255, m_Mobile.Name ); // ~1_NAME~ appears to have decided that is better off without a master!
						m_Mobile.AIObject.DoOrderRelease();
					}
					m_Mobile.Tamable = false;
				}
				else
				{
					m_Mobile.SummonWraith( m_Owner );
				}
			}
		}
	}
}

