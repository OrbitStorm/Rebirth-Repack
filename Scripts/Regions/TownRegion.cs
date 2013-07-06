using System;
using System.Xml;
using Server;
using Server.Mobiles;

namespace Server.Regions
{
	public class TownRegion : GuardedRegion
	{
		public TownRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
            string frag = Name.ToLower();
            if (frag == "serpent's hold")
                frag = "serphold";
            else if (frag == "skara brae")
                frag = "skara";
            else if (frag == "nujel'm")
                frag = "nujelm";
            else if (frag == "buccaneer's den")
                frag = "bucden";

            try
            {
                m_Frag = (RegionFragment)Enum.Parse(typeof(RegionFragment), frag, true);
            }
            catch (Exception)
            {
                m_Frag = RegionFragment.Wilderness;
            }
		}

        private Mobiles.RegionFragment m_Frag;
        public override Mobiles.RegionFragment Fragment
        {
            get
            {
                return m_Frag;
            }
        }
	}
}