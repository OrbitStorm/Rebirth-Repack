using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Misc;
using System.Text;

namespace Server.Mobiles
{
	public abstract class BaseVendor : BaseConvo, IVendor
	{
		private const int MaxSell = 5;

		protected abstract ArrayList SBInfos{ get; }

		private ArrayList m_ArmorBuyInfo = new ArrayList();
		private ArrayList m_ArmorSellInfo = new ArrayList();

		private DateTime m_LastRestock;

		private int m_BankAccount, m_BankRestock;

		public override bool CanTeach{ get{ return true; } }

		public override bool PlayerRangeSensitive{ get{ return true; } }

		public virtual bool IsActiveVendor{ get{ return true; } }
		public virtual bool IsActiveBuyer{ get{ return IsActiveVendor; } } // response to vendor SELL
		public virtual bool IsActiveSeller{ get{ return IsActiveVendor; } } // repsonse to vendor BUY

		public virtual NpcGuild NpcGuild{ get{ return NpcGuild.None; } }

		public virtual bool IsInvulnerable{ get{ return false; } }

		public override bool ShowFameTitle{ get{ return false; } }

		protected override void GetConvoFragments(ArrayList list)
		{
			list.Add( (int)JobFragment.shopkeep );
			base.GetConvoFragments (list);
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int BankAccount
		{
			get
			{
				return m_BankAccount;
			}
			set
			{
				m_BankAccount = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int BankRestockAmount
		{
			get
			{
				return m_BankRestock;
			}
			set
			{
				m_BankRestock = value;
			}
		}

		public virtual bool IsValidBulkOrder( Item item )
		{
			return false;
		}

		public virtual Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
			return null;
		}

		public virtual bool SupportsBulkOrders( Mobile from )
		{
			return false;
		}

		public virtual TimeSpan GetNextBulkOrder( Mobile from )
		{
			return TimeSpan.Zero;
		}

		public BaseVendor( string title ) : base( AIType.AI_Vendor, FightMode.Agressor, 10, 1, 0.4, 1.0 )
		{
			LoadSBInfo();

			this.Title = title;
			InitBody();
			InitOutfit();

			Container pack;
			//these packs MUST exist, or the client will crash when the packets are sent
			pack = new Backpack();
			pack.Layer = Layer.ShopBuy;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			pack = new Backpack();
			pack.Layer = Layer.ShopResale;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			m_BankAccount = m_BankRestock = 1000;
			m_LastRestock = DateTime.Now;
		}
		
		public BaseVendor( Serial serial ) : base( serial )
		{
		}

		public DateTime LastRestock
		{
			get
			{
				return m_LastRestock;
			}
			set
			{
				m_LastRestock = value;
			}
		}

		public virtual TimeSpan RestockDelay
		{
			get
			{
				return TimeSpan.FromHours( 1 );
			}
		}

		public Container BuyPack
		{
			get
			{
				Container pack = FindItemOnLayer( Layer.ShopBuy ) as Container;

				if ( pack == null )
				{
					pack = new Backpack();
					pack.Layer = Layer.ShopBuy;
					pack.Visible = false;
					AddItem( pack );
				}

				return pack;
			}
		}

		public abstract void InitSBInfo();

		protected void LoadSBInfo()
		{
			m_LastRestock = DateTime.Now;

			InitSBInfo();

			m_ArmorBuyInfo.Clear();
			m_ArmorSellInfo.Clear();

			for ( int i = 0; i < SBInfos.Count; i++ )
			{
				SBInfo sbInfo = (SBInfo)SBInfos[i];
				m_ArmorBuyInfo.AddRange( sbInfo.BuyInfo );
				m_ArmorSellInfo.Add( sbInfo.SellInfo );
			}
		}

		public virtual bool GetGender()
		{
			return Utility.RandomBool();
		}

		public virtual void InitBody()
		{
			InitStats( Utility.RandomMinMax( 40, 80 ), Utility.RandomMinMax( 40, 80 ), Utility.RandomMinMax( 40, 80 ) );

			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if ( Female = GetGender() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
		}

		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomNeutralHue();
		}

		public virtual int RandomBrightHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return Utility.RandomList( 0x62, 0x71 );

			return Utility.RandomList( 0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59 );
		}

		public virtual void CheckMorph()
		{
			if ( CheckGargoyle() )
				return;

			CheckNecromancer();
		}

		public virtual bool CheckGargoyle()
		{
			Map map = this.Map;

			if ( map != Map.Ilshenar )
				return false;

			if ( Region.Name != "Gargoyle City" )
				return false;

			if ( Body != 0x2F6 || (Hue & 0x8000) == 0 )
				TurnToGargoyle();

			return true;
		}

		public virtual bool CheckNecromancer()
		{
			Map map = this.Map;

			if ( map != Map.Malas )
				return false;

			if ( Region.Name != "Umbra" )
				return false;

			if ( Hue != 0x83E8 )
				TurnToNecromancer();

			return true;
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{
			base.OnLocationChange( oldLocation );

			CheckMorph();
		}

		protected override void OnMapChange( Map oldMap )
		{
			base.OnMapChange( oldMap );

			CheckMorph();
		}

		public virtual int GetRandomNecromancerHue()
		{
			switch ( Utility.Random( 20 ) )
			{
				case 0: return 0;
				case 1: return 0x4E9;
				default: return Utility.RandomList( 0x485, 0x497 );
			}
		}

		public virtual void TurnToNecromancer()
		{
			ArrayList items = new ArrayList( this.Items );

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = (Item)items[i];

				if ( item is Hair || item is Beard )
					item.Hue = 0;
				else if ( item is BaseClothing || item is BaseWeapon || item is BaseArmor || item is BaseTool )
					item.Hue = GetRandomNecromancerHue();
			}

			Hue = 0x83E8;
		}

		public virtual void TurnToGargoyle()
		{
			ArrayList items = new ArrayList( this.Items );

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = (Item)items[i];

				if ( item is BaseClothing || item is Hair || item is Beard )
					item.Delete();
			}

			Body = 0x2F6;
			Hue = RandomBrightHue() | 0x8000;
			Name = NameList.RandomName( "gargoyle vendor" );

			CapitalizeTitle();
		}

