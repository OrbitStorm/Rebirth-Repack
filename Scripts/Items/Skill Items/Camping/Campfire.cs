using System;
using Server.Network;
using Server.Mobiles;
using System.Collections; using System.Collections.Generic;

namespace Server.Items
{
	public class Campfire : BaseItem
	{
		private Timer m_Timer;
		private ArrayList m_Records;

		[Constructable]
		public Campfire() : base( 0xDE3 )
		{
			Movable = false;
			Light = LightType.Circle300;
			m_Records = new ArrayList();

			m_Timer = new DecayTimer( this );
			m_Timer.Start();
		}

		public Campfire( Serial serial ) : base( serial )
		{
			m_Records = new ArrayList();
			m_Timer = new DecayTimer( this );
			m_Timer.Start();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnAfterDelete()
		{
			if ( m_Timer != null )
				m_Timer.Stop();
			if ( m_Records != null )
				m_Records.Clear();
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			m_Records.Clear();
			IPooledEnumerable eable = GetClientsInRange( 7 );
			foreach ( NetState ns in eable )
			{
				m_Records.Add( new CampRecord( ns.Mobile ) );
				ns.Mobile.SendAsciiMessage( "You feel it would take a few moments to secure your camp." );
			}
			eable.Free();
		}

		public bool CanLogout( Mobile m )
		{
			for(int i=0;i<m_Records.Count;i++)
			{
				CampRecord cr = (CampRecord)m_Records[i];

				if ( cr.Mob == m )
					return cr.LogoutOK;
			}

			return false;
		}

		public override bool HandlesOnMovement
		{
			get
			{
				return true;
			}
		}


		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement (m, oldLocation);

			if ( m.NetState != null )
			{
				CampRecord oldRec = null;

				for(int i=0;i<m_Records.Count;i++)
				{
					CampRecord cr = (CampRecord)m_Records[i];

					if ( cr.Mob == m )
					{
						oldRec = cr;
						break;
					}
				}

				bool inRange = m.InRange( this, 7 );
				if ( oldRec != null && !inRange )
				{
					m_Records.Remove( oldRec );
				}
				else if ( oldRec == null && inRange )
				{
					m_Records.Add( new CampRecord( m ) );
					m.SendAsciiMessage( "You feel it would take a few moments to secure your camp." );
				}
			}
		}

		private void OnTick()
		{
			for(int i=0;i<m_Records.Count;i++)
			{
				CampRecord cr = (CampRecord)m_Records[i];

				if ( cr == null || cr.Mob == null )
					continue;

				if ( Server.SkillHandlers.Hiding.CheckCombat( cr.Mob, 7 ) )
				{
					if ( cr.LogoutOK )
					{
						cr.LogoutOK = false;
						cr.Mob.SendAsciiMessage( "You feel your camp will need to be re-secured." );
					}
					cr.StartTime = DateTime.Now;
				}
				else if ( !cr.LogoutOK && cr.StartTime + TimeSpan.FromSeconds( 30 ) <= DateTime.Now )
				{
					cr.LogoutOK = true;
					cr.Mob.SendAsciiMessage( "The camp is now secure." );
				}
			}
		}

		private class CampRecord
		{
			private Mobile m_Mob;
			private DateTime m_Time;
			private bool m_OK;

			public CampRecord( Mobile m )
			{
				m_Mob = m;
				m_Time = DateTime.Now;
				m_OK = false;
			}

			public Mobile Mob { get { return m_Mob; } }
			public DateTime StartTime { get { return m_Time; } set { m_Time = value; } }
			public bool LogoutOK { get { return m_OK; } set { m_OK = value; } }
		}

		private class DecayTimer : Timer
		{
			private Campfire m_Owner;
			private DateTime m_StartTime;

			public DecayTimer( Campfire owner ) : base( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 1 ) )
			{
				Priority = TimerPriority.TwoFiftyMS;

				m_Owner = owner;
				m_StartTime = DateTime.Now;
			}

			protected override void OnTick()
			{
				if ( m_StartTime+TimeSpan.FromMinutes( 2.0 ) < DateTime.Now )
				{
					Stop();
					m_Owner.Delete();
				}
				else
				{
					m_Owner.OnTick();
				}
			}
		}
	}
}
