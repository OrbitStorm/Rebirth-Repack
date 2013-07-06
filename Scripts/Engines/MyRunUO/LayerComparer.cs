using System;
using System.Collections; using System.Collections.Generic;

namespace Server.Engines.MyRunUO
{
	public class LayerComparer : IComparer
	{
		private static Layer[] m_DesiredLayerOrder = new Layer[]
		{
			Layer.Cloak,
			Layer.Shirt,
			Layer.Pants,
			Layer.Shoes,
			Layer.InnerLegs,
			Layer.OuterLegs,
			Layer.InnerTorso,
			Layer.Arms,
			Layer.MiddleTorso,
			Layer.OuterTorso,
			Layer.Waist,
			Layer.Bracelet,
			Layer.Ring,
			Layer.Gloves,
			Layer.OneHanded,
			Layer.TwoHanded,
			Layer.Neck,
			Layer.FacialHair,
			Layer.Hair,
			Layer.Helm
		};

		private static int[] m_TranslationTable;

		public static int[] TranslationTable
		{
			get{ return m_TranslationTable; }
		}

		static LayerComparer()
		{
			m_TranslationTable = new int[256];

			for ( int i = 0; i < m_DesiredLayerOrder.Length; ++i )
				m_TranslationTable[(int)m_DesiredLayerOrder[i]] = m_DesiredLayerOrder.Length - i;
		}

		public static bool IsValid( Item item )
		{
			return ( m_TranslationTable[(int)item.Layer] > 0 );
		}

		public static readonly IComparer Instance = new LayerComparer();

		public LayerComparer()
		{
		}

		public int Compare( object x, object y )
		{
			Item a = (Item)x;
			Item b = (Item)y;

			return m_TranslationTable[(int)b.Layer] - m_TranslationTable[(int)a.Layer];
		}
	}
}