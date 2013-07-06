using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Items
{
	public class Ankhs
	{
		public const int ResurrectRange = 2;
		public const int TitheRange = 2;
		public const int LockRange = 2;

        public static void GetContextMenuEntries(Mobile from, Item item, List<ContextMenus.ContextMenuEntry> list)
		{
		}

		public static void Resurrect( Mobile m, Item item, bool reds )
		{
			if ( m.Alive )
				return;

			if ( !m.InRange( item.GetWorldLocation(), ResurrectRange ) )
				m.SendLocalizedMessage( 500446 ); // That is too far away.
			else if ( m.Map != null && m.Map.CanFit( m.Location, 16, false, false ) && ( reds || m.Karma > (int)Noto.Dark ) )
				m.SendMenu( new ResurrectMenu( m, reds ? ResurrectMessage.ChaosShrine : ResurrectMessage.VirtueShrine ) );
			else
				m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
		}
	}

	public class AnkhWest : BaseItem
	{
		private InternalItem m_Item;

		private bool m_Reds;
		[CommandProperty( AccessLevel.GameMaster, AccessLevel.GameMaster )]
		public bool Reds { get { return m_Reds; } set { m_Reds = value; } }

		[Constructable]
		public AnkhWest() : this( false )
		{
		}

		[Constructable]
		public AnkhWest( bool bloodied ) : base( bloodied ? 0x1D98 : 0x3 )
		{
			Movable = false;

			m_Item = new InternalItem( bloodied, this );
		}

		public AnkhWest( Serial serial ) : base( serial )
		{
		}

		public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( Parent == null && Utility.InRange( Location, m.Location, 1 ) && !Utility.InRange( Location, oldLocation, 1 ) )
				Ankhs.Resurrect( m, this, m_Reds );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			Ankhs.GetContextMenuEntries( from, this, list );
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
		}

		public override void OnDoubleClickDead( Mobile m )
		{
			Ankhs.Resurrect( m, this, m_Reds );
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			if ( m_Item != null )
				m_Item.Location = new Point3D( X, Y + 1, Z );
		}

		public override void OnMapChange()
		{
			if ( m_Item != null )
				m_Item.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Item != null )
				m_Item.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_Reds );

			writer.Write( m_Item );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 1 )
				m_Reds = reader.ReadBool();

			m_Item = reader.ReadItem() as InternalItem;
		}

		private class InternalItem : BaseItem
		{
			private AnkhWest m_Item;

			public InternalItem( bool bloodied, AnkhWest item ) : base( bloodied ? 0x1D97 : 0x2 )
			{
				Movable = false;

				m_Item = item;
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void OnLocationChange( Point3D oldLocation )
			{
				if ( m_Item != null )
					m_Item.Location = new Point3D( X, Y - 1, Z );
			}

			public override void OnMapChange()
			{
				if ( m_Item != null )
					m_Item.Map = Map;
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Item != null )
					m_Item.Delete();
			}

			public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

			public override void OnMovement( Mobile m, Point3D oldLocation )
			{
				if ( Parent == null && Utility.InRange( Location, m.Location, 1 ) && !Utility.InRange( Location, oldLocation, 1 ) )
					Ankhs.Resurrect( m, this, m_Item.m_Reds );
			}

			public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
			{
				base.GetContextMenuEntries( from, list );
				Ankhs.GetContextMenuEntries( from, this, list );
			}

			[Hue, CommandProperty( AccessLevel.GameMaster )]
			public override int Hue
			{
				get{ return base.Hue; }
				set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
			}

			public override void OnDoubleClickDead( Mobile m )
			{
				Ankhs.Resurrect( m, this, m_Item.m_Reds );
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( m_Item );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Item = reader.ReadItem() as AnkhWest;
			}
		}
	}

	public class AnkhEast : BaseItem
	{
		private InternalItem m_Item;

		private bool m_Reds;
		[CommandProperty( AccessLevel.GameMaster, AccessLevel.GameMaster )]
		public bool Reds { get { return m_Reds; } set { m_Reds = value; } }

		[Constructable]
		public AnkhEast() : this( false )
		{
		}

		[Constructable]
		public AnkhEast( bool bloodied ) : base( bloodied ? 0x1E5D : 0x4 )
		{
			Movable = false;

			m_Item = new InternalItem( bloodied, this );
		}

		public AnkhEast( Serial serial ) : base( serial )
		{
		}

		public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( Parent == null && Utility.InRange( Location, m.Location, 1 ) && !Utility.InRange( Location, oldLocation, 1 ) )
				Ankhs.Resurrect( m, this, m_Reds );
		}

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries( from, list );
			Ankhs.GetContextMenuEntries( from, this, list );
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
		}

		public override void OnDoubleClickDead( Mobile m )
		{
			Ankhs.Resurrect( m, this, m_Reds );
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			if ( m_Item != null )
				m_Item.Location = new Point3D( X + 1, Y, Z );
		}

		public override void OnMapChange()
		{
			if ( m_Item != null )
				m_Item.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Item != null )
				m_Item.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_Reds );

			writer.Write( m_Item );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 1 )
				m_Reds = reader.ReadBool();

			m_Item = reader.ReadItem() as InternalItem;
		}

		private class InternalItem : BaseItem
		{
			private AnkhEast m_Item;

			public InternalItem( bool bloodied, AnkhEast item ) : base( bloodied ? 0x1E5C : 0x5 )
			{
				Movable = false;

				m_Item = item;
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void OnLocationChange( Point3D oldLocation )
			{
				if ( m_Item != null )
					m_Item.Location = new Point3D( X - 1, Y, Z );
			}

			public override void OnMapChange()
			{
				if ( m_Item != null )
					m_Item.Map = Map;
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Item != null )
					m_Item.Delete();
			}

			public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

			public override void OnMovement( Mobile m, Point3D oldLocation )
			{
				if ( Parent == null && Utility.InRange( Location, m.Location, 1 ) && !Utility.InRange( Location, oldLocation, 1 ) )
					Ankhs.Resurrect( m, this, m_Item.m_Reds );
			}

            public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
			{
				base.GetContextMenuEntries( from, list );
				Ankhs.GetContextMenuEntries( from, this, list );
			}

			[Hue, CommandProperty( AccessLevel.GameMaster )]
			public override int Hue
			{
				get{ return base.Hue; }
				set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
			}

			public override void OnDoubleClickDead( Mobile m )
			{
				Ankhs.Resurrect( m, this, m_Item.m_Reds );
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( m_Item );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Item = reader.ReadItem() as AnkhEast;
			}
		}
	}
}
