using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xe80, 0x9a8 )]
	public class StrongBox : BaseContainer, IChopable
	{
		private static Hashtable m_Table = new Hashtable();
		public static Hashtable Table { get { return m_Table; } }

		private Mobile m_Owner;
		private BaseHouse m_House;

		public override int DefaultGumpID{ get{ return 0x4B; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 16, 51, 168, 73 ); }
		}

		public StrongBox( Mobile owner, BaseHouse house ) : base( 0xE80 )
		{
			Movable = false;

			m_Owner = owner;
			m_House = house;

			if ( m_Owner != null && !m_Owner.Deleted )
			{
				ArrayList list = m_Table[m_Owner] as ArrayList;
				if ( list == null )
					m_Table[m_Owner] = list = new ArrayList( 1 );
				list.Add( this );
			}

			MaxItems = 25;

			Name = String.Format( "a strong box owned by {0}", owner.Name );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get
			{
				return m_Owner;
			}
			set
			{
				if ( m_Owner != value )
				{
					ArrayList list;
					if ( m_Owner != null )
					{
						list = m_Table[m_Owner] as ArrayList;
						if ( list != null )
						{
							list.Remove( this );
							if ( list.Count <= 0 )
								m_Table.Remove( m_Owner );
						}
					}

					m_Owner = value;

					if ( m_Owner != null && !m_Owner.Deleted )
					{
						Name = String.Format( "a strong box owned by {0}", m_Owner.Name );

						list = m_Table[m_Owner] as ArrayList;
						if ( list == null )
							m_Table[m_Owner] = list = new ArrayList( 1 );
						list.Add( this );
					}

					InvalidateProperties();
				}
			}
		}

		public override int DefaultMaxWeight{ get{ return 800; } }

		public StrongBox( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Owner );
			writer.Write( m_House );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Owner = reader.ReadMobile();
					m_House = reader.ReadItem() as BaseHouse;

					break;
				}
			}

			if ( m_Owner != null )
			{
				ArrayList list = m_Table[m_Owner] as ArrayList;
				if ( list == null )
					m_Table[m_Owner] = list = new ArrayList( 1 );
				list.Add( this );
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick (from);

			if ( !IsOkay )
				BaseItem.LabelTo( this, from, true, "(improperly placed)" );
		}

		public override bool DisplaysContent
		{
			get
			{
				return false;
			}
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete ();

			if ( m_Owner != null )
			{
				ArrayList list = m_Table[m_Owner] as ArrayList;

				if ( list != null )
				{
					list.Remove( this );

					if ( list.Count <= 0 )
						m_Table.Remove( m_Owner );
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( !IsOkay && from == m_Owner )
			{
				ArrayList list = m_Table[m_Owner] as ArrayList;
				if ( list != null && list.Count > 1 )
					from.SendAsciiMessage( "Warning, you own more than one strong box.  Therefore, they are all accessable to anyone!" );
				else
					from.SendAsciiMessage( "Warning, this strong box has been improperly placed and is accessable to anyone!" );
			}

			base.OnDoubleClick (from);
		}

		public override bool Decays
		{
			get
			{
				return m_House == null || m_House.Deleted || m_Owner == null || m_Owner.Deleted || !m_House.Contains( this );
			}
		}

		public bool IsOkay
		{
			get 
			{
				if ( m_Owner == null || m_Owner.Deleted || m_House == null || m_House.Deleted || !m_House.Contains( this ) )
					return false;

				ArrayList list = m_Table[m_Owner] as ArrayList;
				if ( list != null && list.Count > 1 )
					return false;

				IPooledEnumerable eable = GetItemsInRange( 18 );
				foreach ( Item i in eable )
				{
					if ( i is StrongBox && i != this )
					{
						if ( m_House.Contains( i ) )
							return false;
					}
				}
				eable.Free();

				return true;
			}
		}

		public override bool IsAccessibleTo( Mobile m )
		{
			return ( m == m_Owner || m.AccessLevel >= AccessLevel.GameMaster || !IsOkay ) && base.IsAccessibleTo( m );
		}

		public void OnChop(Mobile from)
		{
			if ( !IsOkay && from.InLOS( this ) )
			{
				bool ok = m_House == null || m_House.Deleted || m_Owner == from;

				if ( !ok )
				{
					IPooledEnumerable eable = GetItemsInRange( 1 );
					int count = 0;
					foreach ( Item i in eable )
					{
						if ( i is StrongBox && i != this )
						{
							count++;
							if ( count > 1 )
							{
								ok = true;
								break;
							}
						}
					}
					eable.Free();
				}

				if ( ok )
				{
					if ( Utility.Random( 10 ) != 0 )
						from.SendAsciiMessage( "You try to break the strong box, but fail to damage it." );
					else
						this.Destroy();
				}
			}
		}
	}

	public class StrongBoxDeed : BaseItem
	{
		[Constructable]
		public StrongBoxDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "a deed to a strong box";
			//LootType = LootType.Newbied;
		}

		public StrongBoxDeed( Serial serial ) : base( serial )
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
			ArrayList list = StrongBox.Table[from] as ArrayList;
			if ( list != null && list.Count > 1 )
			{
				from.SendAsciiMessage( "You already own a strong box, you cannot place another!" );
				return;
			}

			BaseHouse h = BaseHouse.FindHouseAt( from );
			if ( h != null && h.IsOwner( from ) && !(h is Tent) && h.IsInside( from ) )
			{
				IPooledEnumerable eable = h.GetItemsInRange( 18 );
				foreach ( Item i in eable )
				{
					if ( h.IsInside( i ) )
					{
						if ( i is BaseDoor && ( i.X == from.X || i.Y == from.Y ) && from.GetDistanceToSqrt( i ) < 2 && i.Z-5 < from.Z && i.Z+5 > from.Z )
						{
							from.SendAsciiMessage( "You cannot place this in front of a door." );
							eable.Free();
							return;
						}
						else if ( i is StrongBox )
						{
							from.SendAsciiMessage( "There is already a strong box in this house." );
							eable.Free();
							return;
						}
					}
				}
				eable.Free();
				
				StrongBox s = new StrongBox( from, h );
				s.MoveToWorld( from.Location, from.Map );
				this.Delete();
			}
			else
			{
				from.SendAsciiMessage( "You must be in a house you own in order to place a strong box." );
			}
		}
	}
}
