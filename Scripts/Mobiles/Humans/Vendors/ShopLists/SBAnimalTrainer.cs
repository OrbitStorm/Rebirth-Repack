
using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAnimalTrainer : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAnimalTrainer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new AnimalBuyInfo( 1, typeof( Eagle ), 402, 10, 0x211D, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( Cat ), 138, 10, 0x211B, 0 ) );
				//Add( new AnimalBuyInfo( 1, typeof( Horse ), 2087, 10, 0x2124, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( Rabbit ), 78, 10, 0x2125, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( BrownBear ), 855, 10, 0x2118, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( GrizzlyBear ), 1767, 10, 0x211E, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( Panther ), 1271, 10, 0x2119, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( Dog ), 181, 10, 0x211C, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( TimberWolf ), 768, 10, 0x2122, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( PackHorse ), 1102, 10, 0x2126, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( GiantRat ), 107, 10, 0x2123, 0 ) );
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
