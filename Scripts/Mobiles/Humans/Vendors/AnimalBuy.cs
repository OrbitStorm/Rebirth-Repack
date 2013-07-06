using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class AnimalBuyInfo : GenericBuyInfo
	{
		private int m_ControlSlots;

		public AnimalBuyInfo( int controlSlots, Type type, int price, int amount, int itemID, int hue ) : base( 0, type, price, amount, itemID, hue )
		{
			m_ControlSlots = controlSlots;
		}

		public override int ControlSlots
		{
			get
			{
				return m_ControlSlots;
			}
		}
	}
}