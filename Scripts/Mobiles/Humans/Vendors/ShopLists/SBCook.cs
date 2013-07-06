using System; 
using System.Collections; using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBCook : SBInfo 
	{ 
		private ArrayList m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBCook() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override ArrayList BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : ArrayList 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( typeof( CheeseWheel ), 25, 20, 0x97E, 0 ) );
				Add( new GenericBuyInfo( 1044567, typeof( Skillet ), 3, 20, 0x97F, 0 ) );
				Add( new GenericBuyInfo( typeof( CookedBird ), 17, 20, 0x9B7, 0 ) );
				Add( new GenericBuyInfo( typeof( RoastPig ), 106, 20, 0x9BB, 0 ) );
				Add( new GenericBuyInfo( typeof( Cake ), 11, 20, 0x9E9, 0 ) );
				// TODO: Muffin @ 3gp
				Add( new GenericBuyInfo( typeof( JarHoney ), 3, 20, 0x9EC, 0 ) );
				Add( new GenericBuyInfo( typeof( SackFlour ), 3, 20, 0x1039, 0 ) );
				Add( new GenericBuyInfo( typeof( BreadLoaf ), 7, 20, 0x103B, 0 ) );
				Add( new GenericBuyInfo( typeof( FlourSifter ), 2, 20, 0x103E, 0 ) );
				//Add( new GenericBuyInfo( typeof( BakedPie ), 7, 20, 0x1041, 0 ) );
				Add( new GenericBuyInfo( typeof( RollingPin ), 2, 20, 0x1043, 0 ) );
				// TODO: Bowl of carrots/corn/lettuce/peas/potatoes/stew/tomato soup @ 3gp
				// TODO: Pewter bowl @ 2gp
				Add( new GenericBuyInfo( typeof( ChickenLeg ), 6, 20, 0x1608, 0 ) );
				Add( new GenericBuyInfo( typeof( LambLeg ), 8, 20, 0x1609, 0 ) );
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( CheeseWheel ), 12 );
				Add( typeof( CookedBird ), 8 );
				Add( typeof( RoastPig ), 53 );
				Add( typeof( Cake ), 5 );
				Add( typeof( JarHoney ), 1 );
				Add( typeof( SackFlour ), 1 );
				Add( typeof( BreadLoaf ), 3 );
				Add( typeof( ChickenLeg ), 3 );
				Add( typeof( LambLeg ), 4 );
				Add( typeof( Skillet ), 1 );
				Add( typeof( FlourSifter ), 1 );
				Add( typeof( RollingPin ), 1 );
			} 
		} 
	} 
}