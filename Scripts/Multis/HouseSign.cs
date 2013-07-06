using System;
using Server;
using Server.Multis;
using Server.Gumps;
using Server.Items;

namespace Server.Multis
{
	public class HouseSign : BaseItem
	{
		// NOTE: this does not change what the house sign says!!
		private static readonly TimeSpan HouseLifeSpan = TimeSpan.FromDays( 18.0 );

		private BaseHouse m_Owner;
		private Mobile m_OrgOwner;
		private DateTime m_HouseDecay;

		public HouseSign( BaseHouse owner ) : base( 0xBD2 )
		{
			m_Owner = owner;
			m_OrgOwner = m_Owner.Owner;
			Name = "a house sign";
			Movable = false;
			m_HouseDecay = DateTime.Now + HouseLifeSpan;
		}

		public HouseSign( Serial serial ) : base( serial )
		{
		}

		public override bool Decays { get { return false; } } 

		public BaseHouse Owner
		{
			get
			{
				return m_Owner;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile OriginalOwner
		{
			get
			{
				return m_OrgOwner;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime HouseDecayDate
		{
			get { return m_HouseDecay; }
			set { m_HouseDecay = value; }
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Owner != null && !m_Owner.Deleted )
				m_Owner.Delete();
		}

		public void RefreshHouse( Mobile from )
		{
			CheckDecay();

			if ( !Deleted && m_HouseDecay != DateTime.MinValue && m_Owner != null )
			{	
				TimeSpan lastVisit = (DateTime.Now - m_HouseDecay) - HouseLifeSpan;
				if ( lastVisit > TimeSpan.FromDays( 0.5 ) )
					from.SendAsciiMessage( "Your {0}'s age and contents have been refreshed.", m_Owner is Tent ? "tent" : "house" );
				DateTime newDate = DateTime.Now + HouseLifeSpan;
				if ( m_HouseDecay < newDate )
					m_HouseDecay = newDate;
			}
		}

		public void CheckDecay()
		{
			if ( m_HouseDecay < DateTime.Now && m_HouseDecay != DateTime.MinValue )
			{
				if ( m_Owner != null && !m_Owner.Deleted )
					m_Owner.OnDecayed();
				this.Delete();
			}
		}

		public override void SendInfoTo(Server.Network.NetState state)
		{
			// try and decay it when it comes on screen
			base.SendInfoTo (state);

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckDecay ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			OnSingleClick( from );

			if ( Deleted )
				return;
			
			Container pack = from.Backpack;
			if ( pack == null || m_Owner == null )
				return;

			Item[] items = pack.FindItemsByType( typeof( Key ) );
			foreach( Key k in items )
			{
				if ( k.KeyValue == m_Owner.KeyValue )
				{
					from.Prompt = new Prompts.HouseRenamePrompt( m_Owner );
					from.SendLocalizedMessage( 1060767 ); // Enter the new name of your house.
					return;
				}
			}
		}

		public string GetDecayString()
		{
			CheckDecay();

			TimeSpan left = m_HouseDecay - DateTime.Now;
			string str;

			if ( Deleted )
				str = "This {0} collapsed.";
			else if ( m_HouseDecay == DateTime.MinValue )
				str = "This {0} is ageless.";
			else if ( left < TimeSpan.FromDays( 1 ) )
				str = "This {0} is in danger of collapsing.";
			else if ( left < TimeSpan.FromDays( 5 ) )
				str = "This {0} is greatly worn.";
			else if ( left < TimeSpan.FromDays( 9 ) )
				str = "This {0} is fairly worn.";
			else if ( left < TimeSpan.FromDays( 13 ) )
				str = "This {0} is somewhat worn.";
			else if ( left < TimeSpan.FromDays( 17 ) )
				str = "This {0} is slightly worn." ;
			else
				str = "This {0} is like new.";

			return String.Format( str, m_Owner is Tent ? "tent" : "house" );
		}
		
		public override void OnSingleClick( Mobile from )
		{
			if ( Deleted )
				return;
			AsciiLabelTo( from, Name );
			AsciiLabelTo( from, GetDecayString() );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			
			writer.Write( m_HouseDecay );
			writer.Write( m_Owner );
			writer.Write( m_OrgOwner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_HouseDecay = reader.ReadDateTime();
					goto case 0;
				}
				case 0:
				{
					m_Owner = reader.ReadItem() as BaseHouse;
					m_OrgOwner = reader.ReadMobile();

					break;
				}
			}

			if ( version < 1 )
				m_HouseDecay = DateTime.Now + HouseLifeSpan;
			else 
				CheckDecay();
		}
	}
}
