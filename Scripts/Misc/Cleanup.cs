using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Misc
{
	public class Cleanup
	{
		public static void Initialize()
		{
			Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), new TimerCallback( Run ) );
		}

		public static void Run()
		{
			ArrayList items = new ArrayList();
			ArrayList commodities = new ArrayList();

			int boxes = 0;

			foreach ( Item item in World.Items.Values )
			{
				if ( item is CommodityDeed )
				{
					CommodityDeed deed = (CommodityDeed)item;

					if ( deed.Commodity != null )
						commodities.Add( deed.Commodity );

					continue;
				}
				else if ( item is BankBox )
				{
					BankBox box = (BankBox)item;
					Mobile owner = box.Owner;

					if ( owner == null )
					{
						items.Add( box );
						++boxes;
					}
					else if ( !owner.Player && box.Items.Count == 0 )
					{
						items.Add( box );
						++boxes;
					}

					continue;
				}

				if ( item.Parent != null || item.Map != Map.Internal || item.HeldBy != null )
					continue;

				if ( item.Location != Point3D.Zero )
					continue;

				if ( !IsBuggable( item ) )
					continue;

				items.Add( item );
			}

			for ( int i = 0; i < commodities.Count; ++i )
				items.Remove( commodities[i] );

			if ( items.Count > 0 )
			{
				if ( boxes > 0 )
					Console.WriteLine( "Cleanup: Detected {0} inaccessible items, including {1} bank boxes, removing..", items.Count, boxes );
				else
					Console.WriteLine( "Cleanup: Detected {0} inaccessible items, removing..", items.Count );

				for ( int i = 0; i < items.Count; ++i )
					((Item)items[i]).Delete();
			}
		}

		public static bool IsBuggable( Item item )
		{
			if ( item is Fists )
				return false;

			if ( item is ICommodity || item is Multis.BaseBoat
				|| item is Fish || item is BigFish
				|| item is BasePotion || item is Food || item is CookableFood
				|| item is SpecialFishingNet || item is BaseMagicFish
				|| item is Shoes || item is Sandals
				|| item is Boots || item is ThighBoots
				|| item is MessageInABottle
				|| item is BaseArmor || item is BaseWeapon
				|| item is BaseClothing )
				return true;

			return false;
		}
	}
}