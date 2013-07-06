using System;
using Server;
using Server.Targeting;

namespace Server
{
	public class Catrography
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Cartography].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();
			
			new Engines.Craft.CartographySystem().Begin( m, null );

			return TimeSpan.FromSeconds( 10.0 );
		}
	}
}
