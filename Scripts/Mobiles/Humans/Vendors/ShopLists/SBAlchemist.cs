using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAlchemist : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAlchemist()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new VarAmtBuyInfo( typeof( Bloodmoss ), 7, 0xF7B ) ); 
				Add( new VarAmtBuyInfo( typeof( MandrakeRoot ), 8, 0xF86 ) ); 
				Add( new VarAmtBuyInfo( typeof( Garlic ), 5, 0xF84 ) ); 
				Add( new VarAmtBuyInfo( typeof( Ginseng ), 6, 0xF85 ) ); 
				Add( new VarAmtBuyInfo( typeof( Nightshade ), 6, 0xF88 ) ); 
				Add( new VarAmtBuyInfo( typeof( BlackPearl ), 7, 0xF7A ) ); 
				Add( new VarAmtBuyInfo( typeof( SpidersSilk ), 5, 0xF8D ) ); 
				Add( new VarAmtBuyInfo( typeof( SulfurousAsh ), 7, 0xF8C ) ); 

				Add( new GenericBuyInfo( typeof( Bottle ), 5, 100, 0xF0E, 0 ) ); 
				Add( new GenericBuyInfo( typeof( MortarPestle ), 8, 10, 0xE9B, 0 ) );

				Add( new GenericBuyInfo( 1041060, typeof( HairDye ), 265, 10, 0xEFF, 0 ) );

				Add( new GenericBuyInfo( typeof( NightSightPotion ), 15, 10, 0xF06, 0 ) ); 
				Add( new GenericBuyInfo( typeof( AgilityPotion ), 15, 10, 0xF08, 0 ) );
				Add( new GenericBuyInfo( typeof( StrengthPotion ), 15, 10, 0xF09, 0 ) );
				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 10, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 10, 0xF07, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 10, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserPoisonPotion ), 15, 10, 0xF0A, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserExplosionPotion ), 21, 10, 0xF0D, 0 ) );
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
				Add( typeof( Bottle ), 2 );
				Add( typeof( MortarPestle ), 4 );
				Add( typeof( HairDye ), 130 );

				Add( typeof( NightSightPotion ), 7 );
				Add( typeof( AgilityPotion ), 7 );
				Add( typeof( StrengthPotion ), 7 );
				Add( typeof( RefreshPotion ), 7 );
				Add( typeof( LesserCurePotion ), 7 );
				Add( typeof( LesserHealPotion ), 7 );
				Add( typeof( LesserPoisonPotion ), 7 );
				Add( typeof( LesserExplosionPotion ), 10 );
			}
		}
	}
}
