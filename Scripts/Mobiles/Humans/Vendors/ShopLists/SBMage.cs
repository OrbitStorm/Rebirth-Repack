using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMage : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMage()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Type[] types = Loot.RegularScrollTypes;

				for ( int i = 0; i < types.Length / 2; ++i )
				{
					if ( Utility.Random( 2 ) == 1 )
						continue;
					int itemID = 0x1F2E + i;

					if ( i == 6 )
						itemID = 0x1F2D;
					else if ( i > 6 )
						--itemID;

					Add( new GenericBuyInfo( types[i], 12 + ((i / 8) * 10), 5, itemID, 0 ) );
				}

				
				Add( new VarAmtBuyInfo( typeof( Bloodmoss ), 7, 0xF7B ) ); 
				Add( new VarAmtBuyInfo( typeof( MandrakeRoot ), 8, 0xF86 ) ); 
				Add( new VarAmtBuyInfo( typeof( Garlic ), 5, 0xF84 ) ); 
				Add( new VarAmtBuyInfo( typeof( Ginseng ), 6, 0xF85 ) ); 
				Add( new VarAmtBuyInfo( typeof( Nightshade ), 6, 0xF88 ) ); 
				Add( new VarAmtBuyInfo( typeof( BlackPearl ), 7, 0xF7A ) ); 
				Add( new VarAmtBuyInfo( typeof( SpidersSilk ), 5, 0xF8D ) ); 
				Add( new VarAmtBuyInfo( typeof( SulfurousAsh ), 7, 0xF8C ) ); 

				Add( new GenericBuyInfo( 1041072, typeof( MagicWizardsHat ), 11, 10, 0x1718, 0 ) );

				Add( new GenericBuyInfo( typeof( RecallRune ), 15, 10, 0x1f14, 0 ) );
				Add( new GenericBuyInfo( typeof( Spellbook ), 18, 10, 0xEFA, 0 ) );

				// Add( new GenericBuyInfo( typeof( ScribesPen ), 8, 10, 0xFBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 5, 20, 0x0E34, 0 ) );
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
				Add( typeof( BlackPearl ), 4 ); 
				Add( typeof( SpidersSilk ), 4 ); 
				Add( typeof( SulfurousAsh ), 4 ); 

				Add( typeof( RecallRune ), 8 );
				Add( typeof( Spellbook ), 9 );
				Add( typeof( BlankScroll ), 3 );

				Type[] types = Loot.RegularScrollTypes;
				for ( int i = 0; i < types.Length; ++i )
					Add( types[i], 6 + ((i / 8) * 5) );
			}
		}
	}

	public class VarAmtBuyInfo : GenericBuyInfo
	{
		public VarAmtBuyInfo( Type type, int price, int itemid ) : base( type, price, 0, itemid, 0 )
		{
			Amount = MaxAmount = Utility.Random( 50 ) + 100;
		}

		public override void OnRestock()
		{
			//base.OnRestock ();
			if ( Amount <= MaxAmount/5 )
				MaxAmount = (int)(MaxAmount*1.25);
			else if ( Amount >= MaxAmount*0.80 )
				MaxAmount = (int)(MaxAmount*0.75);
			if ( MaxAmount > 400 )
				MaxAmount = 400;
			else if ( MaxAmount < 75 )
				MaxAmount = 100;
			MaxAmount += Utility.Random( 25 );
			MaxAmount = Amount = (int)(Utility.Random( MaxAmount/4 ) + (3*MaxAmount/4));
		}
	}
}