		public virtual void CapitalizeTitle()
		{
			string title = this.Title;

			if ( title == null )
				return;

			string[] split = title.Split( ' ' );

			for ( int i = 0; i < split.Length; ++i )
			{
				if ( Insensitive.Equals( split[i], "the" ) )
					continue;

				if ( split[i].Length > 1 )
					split[i] = Char.ToUpper( split[i][0] ) + split[i].Substring( 1 );
				else if ( split[i].Length > 0 )
					split[i] = Char.ToUpper( split[i][0] ).ToString();
			}

			this.Title = String.Join( " ", split );
		}

		public virtual int GetHairHue()
		{
			return Utility.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0: AddItem( new FancyShirt( GetRandomHue() ) ); break;
				case 1: AddItem( new Doublet( GetRandomHue() ) ); break;
				case 2: AddItem( new Shirt( GetRandomHue() ) ); break;
			}

			AddItem( new Shoes( GetShoeHue() ) );

			if ( Female )
			{
				switch ( Utility.Random( 6 ) )
				{
					case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
					case 1:
					case 2: AddItem( new Kilt( GetRandomHue() ) ); break;
					case 3:
					case 4:
					case 5: AddItem( new Skirt( GetRandomHue() ) ); break;
				}

				Item hair = AddRandomHair();
				hair.Hue = GetHairHue();
			}
			else
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: AddItem( new LongPants( GetRandomHue() ) ); break;
					case 1: AddItem( new ShortPants( GetRandomHue() ) ); break;
				}

