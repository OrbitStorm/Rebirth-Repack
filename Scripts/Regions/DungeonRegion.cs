using System;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Regions
{
	public class DungeonRegion : BaseRegion
	{
		private Point3D m_EntranceLocation;
		private Map m_EntranceMap;

		public Point3D EntranceLocation{ get{ return m_EntranceLocation; } set{ m_EntranceLocation = value; } }
		public Map EntranceMap{ get{ return m_EntranceMap; } set{ m_EntranceMap = value; } }

        public bool CanRecall { get { return true; } }
        public bool CanGate { get { return true; } }

		public DungeonRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
			XmlElement entrEl = xml["entrance"];

			Map entrMap = map;
			ReadMap( entrEl, "map", ref entrMap, false );

			if ( ReadPoint3D( entrEl, entrMap, ref m_EntranceLocation, false ) )
				m_EntranceMap = entrMap;
		}

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override void OnEnter(Mobile m)
        {
        }

        public override void OnExit(Mobile m)
        {
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            global = LightCycle.DungeonLevel;
        }
	}
}