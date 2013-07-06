using System;
using Server;
using Server.Items;
using Server.Regions;

namespace Server.Misc
{
	public class FindRunes
	{
		public static void Initialize()
		{
			Server.Commands.CommandSystem.Register( "FindRunes", AccessLevel.Administrator, new Server.Commands.CommandEventHandler( FindRunes_OnCommand ) );
		}

		[Usage( "FindRunes" )]
		[Description( "Lists runes (or runebooks with runes) to green acres." )]
		public static void FindRunes_OnCommand( Server.Commands.CommandEventArgs e )
		{
			foreach ( Item item in World.Items.Values )
			{
				if ( item is RecallRune )
				{
					RecallRune rune = (RecallRune)item;

					if ( rune.Marked && rune.TargetMap != null && IsBad( rune.Target, rune.TargetMap ) )
					{
						object root = item.RootParent;

						if ( root is Mobile )
						{
							if ( ((Mobile)root).AccessLevel < AccessLevel.GameMaster )
								e.Mobile.SendAsciiMessage( "Rune: '{4}' {0} [{1}]: {2} ({3})", item.GetWorldLocation(), item.Map, root.GetType().Name, ((Mobile)root).Name, rune.Description );
						}
						else
						{
							e.Mobile.SendAsciiMessage( "Rune: '{3}' {0} [{1}]: {2}", item.GetWorldLocation(), item.Map, root==null ? "(null)" : root.GetType().Name, rune.Description );
						}
					}
				}
			}
		}

		public static bool IsBad( Point3D loc, Map map )
		{
			if ( loc.X < 0 || loc.Y < 0 || loc.X >= map.Width || loc.Y >= map.Height )
			{
				return true;
			}
			else if ( map == Map.Felucca || map == Map.Trammel )
			{
				if ( loc.X >= 5120 && loc.Y >= 0 && loc.X <= 6143 && loc.Y <= 2304 )
				{
					Region r = Region.Find( loc, map );

					if ( !(r is DungeonRegion) )
						return true;
				}
			}
			
			return false;
		}
	}
}