				Item hair = AddRandomHair();
				hair.Hue = GetHairHue();
				AddRandomFacialHair( hair.Hue );
			}

			PackGold( 100, 200 );
		}

		public virtual void Restock()
		{
			m_LastRestock = DateTime.Now;
			m_BankAccount = (int)(m_BankRestock*0.75 + Utility.Random( m_BankRestock/4 ));

			if ( Home != Point3D.Zero && !this.InRange( this.Home, this.RangeHome+1 ) )
			{
				this.Say( "I do not have my goods with me here, I must return to my shop." );
				this.Location = this.Home;
			}

			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			foreach ( IBuyItemInfo bii in buyInfo )
				bii.OnRestock();
		}

		private static TimeSpan InventoryDecayTime = TimeSpan.FromHours( 2.0 );

		public virtual double GetSellDiscountFor( Mobile from )
		{
			return 0.75;
		}
		
		public virtual double GetBuyDiscountFor( Mobile from )
		{
			double scale;
			
			scale = (from.Karma / Titles.MaxKarma)*0.1;// +/-10% based on noto

			// inverse discounts on red npcs (bucs den)
			if ( Notoriety.Compute( this, this ) == Notoriety.Murderer )
				scale = -(scale/2);

			scale = 1.0 - scale;
			
			if ( this.NpcGuild != NpcGuild.None && this.NpcGuild != NpcGuild.MerchantsGuild && from is PlayerMobile && this.NpcGuild == ((PlayerMobile)from).NpcGuild )
				scale -= 0.1;

			if ( scale < 0.85 )
				scale = 0.85;
			else if ( scale > 1.15 )
				scale = 1.15;

			return scale;
		}

		public virtual void VendorBuy( Mobile from )
		{
			if ( !IsActiveSeller )
				return;

			if ( !from.CheckAlive() )
				return;

			if ( this.Home != Point3D.Zero && !this.InRange( this.Home, this.RangeHome+5 ) )
			{
				this.Say( "Please allow me to return to my shop so that I might assist thee." );
				this.Location = this.Home;
				return;
			}

			if ( DateTime.Now - m_LastRestock > RestockDelay )
				Restock();

			double discount = GetBuyDiscountFor( from );

			int count = 0;
			List<BuyItemState> list;
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] sellInfo = this.GetSellInfo();

			list = new List<BuyItemState>( buyInfo.Length );
			Container cont = this.BuyPack;

			for (int idx=0;idx<buyInfo.Length;idx++)
			{
				IBuyItemInfo buyItem = (IBuyItemInfo)buyInfo[idx];
				if ( buyItem.Amount > 0 )
				{
					if ( list.Count < 250 )
					{
						int price = (int)Math.Round( buyItem.Price*discount);
						if ( price < 1 )
							price = 1;
						list.Add( new BuyItemState( buyItem.Name, cont.Serial, 0x7FFFFEFF - idx, price, buyItem.Amount, buyItem.ItemID, buyItem.Hue ) );
						count++;
					}
				}
			}

			List<Item> playerItems = cont.Items;

			for ( int i = playerItems.Count - 1; i >= 0; --i )
			{
				if ( i >= playerItems.Count )
					continue;

				Item item = (Item)playerItems[i];

				if ( (item.LastMoved + InventoryDecayTime) <= DateTime.Now )
					item.Delete();
			}

			for ( int i = 0; i < playerItems.Count; ++i )
			{
				Item item = (Item)playerItems[i];

				int price = 0;
				string name = null;

				foreach( IShopSellInfo ssi in sellInfo )
				{
					if ( ssi.IsSellable( item ) )
					{
						price = (int)Math.Round( ssi.GetBuyPriceFor( item ) * discount );
						name = ssi.GetNameFor( item );
						break;
					}
				}

				if ( name != null && list.Count < 250 )
				{
					if ( price < 1 )
						price = 1;
					list.Add( new BuyItemState( name, cont.Serial, item.Serial, price, item.Amount, item.ItemID, item.Hue ) );
					count++;
				}
			}

			//one (not all) of the packets uses a byte to describe number of items in the list.  Osi = dumb.
			//if ( list.Count > 255 )
			//	Console.WriteLine( "Vendor Warning: Vendor {0} has more than 255 buy items, may cause client errors!", this );

			if ( list.Count > 0 )
			{
				list.Sort( new BuyItemStateComparer() );

				SendPacksTo( from );

                NetState ns = from.NetState;

                if (ns == null)
                    return;

                if (ns.ContainerGridLines)
                    from.Send(new VendorBuyContent6017(list));
                else
                    from.Send(new VendorBuyContent(list));

                from.Send(new VendorBuyList(this, list));

                if (ns.HighSeas)
                    from.Send(new DisplayBuyListHS(this));
                else
                    from.Send(new DisplayBuyList(this));

                from.Send(new MobileStatusExtended(from));//make sure their gold amount is sent

				foreach ( BuyItemState bis in list )
				{
					int loc;
					try { loc = Utility.ToInt32( bis.Description ); }
					catch { loc = 0; }

					if ( loc > 500000 )
						from.Send( new FakeOPL( bis.MySerial, loc ) );
					else
						from.Send( new FakeOPL( bis.MySerial, bis.Description ) );
				}

				SayTo( from, true, "Greetings.  Have a look around." );
			}
		}
		
		private class FakeOPL : Packet
		{
			public FakeOPL( Serial serial, int locNum ) : base( 0xD6 )
			{
				EnsureCapacity( 1+2+2+4+1+1+4 + 4 + 2 + 4 );

				int hash = (locNum & 0x3FFFFFF);
				hash ^= (locNum >> 26) & 0x3F;

				m_Stream.Write( (short) 1 );
				m_Stream.Write( (int) serial );
				m_Stream.Write( (byte) 0 );
				m_Stream.Write( (byte) 0 );
				m_Stream.Write( (int) hash );

				m_Stream.Write( locNum );
				m_Stream.Write( (short) 0 );

				m_Stream.Write( (int) 0 ); // terminator
			}
	
			private static byte[] m_Buffer = new byte[0];
			private static Encoding m_Encoding = Encoding.Unicode;

			public FakeOPL( Serial serial, string desc ) : base( 0xD6 )
			{
				int byteCount = m_Encoding.GetByteCount( desc );
				if ( byteCount > m_Buffer.Length )
					m_Buffer = new byte[byteCount];
				byteCount = m_Encoding.GetBytes( desc, 0, desc.Length, m_Buffer, 0 );

				EnsureCapacity( 1+2+2+4+1+1+4 + 4 + 2 + byteCount + 4 );

				int hash = (1042971 & 0x3FFFFFF);
				hash ^= (1042971 >> 26) & 0x3F;

				int code = desc.GetHashCode();
				hash ^= (code & 0x3FFFFFF);
				hash ^= (code >> 26) & 0x3F;

				m_Stream.Write( (short) 1 );
				m_Stream.Write( (int) serial );
				m_Stream.Write( (byte) 0 );
				m_Stream.Write( (byte) 0 );
				m_Stream.Write( (int) hash );

				m_Stream.Write( (int) 1042971 );
				
				m_Stream.Write( (short) byteCount );
				m_Stream.Write( m_Buffer, 0, byteCount );

				m_Stream.Write( (int) 0 ); // terminator
			}
		}

		public virtual void SendPacksTo( Mobile from )
		{
			Item pack = FindItemOnLayer( Layer.ShopBuy );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopBuy;
				pack.Movable = false;
				pack.Visible = false;
				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopSell );

			if ( pack != null )
				from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopResale );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopResale;
				pack.Movable = false;
				pack.Visible = false;
				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );
		}

		public virtual void VendorSell( Mobile from )
		{
			if ( !IsActiveBuyer )
				return;

			if ( !from.CheckAlive() )
				return;

			if ( this.Home != Point3D.Zero && !this.InRange( this.Home, this.RangeHome+5 ) )
			{
				this.Say( "Please allow me to return to my shop so that I might assist thee." );
				this.Location = this.Home;
				return;
			}

			double discount = GetSellDiscountFor( from );

			if ( DateTime.Now - m_LastRestock > RestockDelay )
				Restock();// restocks the bank account too so must do it on sell also

			Container pack = from.Backpack;

			bool noMoney = false;

			if ( pack != null )
			{
				IShopSellInfo[] info = GetSellInfo();

				Hashtable table = new Hashtable();

				foreach ( IShopSellInfo ssi in info )
				{
					Item[] items = pack.FindItemsByType( ssi.Types );

					foreach ( Item item in items )
					{
						if ( item is Container && ((Container)item).Items.Count != 0 )
							continue;

						if ( item.IsStandardLoot() && item.Movable && ssi.IsSellable( item ) )
						{
							int price = (int)Math.Round( ssi.GetSellPriceFor( item ) * discount );

							if ( price < 1 )
								price = 1;
							
							if ( price <= m_BankAccount )
								table[item] = new SellItemState( item, price, ssi.GetNameFor( item ) );
							else
								noMoney = true;
						}
					}
				}

				if ( table.Count > 0 )
				{
					SendPacksTo( from );

                    from.Send(new VendorSellList(this, table));

					foreach ( SellItemState sis in table.Values )
					{
						int loc;
						try { loc = Utility.ToInt32( sis.Name ); }
						catch { loc = 0; }

						if ( loc > 500000 )
							from.Send( new FakeOPL( sis.Item.Serial, loc ) );
						else
							from.Send( new FakeOPL( sis.Item.Serial, sis.Name ) );
					}
				}
				else
				{
					if ( noMoney )
						Say( true, "I don't have enough money to buy anything right now." );
					else
						Say( true, "You have nothing I would be interested in." );
				}
			}
		}

		//public virtual bool OnBuyItems( Mobile buyer, ArrayList list )
        public virtual bool OnBuyItems(Mobile buyer, System.Collections.Generic.List<BuyItemResponse> list)
		{
			if ( !IsActiveSeller )
				return false;

			if ( !buyer.CheckAlive() )
				return false;

			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] info = GetSellInfo();
			int totalCost = 0;
			ArrayList validBuy = new ArrayList( list.Count );
			Container cont;
			bool bought = false;
			bool fromBank = false;
			bool fullPurchase = true;
			int controlSlots = buyer.FollowersMax - buyer.Followers;
			double discount = GetBuyDiscountFor( buyer );

			foreach ( BuyItemResponse buy in list )
			{
				Serial ser = 0x7FFFFEFF - buy.Serial;
				int amount = buy.Amount;
				if ( ser >= 0 && ser <= buyInfo.Length )
				{
					IBuyItemInfo bii = buyInfo[ser];
					if ( amount > bii.Amount )
						amount = bii.Amount;
					if ( amount <= 0 )
						continue;

					/*int slots = bii.ControlSlots * amount;

					if ( controlSlots >= slots )
					{
						controlSlots -= slots;
					}
					else
					{
						fullPurchase = false;
						continue;
					}*/

					int price = (int)Math.Round( bii.Price * discount );
					if ( price < 1 ) 
						price = 1;
					totalCost += price * amount;
					validBuy.Add( buy );
				}
				else
				{
					Item item = World.FindItem( buy.Serial );

					if ( item == null || item.RootParent != this )
						continue;

					if ( amount > item.Amount )
						amount = item.Amount;
					if ( amount <= 0 )
						continue;

					foreach( IShopSellInfo ssi in info )
					{
						if ( ssi.IsSellable( item ) )
						{
							if ( ssi.IsResellable( item ) )
							{
								int price = (int)Math.Round( ssi.GetBuyPriceFor( item ) * discount );
								if ( price < 1 ) 
									price = 1;
								totalCost += price * amount;
								validBuy.Add( buy );
								break;
							}
						}
					}
				}
			}//foreach

			if ( fullPurchase && validBuy.Count == 0 )
				SayTo( buyer, true, "Thou hast bought nothing!" );
			else if ( validBuy.Count == 0 )
				SayTo( buyer, true, "Your order cannot be fulfilled, please try again." );

			if ( validBuy.Count == 0 )
				return false;

			bought = ( buyer.AccessLevel >= AccessLevel.GameMaster );

			cont = buyer.Backpack;
			if ( !bought && cont != null )
			{
				if ( cont.ConsumeTotal( typeof( Gold ), totalCost ) )
					bought = true;
				else if ( totalCost < 2000 )
					SayTo( buyer, true, "Begging thy pardon, but thou casnt afford that." );
			}

			if ( !bought && totalCost >= 2000 )
			{
				cont = buyer.BankBox;
				if ( cont != null && cont.ConsumeTotal( typeof( Gold ), totalCost ) )
				{
					bought = true;
					fromBank = true;
				}
				else
				{
					SayTo( buyer, true, "Begging thy pardon, but thy bank account lacks these funds." );
				}
			}

			if ( !bought )
				return false;
			else
				buyer.PlaySound( 0x32 );

			if ( buyer.AccessLevel < AccessLevel.GameMaster ) // dont count free purchases
				m_BankAccount += (int)(totalCost * 0.9); // gets back 90%

			cont = buyer.Backpack;
			if ( cont == null )
				cont = buyer.BankBox;

			foreach ( BuyItemResponse buy in validBuy )
			{
				Serial ser = 0x7FFFFEFF - buy.Serial;
				int amount = buy.Amount;

				if ( ser >= 0 && ser <= buyInfo.Length )
				{
					IBuyItemInfo bii = buyInfo[ser];

					if ( amount > bii.Amount )
						amount = bii.Amount;

					bii.Amount -= amount;

					object o = bii.GetObject();

					if ( o is Item )
					{
						Item item = (Item)o;

						if ( item.Stackable )
						{
							item.Amount = amount;

							if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
								item.MoveToWorld( buyer.Location, buyer.Map );
						}
						else
						{
							item.Amount = 1;

							if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
								item.MoveToWorld( buyer.Location, buyer.Map );

							for (int i=1;i<amount;i++)
							{
								item = bii.GetObject() as Item;

								if ( item != null )
								{
									item.Amount = 1;

									if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
										item.MoveToWorld( buyer.Location, buyer.Map );
								}
							}
						}
					}
					else if ( o is Mobile )
					{
						Mobile m = (Mobile)o;

						m.Direction = (Direction)Utility.Random( 8 );
						//m.MoveToWorld( buyer.Location, buyer.Map );
						m.Location = buyer.Location;
						m.Map = buyer.Map;
						m.PlaySound( m.GetIdleSound() );

						if ( m is BaseCreature )
							((BaseCreature)m).SetControlMaster( buyer );

						for ( int i = 1; i < amount; ++i )
						{
							m = bii.GetObject() as Mobile;

							if ( m != null )
							{
								m.Direction = (Direction)Utility.Random( 8 );
								m.Location = buyer.Location;
								m.Map = buyer.Map;

								if ( m is BaseCreature )
									((BaseCreature)m).SetControlMaster( buyer );
							}
						}
					}
				}
				else
				{
					Item item = World.FindItem( buy.Serial );

					if ( item == null )
						continue;

					if ( amount > item.Amount )
						amount = item.Amount;

					foreach( IShopSellInfo ssi in info )
					{
						if ( ssi.IsSellable( item ) )
						{
							if ( ssi.IsResellable( item ) )
							{
								Item buyItem;
								if ( amount >= item.Amount )
								{
									buyItem = item;
								}
								else
								{
									buyItem = item.Dupe( amount );
									item.Amount -= amount;
								}

								if ( cont == null || !cont.TryDropItem( buyer, buyItem, false ) )
									buyItem.MoveToWorld( buyer.Location, buyer.Map );

								break;
							}
						}
					}
				}
			}//foreach

			if ( fullPurchase )
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested." );
				else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.", totalCost );
			}
			else
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Unfortunately, I could not sell you all the goods you requested." );
				else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
			}

			return true;
		}

        //public virtual bool OnSellItems( Mobile seller, ArrayList list )
        public virtual bool OnSellItems( Mobile seller, System.Collections.Generic.List<SellItemResponse> list )
		{
			if ( !IsActiveBuyer )
				return false;

			if ( !seller.CheckAlive() )
				return false;

			seller.PlaySound( 0x32 );

			IShopSellInfo[] info = GetSellInfo();
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			int GiveGold = 0;
			int Sold = 0;
			Container cont;
			ArrayList delete = new ArrayList();
			ArrayList drop = new ArrayList();

			double discount = GetSellDiscountFor( seller );

			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 )
					continue;

				foreach( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						Sold += resp.Amount;
						break;
					}
				}
			}

			if ( Sold > MaxSell )
			{
				SayTo( seller, true, "You may only sell {0} items at a time!", MaxSell );
				return false;
			} 
			else if ( Sold == 0 )
			{
				return true;
			}

			bool lowMoney = false;
			foreach ( SellItemResponse resp in list )
			{
				if ( GiveGold >= m_BankAccount )
				{
					lowMoney = true;
					break;
				}

				if ( resp.Item.RootParent != seller || resp.Amount <= 0 )
					continue;

				foreach( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						int sellPrice = (int)Math.Round( ssi.GetSellPriceFor( resp.Item ) * discount );
						if ( sellPrice < 1 )
							sellPrice = 1;
						int amount = resp.Amount;
						int maxAfford = (int)((m_BankAccount - GiveGold) / sellPrice);

						if ( maxAfford <= 0 )
						{
							lowMoney = true;
							break;
						}

						if ( amount > resp.Item.Amount )
							amount = resp.Item.Amount;
						
						if ( amount > maxAfford )
						{
							lowMoney = true;
							amount = maxAfford;
						}

						if ( ssi.IsResellable( resp.Item ) )
						{
							bool found = false;

							foreach ( IBuyItemInfo bii in buyInfo )
							{
								if ( bii.Restock( resp.Item, amount ) )
								{
									resp.Item.Consume( amount );
									found = true;

									break;
								}
							}

							if ( !found )
							{
								cont = this.BuyPack;

								if ( amount < resp.Item.Amount )
								{
									resp.Item.Amount -= amount;
									try
									{
										Item item = resp.Item.Dupe( amount );
										item.SetLastMoved();
										cont.DropItem( item );
									}
									catch
									{
									}
								}
								else
								{
									resp.Item.SetLastMoved();
									cont.DropItem( resp.Item );
								}
							}
						}
						else
						{
							if ( amount < resp.Item.Amount )
								resp.Item.Amount -= amount;
							else
								resp.Item.Delete();
						}

						GiveGold += (int)(sellPrice*amount);
						break;
					}
				}
			}

			if ( lowMoney )
				SayTo( seller, true, "Sorry, I cannot afford to buy all of that right now." );

			if ( GiveGold > 0 )
			{
				m_BankAccount -= GiveGold;
				
				while ( GiveGold > 60000 )
				{
					seller.AddToBackpack( new Gold( 60000 ) );
					GiveGold -= 60000;
				}

				seller.AddToBackpack( new Gold( GiveGold ) );

				seller.PlaySound( 0x0037 );//Gold dropping sound
			}
			//no cliloc for this?
			//SayTo( seller, true, "Thank you! I bought {0} item{1}. Here is your {2}gp.", Sold, (Sold > 1 ? "s" : ""), GiveGold );
			
			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( m_BankRestock );

			/*ArrayList sbInfos = this.SBInfos;

			for ( int i = 0; sbInfos != null && i < sbInfos.Count; ++i )
			{
				SBInfo sbInfo = (SBInfo)sbInfos[i];
				ArrayList buyInfo = sbInfo.BuyInfo;

				for ( int j = 0; buyInfo != null && j < buyInfo.Count; ++j )
				{
					GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[j];

					int maxAmount = gbi.MaxAmount;
					int doubled = 0;

					switch ( maxAmount )
					{
						case  40: doubled = 1; break;
						case  80: doubled = 2; break;
						case 160: doubled = 3; break;
						case 320: doubled = 4; break;
						case 640: doubled = 5; break;
						case 999: doubled = 6; break;
					}

					if ( doubled > 0 )
					{
						writer.WriteEncodedInt( 1 + ((j * sbInfos.Count) + i) );
						writer.WriteEncodedInt( doubled );
					}
				}
			}*/

			writer.WriteEncodedInt( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			LoadSBInfo();

			ArrayList sbInfos = this.SBInfos;

			switch ( version )
			{
				case 2:
				{
					m_BankAccount = m_BankRestock = reader.ReadInt();
					goto case 1;
				}
				case 1:
				{
					int index;

					while ( (index = reader.ReadEncodedInt()) > 0 )
					{
						int doubled = reader.ReadEncodedInt();

						/*if ( sbInfos != null )
						{
							index -= 1;
							int sbInfoIndex = index % sbInfos.Count;
							int buyInfoIndex = index / sbInfos.Count;
 
							if ( sbInfoIndex >= 0 && sbInfoIndex < sbInfos.Count )
							{
								SBInfo sbInfo = (SBInfo)sbInfos[sbInfoIndex];
								ArrayList buyInfo = sbInfo.BuyInfo;

								if ( buyInfo != null && buyInfoIndex >= 0 && buyInfoIndex < buyInfo.Count )
								{
									GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[buyInfoIndex];

									int amount = 20;

									switch ( doubled )
									{
										case 1: amount =  40; break;
										case 2: amount =  80; break;
										case 3: amount = 160; break;
										case 4: amount = 320; break;
										case 5: amount = 640; break;
										case 6: amount = 999; break;
									}

									gbi.Amount = gbi.MaxAmount = amount;
								}
							}
						}*/
					}

					break;
				}
			}

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( AfterLoad ) );

			if ( version < 2 || m_BankRestock <= 0 )
				m_BankRestock = 1000;
			m_BankAccount = m_BankRestock;

			if ( RestockDelay.TotalMinutes >= 2 )
				m_LastRestock += TimeSpan.FromMinutes( Utility.Random( (int)RestockDelay.TotalMinutes ) );

			CheckMorph();
		}

		private void AfterLoad()
		{
			if ( Backpack != null )
			{
				if ( Backpack.GetAmount( typeof( Gold ), true ) < 15 )
					PackGold( 15, 50 );
			}
		}

        public override void AddCustomContextEntries(Mobile from, List<ContextMenus.ContextMenuEntry> list)
		{
			if ( from.Alive && IsActiveVendor )
			{
				if ( IsActiveSeller )
					list.Add( new VendorBuyEntry( from, this ) );

				if ( IsActiveBuyer )
					list.Add( new VendorSellEntry( from, this ) );
			}

			base.AddCustomContextEntries( from, list );
		}

		public virtual IShopSellInfo[] GetSellInfo()
		{
			return (IShopSellInfo[])m_ArmorSellInfo.ToArray( typeof( IShopSellInfo ) );
		}

		public virtual IBuyItemInfo[] GetBuyInfo()
		{
			return (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );
		}

		public override bool CanBeDamaged()
		{
			return !IsInvulnerable;
		}
	}
}

