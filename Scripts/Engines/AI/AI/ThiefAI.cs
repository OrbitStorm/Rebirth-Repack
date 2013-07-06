using System;
using System.Collections; using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Items;

//
// This is a first simple AI
//
//

namespace Server.Mobiles
{
	public class ThiefAI : MeleeAI
	{
		public ThiefAI(BaseCreature m) : base (m)
		{
		}
	}
}
