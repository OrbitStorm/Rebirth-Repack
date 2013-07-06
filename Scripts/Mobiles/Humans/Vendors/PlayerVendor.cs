using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Prompts;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	public class VendorItem
	{
		private int m_Price;
		private string m_Description;
		private Item m_Item;
		
		public VendorItem( Item item, int price, string description )
		{
			m_Item = item;
			m_Price = price;
			m_Description = description;
		}
		
		public Item Item{ get{ return m_Item; } }
		public int Price{ get{ return m_Price; } set{ m_Price = value; } }
		public string Description{ get{ return m_Description; } set{ m_Description = value; } }
		
		public bool IsForSale{ get{ return ( m_Item != null && !m_Item.Deleted && m_Price > 0 ); } }
	}
	
	public class VendorBackpack : Backpack
	{
		[Constructable]
		public VendorBackpack()
		{
			Layer = Layer.Backpack;
			Weight = 1.0;
			MaxItems = 500;
		}

		public override int MaxWeight{ get{ return 0; } }

		public override bool CheckContentDisplay( Mobile from )
		{
			object root = this.RootParent;

			if ( root is PlayerVendor && ((PlayerVendor)root).IsOwner( from ) )
				return true;
			else
				return base.CheckContentDisplay( from );
		}
		
		public override bool IsAccessibleTo( Mobile m )
		{
			return true;
		}

		public override bool CheckItemUse( Mobile from, Item item )
		{
			if ( !base.CheckItemUse( from, item ) )
				return false;

			object root = this.RootParent;

			if ( root is PlayerVendor && ((PlayerVendor)root).IsOwner( from ) )
				return true;

			if ( item is Container )
				return true;

			from.SendLocalizedMessage( 500447 ); // That is not accessible.
			return false;
		}
		
		public override bool CheckTarget( Mobile from, Target targ, object targeted )
		{
			if ( !base.CheckTarget( from, targ, targeted ) )
				return false;

			object root = this.RootParent;

			if ( root is PlayerVendor && ((PlayerVendor)root).IsOwner( from ) )
				return true;

			return ( targ is PlayerVendor.PVBuyTarget );
		}

		public override void OnSingleClickContained( Mobile from, Item item )
		{
			if ( RootParent is PlayerVendor )
			{
				PlayerVendor vend = (PlayerVendor)RootParent;
				
				VendorItem vi = (VendorItem)vend.SellItems[item];
				
				if ( vi != null )
				{
					if ( vi.IsForSale )
						BaseItem.LabelTo( item, from, true,"Price: {0}gp", vi.Price );//item.LabelTo( from, 1043304, vi.Price.ToString() ); // Price: ~1_COST~
					else
						BaseItem.LabelTo( item, from, true,"Price: Not for sale." );//item.LabelTo( from, 1043307 ); // Price: Not for sale.
					
					if ( vi.Description != null && vi.Description != "" )
						BaseItem.LabelTo( item, from, true, "Description: {0}", vi.Description );//item.LabelTo( from, 1043305, vi.Description ); // Description: ~1_DESC~
				}
			}
			
			base.OnSingleClickContained( from, item );
		}

		public VendorBackpack( Serial serial ) : base( serial )
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
	
	public class PlayerVendor : Mobile
	{
		private Hashtable m_SellItems;
		private Mobile m_Owner;
		private int m_BankAccount;
		private int m_HoldGold;
		private Timer m_PayTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }
		
		public PlayerVendor( Mobile owner )
		{
			m_Owner = owner;

			m_BankAccount = 1000;
			m_HoldGold = 0;
			m_SellItems = new Hashtable();

			CantWalk = true;
			Blessed = true;

			InitStats( 75, 75, 75 );
			InitBody();
			InitOutfit();

			m_PayTimer = new PayTimer( this );
			m_PayTimer.Start();
		}
		
		public PlayerVendor( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int)0 );//version
			writer.Write( m_Owner );
			writer.Write( m_BankAccount );
			writer.Write( m_HoldGold );
			
			ArrayList list = new ArrayList( m_SellItems.Values );
			
			writer.Write( list.Count );
			for (int i=0;i<list.Count;i++)
			{
				VendorItem vi = (VendorItem)list[i];
				writer.Write( vi.Item );
				writer.Write( vi.Price );
				writer.Write( vi.Description );
			}
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
					m_BankAccount = reader.ReadInt();
					m_HoldGold = reader.ReadInt();
					
					int count = reader.ReadInt();
					m_SellItems = new Hashtable();
					for (int i=0;i<count;i++)
					{
						Item item = reader.ReadItem();
						int p = reader.ReadInt();
						string d = reader.ReadString();
						
						if ( item != null && !item.Deleted )
							m_SellItems[item] = new VendorItem( item, p, d );
					}
					break;	
				}
			}
			
			m_PayTimer = new PayTimer( this );
			m_PayTimer.Start();
			Blessed = true;
		}
		
		public void InitBody()
		{
			Hue = Utility.RandomSkinHue();
			SpeechHue = 0x3B2;

			if ( this.Female = Utility.RandomBool() )
			{
				this.Body = 0x191;
				this.Name = NameList.RandomName( "female" );
			}
			else
			{
				this.Body = 0x190;
				this.Name = NameList.RandomName( "male" );
			}
		}
		
		public virtual void InitOutfit()
		{
			AddItem( new FancyShirt( Utility.RandomNeutralHue() ) );
			AddItem( new LongPants( Utility.RandomNeutralHue() ) );
			AddItem( new BodySash( Utility.RandomNeutralHue() ) );
			AddItem( new Boots( Utility.RandomNeutralHue() ) );
			AddItem( new Cloak( Utility.RandomNeutralHue()) );

			Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ) );
			hair.Hue = Utility.RandomNondyedHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );

			Container pack = new VendorBackpack();
			pack.Movable = false;
			AddItem( pack );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Deleted )
				return;

			string str = null;
			if ( Blessed )
				str = String.Format( "{0} (invulnerable)", Name );
			else
				str = Name;
			PrivateOverheadMessage( Network.MessageType.Label, Notoriety.GetHue( Notoriety.Compute( from, this ) ), Mobile.AsciiClickMessage, str, from.NetState );
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int BankAccount
		{
			get { return m_BankAccount; }
			set { m_BankAccount = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int HoldGold
		{
			get { return m_HoldGold; }
			set { m_HoldGold = value; }
		}

		public int ChargePerDay
		{
			get
			{ 
				int total = 0;
				foreach (VendorItem v in m_SellItems.Values)
					total += v.Price;

				total /= 500;

				if ( total < 2 )
					total = 2;

				return total;
			}
		}

		public Hashtable SellItems { get{ return m_SellItems; } }
		
		public bool IsOwner( Mobile m )
		{
			return ( m == m_Owner || m.AccessLevel >= AccessLevel.GameMaster );
		}

		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;

			Item shoes = this.FindItemOnLayer( Layer.Shoes );

			if ( shoes is Sandals )
				shoes.Hue = 0;

			return true;
		}
		
		public override void OnAfterDelete()
		{
			m_PayTimer.Stop();
		}
		
		public override bool IsSnoop( Mobile from )
		{
			return false;
		}
		
		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( IsOwner( from ) )
			{
				if ( item is Gold )
				{
					SayTo( from, 503210 ); // I'll take that to fund my services.
					m_BankAccount += item.Amount;
					item.Delete();
					return true;
				}
				else
				{
					Container pack = Backpack;

					if ( pack != null && pack.TryDropItem( from, item, false ) )
					{
						GiveItem( from, item, null );
						return true;
					}
					else
					{
						SayTo( from, 503211 ); // I can't carry any more.
						return false;
					}
				}
			}
			else
			{
				SayTo( from, 503209 );// I can only take item from the shop owner.
				return false;
			}
		}
		
		private void GiveItem( Mobile from, Item item, Item target )
		{
			if ( !(target is Container) )
				target = null;

			if ( target != null )
			{
				VendorItem targetVI = (VendorItem) m_SellItems[target];

				if ( targetVI != null && targetVI.IsForSale )
					return;
			}

			VendorItem vi;
			string name;

			vi = AddInfo( item, target );

			if ( item.Name != null && item.Name != "" )
				name = item.Name;
			else
				name = item.ItemData.Name;

			from.SendLocalizedMessage( 1043303, name  ); // Type in a price and description for ~1_ITEM~ (ESC=not for sale)
			from.Prompt = new VendorPricePrompt( this, item, target, vi );	
		}
		
		private VendorItem AddInfo( Item item, Item target )
		{
			int defPrice = 999;
			VendorItem vi;
			
			if ( item is BaseBook || ( item is Container && item.Items.Count == 0 ) || item is KeyRing )
			{
				defPrice = 0;
			}
			else if ( target != null )
			{
				vi = (VendorItem)m_SellItems[target];
				
				if ( vi != null && vi.Price > 0 )
					defPrice = 0;
			}
			
			vi = new VendorItem( item, defPrice, "" );
			m_SellItems[item] = vi;

			if ( defPrice == 0 )
			{
				for (int i=0;i<item.Items.Count;i++)
					AddInfo( (Item)item.Items[i], null );
			}

			item.InvalidateProperties();
			
			return vi;
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			if ( IsOwner( from ) )
			{
				GiveItem( from, item, target );
				return true;
			}
			else
			{
				SayTo( from, 503209 );// I can only take item from the shop owner.
				return false;
			}
		}
		
		public void RemoveInfo( Item item )
		{
			m_SellItems.Remove( item );
			for (int i=0;i<item.Items.Count;i++)
				RemoveInfo( (Item)item.Items[i] );

			item.InvalidateProperties();
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			if ( IsOwner( from ) )
			{
				RemoveInfo( item );
				return true;
			}
			else
			{
				SayTo( from, 503223 );// If you'd like to purchase an item, just ask.
				return false;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsOwner( from ) )
			{
				from.SendGump( new PlayerVendorOwnerGump( this, from ) );
			}
			else
			{
				Container pack = Backpack;

				if ( pack != null )
				{
					SayTo( from, 503208 );// Take a look at my goods.
					pack.DisplayTo( from );
				}
			}
		}

		public override void GetChildProperties( ObjectPropertyList list, Item item )
		{
			base.GetChildProperties( list, item );

			VendorItem vi = (VendorItem)m_SellItems[item];

			if ( vi != null )
			{
				if ( !vi.IsForSale )
					list.Add( 1043307 ); // Price: Not for sale.
				else if ( vi.Price <= 0 )
					list.Add( 1043306 ); // Price: FREE!
				else
					list.Add( 1043304, vi.Price.ToString() ); // Price: ~1_COST~

				if ( vi.Description != null && vi.Description.Length > 0 )
					list.Add( 1043305, vi.Description ); // Description: ~1_DESC~
			}
		}

		public void GiveGold( Mobile from )
		{
			if ( m_HoldGold > 0 )
			{
				Item item = null;

				if ( m_HoldGold < 65000 )
				{
					item = new Gold( m_HoldGold );
					m_HoldGold = 0;
				}
				else
				{
					item = new Gold( 65000 );
					m_HoldGold -= 65000;
				}
				
				if ( item != null )
				{
					from.BankBox.DropItem( item );
					SayTo( from, 503234 ); // All the gold I have been carrying for you has been deposited into your bank account.
				}
			}
			else
			{
				SayTo( from, 503215 ); // I am holding no gold for you.
			}
		}
		
		private void Dismiss( Mobile from )
		{
			Container pack = this.Backpack;

			if ( pack != null && pack.Items.Count > 0 )
			{
				SayTo( from, 503229 ); // Thou canst replace me until thy removest all the item from my stock.
				return;
			}

			while ( m_HoldGold > 0 )
				GiveGold( from );

			Say( 503235 ); // I regret nothing!postal
			Blessed = false;
			Kill();
		}
		
		public override void DisplayPaperdollTo( Mobile m )
		{
			Container pack = Backpack;

			if ( pack != null )
			{
				SayTo( m, 503208 );// Take a look at my goods.
				pack.DisplayTo( m );
			}
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			return ( from.GetDistanceToSqrt( this ) <= 3 );
		}

		public bool WasNamed( string speech )
		{
			string name = this.Name;

			return ( name != null && Insensitive.StartsWith( speech, name ) );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;
			
			if ( e.Handled )
				return;
			
			if ( e.HasKeyword( 0x3C ) || (e.HasKeyword( 0x171 ) && WasNamed( e.Speech ))  ) // vendor buy, *buy*
			{
				if ( IsOwner( from ) )
				{
					SayTo( from, 503212 ); // You own this shop, just take what you want.
				}
				else
				{
					from.SendLocalizedMessage( 503213 );// Select the item you wish to buy.
					from.Target = new PVBuyTarget();
					e.Handled = true;
				}
			} 
			else if ( e.HasKeyword( 0x3D ) || (e.HasKeyword( 0x172 ) && WasNamed( e.Speech )) ) // vendor browse, *browse
			{
				Container pack = Backpack;

				if ( pack != null )
				{
					SayTo( from, IsOwner( from ) ? 1010642 : 503208 );// Take a look at my/your goods.
					pack.DisplayTo( from );
					e.Handled = true;
				}
			}
			else if ( e.HasKeyword( 0x3E ) || (e.HasKeyword( 0x173 ) && WasNamed( e.Speech )) ) // vendor collect, *collect
			{
				if ( IsOwner( from ) )
				{
					GiveGold( from );
					e.Handled = true;
				}
			}
			else if ( e.HasKeyword( 0x3F ) || (e.HasKeyword( 0x174 ) && WasNamed( e.Speech )) ) // vendor status, *status
			{
				if ( IsOwner( from  ) )
				{
					from.SendGump( new PlayerVendorOwnerGump( this, from ) );
					e.Handled = true;
				}
				else
				{
					SayTo( from, 503226 ); // What do you care? You don't run this shop.	
				}
			}
			else if ( e.HasKeyword( 0x40 ) || (e.HasKeyword( 0x175 ) && WasNamed( e.Speech )) ) // vendor dismiss, *dismiss
			{
				if ( IsOwner( from ) )
					Dismiss( from );
			}
			else if ( e.HasKeyword( 0x41 ) || (e.HasKeyword( 0x176 ) && WasNamed( e.Speech )) ) // vendor cycle, *cycle
			{
				if ( IsOwner( from ) )
					this.Direction = this.GetDirectionTo( from );
			}
		}
		
		private class PayTimer : Timer
		{
			private PlayerVendor m_Vendor;
			
			public PayTimer( PlayerVendor vend ) : base( TimeSpan.FromMinutes( Clock.MinutesPerUODay ), TimeSpan.FromMinutes( Clock.MinutesPerUODay ) )
			{
				m_Vendor = vend;
				Priority = TimerPriority.OneMinute;
			}
			
			protected override void OnTick()
			{
				int pay = m_Vendor.ChargePerDay;
				if ( m_Vendor.BankAccount < pay )
				{
					pay -= m_Vendor.BankAccount;
					m_Vendor.BankAccount = 0;
					
					if ( m_Vendor.HoldGold < pay )
					{
						m_Vendor.Say( 503235 ); // I regret nothing!postal
						m_Vendor.Blessed = false;
						m_Vendor.Kill();
					}
					else
					{
						m_Vendor.HoldGold -= pay;
					}
				}
				else
				{
					m_Vendor.BankAccount -= pay;
				}
			}
		}
		
		public class PVBuyTarget : Target
		{
			public PVBuyTarget() : base( 3, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}
			
			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					Item item = (Item)targeted;

					PlayerVendor vendor = item.RootParent as PlayerVendor;

					if ( vendor == null )
						return;

					VendorItem vi = (VendorItem)vendor.SellItems[item];
					
					if ( vi != null )
					{
						if ( vi.IsForSale )
							from.SendGump( new PlayerVendorBuyGump( item, vendor, vi ) );
						else
							vendor.SayTo( from, 503202 ); // This item is not for sale.

						return;
					}

					vendor.SayTo( from, 503216 ); // You can't buy that.
				}
			}
		}
	}
}

