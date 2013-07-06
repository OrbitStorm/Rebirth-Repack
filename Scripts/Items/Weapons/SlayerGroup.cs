using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class SlayerGroup
	{
		private static SlayerEntry[] m_TotalEntries;
		private static SlayerGroup[] m_Groups;

		public static SlayerEntry GetEntryByName( SlayerName name )
		{
			int v = (int)name;

			if ( v >= 0 && v < m_TotalEntries.Length )
				return m_TotalEntries[v];

			return null;
		}

		public static SlayerName GetLootSlayerType( Type type )
		{
			return SlayerName.Silver;
		}

		static SlayerGroup()
		{
			SlayerGroup undead = new SlayerGroup();
			
			undead.Super = new SlayerEntry( SlayerName.Silver, 
				typeof( SkeletalMage ), typeof( BoneMagi ), typeof( Lich ), typeof( LichLord ),  
				typeof( SkeletalKnight ), typeof( BoneKnight ), typeof( Skeleton ), typeof( HeadlessOne ),
				typeof( Zombie ), typeof( Ghoul ) );
			undead.Entries = new SlayerEntry[0];
			m_Groups = new SlayerGroup[]{ undead };

			m_TotalEntries = CompileEntries( m_Groups );
		}

		private static SlayerEntry[] CompileEntries( SlayerGroup[] groups )
		{
			SlayerEntry[] entries = new SlayerEntry[27];

			for ( int i = 0; i < groups.Length; ++i )
			{
				SlayerGroup g = groups[i];

				g.Super.Group = g;

				entries[(int)g.Super.Name] = g.Super;

				for ( int j = 0; j < g.Entries.Length; ++j )
				{
					g.Entries[j].Group = g;
					entries[(int)g.Entries[j].Name] = g.Entries[j];
				}
			}

			return entries;
		}

		private SlayerGroup m_Opposition;
		private SlayerEntry m_Super;
		private SlayerEntry[] m_Entries;
		private Type[] m_FoundOn;

		public SlayerGroup Opposition{ get{ return m_Opposition; } set{ m_Opposition = value; } }
		public SlayerEntry Super{ get{ return m_Super; } set{ m_Super = value; } }
		public SlayerEntry[] Entries{ get{ return m_Entries; } set{ m_Entries = value; } }
		public Type[] FoundOn{ get{ return m_FoundOn; } set{ m_FoundOn = value; } }

		public SlayerGroup()
		{
		}
	}
}