using System;
using System.Text;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System.Text.RegularExpressions;

namespace Server
{
	public class BaseItem : Item
	{
		private static Regex m_PluralRegEx = new Regex( @"([^%]+)%([^%/ ]+)(/([^% ]+))*%*([^%]*)", RegexOptions.Compiled|RegexOptions.Singleline );
		
		private Packet m_SingleClick = null;

		public virtual void AppendClickName( StringBuilder sb )
		{
			if ( Name == null || Name.Length <= 0 )
			{
				bool plural = Amount != 1;

				// bread loa%ves/f%, black pearl%s%, log%s, etc
				Match match = m_PluralRegEx.Match( ItemData.Name );
				if ( match.Success )
				{
					if ( match.Groups[1].Value.Length > 0 )
						sb.Append( match.Groups[1].Value );

					if ( plural )
					{
						if ( match.Groups[2].Success && match.Groups[2].Value.Length > 0 )
							sb.Append( match.Groups[2].Value );
					}
					else
					{
						if ( match.Groups[4].Success && match.Groups[4].Value.Length > 0 )
							sb.Append( match.Groups[4].Value );
					}

					if ( match.Groups[5].Value.Length > 0 )
						sb.Append( match.Groups[5].Value );
				}
				else
				{
					sb.Append( ItemData.Name );
					if ( plural && ItemID == 0x0EED )// gold coinS dont ever get the s (unless we put it there <--)
						sb.Append( 's' );
				}
			}
			else
			{
				sb.Append( Name );
				if ( Amount != 1 && ItemID != 0x2006 )
					sb.Append( 's' );
			}
		}

		public virtual void InsertNamePrefix( StringBuilder sb )
		{
			//while ( sb.Length > 0 && sb[0] == ' ' )
			//	sb.Remove( 0, 1 );

			if ( Name != null && Name.Length > 0 )
				return;

			if ( Amount == 1 && sb.Length > 0 && Char.IsLetter( sb[0] ) && ( (ItemData.Flags&TileFlag.ArticleAn) != 0 || (ItemData.Flags&TileFlag.ArticleA) != 0 ) )
			{
				switch ( Char.ToUpper( sb[0] ) )
				{
					case 'A':
					case 'E':
					case 'I':
					case 'O':
					case 'U':
					case 'Y':
						sb.Insert( 0, "an " );
						break;
					default:
						sb.Insert( 0, "a " );
						break;
				}
			}
		}

		public bool AppendLootType( StringBuilder sb )
		{
			if ( DisplayLootType )
			{
				switch ( LootType )
				{
					case LootType.Blessed:
						sb.Append( "blessed" );
						return true;
					case LootType.Cursed:
						sb.Append( "cursed" );
						return true;
				}
			}

			return false;
		}

		public virtual string BuildSingleClick()
		{
			StringBuilder sb = new StringBuilder();
					
			if ( Amount != 1 && ItemID != 0x2006 )
				sb.AppendFormat( "{0} ", Amount );

			if ( AppendLootType( sb ) )
				sb.Append( " " );
			AppendClickName( sb );
			InsertNamePrefix( sb );

			return sb.ToString();
		}

		public virtual void SingleClickChanged()
		{
			Packet.Release( ref m_SingleClick );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( this is SpellScroll || ( LabelNumber != (1020000 + (ItemID & 0x3FFF)) && ( Name == null || Name.Length <= 0 ) ) )
			{
				// scroll names are all fucked up... also just use the default single click for items with localized names (non-defualt)
				base.OnSingleClick( from );
			} 
			else if ( !Deleted && from.CanSee( this ) && from.NetState != null )
			{
				SendSingleClickTo( from );
			}
		}

		public virtual void SendSingleClickTo( Mobile from )
		{
			if ( m_SingleClick == null )
			{
				m_SingleClick = new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", BuildSingleClick() );
				m_SingleClick.SetStatic();
			}

			from.NetState.Send( m_SingleClick );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override string Name
		{
			get	
			{
				return base.Name;	
			}
			set
			{
				if ( Name != value )
				{
					SingleClickChanged();
					base.Name = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int ItemID
		{
			get
			{
				return base.ItemID;
			}
			set
			{
				if ( ItemID != value )
				{
					if ( Name == null || Name.Length <= 0 )
						SingleClickChanged();
					base.ItemID = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile MobileParent { get { return Parent as Mobile; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Item ItemParent { get { return Parent as Item; } }

		[CommandProperty( AccessLevel.Seer )]
		public Mobile MobileRoot { get { return RootParent as Mobile; } }

		protected override void OnAmountChange( int oldValue )
		{
			SingleClickChanged();
			base.OnAmountChange( oldValue );
		}

		public override bool IsVirtualItem{ get{ return !Visible; } } // dont count invis things in countainer weights/items

		public void PublicLOSMessage( MessageType type, int hue, bool ascii, string str, params object[] args )
		{
			PublicLOSMessage( type, hue, ascii, String.Format( str, args ) );
		}

		public void PublicLOSMessage( MessageType type, int hue, bool ascii, string text )
		{
			Packet p = null;
			Point3D worldLoc = GetWorldLocation();

			IPooledEnumerable eable = GetClientsInRange( GetMaxUpdateRange() );
			foreach ( NetState state in eable )
			{
				Mobile m = state.Mobile;

				if ( m.CanSee( this ) && m.InRange( worldLoc, GetUpdateRange( m ) ) && this.Map.LineOfSight( this.Map.GetPoint( m, true ), worldLoc ) )
				{
					if ( p == null )
					{
						if ( ascii )
							p = new AsciiMessage( Serial, ItemID, type, hue, 3, Name, text );
						else
							p = new UnicodeMessage( Serial, ItemID, type, hue, 3, "ENU", Name, text );
						p.SetStatic();
					}
					state.Send( p );
				}
				Packet.Release( ref p );
			}
			eable.Free();
		}

		public void AsciiLabelTo( Mobile to, string label )
		{
			to.Send( new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", label ) );
		}

		public void AsciiLabelTo( Mobile to, string label, params object[] args )
		{
			to.Send( new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", String.Format( label, args ) ) );
		}
		
		public void LabelTo( Mobile to, bool ascii, string label )
		{
			if ( ascii )
				to.Send( new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", label ) );
			else
				LabelTo( to, label );
		}

		public void LabelTo( Mobile to, bool ascii, string label, params object[] args )
		{
			LabelTo( to, ascii, String.Format( label, args ) );
		}

		public static void LabelTo( Item i, Mobile to, bool ascii, string label )
		{
			if ( ascii )
				to.Send( new AsciiMessage( i.Serial, i.ItemID, MessageType.Label, 0x3B2, 3, "", label ) );
			else
				i.LabelTo( to, label );
		}

		public static void LabelTo( Item i, Mobile to, bool ascii, string label, params object[] args )
		{
			LabelTo( i, to, ascii, String.Format( label, args ) );
		}

		public BaseItem( int itemid ) : base( itemid )
		{
		}

		public BaseItem() : base()
		{
		}

		public BaseItem( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( (int)0 ); // version
		}
	}
}
