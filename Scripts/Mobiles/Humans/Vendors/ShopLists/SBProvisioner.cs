using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBProvisioner : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBProvisioner()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Arrow ), 3, 20, 0xF3F, 0 ) );
				Add( new GenericBuyInfo( typeof( Bolt ), 6, 20, 0x1BFB, 0 ) );
				Add( new GenericBuyInfo( typeof( Backpack ), 15, 20, 0x9B2, 0 ) );
				Add( new GenericBuyInfo( typeof( Pouch ), 6, 20, 0xE79, 0 ) );
				Add( new GenericBuyInfo( typeof( Bag ), 6, 20, 0xE76, 0 ) );
				Add( new GenericBuyInfo( typeof( Candle ), 6, 20, 0xA28, 0 ) );
				Add( new GenericBuyInfo( typeof( Torch ), 7, 20, 0xF6B, 0 ) );
				Add( new GenericBuyInfo( typeof( Lantern ), 2, 20, 0xA25, 0 ) );
				Add( new GenericBuyInfo( typeof( Lockpick ), 12, 20, 0x14FC, 0 ) );
				// TODO: Array of hats, randomly colored
				Add( new GenericBuyInfo( typeof( BreadLoaf ), 7, 20, 0x103B, 0 ) );
				Add( new GenericBuyInfo( typeof( LambLeg ), 8, 20, 0x160A, 0 ) );
				Add( new GenericBuyInfo( typeof( ChickenLeg ), 6, 20, 0x1608, 0 ) );
				Add( new GenericBuyInfo( typeof( CookedBird ), 17, 20, 0x9B7, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Ale, 7, 20, 0x99F, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Wine, 7, 20, 0x9C7, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Liquor, 7, 20, 0x99B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Cider, 13, 20, 0x9C8, 0 ) );
				Add( new GenericBuyInfo( typeof( Pear ), 3, 20, 0x994, 0 ) );
				Add( new GenericBuyInfo( typeof( Apple ), 3, 20, 0x9D0, 0 ) );
				Add( new GenericBuyInfo( typeof( Garlic ), 8, 100, 0xF84, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Ginseng ), 9, 100, 0xF85, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Beeswax ), 1, 20, 0x1422, 0 ) );
				Add( new GenericBuyInfo( typeof( Bottle ), 5, 20, 0xF0E, 0 ) );
				Add( new GenericBuyInfo( typeof( RedBook ), 15, 20, 0xFF1, 0 ) );
				Add( new GenericBuyInfo( typeof( BlueBook ), 15, 20, 0xFF2, 0 ) );
				Add( new GenericBuyInfo( typeof( TanBook ), 15, 20, 0xFF0, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenBox ), 14, 20, 0xE7D, 0 ) );
				// TODO: Copper key, bedroll
				Add( new GenericBuyInfo( typeof( BedRoll ), 18, 20, 0x0A58, 0 ) );
				Add( new GenericBuyInfo( typeof( Kindling ), 3, 20, 0xDE1, 0 ) );
				Add( new GenericBuyInfo( 1041205, typeof( Multis.SmallBoatDeed ), 12500, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( 1041055, typeof( GuildDeed ), 12450, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041060, typeof( HairDye ), 265, 20, 0xEFF, 0 ) );
				Add( new GenericBuyInfo( 1016450, typeof( Chessboard ), 2, 20, 0xFA6, 0 ) );
				Add( new GenericBuyInfo( 1016449, typeof( CheckerBoard ), 2, 20, 0xFA6, 0 ) );
				Add( new GenericBuyInfo( 1041243, typeof( ContractOfEmployment ), 1025, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( typeof( Backgammon ), 2, 20, 0xE1C, 0 ) );
				Add( new GenericBuyInfo( typeof( Dices ), 2, 20, 0xFA7, 0 ) );
				Add( new GenericBuyInfo( typeof( KeyRing ), 12, 10, 0x1011, 0 ) );

				if ( Core.AOS )
					Add( new GenericBuyInfo( typeof( Engines.Mahjong.MahjongGame ), 6, 20, 0xFAA, 0 ) );
				// TODO: Plant bowl, bagballs
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Arrow ), 2 );
				Add( typeof( Bolt ), 3 );
				Add( typeof( Backpack ), 7 );
				Add( typeof( Pouch ), 3 );
				Add( typeof( Bag ), 3 );
				Add( typeof( Candle ), 3 );
				Add( typeof( Torch ), 3 );
				Add( typeof( Lantern ), 1 );
				Add( typeof( Lockpick ), 6 );
				Add( typeof( Bottle ), 3 );
				Add( typeof( RedBook ), 7 );
				Add( typeof( BlueBook ), 7 );
				Add( typeof( TanBook ), 7 );
				Add( typeof( WoodenBox ), 7 );
				Add( typeof( HairDye ), 130 );
				Add( typeof( Chessboard ), 1 );
				Add( typeof( CheckerBoard ), 1 );
				Add( typeof( Backgammon ), 1 );
				Add( typeof( Dices ), 1 );
				Add( typeof( GuildDeed ), 6225 );
				Add( typeof( Beeswax ), 1 );
				Add( typeof( ContractOfEmployment ), 513 );
				Add( typeof( BedRoll ), 9 );
				Add( typeof( KeyRing ), 5 );
				Add( typeof( ComCrystal ), 78 );
			}
		}
	}
}
