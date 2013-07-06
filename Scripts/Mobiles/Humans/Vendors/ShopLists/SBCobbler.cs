using System; 
using System.Collections; using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBCobbler : SBInfo 
	{ 
		private ArrayList m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBCobbler() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override ArrayList BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : ArrayList 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( typeof( Shoes ), 23, 20, 0x170f, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Boots ), 38, 20, 0x170b, 0 ) ); 
				Add( new GenericBuyInfo( typeof( ThighBoots ), 56, 20, 0x1711, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Sandals ), 18, 20, 0x170d, 0 ) ); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Shoes ), 11 ); 
				Add( typeof( Boots ), 19 ); 
				Add( typeof( ThighBoots ), 28 ); 
				Add( typeof( Sandals ), 9 ); 
			} 
		} 
	} 
}