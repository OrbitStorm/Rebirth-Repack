using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBJewel: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBJewel()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Amber ), 90, 20, 0xF25, 0 ) );
				Add( new GenericBuyInfo( typeof( Amethyst ), 120, 20, 0xF16, 0 ) );
				Add( new GenericBuyInfo( typeof( Citrine ), 60, 20, 0xF15, 0 ) );
				Add( new GenericBuyInfo( typeof( Diamond ), 240, 20, 0xF26, 0 ) );
				Add( new GenericBuyInfo( typeof( Emerald ), 120, 20, 0xF10, 0 ) );
				Add( new GenericBuyInfo( typeof( Ruby ), 90, 20, 0xF13, 0 ) );
				Add( new GenericBuyInfo( typeof( Sapphire ), 120, 20, 0xF19, 0 ) );
				Add( new GenericBuyInfo( typeof( StarSapphire ), 150, 20, 0xF21, 0 ) );
				Add( new GenericBuyInfo( typeof( Tourmaline ), 90, 20, 0xF2D, 0 ) );

				Add( new GenericBuyInfo( 1060740, typeof( ComCrystal ), 578, 10, 0x1ECD, 0 ) );// communication crystal
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Amber ), 45 );
				Add( typeof( Amethyst ), 60 );
				Add( typeof( Citrine ), 30 );
				Add( typeof( Diamond ), 120 );
				Add( typeof( Emerald ), 60 );
				Add( typeof( Ruby ), 45 );
				Add( typeof( Sapphire ), 60 );
				Add( typeof( StarSapphire ), 75 );
				Add( typeof( Tourmaline ), 45 );

				Add( typeof( ComCrystal ), 78 );
			}
		}
	}
}
