using System; 
using System.Collections; using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBHerbalist : SBInfo 
	{ 
		private ArrayList m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBHerbalist() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override ArrayList BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : ArrayList 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new VarAmtBuyInfo( typeof( Bloodmoss ), 9, 0xF7B ) ); 
				Add( new VarAmtBuyInfo( typeof( MandrakeRoot ), 11, 0xF86 ) ); 
				Add( new VarAmtBuyInfo( typeof( Garlic ), 7, 0xF84 ) ); 
				Add( new VarAmtBuyInfo( typeof( Ginseng ), 8, 0xF85 ) ); 
				Add( new VarAmtBuyInfo( typeof( Nightshade ), 8, 0xF88 ) ); 

				Add( new GenericBuyInfo( typeof( Bottle ), 5, 100, 0xF0E, 0 ) ); 
				Add( new GenericBuyInfo( typeof( MortarPestle ), 8, 20, 0xE9B, 0 ) ); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Bloodmoss ), 4 ); 
				Add( typeof( MandrakeRoot ), 4 ); 
				Add( typeof( Garlic ), 3 ); 
				Add( typeof( Ginseng ), 3 ); 
				Add( typeof( Nightshade ), 3 ); 
				Add( typeof( Bottle ), 2 ); 
				Add( typeof( MortarPestle ), 4 ); 
			} 
		} 
	} 
}
