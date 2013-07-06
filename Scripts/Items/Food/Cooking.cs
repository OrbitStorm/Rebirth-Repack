using System;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class UtilityItem
	{
		static public int RandomChoice( int itemID1, int itemID2 )
		{
			int iRet = 0;
			switch ( Utility.Random( 2 ) )
			{
				default:
				case 0: iRet = itemID1; break;
				case 1: iRet = itemID2; break;
			}
			return iRet;
		}
	}

	// ********** Dough **********
	public class Dough : BaseItem
	{
		[Constructable]
		public Dough() : base( 0x103d )
		{
			Weight = 1.0;
		}

		public Dough( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private Dough m_Item;

			public InternalTarget( Dough item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted ) return;

				if ( targeted is Eggs )
				{
					m_Item.Consume();

					((Eggs)targeted).Consume();

					from.AddToBackpack( new UnbakedQuiche() );
					from.AddToBackpack( new Eggshells() );
				}
				else if ( targeted is CheeseWheel )
				{
					m_Item.Consume();

					((CheeseWheel)targeted).Consume();

					from.AddToBackpack( new UncookedCheesePizza() );
				}
				else if ( targeted is Sausage )
				{
					m_Item.Consume();

					((Sausage)targeted).Consume();

					from.AddToBackpack( new UncookedSausagePizza() );
				}
				else if ( targeted is Apple )
				{
					m_Item.Consume();

					((Apple)targeted).Consume();

					from.AddToBackpack( new UnbakedApplePie() );
				}
				else if ( targeted is Pear )
				{
					m_Item.Consume();

					((Pear)targeted).Consume();

					from.AddToBackpack( new UnbakedFruitPie() );
				}
				else if ( targeted is CookedBird )
				{
					m_Item.Consume();

					((Item)targeted).Consume();

					from.AddToBackpack( new UnbakedMeatPie() );
				}
				else if ( targeted is FishSteak )
				{
					m_Item.Consume();

					((Item)targeted).Consume();

					from.AddToBackpack( new UnbakedMeatPie() );
				}
				else if ( targeted is Ribs )
				{
					m_Item.Consume();

					((Item)targeted).Consume();

					from.AddToBackpack( new UnbakedMeatPie() );
				}
				else if ( targeted is Peach )
				{
					m_Item.Consume();

					((Peach)targeted).Consume();

					from.AddToBackpack( new UnbakedPeachCobbler() );
				}
				else if ( targeted is JarHoney )
				{
					m_Item.Consume();
					((JarHoney)targeted).Consume();

					from.AddToBackpack( new SweetDough() );
				}
				else if ( CookableFood.IsHeatSource( targeted ) )
				{
					if ( from.BeginAction( typeof( CookableFood ) ) )
					{
						from.PlaySound( 0x225 );
						m_Item.Consume();
						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( OnCooked ), from );
					}
					else
					{
						from.SendAsciiMessage( "You are already cooking something." );
					}
				}
			}
		}

		private static void OnCooked( object state )
		{
			Mobile from = (Mobile)state;

			from.EndAction( typeof( CookableFood ) );

			if ( from.CheckSkill( SkillName.Cooking, 0, 100 ) )
			{
				from.AddToBackpack( new BreadLoaf() );
				from.PlaySound( 0x57 );
			}
			else
			{
				from.SendLocalizedMessage( 500686 ); // You burn the food to a crisp! It's ruined.
			}
		}
	}

	// ********** SweetDough **********
	public class SweetDough : BaseItem
	{
		public override int LabelNumber{ get{ return 1041340; } } // sweet dough

		[Constructable]
		public SweetDough() : base( 0x103d )
		{
			Weight = 1.0;
			Hue = 150;
		}

		public SweetDough( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Hue == 51 )
				Hue = 150;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private SweetDough m_Item;

			public InternalTarget( SweetDough item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted ) return;

				if ( targeted is BowlFlour )
				{
					m_Item.Delete();
					((Item)targeted).Delete();

					from.AddToBackpack( new CakeMix() );
				}
				else if ( targeted is SackFlour )
				{
					((SackFlour)targeted).Quantity--;
					m_Item.Delete();
					from.AddToBackpack( new CakeMix() );
				}
				else if ( targeted is JarHoney )
				{
					m_Item.Delete();
					((JarHoney)targeted).Consume();
					from.AddToBackpack( new CookieMix() );
				}
				else if ( CookableFood.IsHeatSource( targeted ) )
				{
					from.PlaySound( 0x225 );
					m_Item.Delete();
					InternalTimer t = new InternalTimer( from );
					t.Start();
				}
			}
			
			private class InternalTimer : Timer
			{
				private Mobile m_From;
			
				public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 5.0 ) )
				{
					m_From = from;
				}

				protected override void OnTick()
				{
					if ( m_From.CheckSkill( SkillName.Cooking, 0, 100 ) )
					{
						if ( m_From.AddToBackpack( new Muffins() ) )
							m_From.PlaySound( 0x57 );
					}
					else
					{
						m_From.SendLocalizedMessage( 500686 ); // You burn the food to a crisp! It's ruined.
					}
				}
			}
		}
	}

	// ********** JarHoney **********
	public class JarHoney : BaseItem
	{
		[Constructable]
		public JarHoney() : base( 0x9ec )
		{
			Weight = 1.0;
			Stackable = true;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new JarHoney(), amount );
		}

		public JarHoney( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			Stackable = true;
		}

		/*public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}*/

		private class InternalTarget : Target
		{
			private JarHoney m_Item;

			public InternalTarget( JarHoney item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted ) return;

				if ( targeted is Dough )
				{
					m_Item.Delete();
					((Dough)targeted).Consume();

					from.AddToBackpack( new SweetDough() );
				}
				
				if (targeted is BowlFlour)
				{
					m_Item.Consume();
					((BowlFlour)targeted).Delete();

					from.AddToBackpack( new CookieMix() );
				}
			}
		}
	}

	// ********** BowlFlour **********
	public class BowlFlour : BaseItem
	{
		[Constructable]
		public BowlFlour() : base( 0xa1e )
		{
			Weight = 1.0;
		}

		public BowlFlour( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	// ********** WoodenBowl **********
	public class WoodenBowl : BaseItem
	{
		[Constructable]
		public WoodenBowl() : base( 0x15f8 )
		{
			Weight = 1.0;
		}

		public WoodenBowl( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	// ********** PitcherWater **********
	/*public class PitcherWater : BaseItem
	{
		[Constructable]
		public PitcherWater() : base(Utility.Random( 0x1f9d, 2 ))
		{
			Weight = 1.0;
		}

		public PitcherWater( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private PitcherWater m_Item;

			public InternalTarget( PitcherWater item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted ) return;

				if ( targeted is BowlFlour )
				{
					m_Item.Delete();
					((BowlFlour)targeted).Delete();

					from.AddToBackpack( new Dough() );
					from.AddToBackpack( new WoodenBowl() );
				}
			}
		}
	}*/

	// ********** SackFlour **********
	[TypeAlias( "Server.Items.SackFlourOpen" )]
	public class SackFlour : BaseItem, IHasQuantity
	{
		private int m_Quantity;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Quantity
		{
			get{ return m_Quantity; }
			set
			{
				if ( value < 0 )
					value = 0;
				else if ( value > 20 )
					value = 20;

				m_Quantity = value;

				if ( m_Quantity == 0 )
					Delete();
				else if ( m_Quantity < 20 && (ItemID == 0x1039 || ItemID == 0x1045) )
					++ItemID;
			}
		}

		[Constructable]
		public SackFlour() : base( 0x1039 )
		{
			Weight = 1.0;
			m_Quantity = 20;
		}

		public SackFlour( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int) m_Quantity );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Quantity = reader.ReadInt();
					break;
				}
				case 0:
				{
					m_Quantity = 20;
					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( (ItemID == 0x1039 || ItemID == 0x1045) )
				++ItemID;
		}
	}

	// ********** Eggshells **********
	public class Eggshells : BaseItem
	{
		[Constructable]
		public Eggshells() : base( 0x9b4 )
		{
			Weight = 0.5;
		}

		public Eggshells( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
