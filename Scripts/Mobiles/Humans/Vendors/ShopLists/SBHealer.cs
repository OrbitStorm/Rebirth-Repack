using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBHealer : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHealer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Bandage ), 5, 20, 0xE21, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 20, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 20, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 20, 0xF0B, 0 ) );

				Add( new VarAmtBuyInfo( typeof( Garlic ), 7, 0xF84 ) ); 
				Add( new VarAmtBuyInfo( typeof( Ginseng ), 8, 0xF85 ) ); 
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Bandage ), 2 );
				Add( typeof( LesserHealPotion ), 7 );
				Add( typeof( RefreshPotion ), 7 );
				Add( typeof( Garlic ), 3 );
				Add( typeof( Ginseng ), 3 );
				Add( typeof( SpidersSilk ), 4 );
			}
		}
	}
}