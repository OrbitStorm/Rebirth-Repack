using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class GenericBuyInfo : IBuyItemInfo
	{
		private Type m_Type;
		private string m_Name;
		private int m_Price;
		private int m_MaxAmount, m_Amount;
		private int m_ItemID;
		private int m_Hue;
		private object[] m_Args;

		public virtual int ControlSlots{ get{ return 0; } }

		public Type Type
		{
			get{ return m_Type; }
			set{ m_Type = value; }
		}

		public string Name
		{
			get{ return m_Name; }
			set{ m_Name = value; }
		}

		public int Price
		{
			get{ return m_Price; }
			set{ m_Price = value; }
		}

		public int ItemID
		{
			get{ return m_ItemID; }
			set{ m_ItemID = value; }
		}

		public int Hue
		{
			get{ return m_Hue; }
			set{ m_Hue = value; }
		}

		public int Amount
		{
			get{ return m_Amount; }
			set{ if ( value < 0 ) value = 0; m_Amount = value; }
		}

		public int MaxAmount
		{
			get{ return m_MaxAmount; }
			set{ m_MaxAmount = value; }
		}

		public object[] Args
		{
			get{ return m_Args; }
			set{ m_Args = value; }
		}

		public GenericBuyInfo( Type type, int price, int amount, int itemID, int hue ) : this( null, type, price, amount, itemID, hue, null )
		{
		}

		public GenericBuyInfo( string name, Type type, int price, int amount, int itemID, int hue ) : this( name, type, price, amount, itemID, hue, null )
		{
		}

		public GenericBuyInfo( int name, Type type, int price, int amount, int itemID, int hue ) : this( name.ToString(), type, price, amount, itemID, hue, null )
		{
		}

		public GenericBuyInfo( Type type, int price, int amount, int itemID, int hue, object[] args ) : this( null, type, price, amount, itemID, hue, args )
		{
		}

		public GenericBuyInfo( string name, Type type, int price, int amount, int itemID, int hue, object[] args )
		{
			amount = 20;

			m_Type = type;
			m_Price = price;
			m_MaxAmount = m_Amount = amount;
			m_ItemID = itemID;
			m_Hue = hue;

			if ( name == null )
				m_Name = (1020000 + (itemID & 0x3FFF)).ToString();
			else
				m_Name = name;
		}

		//get a new instance of an object (we just bought it)
		public virtual object GetObject()
		{
			if ( m_Args == null || m_Args.Length == 0 )
				return Activator.CreateInstance( m_Type );

			return Activator.CreateInstance( m_Type, m_Args );
			//return (Item)Activator.CreateInstance( m_Type );
		}

		//Attempt to restock with item, (return true if restock sucessful)
		public virtual bool Restock( Item item, int amount )
		{
			return false;
			/*if ( item.GetType() == m_Type )
			{
				if ( item is BaseWeapon )
				{
					BaseWeapon weapon = (BaseWeapon)item;

					if ( weapon.Quality == CraftQuality.Low || weapon.Quality == CraftQuality.Exceptional || (int)weapon.DurabilityLevel > 0 || (int)weapon.DamageLevel > 0 || (int)weapon.AccuracyLevel > 0 )
						return false;
				}

				if ( item is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)item;

					if ( armor.Quality == CraftQuality.Low || armor.Quality == CraftQuality.Exceptional || (int)armor.Durability > 0 || (int)armor.ProtectionLevel > 0 )
						return false;
				}

				m_Amount += amount;

				return true;
			}
			else
			{
				return false;
			}*/
		}

		public virtual void OnRestock()
		{
			/*if ( m_Amount <= 0 )
			{
				m_MaxAmount *= 2;

				if ( m_MaxAmount >= 999 )
					m_MaxAmount = 999;
			}
			else
			{
				int halfQuantity = m_MaxAmount;

				if ( halfQuantity >= 999 )
					halfQuantity = 640;
				else if ( halfQuantity > 20 )
					halfQuantity /= 2;

				if ( m_Amount >= halfQuantity )
					m_MaxAmount = halfQuantity;
			}*/

			m_Amount = m_MaxAmount;
		}
	}

	public class ColoredPlateBuyInfo : IBuyItemInfo
	{
		private Type m_Type;
		private string m_Name;
		private int m_Price;
		private int m_MaxAmount, m_Amount;
		private int m_ItemID;
		private int m_Hue;
		private object[] m_Args;

		public virtual int ControlSlots{ get{ return 0; } }

		public Type Type
		{
			get{ return m_Type; }
			set{ m_Type = value; }
		}

		public string Name
		{
			get{ return m_Name; }
			set{ m_Name = value; }
		}

		public int Price
		{
			get{ return m_Price; }
			set{ m_Price = value; }
		}

		public int ItemID
		{
			get{ return m_ItemID; }
			set{ m_ItemID = value; }
		}

		public int Hue
		{
			get{ return m_Hue; }
			set{ m_Hue = value; }
		}

		public int Amount
		{
			get{ return m_Amount; }
			set{ if ( value < 0 ) value = 0; m_Amount = value; }
		}

		public int MaxAmount
		{
			get{ return m_MaxAmount; }
			set{ m_MaxAmount = value; }
		}

		public object[] Args
		{
			get{ return m_Args; }
			set{ m_Args = value; }
		}

		public ColoredPlateBuyInfo( Type type, int price, int amount, int itemID ) : this( null, type, price, amount, itemID, null )
		{
		}

		public ColoredPlateBuyInfo( string name, Type type, int price, int amount, int itemID ) : this( name, type, price, amount, itemID, null )
		{
		}

		public ColoredPlateBuyInfo( Type type, int price, int amount, int itemID, object[] args ) : this( null, type, price, amount, itemID, args )
		{
		}

		public ColoredPlateBuyInfo( string name, Type type, int price, int amount, int itemID, object[] args )
		{
			amount = 20;

			m_Type = type;
			m_Price = price;
			m_MaxAmount = m_Amount = amount;
			m_ItemID = itemID;
			m_Hue = GetRandomPlateHue();

			if ( name == null )
				m_Name = (1020000 + (itemID & 0x3FFF)).ToString();
			else
				m_Name = name;
		}

		//get a new instance of an object (we just bought it)
		public virtual object GetObject()
		{
			object o = null;
			if ( m_Args == null || m_Args.Length == 0 )
				o = Activator.CreateInstance( m_Type );
			else
				o = Activator.CreateInstance( m_Type, m_Args );
			
			if ( o is Item )
				((Item)o).Hue = m_Hue;
			return o;
		}

		//Attempt to restock with item, (return true if restock sucessful)
		public bool Restock( Item item, int amount )
		{
			return false;
		}
		
		public int GetRandomPlateHue()
		{
			if ( Utility.RandomBool() )
				return 0; // regular silver 50% of the time
			else
				return 2401 + Utility.Random( 30 );
		}

		public void OnRestock()
		{
			// CHANGE color 25% (if it was colored, it will keep the old color...)
			if ( Utility.RandomBool() )
				m_Hue = GetRandomPlateHue();
			m_Amount = m_MaxAmount;
		}
	}

	public class BoltOfClothBuyInfo : IBuyItemInfo
	{
		private int m_Price;
		private int m_MaxAmount, m_Amount;
		private int m_ItemID, m_Hue;

		public int ControlSlots{ get { return 0; } }

		public string Name{ get { return (1020000 + (m_ItemID & 0x3FFF)).ToString(); } }

		public int Hue{ get { return m_Hue; } }
		
		public int Price
		{
			get{ return m_Hue != 0 ? (int)( m_Price * 1.5 ) : m_Price; }
			set{ m_Price = value; }
		}

		public int ItemID
		{
			get{ return m_ItemID; }
			set{ m_ItemID = value; }
		}

		public int Amount
		{
			get{ return m_Amount; }
			set{ if ( value < 0 ) value = 0; m_Amount = value; }
		}

		public int MaxAmount
		{
			get{ return m_MaxAmount; }
			set{ m_MaxAmount = value; }
		}

		public BoltOfClothBuyInfo( int price, int amount ) 
		{
			m_Price = price;
			m_MaxAmount = amount;
			 
			OnRestock();
		}

		//get a new instance of an object (we just bought it)
		public object GetObject()
		{
			BoltOfCloth bc = new BoltOfCloth();
			bc.Hue = m_Hue;
			bc.ItemID = m_ItemID;
			return bc;
		}

		//Attempt to restock with item, (return true if restock sucessful)
		public bool Restock( Item item, int amount )
		{
			return false;
		}

		public void OnRestock()
		{
			m_ItemID = 0xF95;
			if ( Utility.RandomBool() )
				m_ItemID += Utility.Random( 8 );
			
			if ( Utility.Random( 3 ) == 0 )
				m_Hue = Utility.RandomNondyedHue();
			else
				m_Hue = 0;

			m_Amount = m_MaxAmount;
		}
	}
}