namespace Server.Prompts
{
	public class VendorPricePrompt : Prompt
	{
		private PlayerVendor m_Vendor;
		private Item m_Item;
		private Item m_Cont;
		private VendorItem m_VI;

		public VendorPricePrompt( PlayerVendor vendor, Item item, Item target, VendorItem vi )
		{
			m_Vendor = vendor;
			m_Item = item;
			m_Cont = target;
			m_VI = vi;
		}

		private void RecurseClearSales( Container cont )
		{
			ArrayList items = new ArrayList(cont.Items);

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = (Item)items[i];

				m_Vendor.SellItems.Remove( item );
				item.InvalidateProperties();

				if ( item is Container )
					RecurseClearSales( (Container) item );
			}
		}

		private void SetInfo( Mobile from, int price, string desc )
		{
			bool allowed = true;

			if ( price <= 0 )
			{
				VendorItem vi = null;

				if ( m_Cont != null )
					vi = (VendorItem)m_Vendor.SellItems[m_Cont];

				if ( vi == null || vi.Price == 0 )
				{
					if ( m_Item is Container )
					{
						if ( !( m_Item is KeyRing ) )
						{
							if ( m_Item is LockableContainer && ((LockableContainer)m_Item).Locked )
							{
								price = 999;
								m_Vendor.SayTo( from, 1043298 ); // Locked items may not be made not-for-sale.
							}
							else if ( m_Item.Items.Count > 0 )
							{
								price = 999;
								m_Vendor.SayTo( from, 1043299 ); // To be not for sale, all items in a container must be for sale.
							}
						}
					}
					else if ( !(m_Item is BaseBook) )
					{
						m_Vendor.SayTo( from, 1043301 );// Only the following may be made not-for-sale: books, containers, keyrings, and items in for-sale containers.
						allowed = false;
					}
				}
			}

			if ( price > 0 && m_Item is Container )
				RecurseClearSales( (Container) m_Item );
			
			if ( allowed )
			{
				m_VI.Price = price;
				m_VI.Description = desc;

				m_VI.Item.InvalidateProperties();
			}
		}
		
		public override void OnResponse( Mobile from, string text )
		{
			int space = text.IndexOf( ' ' );
			if ( space == -1 )
				space = text.Length;
			int price = 0;
			string desc = "";
			
			try
			{
				price = Convert.ToInt32( text.Substring( 0, space ) );
			}
			catch
			{
				price = 0;
			}
			
			if ( space < text.Length )
				desc = text.Substring( space+1 );
			
			SetInfo( from, price, desc );
		}
		
		public override void OnCancel( Mobile from )
		{
			SetInfo( from, 0, "" );
		}
	}
}
