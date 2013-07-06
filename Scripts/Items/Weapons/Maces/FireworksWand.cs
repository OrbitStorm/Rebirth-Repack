using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class FireworksWand : MagicWand
	{
		//public override int LabelNumber{ get{ return 1041424; } } // a fireworks wand
		public override string BuildSingleClick()
		{
			if ( m_Charges < 100 )
				return String.Format( "a fireworks wand with {0} charge{1}", m_Charges, m_Charges != 1 ? "s" : "" );
			else
				return "a fireworks wand";
		}

		private int m_Charges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[Constructable]
		public FireworksWand() : this( 100 )
		{
		}

		[Constructable]
		public FireworksWand( int charges )
		{
			m_Charges = charges;
			LootType = LootType.Blessed;
		}

		public FireworksWand( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			list.Add( 1060741, m_Charges.ToString() ); // charges: ~1_val~
		}

		public override void OnDoubleClick( Mobile from )
		{
			BeginLaunch( from );
		}

		public void BeginLaunch( Mobile from )
		{
			Map map = from.Map;

			if ( map == null || map == Map.Internal )
				return;

			from.SendLocalizedMessage( 502615 ); // You launch a firework!

			Point3D ourLoc = GetWorldLocation();

			Point3D startLoc = new Point3D( ourLoc.X, ourLoc.Y, ourLoc.Z + 10 );
			Point3D endLoc = new Point3D( startLoc.X + Utility.RandomMinMax( -2, 2 ), startLoc.Y + Utility.RandomMinMax( -2, 2 ), startLoc.Z + 32 );

			Effects.SendMovingEffect( new Entity( Serial.Zero, startLoc, map ), new Entity( Serial.Zero, endLoc, map ),
				0x36E4, 5, 0, false, false );

			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( FinishLaunch ), new object[]{ from, endLoc, map } );
		}

		private void FinishLaunch( object state )
		{
			object[] states = (object[])state;

			Mobile from = (Mobile)states[0];
			Point3D endLoc = (Point3D)states[1];
			Map map = (Map)states[2];

			int hue = Utility.Random( 40 );

			if ( hue < 8 )
				hue = 0x66D;
			else if ( hue < 10 )
				hue = 0x482;
			else if ( hue < 12 )
				hue = 0x47E;
			else if ( hue < 16 )
				hue = 0x480;
			else if ( hue < 20 )
				hue = 0x47F;
			else
				hue = 0;

			if ( Utility.RandomBool() )
				hue = Utility.RandomList( 0x47E, 0x47F, 0x480, 0x482, 0x66D );

			int renderMode = Utility.RandomList( 0, 2, 3, 4, 5, 7 );

			Effects.PlaySound( endLoc, map, Utility.Random( 0x11B, 4 ) );
			Effects.SendLocationEffect( endLoc, map, 0x373A + (0x10 * Utility.Random( 4 )), 16, 10, hue, renderMode );

			Charges--;
			if ( Charges <= 0 )
				Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Charges = reader.ReadInt();
					break;
				}
			}
		}
	}
}
