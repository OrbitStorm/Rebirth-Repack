using System;
using System.Collections; using System.Collections.Generic;
using Server.Multis.Deeds;

namespace Server.Mobiles
{
	public class SBHouseDeed: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHouseDeed()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{				 
				Add( new GenericBuyInfo( 1041217, typeof( BlueTentDeed ), 4300, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041218, typeof( GreenTentDeed ), 4300, 10, 0x14F0, 0 ) );

				Add( new GenericBuyInfo( 1041211, typeof( StonePlasterHouseDeed ), 11500, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041212, typeof( FieldStoneHouseDeed ), 11500, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041213, typeof( SmallBrickHouseDeed), 11500, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041214, typeof( WoodHouseDeed ), 11500, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041215, typeof( WoodPlasterHouseDeed ), 11500, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041216, typeof( ThatchedRoofCottageDeed ), 11500, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "Deed to a Small Smith's Shop", typeof( SmallForgeHouseDeed ), 16400, 10, 0x14f0, 0 ) );
				Add( new GenericBuyInfo( "Deed to a Weapon Training Hut", typeof( SmallTrainingHouseDeed ), 16400, 10, 0x14f0, 0 ) );
				Add( new GenericBuyInfo( "Deed to a Pickpocket's Den", typeof( SmallPickpocketHouseDeed ), 16400, 10, 0x14f0, 0 ) );
				Add( new GenericBuyInfo( "Deed to a Small Weaver's Shop", typeof( SmallTailorHouseDeed ), 16400, 10, 0x14f0, 0 ) );
				Add( new GenericBuyInfo( "Deed to a Small Baker's Shop", typeof( SmallBakeryHouseDeed ), 16400, 10, 0x14f0, 0 ) );
				Add( new GenericBuyInfo( 1041225, typeof( LargeForgeHouseDeed ), 38400, 10, 0x14f0, 0 ) );// deed to a blacksmith's shop
				Add( new GenericBuyInfo( 1041219, typeof( BrickHouseDeed ), 38400, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041220, typeof( TwoStoryWoodPlasterHouseDeed ), 57300, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041221, typeof( TwoStoryStonePlasterHouseDeed ), 57300, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041222, typeof( TowerDeed ), 132000, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041223, typeof( KeepDeed ), 288000, 10, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( 1041224, typeof( CastleDeed ), 490000, 10, 0x14F0, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BlueTentDeed ), 4300 );
				Add( typeof( GreenTentDeed ), 4300 );
				Add( typeof( StonePlasterHouseDeed ), 11500 );
				Add( typeof( FieldStoneHouseDeed ), 11500 );
				Add( typeof( SmallBrickHouseDeed ), 11500 );
				Add( typeof( WoodHouseDeed ), 11500 );
				Add( typeof( WoodPlasterHouseDeed ), 11500 );
				Add( typeof( ThatchedRoofCottageDeed ), 11500 );
				Add( typeof( SmallForgeHouseDeed ), 16400 );
				Add( typeof( BrickHouseDeed ), 38400 );
				Add( typeof( LargeForgeHouseDeed ), 38400 );
				Add( typeof( TwoStoryWoodPlasterHouseDeed ), 57300 );
				Add( typeof( TwoStoryStonePlasterHouseDeed ), 57300 );
				Add( typeof( TowerDeed ), 132000 );
				Add( typeof( KeepDeed ), 288000 );
				Add( typeof( CastleDeed ), 490000 );
			}
		}
	}
}
