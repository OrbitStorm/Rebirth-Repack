using System;
using Server;
using Server.Mobiles;
using System.Collections; using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
	public class QuestChest1 : WoodenChest
	{
		[Constructable]
		public QuestChest1() 
		{
			Movable = false;
		}

		public QuestChest1( Serial s ) : base( s )
		{
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);
			int ver = reader.ReadInt();

			switch ( ver )
			{
				case 0:
					m_Trap1 = (TrapType)reader.ReadInt();
					m_Trap2 = (TrapType)reader.ReadInt();
					m_T1Pwr = reader.ReadInt();
					m_T2Pwr = reader.ReadInt();

					m_ExpRange = reader.ReadInt();
					m_ExpPower = reader.ReadInt();
					break;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( (int)0 ); // ver

			writer.Write( (int)m_Trap1 );
			writer.Write( (int)m_Trap2 );
			writer.Write( (int)m_T1Pwr );
			writer.Write( (int)m_T2Pwr );
			writer.Write( (int)m_ExpRange );
			writer.Write( (int)m_ExpPower );
		}

		private TrapType m_Trap1, m_Trap2;
		private int m_T1Pwr, m_T2Pwr;
		private int m_ExpRange, m_ExpPower;

		private int m_Stage;

		[CommandProperty( AccessLevel.GameMaster )]
		public TrapType Trap1
		{
			get { return m_Trap1; }
			set { m_Trap1 = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TrapType Trap2
		{
			get { return m_Trap2; }
			set { m_Trap2 = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Trap1Power
		{
			get { return m_T1Pwr; }
			set { m_T1Pwr = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Trap2Power
		{
			get { return m_T2Pwr; }
			set { m_T2Pwr = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ExplodeRange
		{
			get { return m_ExpRange; }
			set { m_ExpRange = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ExplodePower
		{
			get { return m_ExpPower; }
			set { m_ExpPower = value; }
		}

		private DateTime m_LastUsed = DateTime.MinValue;
		public override void OnDoubleClick(Mobile from)
		{
			if ( !this.Locked && from.AccessLevel == AccessLevel.Player )
			{
				if ( m_LastUsed+TimeSpan.FromSeconds( 15.0 ) >= DateTime.Now )
				{
					from.SendAsciiMessage( "You must wait a few moments before attempting to open the chest again." );
					return;
				}
				else
				{
					m_LastUsed = DateTime.Now;
				}

				switch ( m_Stage++ )
				{
					case 0:
						TrapType = m_Trap1;
						TrapPower = m_T1Pwr;
						Trapped = true;
						Trapper = null;
						break;
					case 1:
						TrapType = m_Trap2;
						TrapPower = m_T2Pwr;
						Trapped = true;
						Trapper = null;
						break;
					case 2:
						FinalExplode();
						break;
				}
			}

			if ( !Deleted )
				base.OnDoubleClick (from);
		}

		private void FinalExplode()
		{
			ArrayList list = new ArrayList();
			IPooledEnumerable eable = GetObjectsInRange( m_ExpRange );
			foreach ( object o in eable )
			{
				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;
					if ( ( m.Player && m.AccessLevel == AccessLevel.Player ) || ( m is BaseCreature && ((BaseCreature)m).Controled ) )
						list.Add( o );
				}
				else if ( o is BaseHouse )
				{
					list.Add( o );
				}
			}
			eable.Free();

			foreach ( object o in list )
			{
				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;
					if ( ( m.Player && m.AccessLevel == AccessLevel.Player ) || ( m is BaseCreature && ((BaseCreature)m).Controled ) )
					{

						AOS.Damage( m, 20*( m_ExpPower - 1 ) + Utility.Random( 25 ), 0, 100, 0, 0, 0 );
						m.SendLocalizedMessage( 503000 ); // Your skin blisters from the heat!
					}
				}
				else if ( o is BaseHouse )
				{
					BaseHouse h = (BaseHouse)o;

					h.OnDecayed();
					h.Delete();
				}
			}

			ArrayList items = new ArrayList( this.Items );
			foreach ( Item item in items )
			{
				item.MoveToWorld( this.Location, this.Map );
				if ( item is Spawner )
				{
					Spawner s = (Spawner)item;
					s.Running = true;
					s.Respawn();
					s.Running = false;
				}
			}

			Delete();
		}
	}
}
