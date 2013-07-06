using System;
using Server;
using Server.Targeting;

namespace Server.Targets
{
	public class PickMoveTarget : Target
	{
		public PickMoveTarget() : base( -1, false, TargetFlags.None )
		{
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if ( o is Item || o is Mobile )
				from.Target = new MoveTarget( o );
		}
	}
}