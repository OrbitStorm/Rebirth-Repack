using System;
using Server;
using Server.Targeting;
using System.Collections; using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
	public class KeyRing : Container
	{
		[Constructable]
		public KeyRing() : base( 0x1011 )
		{
			Weight = 1.0;
			//TotalItems = TotalWeight = 0;
			MaxItems = 25;
		}

		public override bool IsDecoContainer
		{
			get
			{
				return false;
			}
		}

		public KeyRing( Serial serial ) : base( serial )
		{
		}

		public override bool IsVirtualItem { get { return true; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			//TotalItems = TotalWeight = 0;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( !IsChildOf( from, true ) )
			{
				from.SendAsciiMessage( "That must be in your backpack to use it." );
				return;
			}
			else if ( !from.InRange( GetWorldLocation(), 3 ) || !from.InLOS( GetWorldLocation() ) )
			{
				from.SendAsciiMessage( "You cannot reach that." );
				return;
			}

			from.BeginTarget( 10, false, TargetFlags.None, new TargetCallback( OnTarget ) );
			if ( from.Target != null )
			{
				from.Target.CheckLOS = false;
				from.Target.BeginTimeout( from, TimeSpan.FromSeconds( 15.0 ) );
			}
		}

		public override void UpdateTotals()
		{
		}

        public override int GetTotal(TotalType type)
        {
            switch (type)
            {
                default:
                    return 0;
                case TotalType.Items:
                    return 1;
                case TotalType.Weight:
                    return 1;
            }
        }
        
		public override void OnSingleClick(Mobile from)
		{
			BaseItem.LabelTo( this, from, true, "a key ring" );
			BaseItem.LabelTo( this, from, true, "({0} key{1})", Items.Count, Items.Count != 1 ? "s" : "" );
		}

		public void OnTarget( Mobile from, object target )
		{
			object root = this.RootParent;
			if ( root != from && root != null )
				return;

			bool used = false;
			if ( target == this )
			{
				Container parent = this.Parent as Container;
				if ( parent == null )
					return;
				ArrayList list = new ArrayList( this.Items );
				for(int i=0;i<list.Count;i++)
				{
					if ( !parent.TryDropItem( from, (Item)list[i], false ) )
						((Item)list[i]).MoveToWorld( from.Location, from.Map );
				}
				UpdateItemID();
				//TotalItems = TotalWeight = 0;
				used = true;
			}
			else if ( target is HouseSign )
			{
				HouseSign sign = (HouseSign)target;
				if ( sign.Owner != null )
				{
					foreach ( Key k in this.Items )
					{
						if ( k.KeyValue == sign.Owner.KeyValue )
						{
							used = true;
							if ( from.InRange( sign, k.MaxRange ) )
							{
								from.Prompt = new Prompts.HouseRenamePrompt( sign.Owner );
								from.SendLocalizedMessage( 1060767 ); // Enter the new name of your house.
							}
							else
							{
								from.LocalOverheadMessage( Network.MessageType.Label, 0x3b2, true, "That is too far away." );
							}
							break;
						}
					}
				}
			}
			else if ( target is ILockable && target is Item )
			{
				ILockable able = (ILockable)target;
				foreach ( Key k in this.Items )
				{
					if ( k.KeyValue == able.KeyValue )
					{
						new Key.UnlockTarget( k ).Invoke( from, able );
						used = true;
						break;
					}
				}
			}

			if ( !used )
				from.SendAsciiMessage( "None of these keys seem to unlock that." );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Key && ((Key)dropped).KeyValue != 0 )
			{
				bool ret = base.OnDragDrop( from, dropped );
				//TotalItems = TotalWeight = 0;
				return ret;
			}
			else
			{
				//TotalItems = TotalWeight = 0;
				return false;
			}
		}

		public override bool CheckLift(Mobile from, Item item, ref Server.Network.LRReason reject )
		{
			if ( item.Parent == this )
				return false;
			else
				return base.CheckLift (from, item,ref  reject);
		}

		public override void OnItemAdded(Item item)
		{
			base.OnItemAdded(item);
			UpdateItemID();
			//TotalItems = TotalWeight = 0;
		}

		public override void OnItemRemoved(Item item)
		{
			base.OnItemRemoved (item);
			UpdateItemID();
			//TotalItems = TotalWeight = 0;
		}

		public void UpdateItemID()
		{
			if ( this.Items.Count <= 0 )
				this.ItemID = 0x1011;
			else if ( this.Items.Count < 3 )
				this.ItemID = 0x1769;
			else if ( this.Items.Count < 5 )
				this.ItemID = 0x176A;
			else 
				this.ItemID = 0x176B;
		}
	}
}

