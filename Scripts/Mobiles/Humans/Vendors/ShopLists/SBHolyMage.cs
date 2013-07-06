using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBHolyMage : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHolyMage()
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

					Add( new GenericBuyInfo( types[i], 12 + ((i / 8) * 10), 20, itemID, 0 ) );
				}

				Add( new GenericBuyInfo( typeof( NightSightPotion ), 15, 20, 0xF06, 0 ) ); 
				Add( new GenericBuyInfo( typeof( AgilityPotion ), 15, 20, 0xF08, 0 ) );
				Add( new GenericBuyInfo( typeof( StrengthPotion ), 15, 20, 0xF09, 0 ) );
				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 20, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 20, 0xF07, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 20, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserPoisonPotion ), 15, 20, 0xF0A, 0 ) );

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
				Add( typeof( BlackPearl ), 4 ); 
				Add( typeof( Bloodmoss ), 4 ); 
				Add( typeof( MandrakeRoot ), 4 ); 
				Add( typeof( Garlic ), 3 ); 
				Add( typeof( Ginseng ), 3 ); 
				Add( typeof( Nightshade ), 3 ); 
				Add( typeof( SpidersSilk ), 4 ); 
				Add( typeof( SulfurousAsh ), 4 ); 
				Add( typeof( RecallRune ), 8 );
				Add( typeof( Spellbook ), 9 );
				Add( typeof( BlankScroll ), 3 );

				Add( typeof( NightSightPotion ), 7 );
				Add( typeof( AgilityPotion ), 7 );
				Add( typeof( StrengthPotion ), 7 );
				Add( typeof( RefreshPotion ), 7 );
				Add( typeof( LesserCurePotion ), 7 );
				Add( typeof( LesserHealPotion ), 7 );
				Add( typeof( LesserPoisonPotion ), 7 );

				Type[] types = Loot.RegularScrollTypes;

				for ( int i = 0; i < types.Length; ++i )
					Add( types[i], 6 + ((i / 8) * 5) );
			}
		}
	}
}