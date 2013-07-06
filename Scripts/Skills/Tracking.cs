using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Menus.ItemLists;
using Server.Network;

namespace Server.SkillHandlers
{
	public class Tracking
	{
		private static Hashtable m_Table = new Hashtable();

		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Tracking].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			if ( m.NetState != null )
				new TrackingMenu().SendTo( m.NetState );

			return TimeSpan.FromSeconds( 10.0 ); // 10 second delay before beign able to re-use a skill
		}

		private class TrackingMenu : ItemListMenu
		{
			private static ItemListEntry[] m_Categories = new ItemListEntry[]
			{
				new ItemListEntry( "Animals", 8482 ), 
				new ItemListEntry( "Creatures", 8408 ),
				new ItemListEntry( "People", 8454 ),
			};

			public TrackingMenu() : base( "What do you wish to track?", m_Categories )
			{
			}

			public override void OnResponse(NetState state, int index)
			{
				//base.OnResponse (state, index);
				Mobile from = state.Mobile;
				if ( from == null )
					return;

				int range = (int)(( from.Skills[SkillName.Tracking].Value / 2.0 ));
				if ( range < 5 )
					range = 5;
				else if ( range > 50 )
					range = 50;

				IPooledEnumerable eable = from.GetMobilesInRange( range );
				ArrayList list = new ArrayList();
				foreach ( Mobile m in eable )
				{
					if ( m != from && m.AccessLevel <= from.AccessLevel && m.Alive  )
					{
						if ( index == 2 && ( m.Body.IsHuman || m.Player ) )
							list.Add( m );
						else if ( index == 1 && m.Body.IsMonster )
							list.Add( m );
						else if ( index == 0 && m.Body.IsAnimal )
							list.Add( m );
					}
				}
				eable.Free();

				if ( !from.CheckSkill( SkillName.Tracking, 0, 100 ) || list.Count <= 0 )
				{
					from.SendAsciiMessage( "You see no evidence of anything in the area." );
				}
				else
				{
					new TrackingList( list ).SendTo( state );
				}
			}
		}

		private class TrackingList : ItemListMenu
		{
			private ArrayList m_List;
			public TrackingList( ArrayList list ) : base( "What would you like to track?", MakeItemList( list ) )
			{
				m_List = list;
			}

			public override void OnResponse(NetState state, int index)
			{
				if ( m_List == null || index < 0 || index >= m_List.Count )
					return;

				Mobile m = m_List[index] as Mobile;
				if ( m == null || m.Deleted )
					return;

				Timer timer = m_Table[m] as Timer;
				if ( timer != null && timer.Running )
					timer.Stop();
				m_Table[m] = timer = new TrackingTimer( m, state.Mobile );
				timer.Start();
			}

			private static ItemListEntry[] MakeItemList( ArrayList list )
			{
				ArrayList newList = new ArrayList( list.Count );
				for(int i=0;i<list.Count;i++)
				{
					Mobile m = (Mobile)list[i];
					newList.Add( new ItemListEntry( m.Name != null && m.Name.Length > 0 ? m.Name : "-no name-", ShrinkTable.Lookup( m ) ) );
				}
				return (ItemListEntry[])newList.ToArray( typeof( ItemListEntry ) );
			}
		}

		private class TrackingTimer : Timer
		{
			private Mobile m_ToTrack, m_From;
			private int m_Range;
			private int m_Count;
			private Direction m_LastDir;

			public TrackingTimer( Mobile track, Mobile from ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1 ), 150 )
			{
				m_ToTrack = track;
				m_From = from;

				m_Count = 0;

				m_Range = 5 + (int)((from.Skills[SkillName.Tracking].Value / 100.0) * 15.0);
				if ( m_Range < 5 )
					m_Range = 5;
				else if ( m_Range > 20 )
					m_Range = 20;
				m_Range *= 2;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_ToTrack.Deleted || !m_ToTrack.Alive || m_From.NetState == null || !m_From.InRange( m_ToTrack, m_Range ) )
				{
					m_From.SendAsciiMessage( "You have lost your quarry." );
					m_Table.Remove( m_From );
					Stop();
					return;
				}

				Direction dir = m_From.GetDirectionTo( m_ToTrack );
				if ( m_Count++%6 == 0 || dir != m_LastDir )
				{
					m_Count = 1;
					m_LastDir = dir;
					m_From.PrivateOverheadMessage( MessageType.Emote, 0x3B2, true, String.Format( "{0} is to the {1}.", m_ToTrack.Name == null || m_ToTrack.Name.Length <= 0 ? "It" : m_ToTrack.Name, GetDirectionString( dir ).ToLower() ), m_From.NetState );
				}
			}
		}

		public static string GetDirectionString( Direction dir )
		{
			switch ( dir&Direction.Mask )
			{
				case Direction.North:
					return "North";
				case Direction.Right:
					return "Northeast";
				case Direction.East:
					return "East";
				case Direction.Down:
					return "Southeast";
				case Direction.South:
					return "South";
				case Direction.Left:
					return "Southwest";
				case Direction.West:
					return "West";
				case Direction.Up:
					return "Northwest";
				default:
					return "Over There";
			}
		}
	}
}

