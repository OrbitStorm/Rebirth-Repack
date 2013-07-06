using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class Teleporter : BaseItem
	{
		private bool m_Active, m_Creatures;
		private Point3D m_PointDest;
		private Map m_MapDest;
		private bool m_SourceEffect;
		private bool m_DestEffect;
		private int m_SoundID;
		private TimeSpan m_Delay;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool SourceEffect
		{
			get{ return m_SourceEffect; }
			set{ m_SourceEffect = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool DestEffect
		{
			get{ return m_DestEffect; }
			set{ m_DestEffect = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SoundID
		{
			get{ return m_SoundID; }
			set{ m_SoundID = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Delay
		{
			get{ return m_Delay; }
			set{ m_Delay = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D PointDest
		{
			get { return m_PointDest; }
			set { m_PointDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest
		{
			get { return m_MapDest; }
			set { m_MapDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Creatures
		{
			get { return m_Creatures; }
			set { m_Creatures = value; }
		}

		public override int LabelNumber{ get{ return 1026095; } } // teleporter

		[Constructable]
		public Teleporter() : this( new Point3D( 0, 0, 0 ), null, false )
		{
		}

		[Constructable]
		public Teleporter( Point3D pointDest, Map mapDest ) : this( pointDest, mapDest, false )
		{
		}

		[Constructable]
		public Teleporter( Point3D pointDest, Map mapDest, bool creatures ) : base( 0x1BC3 )
		{
			Movable = false;
			Visible = false;

			m_Active = true;
			m_PointDest = pointDest;
			m_MapDest = mapDest;
			m_Creatures = creatures;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Active )
				list.Add( 1060742 ); // active
			else
				list.Add( 1060743 ); // inactive

			if ( m_MapDest != null )
				list.Add( 1060658, "Map\t{0}", m_MapDest );

			if ( m_PointDest != Point3D.Zero )
				list.Add( 1060659, "Coords\t{0}", m_PointDest );

			list.Add( 1060660, "Creatures\t{0}", m_Creatures ? "Yes" : "No" );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( m_Active )
			{
				if ( m_MapDest != null && m_PointDest != Point3D.Zero )
					LabelTo( from, "{0} [{1}]", m_PointDest, m_MapDest );
				else if ( m_MapDest != null )
					LabelTo( from, "[{0}]", m_MapDest );
				else if ( m_PointDest != Point3D.Zero )
					LabelTo( from, m_PointDest.ToString() );
			}
			else
			{
				LabelTo( from, "(inactive)" );
			}
		}

		public virtual void StartTeleport( Mobile m )
		{
			if ( m_Delay == TimeSpan.Zero )
				DoTeleport( m );
			else
				Timer.DelayCall( m_Delay, new TimerStateCallback( DoTeleport_Callback ), m );
		}

		private void DoTeleport_Callback( object state )
		{
			DoTeleport( (Mobile) state );
		}

		public virtual void DoTeleport( Mobile m )
		{
			Map map = m_MapDest;

			if ( map == null || map == Map.Internal )
				map = m.Map;

			Point3D p = m_PointDest;

			if ( p == Point3D.Zero )
				p = m.Location;

			Server.Mobiles.BaseCreature.TeleportPets( m, p, map );

			if ( m_SourceEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			m.Map = map;
			m.Location = p;

			if ( m_DestEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			if ( m_SoundID > 0 )
				Effects.PlaySound( m.Location, m.Map, m_SoundID );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m_Active )
			{
				if ( !m_Creatures && !m.Player )
					return true;

				StartTeleport( m );
				return false;
			}

			return true;
		}

		public Teleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( (bool) m_SourceEffect );
			writer.Write( (bool) m_DestEffect );
			writer.Write( (TimeSpan) m_Delay );
			writer.WriteEncodedInt( (int) m_SoundID );

			writer.Write( m_Creatures );

			writer.Write( m_Active );
			writer.Write( m_PointDest );
			writer.Write( m_MapDest );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_SourceEffect = reader.ReadBool();
					m_DestEffect = reader.ReadBool();
					m_Delay = reader.ReadTimeSpan();
					m_SoundID = reader.ReadEncodedInt();

					goto case 1;
				}
				case 1:
				{
					m_Creatures = reader.ReadBool();

					goto case 0;
				}
				case 0:
				{
					m_Active = reader.ReadBool();
					m_PointDest = reader.ReadPoint3D();
					m_MapDest = reader.ReadMap();

					break;
				}
			}
		}
	}

	public class SkillTeleporter : Teleporter
	{
		private SkillName m_Skill;
		private double m_Required;
		private string m_MessageString;
		private int m_MessageNumber;

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get{ return m_Skill; }
			set{ m_Skill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double Required
		{
			get{ return m_Required; }
			set{ m_Required = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string MessageString
		{
			get{ return m_MessageString; }
			set{ m_MessageString = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MessageNumber
		{
			get{ return m_MessageNumber; }
			set{ m_MessageNumber = value; InvalidateProperties(); }
		}

		private void EndMessageLock( object state )
		{
			((Mobile)state).EndAction( this );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( Active )
			{
				if ( !Creatures && !m.Player )
					return true;

				Skill sk = m.Skills[m_Skill];

				if ( sk == null || sk.Base < m_Required )
				{
					if ( m.BeginAction( this ) )
					{
						if ( m_MessageString != null )
							m.Send( new AsciiMessage( Serial, ItemID, MessageType.Regular, 0x3B2, 3, "", m_MessageString ) );
						else if ( m_MessageNumber != 0 )
							m.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, m_MessageNumber, null, "" ) );

						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
					}

					return false;
				}

				StartTeleport( m );
				return false;
			}

			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			int skillIndex = (int)m_Skill;
			string skillName;

			if ( skillIndex >= 0 && skillIndex < SkillInfo.Table.Length )
				skillName = SkillInfo.Table[skillIndex].Name;
			else
				skillName = "(Invalid)";

			list.Add( 1060661, "{0}\t{1:F1}", skillName, m_Required );

			if ( m_MessageString != null )
				list.Add( 1060662, "Message\t{0}", m_MessageString );
			else if ( m_MessageNumber != 0 )
				list.Add( 1060662, "Message\t#{0}", m_MessageNumber );
		}

		[Constructable]
		public SkillTeleporter()
		{
		}

		public SkillTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Skill );
			writer.Write( (double) m_Required );
			writer.Write( (string) m_MessageString );
			writer.Write( (int) m_MessageNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Skill = (SkillName)reader.ReadInt();
					m_Required = reader.ReadDouble();
					m_MessageString = reader.ReadString();
					m_MessageNumber = reader.ReadInt();

					break;
				}
			}
		}
	}

	public class KeywordTeleporter : Teleporter
	{
		private string m_Substring;
		private int m_Keyword;
		private int m_Range;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Substring
		{
			get{ return m_Substring; }
			set{ m_Substring = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Keyword
		{
			get{ return m_Keyword; }
			set{ m_Keyword = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range
		{
			get{ return m_Range; }
			set{ m_Range = value; InvalidateProperties(); }
		}

		public override bool HandlesOnSpeech{ get{ return true; } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && Active )
			{
				Mobile m = e.Mobile;

				if ( !Creatures && !m.Player )
					return;

				if ( !m.InRange( GetWorldLocation(), m_Range ) )
					return;

				bool isMatch = false;

				if ( m_Keyword >= 0 && e.HasKeyword( m_Keyword ) )
					isMatch = true;
				else if ( m_Substring != null && e.Speech.ToLower().IndexOf( m_Substring.ToLower() ) >= 0 )
					isMatch = true;

				if ( !isMatch )
					return;

				e.Handled = true;
				StartTeleport( m );
			}
		}

		public override bool OnMoveOver( Mobile m )
		{
			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060661, "Range\t{0}", m_Range );

			if ( m_Keyword >= 0 )
				list.Add( 1060662, "Keyword\t{0}", m_Keyword );

			if ( m_Substring != null )
				list.Add( 1060663, "Substring\t{0}", m_Substring );
		}

		[Constructable]
		public KeywordTeleporter()
		{
			m_Keyword = -1;
			m_Substring = null;
		}

		public KeywordTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Substring );
			writer.Write( m_Keyword );
			writer.Write( m_Range );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Substring = reader.ReadString();
					m_Keyword = reader.ReadInt();
					m_Range = reader.ReadInt();

					break;
				}
			}
		}
	}

	public class ClickTeleporter : BaseItem
	{
		private Point3D m_TargLoc;
		private Map m_TargMap;
		private string m_Msg;

		private int m_ID1, m_ID2;

		[CommandProperty( AccessLevel.GameMaster )]
		public Map TargetMap
		{
			get { return m_TargMap; }
			set { m_TargMap = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D TargetLocation
		{
			get { return m_TargLoc; }
			set { m_TargLoc = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ID1
		{
			get { return m_ID1; }
			set { m_ID1 = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ID2
		{
			get { return m_ID2; }
			set { m_ID2 = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Message
		{
			get { return m_Msg; }
			set { m_Msg = value; }
		}

		[Constructable]
		public ClickTeleporter( int itemID ) : base( itemID )
		{
			m_ID1 = m_ID2 = itemID;
			Movable = false;
		}

		[Constructable]
		public ClickTeleporter( int id1, int id2 ) : base( id1 )
		{
			m_ID1 = id1; 
			m_ID2 = id2;
			Movable = false;
		}

		[Constructable]
		public ClickTeleporter( int id1, int id2, string message ) : base( id1 )
		{
			m_ID1 = id1; 
			m_ID2 = id2;
			m_Msg = message;
			Movable = false;
		}

		public ClickTeleporter( Serial s ) : base( s )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( (int)0 );
			writer.Write( m_TargMap );
			writer.Write( m_TargLoc );
			writer.Write( m_ID1 );
			writer.Write( m_ID2 );
			writer.Write( m_Msg );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int ver = reader.ReadInt();

			m_TargMap = reader.ReadMap();
			m_TargLoc = reader.ReadPoint3D();
			m_ID1 = reader.ReadInt();
			m_ID2 = reader.ReadInt();
			m_Msg = reader.ReadString();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( m_TargLoc == Point3D.Zero || m_TargMap == null )
				return;

			if ( from.InRange( this, 2 ) && from.InLOS( this ) )
			{
				if ( ItemID == m_ID1 )
					ItemID = m_ID2;
				else
					ItemID = m_ID1;

				if ( m_Msg != null && m_Msg != "" )
					from.SendAsciiMessage( m_Msg );

				Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				
				from.MoveToWorld( m_TargLoc, m_TargMap );
				from.ProcessDelta();

				Effects.SendLocationParticles( EffectItem.Create(   m_TargLoc, m_TargMap, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

				from.PlaySound( 0x1FE );
			}
		}
	}
}
