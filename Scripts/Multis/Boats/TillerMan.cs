using System;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class TillerMan : BaseItem
	{
		private BaseBoat m_Boat;

		public TillerMan( BaseBoat boat ) : base( 0x3E4E )
		{
			m_Boat = boat;
			Movable = false;
		}

		public BaseBoat Boat { get{ return m_Boat; } }

		public TillerMan( Serial serial ) : base(serial)
		{
		}

		public void SetFacing( Direction dir )
		{
			switch ( dir )
			{
				case Direction.South: ItemID = 0x3E4B; break;
				case Direction.North: ItemID = 0x3E4E; break;
				case Direction.West:  ItemID = 0x3E50; break;
				case Direction.East:  ItemID = 0x3E53; break;
			}
		}

		public void Say( int number )
		{
			PublicOverheadMessage( MessageType.Regular, 0x3B2, number );
		}

		public void Say( int number, string args )
		{
			PublicOverheadMessage( MessageType.Regular, 0x3B2, number, args );
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_Boat != null && m_Boat.ShipName != null )
				list.Add( 1042884, m_Boat.ShipName ); // the tiller man of the ~1_SHIP_NAME~
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Boat != null && m_Boat.ShipName != null )
				LabelTo( from, true, "the tiller man of the {0}", m_Boat.ShipName );
			else
				base.OnSingleClick( from );

			if ( m_Boat != null )
			{
				if ( m_Boat.TimeOfDecay == DateTime.MinValue )
				{
					LabelTo( from, true, "This boat is ageless." );
				}
				else
				{
					switch ( (int)( m_Boat.TimeOfDecay - DateTime.Now ).TotalDays )
					{
						case 6:
							LabelTo( from, true, "This boat is like new." ); break;
						case 5: 
						case 4: 
							LabelTo( from, true, "This boat is slightly weathered." ); break;
						case 3: 
							LabelTo( from, true, "This boat is somewhat weathered." ); break;
						case 2: 
							LabelTo( from, true, "This boat is grealy weathered." ); break;
						case 1: 
							LabelTo( from, true, "This boat is barely sea worthy." ); break;
						case 0: 
							LabelTo( from, true, "This boat is in danger of sinking." ); break;
					}
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Boat != null && m_Boat.Contains( from ) )
				m_Boat.BeginRename( from );
			else if ( m_Boat != null )
				m_Boat.BeginDryDock( from );
		}

		public override void OnAfterDelete()
		{
			if ( m_Boat != null )
				m_Boat.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );//version

			writer.Write( m_Boat );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Boat = reader.ReadItem() as BaseBoat;

					if ( m_Boat == null )
						Delete();

					break;
				}
			}
		}
	}
}
