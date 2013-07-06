
using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBVeterinarian : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBVeterinarian()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new AnimalBuyInfo( 1, typeof( Cat ), 138, 10, 0x211B, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( Dog ), 181, 10, 0x211C, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( PackHorse ), 606, 10, 0x2126, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( PackLlama ), 491, 10, 0x2127, 0 ) );
				Add( new GenericBuyInfo( typeof( Bandage ), 5, 20, 0xE21, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Bandage ), 2 );
			}
		}
	}
}