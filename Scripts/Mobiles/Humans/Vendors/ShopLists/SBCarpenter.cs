using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBCarpenter: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBCarpenter()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( WoodenBox ), 14, 20, 0xe7d, 0 ) );
				Add( new GenericBuyInfo( typeof( SmallCrate ), 10, 20, 0xe7e, 0 ) );
				Add( new GenericBuyInfo( typeof( MediumCrate ), 12, 20, 0xe3e, 0 ) );
				Add( new GenericBuyInfo( typeof( LargeCrate ), 14, 20, 0xe3c, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenChest ), 30, 20, 0xe43, 0 ) );
				
				Add( new GenericBuyInfo( typeof( LargeTable ), 20, 20, 0xB7D, 0 ) );
				Add( new GenericBuyInfo( typeof( Nightstand ), 15, 20, 0xB34, 0 ) );
				Add( new GenericBuyInfo( typeof( YewWoodTable ), 20, 20, 0xB7C, 0 ) );

				Add( new GenericBuyInfo( typeof( Throne ), 58, 20, 0xB33, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenThrone ), 12, 20, 0xB2F, 0 ) );
				Add( new GenericBuyInfo( typeof( Stool ), 12, 20, 0xA2A, 0 ) );
				Add( new GenericBuyInfo( typeof( FootStool ), 12, 20, 0xB5E, 0 ) );

				Add( new GenericBuyInfo( typeof( FancyWoodenChairCushion ), 24, 20, 0xB4E, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenChairCushion ), 20, 20, 0xB52, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenChair ), 16, 20, 0xB56, 0 ) );
				Add( new GenericBuyInfo( typeof( BambooChair ), 12, 20, 0xB5A, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenBench ), 12, 20, 0xB2C, 0 ) );

				Add( new GenericBuyInfo( typeof( Saw ), 18, 20, 0x1034, 0 ) );
				Add( new GenericBuyInfo( typeof( Scorp ), 12, 20, 0x10E7, 0 ) );
				Add( new GenericBuyInfo( typeof( SmoothingPlane ), 12, 20, 0x1032, 0 ) );
				Add( new GenericBuyInfo( typeof( DrawKnife ), 12, 20, 0x10E4, 0 ) );
				Add( new GenericBuyInfo( typeof( Froe ), 12, 20, 0x10E5, 0 ) );
				Add( new GenericBuyInfo( typeof( Hammer ), 28, 20, 0x102A, 0 ) );
				//Add( new GenericBuyInfo( typeof( Inshave ), 12, 20, 0x10E6, 0 ) );
				Add( new GenericBuyInfo( typeof( JointingPlane ), 13, 20, 0x1030, 0 ) );
				Add( new GenericBuyInfo( typeof( MouldingPlane ), 13, 20, 0x102C, 0 ) );
				Add( new GenericBuyInfo( typeof( DovetailSaw ), 14, 20, 0x1028, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( WoodenBox ), 7 );
				Add( typeof( SmallCrate ), 5 );
				Add( typeof( MediumCrate ), 6 );
				Add( typeof( LargeCrate ), 7 );
				Add( typeof( WoodenChest ), 15 );
				
				Add( typeof( LargeTable ), 10 );
				Add( typeof( Nightstand ), 7 );
				Add( typeof( YewWoodTable ), 10 );
				Add( typeof( WritingTable ), 9 );

				Add( typeof( Throne ), 24 );
				Add( typeof( WoodenThrone ), 6 );
				Add( typeof( Stool ), 6 );
				Add( typeof( FootStool ), 6 );

				Add( typeof( FancyWoodenChairCushion ), 12 );
				Add( typeof( WoodenChairCushion ), 10 );
				Add( typeof( WoodenChair ), 8 );
				Add( typeof( BambooChair ), 6 );
				Add( typeof( WoodenBench ), 6 );

				Add( typeof( Saw ), 9 );
				Add( typeof( Scorp ), 6 );
				Add( typeof( SmoothingPlane ), 6 );
				Add( typeof( DrawKnife ), 6 );
				Add( typeof( Froe ), 6 );
				Add( typeof( Hammer ), 14 );
				Add( typeof( Inshave ), 6 );
				Add( typeof( JointingPlane ), 6 );
				Add( typeof( MouldingPlane ), 6 );
				Add( typeof( DovetailSaw ), 7 );
			}
		}
	}
}
