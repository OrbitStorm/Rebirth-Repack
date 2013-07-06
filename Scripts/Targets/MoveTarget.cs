using System;
using Server;
using Server.Targeting;

namespace Server.Targets
{
	public class MoveTarget : Target
	{
		private object m_Object;

		public MoveTarget( object o ) : base( -1, true, TargetFlags.None )
		{
			m_Object = o;
		}

		protected override void OnTarget( Mobile from, object o )
		{
			IPoint3D p = o as IPoint3D;

			if ( p != null )
			{
				if ( p is Item )
					p = ((Item)p).GetWorldTop();

				Server.Scripts.Commands.CommandLogging.WriteLine( from, "{0} {1} moving {2} to {3}", from.AccessLevel, Server.Scripts.Commands.CommandLogging.Format( from ), Server.Scripts.Commands.CommandLogging.Format( m_Object ), new Point3D( p ) );

				if ( m_Object is Item )
				{
					Item item = (Item)m_Object;

					if ( !item.Deleted )
						item.MoveToWorld( new Point3D( p ), from.Map );
				}
				else if ( m_Object is Mobile )
				{
					Mobile m = (Mobile)m_Object;

					if ( !m.Deleted )
					{
						m.Map = from.Map;
						m.Location = new Point3D( p );
					}
				}
			}
		}
	}
}