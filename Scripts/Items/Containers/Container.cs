using System;
using System.Collections; using System.Collections.Generic;
using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseContainer : Container
	{
		public BaseContainer( int itemID ) : base( itemID )
		{
		}

		public override bool IsDecoContainer{ get{ return false; } }

		/*public override bool IsAccessibleTo( Mobile m )
		{
			if ( !BaseHouse.CheckAccessible( m, this ) )
				return false;

			return base.IsAccessibleTo( m );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems )
		{
			if ( !BaseHouse.CheckHold( m, this, item, message, checkItems ) )
				return false;

			return base.CheckHold( m, item, message, checkItems );
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );
		}*/

		public override void OnDoubleClick(Mobile from)
		{
			if ( !from.InLOS( GetWorldLocation() ) )
				from.SayTo( from, true, "I cannot see that." );
			else
				base.OnDoubleClick(from);
		}

		public override void OnSingleClick(Mobile from)
		{
			NetState ns = from.NetState;
			if ( ns != null )
			{
				if ( Name == null || Name.Length <= 0 )
				{
					if ( Amount > 1 )
						ns.Send( new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", String.Format( "{0} {1}s", Amount, ItemData.Name ) ) );
					else
						ns.Send( new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", String.Format( "{0} {1}", (ItemData.Flags&TileFlag.ArticleAn) != 0 ? "an" : "a", ItemData.Name ) ) );
				}
				else
				{
					if ( Amount > 1 )
						ns.Send( new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", String.Format( "{0} {1}", Amount, Name ) ) );
					else
						ns.Send( new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", Name ) );
				}
			}

			if ( CheckContentDisplay( from ) )
				BaseItem.LabelTo( this, from, true, "({0} item{1}, {2} stone{3})", TotalItems, TotalItems == 1 ? "" : "s", TotalWeight, TotalWeight == 1 ? "" : "s" );
		}


		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			if ( !CheckHold( from, dropped, sendFullMessage, true ) )
				return false;

			BaseHouse house = BaseHouse.FindHouseAt( this );

			//if ( house != null && house.IsLockedDown( this ) && !house.LockDown( from, dropped, false ) )
			//	return false;

			List<Item> list = this.Items;

			for ( int i = 0; i < list.Count; ++i )
			{
				Item item = (Item)list[i];

				if ( !(item is Container) && item.StackWith( from, dropped, false ) )
					return true;
			}

			DropItem( dropped );

			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			if ( !CheckHold( from, item, true, true ) )
				return false;

			item.Location = new Point3D( p.X, p.Y, 0 );
			AddItem( item );

			from.SendSound( GetDroppedSound( item ), GetWorldLocation() );

			return true;
		}

		public BaseContainer( Serial serial ) : base( serial )
		{
		}

		/* Note: base class insertion; we cannot serialize anything here */
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}

	public class StrongBackpack : Backpack
	{
		[Constructable]
		public StrongBackpack()
		{
			Layer = Layer.Backpack;
			Weight = 3.0;
		}

		public override int MaxWeight{ get{ return 450; } }

		public override bool CheckContentDisplay( Mobile from )
		{
			object root = this.RootParent;

			if ( root is BaseCreature && ((BaseCreature)root).Controled && ((BaseCreature)root).ControlMaster == from )
				return true;

			return base.CheckContentDisplay( from );
		}

		public StrongBackpack( Serial serial ) : base( serial )
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

	public class Backpack : BaseContainer, IDyable
	{
		public override int DefaultGumpID{ get{ return 0x3C; } }
		public override int DefaultDropSound{ get{ return 0x48; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 44, 65, 142, 94 ); }
		}

		[Constructable]
		public Backpack() : base( 0xE75 )
		{
			Layer = Layer.Backpack;
			Weight = 3.0;
		}

		public Backpack( Serial serial ) : base( serial )
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted ) return false;

			Hue = sender.DyedHue;

			return true;
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

	public class Pouch : TrapableContainer
	{
		public override int DefaultGumpID{ get{ return 0x3C; } }
		public override int DefaultDropSound{ get{ return 0x48; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 44, 65, 142, 94 ); }
		}

		[Constructable]
		public Pouch() : base( 0xE79 )
		{
			Weight = 1.0;
		}

		public Pouch( Serial serial ) : base( serial )
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

	public class Bag : BaseContainer, IDyable
	{
		public override int DefaultGumpID{ get{ return 0x3D; } }
		public override int DefaultDropSound{ get{ return 0x48; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 29, 34, 108, 94 ); }
		}

		[Constructable]
		public Bag() : base( 0xE76 )
		{
			Weight = 2.0;
		}

		public Bag( Serial serial ) : base( serial )
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted ) return false;

			Hue = sender.DyedHue;

			return true;
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

	public class Barrel : BaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x3E; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 33, 36, 109, 112 ); }
		}

		[Constructable]
		public Barrel() : base( 0xE77 )
		{
			Weight = 25.0;
		}

		public Barrel( Serial serial ) : base( serial )
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

			if ( Weight == 0.0 )
				Weight = 25.0;
		}
	}

	public class Keg : BaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x3E; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 33, 36, 109, 112 ); }
		}

		[Constructable]
		public Keg() : base( 0xE7F )
		{
			Weight = 15.0;
		}

		public Keg( Serial serial ) : base( serial )
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

	public class PicnicBasket : BaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x3F; } }
		public override int DefaultDropSound{ get{ return 0x4F; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 19, 47, 163, 76 ); }
		}

		[Constructable]
		public PicnicBasket() : base( 0xE7A )
		{
			Weight = 2.0; // Stratics doesn't know weight
		}

		public PicnicBasket( Serial serial ) : base( serial )
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

	public class Basket : BaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x41; } }
		public override int DefaultDropSound{ get{ return 0x4F; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 35, 38, 110, 78 ); }
		}

		[Constructable]
		public Basket() : base( 0x990 )
		{
			Weight = 1.0; // Stratics doesn't know weight
		}

		public Basket( Serial serial ) : base( serial )
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

	[Furniture]
	[Flipable( 0x9AA, 0xE7D )]
	public class WoodenBox : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x43; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 16, 51, 168, 73 ); }
		}

		[Constructable]
		public WoodenBox() : base( 0x9AA )
		{
			Weight = 4.0;
		}

		public WoodenBox( Serial serial ) : base( serial )
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

	[Furniture]
	[Flipable( 0x9A9, 0xE7E )]
	public class SmallCrate : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x44; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public SmallCrate() : base( 0x9A9 )
		{
			Weight = 2.0;
		}

		public SmallCrate( Serial serial ) : base( serial )
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

			if ( Weight == 4.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3F, 0xE3E )]
	public class MediumCrate : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x44; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public MediumCrate() : base( 0xE3F )
		{
			Weight = 2.0;
		}

		public MediumCrate( Serial serial ) : base( serial )
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

			if ( Weight == 6.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3D, 0xE3C )]
	public class LargeCrate : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x44; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public LargeCrate() : base( 0xE3D )
		{
			Weight = 1.0;
		}

		public LargeCrate( Serial serial ) : base( serial )
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

			if ( Weight == 8.0 )
				Weight = 1.0;
		}
	}

	[DynamicFliping]
	[Flipable( 0x9A8, 0xE80 )]
	public class MetalBox : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x4B; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 16, 51, 168, 73 ); }
		}

		[Constructable]
		public MetalBox() : base( 0x9A8 )
		{
			Weight = 3.0; // TODO: Real weight
		}

		public MetalBox( Serial serial ) : base( serial )
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

	[DynamicFliping]
	[Flipable( 0x9AB, 0xE7C )]
	public class MetalChest : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x4A; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 18, 105, 144, 73 ); }
		}

		[Constructable]
		public MetalChest() : base( 0x9AB )
		{
			Weight = 25.0; // TODO: Real weight
		}

		public MetalChest( Serial serial ) : base( serial )
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

	[DynamicFliping]
	[Flipable( 0xE41, 0xE40 )]
	public class MetalGoldenChest : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x42; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 18, 105, 144, 73 ); }
		}

		[Constructable]
		public MetalGoldenChest() : base( 0xE41 )
		{
			Weight = 25.0; // TODO: Real weight
		}

		public MetalGoldenChest( Serial serial ) : base( serial )
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

	[Furniture]
	[Flipable( 0xe43, 0xe42 )]
	public class WoodenChest : LockableContainer
	{
		public override int DefaultGumpID{ get{ return 0x49; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 18, 105, 144, 73 ); }
		}

		[Constructable]
		public WoodenChest() : base( 0xe43 )
		{
			Weight = 15.0; // TODO: Real weight
		}

		public WoodenChest( Serial serial ) : base( serial )
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
