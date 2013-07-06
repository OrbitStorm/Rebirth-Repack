using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public interface ISellPrice
	{
		int GetSellPrice();
	}

	public class GenericSellInfo : IShopSellInfo
	{
		private Hashtable m_Table = new Hashtable();
		private ArrayList m_MyTypes = new ArrayList();
		private Type[] m_Types;

		public GenericSellInfo()
		{
		}

		public void Add( Type type, int price )
		{
			m_Table[type] = price;
			m_MyTypes.Add( type );
			m_Types = null;
		}

		public int GetSellPriceFor( Item item )
		{
			int price = (int)m_Table[item.GetType()];

			if ( item is BaseArmor )
			{
				BaseArmor armor = (BaseArmor)item;

				/*if ( armor.Quality == CraftQuality.Low )
					price = (int)( price * 0.60 );
				else if ( armor.Quality == CraftQuality.Exceptional )
					price = (int)( price * 1.25 );*/

				// quality: -30% to +20%
				if ( armor.InitMaxHits > 0 )
					price = (int)( price * (( armor.MaxHitPoints / ((double)armor.InitMaxHits) )*0.5+0.70) );

				// condition: -10% to +2.5%
				if ( armor.MaxHitPoints > 0 )
					price = (int)( price * ((armor.HitPoints / ((double)armor.MaxHitPoints))*0.125+0.9) ); 

				price += 100 * (int)armor.Durability;
				price += 100 * (int)armor.ProtectionLevel;
			}
			else if ( item is BaseWeapon )
			{
				BaseWeapon weapon = (BaseWeapon)item;

				/*if ( weapon.Quality == CraftQuality.Low )
					price = (int)( price * 0.60 );
				else if ( weapon.Quality == CraftQuality.Exceptional )
					price = (int)( price * 1.25 );*/

				// quality: -30% to +20%
				if ( weapon.InitMaxHits > 0 )
					price = (int)( price * (( weapon.MaxHits / ((double)weapon.InitMaxHits) )*0.5+0.70) );

				// condition: -10% to +2.5%
				if ( weapon.MaxHits > 0 )
					price = (int)( price * ((weapon.Hits / ((double)weapon.MaxHits))*0.125+0.9) ); 
				
				price += 100 * (int)weapon.DurabilityLevel;
				price += 100 * (int)weapon.DamageLevel;
			}
			else if ( item is BaseBeverage )
			{
				int price1 = price, price2 = price;

				if ( item is Pitcher )
				{ price1 = 3; price2 = 5; }
				else if ( item is BeverageBottle )
				{ price1 = 3; price2 = 3; }
				else if ( item is Jug )
				{ price1 = 6; price2 = 6; }

				BaseBeverage bev = (BaseBeverage)item;

				if ( bev.IsEmpty || bev.Content == BeverageType.Milk )
					price = price1;
				else
					price = price2;
			}
			else if ( item is ISellPrice )
			{
				price = ((ISellPrice)item).GetSellPrice();
			}

			if ( price < 1 )
				price = 1;

			return price;
		}

		public int GetBuyPriceFor( Item item )
		{
			return (int)( 1.90 * GetSellPriceFor( item ) );
		}

		public Type[] Types
		{
			get
			{
				if ( m_Types == null )
					m_Types = (Type[])m_MyTypes.ToArray( typeof( Type ) );

				return m_Types;
			}
		}

		public string GetNameFor( Item item )
		{
			if ( item.Name != null )
				return item.Name;
			else
				return item.LabelNumber.ToString();
		}

		public bool IsSellable( Item item )
		{
			//if ( item.Hue != 0 )
				//return false;

			return IsInList( item.GetType() );
		}
	 
		public bool IsResellable( Item item )
		{
			//if ( item.Hue != 0 )
				//return false;

			return IsInList( item.GetType() );
		}

		public bool IsInList( Type type )
		{
			Object o = m_Table[type];

			if ( o == null )
				return false;
			else
				return true;
		}
	}
}