namespace Server.ContextMenus
{
	public class VendorBuyEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorBuyEntry( Mobile from, BaseVendor vendor ) : base( 6103, 8 )
		{
			m_Vendor = vendor;
		}

		public override void OnClick()
		{
			m_Vendor.VendorBuy( this.Owner.From );
		}
	}

	public class VendorSellEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorSellEntry( Mobile from, BaseVendor vendor ) : base( 6104, 8 )
		{
			m_Vendor = vendor;
		}

		public override void OnClick()
		{
			m_Vendor.VendorSell( this.Owner.From );
		}
	}
}

namespace Server
{
	public interface IShopSellInfo
	{
		//get display name for an item
		string GetNameFor( Item item );

		//get price for an item which the player is selling
		int GetSellPriceFor( Item item );

		//get price for an item which the player is buying
		int GetBuyPriceFor( Item item );

		//can we sell this item to this vendor?
		bool IsSellable( Item item );

		//What do we sell?
		Type[] Types{ get; }

		//does the vendor resell this item?
		bool IsResellable( Item item );
	}

	public interface IBuyItemInfo
	{
		//get a new instance of an object (we just bought it)
		object GetObject();

		int ControlSlots{ get; }

		//display price of the item
		int Price{ get; }

		//display name of the item
		string Name{ get; }

		//display hue
		int Hue{ get; }

		//display id
		int ItemID{ get; }

		//amount in stock
		int Amount{ get; set; }

		//max amount in stock
		int MaxAmount{ get; }

		//Attempt to restock with item, (return true if restock sucessful)
		bool Restock( Item item, int amount );

		//called when its time for the whole shop to restock
		void OnRestock();
	}
}

