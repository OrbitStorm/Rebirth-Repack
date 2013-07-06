using System;
using System.Collections; using System.Collections.Generic;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class HolidayTree : BaseItem
	{
		private ArrayList m_Components;
		private Mobile m_Placer;

		private class Ornament : BaseItem
		{
			public override int LabelNumber{ get{ return 1041118; } } // a tree ornament

			public Ornament( int itemID ) : base( itemID )
			{
				Movable = false;
			}

			public Ornament( Serial serial ) : base( serial )
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

		public override int LabelNumber{ get{ return 1041117; } } // a tree for the holidays

		public HolidayTree( Mobile from ) : base( 0xCD7 )
		{
			Movable = false;
			MoveToWorld( from.Location, from.Map );

			m_Placer = from;
			m_Components = new ArrayList();

			AddItem( from, 0, 0, 0, new Static( 0xCD6 ) );

			AddOrnament( from,  0,  0, 34, 0xF29 );
			AddOrnament( from,  0,  0, 30, 0xF23 );
			AddOrnament( from, -1,  0, 19, 0xF21 );
			AddOrnament( from,  0,  0, 26, 0xF2F );
			AddOrnament( from,  0,  0, 18, 0xF2A );
			AddOrnament( from, -1,  0, 12, 0xF24 );
			AddOrnament( from,  0, -1, 14, 0xF22 );
			AddOrnament( from,  0,  0, 12, 0xF29 );
			AddOrnament( from,  0, -1, 10, 0xF2F );
			AddOrnament( from, -1,  0,  2, 0xF21 );
			AddOrnament( from,  0, -1, -2, 0xF29 );
			AddOrnament( from, -2,  0, -2, 0xF2A );
			AddOrnament( from,  0,  0,  4, 0xF22 );
		}

		public override void OnAfterDelete()
		{
			foreach ( Item item in m_Components )
			{
				item.Delete();
			}
		}

		private void AddOrnament( Mobile from, int x, int y, int z, int itemID )
		{
			AddItem( from, x + 1, y + 1, z + 11, new Ornament( itemID ) );
		}

		private void AddItem( Mobile from, int x, int y, int z, Item item )
		{
			item.MoveToWorld( new Point3D( from.Location.X + x, from.Location.Y + y, from.Location.Z + z ), from.Map );

			m_Components.Add( item );
		}

		public HolidayTree( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_Placer );

			writer.Write( (int) m_Components.Count );

			for ( int i = 0; i < m_Components.Count; ++i )
				writer.Write( (Item)m_Components[i] );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Placer = reader.ReadMobile();

					goto case 0;
				}
				case 0:
				{
					int count = reader.ReadInt();

					m_Components = new ArrayList( count );

					for ( int i = 0; i < count; ++i )
					{
						Item item = reader.ReadItem();

						if ( item != null )
							m_Components.Add( item );
					}

					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if ( m_Placer == null || from == m_Placer )
				{
					from.AddToBackpack( new HolidayTreeDeed() );

					this.Delete();

					from.SendAsciiMessage( "You take the tree down." );
				}
				else
				{
					from.SendAsciiMessage( "You can not take that tree down." );
				}
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}
	}
}