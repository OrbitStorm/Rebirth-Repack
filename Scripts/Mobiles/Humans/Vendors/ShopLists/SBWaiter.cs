using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBWaiter : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBWaiter()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Ale, 7, 20, 0x99F, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Wine, 7, 20, 0x9C7, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Liquor, 7, 20, 0x99B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Cider, 13, 20, 0x9C8, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Milk, 7, 20, 0x9F0, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Ale, 11, 20, 0x1F95, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Cider, 11, 20, 0x1F97, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Liquor, 11, 20, 0x1F99, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Wine, 11, 20, 0x1F9B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Water, 11, 20, 0x1F9D, 0 ) );
				Add( new GenericBuyInfo( typeof( Pitcher ), 7, 20, 0xFF6, 0 ) );
				Add( new GenericBuyInfo( typeof( BreadLoaf ), 7, 10, 0x103B, 0 ) );
				Add( new GenericBuyInfo( typeof( CheeseWheel ), 25, 10, 0x97E, 0 ) );
				Add( new GenericBuyInfo( typeof( CookedBird ), 17, 20, 0x9B7, 0 ) );
				Add( new GenericBuyInfo( typeof( LambLeg ), 8, 20, 0x160A, 0 ) );
				// TODO: Bowl of *, tomato soup, baked pie
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
			}
		}
	}
}