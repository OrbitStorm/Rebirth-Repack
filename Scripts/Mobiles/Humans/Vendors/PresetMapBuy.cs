using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class PresetMapBuyInfo : GenericBuyInfo
	{
		private PresetMapEntry m_Entry;

		public PresetMapBuyInfo( PresetMapEntry entry, int price, int amount ) : base( entry.Name, null, price, amount, 0x14EC, 0 )
		{
			m_Entry = entry;
		}

		public override object GetObject()
		{
			return new PresetMap( m_Entry );
		}
	}
}