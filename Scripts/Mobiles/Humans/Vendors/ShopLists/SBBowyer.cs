using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBBowyer : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBowyer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Shaft ), 3, 25, 0x1BD4, 0 ) );
				Add( new GenericBuyInfo( typeof( Feather ), 3, 25, 0x1BD1, 0 ) );
				//Add( new GenericBuyInfo( typeof( FletcherTools ), 20, 20, 0x1022, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Shaft ), 1 );
				Add( typeof( Feather ), 1 );	
				//Add( typeof( FletcherTools ), 10 );
			}
		}
	}
}