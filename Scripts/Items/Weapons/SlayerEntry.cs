using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class SlayerEntry
	{
		private SlayerGroup m_Group;
		private SlayerName m_Name;
		private Type[] m_Types;

		public SlayerGroup Group{ get{ return m_Group; } set{ m_Group = value; } }
		public SlayerName Name{ get{ return m_Name; } }
		public Type[] Types{ get{ return m_Types; } }

		public SlayerEntry( SlayerName name, params Type[] types )
		{
			m_Name = name;
			m_Types = types;
		}

		public bool Slays( Mobile m )
		{
			Type t = m.GetType();

			for ( int i = 0; i < m_Types.Length; ++i )
				if ( m_Types[i] == t )
					return true;

			return false;
		}
	}
}