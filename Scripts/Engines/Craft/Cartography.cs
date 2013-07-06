using System;
using System.Collections; using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Network;

namespace Server.Engines.Craft
{
	public class CartographySystem : CraftSystem
	{
		private static ItemListEntry[] m_Menu = new ItemListEntry[]
		{
			new CraftSystemItem( "A map of the local environs.",	6511, SkillName.Cartography, 0, 100, 1, typeof( LocalMap ) ),
			new CraftSystemItem( "A map suitable for cities.",		6512, SkillName.Cartography, 0, 100, 1, typeof( CityMap ) ),
			new CraftSystemItem( "A moderately sized sea chart.",	6513, SkillName.Cartography, 0, 100, 1, typeof( SeaChart ) ),
			new CraftSystemItem( "A map of the world.",				6514, SkillName.Cartography, 0, 100, 1, typeof( WorldMap ) ),
		};

		private Item m_Res;

		public override string TargetPrompt
		{
			get
			{
				return "Target the map you wish to use.";
			}
		}

		protected override void OnFailure()
		{
			m_Mobile.SendAsciiMessage( "Thy trembling hand results in an unusable map." );
			End();
		}

		protected override void OnSuccess(Item made)
		{
			m_Mobile.SendAsciiMessage( "With great care, thou dost make a chart of the geography." );
			m_Mobile.AddToBackpack( made );
			End();
		}

		protected override bool OnTarget( Item target )
		{
			if ( target is BlankMap )
			{
				m_Res = target;
				return ShowMenu( m_Menu );
			}
			else
			{
				return false;
			}
		}

		protected override bool HasResources(int count, Type toMake)
		{
			return m_Res != null && !m_Res.Deleted && m_Res.Amount >= count && m_Res.IsChildOf( m_Mobile ) && CheckTool();
		}

		protected override bool ConsumeResources(int count, Type toMake)
		{
			if ( HasResources( count, toMake ) )
			{
				m_Res.Amount -= count;
				if ( m_Res.Amount <= 0 )
					m_Res.Delete();
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
