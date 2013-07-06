using System;
using System.IO;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using System.Xml;

namespace Server.Mobiles
{
	public class Spawner : BaseItem
	{
		public static void Initialize()
		{
			Commands.CommandSystem.Register( "RespawnAll", AccessLevel.Administrator, new Server.Commands.CommandEventHandler( RespawnAll_OnCommand ) );
		}

		public static void RespawnAll_OnCommand( Server.Commands.CommandEventArgs args )
		{
			args.Mobile.SendAsciiMessage( "Respawning all spawners... this may be a little slow." );
			ArrayList spawners = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is Spawner && ((Spawner)item).Running )
					spawners.Add( item );
			}

			int count = 0;
			for (int i=0;i<spawners.Count;i++)
			{
				Spawner s = (Spawner)spawners[i];
				s.Respawn();
				count += s.Count;
			}
			args.Mobile.SendAsciiMessage( "Respawn compelete, spawned {0} objects from {1} spawners.", count, spawners.Count );
		}

		private int m_Team;
		private int m_HomeRange;
		private int m_Count;
		private TimeSpan m_MinDelay;
		private TimeSpan m_MaxDelay;
		private ArrayList m_CreaturesName;
		private ArrayList m_Creatures;
		private DateTime m_End;
		private InternalTimer m_Timer;
		private bool m_Running;
		private bool m_Group;
		private WayPoint m_WayPoint;

		// for xmlspawner compat
		private bool m_RelHomeRange;
		private int m_SpawnRange;

		public bool IsFull{ get{ return ( m_Creatures != null && m_Creatures.Count >= m_Count ); } }

		public bool SpawnedObject( object o )
		{
			return m_Creatures.Contains( o );
		}
		
		public ArrayList CreaturesName
		{
			get { return m_CreaturesName; }
			set
			{
				m_CreaturesName = value;
				if ( m_CreaturesName.Count < 1 )
					Stop();

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RelativeHomeRange
		{
			get { return m_RelHomeRange; }
			set { m_RelHomeRange = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpawnRange
		{
			get { return m_SpawnRange; }
			set { m_SpawnRange = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Count
		{
			get { return m_Count; }
			set { m_Count = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WayPoint WayPoint
		{
			get
			{
				return m_WayPoint;
			}
			set
			{
				m_WayPoint = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Running
		{
			get { return m_Running; }
			set
			{
				if ( value )
					Start();
				else
					Stop();

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HomeRange
		{
			get { return m_HomeRange; }
			set { m_HomeRange = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Team
		{
			get { return m_Team; }
			set { m_Team = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan MinDelay
		{
			get { return m_MinDelay; }
			set { m_MinDelay = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan MaxDelay
		{
			get { return m_MaxDelay; }
			set { m_MaxDelay = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan NextSpawn
		{
			get
			{
				if ( m_Running )
					return m_End - DateTime.Now;
				else
					return TimeSpan.FromSeconds( 0 );
			}
			set
			{
				Start();
				DoTimer( value );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Group
		{
			get { return m_Group; }
			set { m_Group = value; InvalidateProperties(); }
		}

		[Constructable]
		public Spawner( int amount, int minDelay, int maxDelay, int team, int homeRange, string creatureName ) : base( 0x1f13 )
		{
			ArrayList creaturesName = new ArrayList();
			creaturesName.Add( creatureName.ToLower() );
			InitSpawn( amount, TimeSpan.FromMinutes( minDelay ), TimeSpan.FromMinutes( maxDelay ), team, homeRange, creaturesName );
		}

		[Constructable]
		public Spawner( string creatureName ) : base( 0x1f13 )
		{
			ArrayList creaturesName = new ArrayList();
			creaturesName.Add( creatureName.ToLower() );
			InitSpawn( 5, TimeSpan.FromMinutes( 15 ), TimeSpan.FromMinutes( 60 ), 0, 5, creaturesName );
		}

		[Constructable]
		public Spawner() : base( 0x1f13 )
		{
			ArrayList creaturesName = new ArrayList();
			InitSpawn( 5, TimeSpan.FromMinutes( 15 ), TimeSpan.FromMinutes( 60 ), 0, 5, creaturesName );
		}

		public Spawner( int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, ArrayList creaturesName ) : base( 0x1f13 )
		{
			InitSpawn( amount, minDelay, maxDelay, team, homeRange, creaturesName );
		}

		public void InitSpawn( int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, ArrayList creaturesName )
		{
			Visible = false;
			Movable = false;
			m_Running = true;
			m_Group = false;
			Name = "Spawner";
			m_MinDelay = minDelay;
			m_MaxDelay = maxDelay;
			m_Count = amount;
			m_Team = team;
			m_HomeRange = homeRange;
			m_CreaturesName = creaturesName;
			m_Creatures = new ArrayList();
			DoTimer( TimeSpan.FromSeconds( 1 ) );
		}
			
		public Spawner( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel < AccessLevel.GameMaster )
				return;

			SpawnerGump g = new SpawnerGump( this );
			from.SendGump( g );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Running )
			{
				list.Add( 1060742 ); // active

				list.Add( 1060656, m_Count.ToString() ); // amount to make: ~1_val~
				list.Add( 1061169, m_HomeRange.ToString() ); // range ~1_val~

				list.Add( 1060658, "group\t{0}", m_Group ); // ~1_val~: ~2_val~
				list.Add( 1060659, "team\t{0}", m_Team ); // ~1_val~: ~2_val~
				list.Add( 1060660, "speed\t{0} to {1}", m_MinDelay, m_MaxDelay ); // ~1_val~: ~2_val~

				for ( int i = 0; i < 3 && i < m_CreaturesName.Count; ++i )
					list.Add( 1060661 + i, "{0}\t{1}", m_CreaturesName[i], CountCreatures( (string)m_CreaturesName[i] ) );
			}
			else
			{
				list.Add( 1060743 ); // inactive
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( from.AccessLevel >= AccessLevel.GameMaster )
			{
				if ( m_Running )
					LabelTo( from, true, "[Running]" );
				else
					LabelTo( from, true, "[Off]" );
			}
		}

		public void Start()
		{
			if ( !m_Running )
			{
				if ( m_CreaturesName.Count > 0 )
				{
					m_Running = true;
					DoTimer();
				}
			}
		}

		public void Stop()
		{
			if ( m_Running )
			{
				m_Timer.Stop();
				m_Running = false;
			}
		}

		public void Defrag()
		{
			bool removed = false;

			for ( int i = 0; i < m_Creatures.Count; ++i )
			{
				object o = m_Creatures[i];

				if ( o is Item )
				{
					Item item = (Item)o;

					if ( item.Deleted || ( item.Parent != null && item.Parent != this.Parent ) || ( this.Parent == null && !Utility.InRange( this.GetWorldLocation(), item.GetWorldLocation(), this.HomeRange ) ) )
					{
						m_Creatures.RemoveAt( i );
						--i;
						removed = true;
					}
				}
				else if ( o is Mobile )
				{
					Mobile m = (Mobile)o;

					if ( m.Deleted )
					{
						m_Creatures.RemoveAt( i );
						--i;
						removed = true;
					}
					else if ( m is BaseCreature )
					{
						BaseCreature bc = (BaseCreature)m;
						if ( bc.Controled || bc.IsStabled || bc.Owners.Count > 0 )
						{
							m_Creatures.RemoveAt( i );
							--i;
							removed = true;
							bc.Home = Point3D.Zero;
						}
						else if ( bc.Home != Point3D.Zero && !bc.InRange( bc.Home, bc.RangeHome * 6 ) && !(bc is BaseVendor) && !bc.Body.IsHuman )
						{
							bc.Home = Point3D.Zero; // dont remove, but make them forget where they are from
						}
					}
				}
				else
				{
					m_Creatures.RemoveAt( i );
					--i;
					removed = true;
				}
			}

			if ( removed )
				InvalidateProperties();
		}

		public void OnTick()
		{
			DoTimer();

			if ( m_Group )
			{
				Defrag();
				if  ( m_Creatures.Count == 0 )
					Respawn();
			}
			else
			{
				Spawn();
			}
		}
		
		public void Respawn()
		{
			RemoveCreatures();

			for ( int i = 0; i < m_Count; i++ )
				Spawn();
		}
		
		public void Spawn()
		{
			if ( m_CreaturesName.Count > 0 )
			{
				Defrag();

				int count = 1;
				if ( m_Count > 2 && m_Creatures.Count == 0 )
				{
					if ( Utility.Random( 5 ) != 0 )
						count = (m_Count+1)/2;
					else
						count = m_Count;
				}

				for(int i=0;i<count;i++)
					Spawn( Utility.Random( m_CreaturesName.Count ) );
			}
		}
		
		public void Spawn( string creatureName )
		{
			for ( int i = 0; i < m_CreaturesName.Count; i++ )
			{
				if ( (string)m_CreaturesName[i] == creatureName )
				{
					Defrag();
					Spawn( i );
					break;
				}
			}
		}

		public virtual void Spawn( int index )
		{
			Map map = Map;

			if ( map == null || map == Map.Internal || m_CreaturesName.Count == 0 || index >= m_CreaturesName.Count )
				return;

			if ( m_Creatures.Count >= m_Count )
				return;

			Type type = SpawnerType.GetType( (string)m_CreaturesName[index] );

			if ( type != null )
			{
				try
				{
					object o = Activator.CreateInstance( type );

					if ( o is Mobile )
					{
						Mobile m = (Mobile)o;

						m_Creatures.Add( m );
						InvalidateProperties();

						m.Map = map;

						if ( m is BaseVendor )
							m.Location = this.Location;
						else
							m.Location = GetSpawnPosition();

						if ( m is BaseCreature )
						{
							BaseCreature c = (BaseCreature)m;

							c.RangeHome = m_HomeRange;
							c.CurrentWayPoint = m_WayPoint;

							if ( m_Team > 0 )
								c.Team = m_Team;
							if ( m_RelHomeRange )
								c.Home = c.Location;
							else
								c.Home = this.Location;
						}
					}
					else if ( o is Item )
					{
						Item item = (Item)o;

						m_Creatures.Add( item );
						InvalidateProperties();

						object parent = Parent;
						if ( parent != null && parent is Container )
						{
							((Container)parent).DropItem( item );
							if ( parent is TrapableContainer )
							{
								TrapableContainer tc = (TrapableContainer)parent;
								if ( tc.TrapType != TrapType.None && !tc.Trapped )
									tc.Trapped = true;
							}

							if ( parent is LockableContainer )
							{
								LockableContainer lc = (LockableContainer)parent;

								if ( lc.LockLevel > 0 )
									lc.Locked = true;
							}
						}
						else
						{
							item.MoveToWorld( GetSpawnPosition(), map );
							item.LastMoved = DateTime.Now + TimeSpan.FromDays( 30 );
						}
					}
				}
				catch
				{
				}
			}
		}

		public virtual Point3D GetSpawnPosition()
		{
			Map map = Map;

			if ( map == null )
				return Location;

			if ( Parent != null )
				return GetWorldLocation();

			for ( int i = 0; i < 15; i++ )
			{
				int range = m_SpawnRange <= 0 ? m_HomeRange : m_SpawnRange;
				Point3D point = new Point3D( Location.X + (Utility.Random( (range * 2) + 1 ) - range), Location.Y + (Utility.Random( (range * 2) + 1 ) - range), this.Z );

				Region r = Region.Find( point, map );
				if ( r != null && !r.AllowSpawn() )
					continue;

				if ( Map.CanSpawnMobile( point ) )
					return point;
					
				int z;
				point.Z = Map.GetAverageZ( point.X, point.Y );
				if ( Map.CanSpawnMobile( point ) )
					return point;
				
				for(z=1;z<=7;z++)
				{
					point.Z = this.Z + z;
					if ( Map.CanSpawnMobile( point ) )
						return point;
						
					point.Z = this.Z - z;
					if ( Map.CanSpawnMobile( point ) )
						return point;
				}
			}

			return this.Location;
		}

		public void DoTimer()
		{
			if ( !m_Running )
				return;

			int minSeconds = (int)m_MinDelay.TotalSeconds;
			int maxSeconds = (int)m_MaxDelay.TotalSeconds;

			TimeSpan delay = TimeSpan.FromSeconds( Utility.RandomMinMax( minSeconds, maxSeconds ) );
			DoTimer( delay );
		}

		public void DoTimer( TimeSpan delay )
		{
			if ( !m_Running )
				return;

			m_End = DateTime.Now + delay;

			if ( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer.Delay = delay;
				m_Timer.Spawner = this;

				if ( IsFull )
					m_Timer.Priority = TimerPriority.FiveSeconds;
				else
					m_Timer.Priority = TimerPriority.OneSecond;
			}
			else
			{
				m_Timer = new InternalTimer( this, delay );
			}
			m_Timer.Start();
		}

		private class InternalTimer : Timer
		{
			private Spawner m_Spawner;

			public Spawner Spawner { get{ return m_Spawner; } set { m_Spawner = value; } }

			public InternalTimer( Spawner spawner, TimeSpan delay ) : base( delay )
			{
				if ( spawner.IsFull )
					Priority = TimerPriority.FiveSeconds;
				else
					Priority = TimerPriority.OneSecond;

				m_Spawner = spawner;
			}

			protected override void OnTick()
			{
				if ( m_Spawner != null && !m_Spawner.Deleted )
					m_Spawner.OnTick();
			}
		}

		public int CountCreatures( string creatureName )
		{
			Defrag();

			int count = 0;

			for ( int i = 0; i < m_Creatures.Count; ++i )
				if ( Insensitive.Equals( creatureName, m_Creatures[i].GetType().Name ) )
					++count;

			return count;
		}

		public void RemoveCreatures( string creatureName )
		{
			Defrag();

			creatureName = creatureName.ToLower();

			for ( int i = 0; i < m_Creatures.Count; ++i )
			{
				object o = m_Creatures[i];

				if ( Insensitive.Equals( creatureName, o.GetType().Name ) )
				{
					if ( o is Item )
						((Item)o).Delete();
					else if ( o is Mobile )
						((Mobile)o).Delete();
				}
			}
		}
		
		public void RemoveCreatures()
		{
			Defrag();

			for ( int i = 0; i < m_Creatures.Count; ++i )
			{
				object o = m_Creatures[i];

				if ( o is Item )
					((Item)o).Delete();
				else if ( o is Mobile )
					((Mobile)o).Delete();
			}
		}
		
		public void BringToHome()
		{
			Defrag();

			for ( int i = 0; i < m_Creatures.Count; ++i )
			{
				object o = m_Creatures[i];

				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;

					m.Map = this.Map;
					m.Location = this.Location;
				}
				else if ( o is Item )
				{
					((Item)o).MoveToWorld( Location, Map );
				}
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			RemoveCreatures();
			if ( m_Timer != null )
				m_Timer.Stop();
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			writer.Write( m_SpawnRange );
			writer.Write( m_RelHomeRange );

			writer.Write( m_WayPoint );

			writer.Write( m_Group );

			writer.Write( m_MinDelay );
			writer.Write( m_MaxDelay );
			writer.Write( m_Count );
			writer.Write( m_Team );
			writer.Write( m_HomeRange );
			writer.Write( m_Running );
			
			if ( m_Running )
				writer.WriteDeltaTime( m_End );

			writer.Write( m_CreaturesName.Count );

			for ( int i = 0; i < m_CreaturesName.Count; ++i )
				writer.Write( (string)m_CreaturesName[i] );

			writer.Write( m_Creatures.Count );

			for ( int i = 0; i < m_Creatures.Count; ++i )
			{
				object o = m_Creatures[i];

				if ( o is Item )
					writer.Write( (Item)o );
				else if ( o is Mobile )
					writer.Write( (Mobile)o );
				else
					writer.Write( Serial.MinusOne );
			}
		}

		private static WarnTimer m_WarnTimer;

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				{
					m_SpawnRange = reader.ReadInt();
					m_RelHomeRange = reader.ReadBool();

					goto case 2;
				}
				case 2:
				{
					m_WayPoint = reader.ReadItem() as WayPoint;

					goto case 1;
				}

				case 1:
				{
					m_Group = reader.ReadBool();
					
					goto case 0;
				}

				case 0:
				{
					m_MinDelay = reader.ReadTimeSpan();
					m_MaxDelay = reader.ReadTimeSpan();
					m_Count = reader.ReadInt();
					m_Team = reader.ReadInt();
					m_HomeRange = reader.ReadInt();
					m_Running = reader.ReadBool();

					TimeSpan ts = TimeSpan.Zero;

					if ( m_Running )
						ts = reader.ReadDeltaTime() - DateTime.Now;
					
					int size = reader.ReadInt();

					m_CreaturesName = new ArrayList( size );

					for ( int i = 0; i < size; ++i )
					{
						string typeName = reader.ReadString();
						/*
						if ( typeName == "JackRabit" )
							typeName = "JackRabbit";
						else if ( typeName == "Rabit" )
							typeName = "Rabbit";
						else if ( typeName.ToLower() == "barkeeper" )
							typeName = "Tavernkeeper";
						*/

						m_CreaturesName.Add( typeName );

						if ( SpawnerType.GetType( typeName ) == null )
						{
							if ( m_WarnTimer == null )
								m_WarnTimer = new WarnTimer();

							m_WarnTimer.Add( Location, Map, typeName );
						}
					}

					int count = reader.ReadInt();

					m_Creatures = new ArrayList( count );

					for ( int i = 0; i < count; ++i )
					{
						IEntity e = World.FindEntity( reader.ReadInt() );

						if ( e != null )
							m_Creatures.Add( e );
					}

					if ( m_Running )
						DoTimer( ts );

					break;
				}
			}
		}

		private class WarnTimer : Timer
		{
			private ArrayList m_List;

			private class WarnEntry
			{
				public Point3D m_Point;
				public Map m_Map;
				public string m_Name;

				public WarnEntry( Point3D p, Map map, string name )
				{
					m_Point = p;
					m_Map = map;
					m_Name = name;
				}
			}

			public WarnTimer() : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_List = new ArrayList();
				Start();
			}

			public void Add( Point3D p, Map map, string name )
			{
				m_List.Add( new WarnEntry( p, map, name ) );
			}

			protected override void OnTick()
			{
				try
				{
					Console.WriteLine( "Warning: {0} bad spawns detected, logged: 'badspawn.log'", m_List.Count );

					using ( StreamWriter op = new StreamWriter( "badspawn.log" ) )
					{
						op.WriteLine( "# Bad spawns : {0}", DateTime.Now );
						op.WriteLine( "# Format: X Y Z F Name" );
						op.WriteLine();

						foreach ( WarnEntry e in m_List )
							op.WriteLine( "{0}\t{1}\t{2}\t{3}\t{4}", e.m_Point.X, e.m_Point.Y, e.m_Point.Z, e.m_Map, e.m_Name );

						op.WriteLine();
						op.WriteLine();
					}
				}
				catch
				{
				}
			}
		}
	}
}
