using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBPlateArmor: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBPlateArmor()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new ColoredPlateBuyInfo( typeof( PlateArms ), 181, 20, 0x1410 ) );
				Add( new ColoredPlateBuyInfo( typeof( PlateChest ), 273, 20, 0x1415 ) );
				Add( new ColoredPlateBuyInfo( typeof( PlateGloves ), 145, 20, 0x1414 ) );
				Add( new ColoredPlateBuyInfo( typeof( PlateGorget ), 124, 20, 0x1413 ) );
				Add( new ColoredPlateBuyInfo( typeof( PlateLegs ), 218, 20, 0x1411 ) );

				Add( new ColoredPlateBuyInfo( typeof( PlateHelm ), 170, 20, 0x1412 ) );
				Add( new GenericBuyInfo( typeof( FemalePlateChest ), 245, 20, 0x1C04, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( PlateArms ), 90 );
				Add( typeof( PlateChest ), 136 );
				Add( typeof( PlateGloves ), 72 );
				Add( typeof( PlateGorget ), 70 );
				Add( typeof( PlateLegs ), 109 );

				Add( typeof( PlateHelm ), 85 );
				Add( typeof( FemalePlateChest ), 122 );
			}
		}
	}
}
