using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBChainmailArmor: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBChainmailArmor()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new ColoredPlateBuyInfo( typeof( ChainChest ), 207, 20, 0x13BF ) );
				Add( new ColoredPlateBuyInfo( typeof( ChainLegs ), 166, 20, 0x13BE ) );
				Add( new ColoredPlateBuyInfo( typeof( ChainCoif ), 130, 20, 0x13BB ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( ChainChest ), 103 );
				Add( typeof( ChainLegs ), 83 );
				Add( typeof( ChainCoif ), 65 );
			}
		}
	}
}
