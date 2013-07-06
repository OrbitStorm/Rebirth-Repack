using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class BaseEscortable : BaseConvo
	{
		private EDI m_Destination;
		private string m_DestinationString;

		private DateTime m_DeleteTime;
		private Timer m_DeleteTimer;

		public override bool Commandable{ get{ return false; } } // Our master cannot boss us around!

		public override bool CanBeRenamedBy( Mobile from )
		{
			return false;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Destination
		{
			get{ return m_Destination == null ? null : m_Destination.Name; }
			set{ m_DestinationString = value; m_Destination = EDI.Find( value ); }
		}

		private static string[] m_TownNames = new string[]
			{
				"Cove", "Britain", "Jhelom",
				"Minoc", "Ocllo", "Trinsic",
				"Vesper", "Yew", "Skara Brae",
				"Nujel'm", "Moonglow", "Magincia"
			};

		[Constructable]
		public BaseEscortable() : base( AIType.AI_Melee, FightMode.Agressor, 22, 1, 0.45, 1.0 )
		{
			InitBody();
			InitOutfit();
		}

		public virtual void InitBody()
		{
			SetStr( 90, 100 );
			SetDex( 90, 100 );
			SetInt( 15, 25 );

			Hue = Utility.RandomSkinHue();

			if ( Body == 0 && ( Name == null || Name.Length <= 0 ) )
			{
				if ( Female = Utility.RandomBool() )
				{
					Body = 401;
					Name = NameList.RandomName( "female" );
				}
				else
				{
					Body = 400;
					Name = NameList.RandomName( "male" );
				}
			}
		}

		public virtual void InitOutfit()
		{
			AddItem( new FancyShirt( Utility.RandomNeutralHue() ) );
			AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			AddItem( new Boots( Utility.RandomNeutralHue() ) );

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new ShortHair( Utility.RandomHairHue() ) ); break;
				case 1: AddItem( new TwoPigTails( Utility.RandomHairHue() ) ); break;
				case 2: AddItem( new ReceedingHair( Utility.RandomHairHue() ) ); break;
				case 3: AddItem( new KrisnaHair( Utility.RandomHairHue() ) ); break;
			}
		}

		public virtual bool SayDestinationTo( Mobile m )
		{
			EDI dest = GetDestination();

			if ( dest == null || !m.Alive || m_DestinationString == null )
				return false;

			Mobile escorter = GetEscorter();

			if ( escorter == null )
			{
				Say( "I am looking to go to {0}, will you take me?", m_DestinationString );
				return true;
			}
			else if ( escorter == m )
			{
				Say( "Lead on! Payment will be made when we arrive in {0}.", m_DestinationString );
				return true;
			}

			return false;
		}

		public virtual bool AcceptEscorter( Mobile m )
		{
			EDI dest = GetDestination();

			if ( dest == null )
				return false;

			Mobile escorter = GetEscorter();

			if ( escorter != null || !m.Alive || m_DestinationString == null )
				return false;

			if ( SetControlMaster( m ) )
			{
				m_LastSeenEscorter = DateTime.Now;

				if ( m is PlayerMobile )
					((PlayerMobile)m).LastEscortTime = DateTime.Now;

				Say( "Lead on! Payment will be made when we arrive in {0}.", m_DestinationString );
				StartFollow();
				return true;
			}

			return false;
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 3 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			base.OnSpeech( e );

			EDI dest = GetDestination();

			if ( dest != null && !e.Handled && e.Mobile.InRange( this.Location, 3 ) )
			{
				if ( e.HasKeyword( 0x1D ) ) // *destination*
					e.Handled = SayDestinationTo( e.Mobile );
				else if ( e.HasKeyword( 0x1E ) ) // *i will take thee*
					e.Handled = AcceptEscorter( e.Mobile );
			}
		}

		public override void OnAfterDelete()
		{
			if ( m_DeleteTimer != null )
				m_DeleteTimer.Stop();

			m_DeleteTimer = null;

			base.OnAfterDelete();
		}

		public override void OnThink()
		{
			base.OnThink();
			CheckAtDestination();
		}

		protected override bool OnMove( Direction d )
		{
			if ( !base.OnMove( d ) )
				return false;

			CheckAtDestination();

			return true;
		}

		public virtual void StartFollow()
		{
			StartFollow( GetEscorter() );
		}

		public virtual void StartFollow( Mobile escorter )
		{
			if ( escorter == null )
				return;
			ControlOrder = OrderType.Follow;
			ControlTarget = escorter;
		}

		public virtual void StopFollow()
		{
			ControlOrder = OrderType.None;
			ControlTarget = null;
		}

		private DateTime m_LastSeenEscorter;

		public virtual Mobile GetEscorter()
		{
			if ( !Controled )
				return null;

			Mobile master = ControlMaster;

			if ( master == null )
				return null;

			if ( master.Deleted || master.Map != this.Map || !master.InRange( Location, 30 ) || !master.Alive )
			{
				StopFollow();

				TimeSpan lastSeenDelay = DateTime.Now - m_LastSeenEscorter;

				if ( lastSeenDelay >= TimeSpan.FromMinutes( 2.0 ) )
				{
					master.SendLocalizedMessage( 1042473 ); // You have lost the person you were escorting.
					Say( 1005653 ); // Hmmm.  I seem to have lost my master.

					SetControlMaster( null );

					BeginDelete();
					return null;
				}
				else
				{
					ControlOrder = OrderType.Stay;
					return master;
				}
			}

			if ( ControlOrder != OrderType.Follow )
				StartFollow( master );

			m_LastSeenEscorter = DateTime.Now;
			return master;
		}

		public virtual void BeginDelete()
		{
			if ( m_DeleteTimer != null )
				m_DeleteTimer.Stop();

			m_DeleteTime = DateTime.Now + TimeSpan.FromMinutes( 5.0 );

			m_DeleteTimer = new DeleteTimer( this, TimeSpan.FromMinutes( 5.0 ) );
			m_DeleteTimer.Start();
		}

		public virtual bool IsAtDest()
		{
			EDI dest = GetDestination();

			if ( dest == null )
				return false;
			else
				return dest.Contains( Location );
		}

		public virtual int GetGoldAmount()
		{
			return Utility.RandomMinMax( 100, 300 );
		}

		public virtual bool CheckAtDestination()
		{
			Mobile escorter = GetEscorter();

			if ( escorter == null )
				return false;

			if ( IsAtDest() )
			{
				Say( 1042809, escorter.Name ); // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.

				// not going anywhere
				m_Destination = null;
				m_DestinationString = null;

				Container cont = escorter.Backpack;

				if ( cont == null )
					cont = escorter.BankBox;

				Gold gold = new Gold( GetGoldAmount() );

				if ( cont == null || !cont.TryDropItem( escorter, gold, false ) )
					gold.MoveToWorld( escorter.Location, escorter.Map );

				StopFollow();
				SetControlMaster( null );
				BeginDelete();

				Misc.Titles.AlterNotoriety( escorter, 1, NotoCap.Honorable );
				return true;
			}

			return false;
		}

		public BaseEscortable( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			EDI dest = GetDestination();

			writer.Write( dest != null );

			if ( dest != null )
				writer.Write( dest.Name );

			writer.Write( m_DeleteTimer != null );

			if ( m_DeleteTimer != null )
				writer.WriteDeltaTime( m_DeleteTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( reader.ReadBool() )
				m_DestinationString = reader.ReadString(); // NOTE: We cannot EDI.Find here, regions have not yet been loaded :-(

			if ( reader.ReadBool() )
			{
				m_DeleteTime = reader.ReadDeltaTime();
				m_DeleteTimer = new DeleteTimer( this, m_DeleteTime - DateTime.Now );
				m_DeleteTimer.Start();
			}
		}

        public override void AddCustomContextEntries(Mobile from, List<ContextMenus.ContextMenuEntry> list)
		{
			EDI dest = GetDestination();

			if ( dest != null && from.Alive )
			{
				Mobile escorter = GetEscorter();

				if ( escorter == null || escorter == from )
					list.Add( new AskDestinationEntry( this, from ) );

				if ( escorter == null )
					list.Add( new AcceptEscortEntry( this, from ) );
				else if ( escorter == from )
					list.Add( new AbandonEscortEntry( this, from ) );
			}

			base.AddCustomContextEntries( from, list );
		}

		public virtual string[] GetPossibleDestinations()
		{
			return m_TownNames;
		}

		public virtual string PickRandomDestination()
		{
			if ( Map.Felucca.Regions.Count == 0 || Map == null || Map == Map.Internal || Location == Point3D.Zero )
				return null; // Not yet fully initialized

			string[] possible = GetPossibleDestinations();
			string picked = null;

			while ( picked == null )
			{
				picked = possible[Utility.Random( possible.Length )];
				EDI test = EDI.Find( picked );

				if ( test != null && test.Contains( Location ) )
					picked = null;
			}

			return picked;
		}

		public EDI GetDestination()
		{
			if ( m_DestinationString == null && m_DeleteTimer == null )
				m_DestinationString = PickRandomDestination();

			if ( m_Destination != null && m_Destination.Name == m_DestinationString )
				return m_Destination;

			if ( Map.Felucca.Regions.Count > 0 )
				return ( m_Destination = EDI.Find( m_DestinationString ) );

			return ( m_Destination = null );
		}

		private class DeleteTimer : Timer
		{
			private Mobile m_Mobile;

			public DeleteTimer( Mobile m, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Mobile.Delete();
			}
		}
	}

	public class EscortDestinationInfo
	{
		private string m_Name;
		private Region m_Region;
		//private Rectangle2D[] m_Bounds;

		public string Name
		{
			get{ return m_Name; }
		}

		public Region Region
		{
			get{ return m_Region; }
		}

		/*public Rectangle2D[] Bounds
		{
			get{ return m_Bounds; }
		}*/

		public bool Contains( Point3D p )
		{
			if ( m_Region != null )
				return m_Region.Contains( p );
			else
				return false;
		}

		public EscortDestinationInfo( string name, Region region )
		{
			m_Name = name;
			m_Region = region;
		}

		private static Hashtable m_Table;

		public static void LoadTable()
		{
			ArrayList list = new ArrayList(Map.Felucca.Regions.Values);

			if ( list.Count == 0 )
				return;

			m_Table = new Hashtable();

			for ( int i = 0; i < list.Count; ++i )
			{
				Region r = (Region)list[i];

                if (r is Regions.DungeonRegion || r is Regions.TownRegion)
                    m_Table[r.Name] = new EscortDestinationInfo(r.Name, r);
                else if (r.Name == "Dungeons Level 1")
                    m_Table["a dungeon"] = new EscortDestinationInfo("a dungeon", r);
			}
		}

		public static EDI Find( string name )
		{
			if ( m_Table == null )
				LoadTable();

			if ( name == null || m_Table == null )
				return null;

			return (EscortDestinationInfo)m_Table[name];
		}
	}

	public class AskDestinationEntry : ContextMenuEntry
	{
		private BaseEscortable m_Mobile;
		private Mobile m_From;

		public AskDestinationEntry( BaseEscortable m, Mobile from ) : base( 6100, 3 )
		{
			m_Mobile = m;
			m_From = from;
		}

		public override void OnClick()
		{
			m_Mobile.SayDestinationTo( m_From );
		}
	}

	public class AcceptEscortEntry : ContextMenuEntry
	{
		private BaseEscortable m_Mobile;
		private Mobile m_From;

		public AcceptEscortEntry( BaseEscortable m, Mobile from ) : base( 6101, 3 )
		{
			m_Mobile = m;
			m_From = from;
		}

		public override void OnClick()
		{
			m_Mobile.AcceptEscorter( m_From );
		}
	}

	public class AbandonEscortEntry : ContextMenuEntry
	{
		private BaseEscortable m_Mobile;
		private Mobile m_From;

		public AbandonEscortEntry( BaseEscortable m, Mobile from ) : base( 6102, 3 )
		{
			m_Mobile = m;
			m_From = from;
		}

		public override void OnClick()
		{
			m_Mobile.Delete(); // OSI just seems to delete instantly
		}
	}
}