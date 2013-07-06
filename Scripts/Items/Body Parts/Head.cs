using System; 
using Server; 
using Server.Mobiles; 

namespace Server.Items 
{ 
	public class Head : BaseItem, ICarvable
	{
		#region ICarvable Members
		public void Carve(Mobile from, Item item)
		{
			Item brain = new BaseItem( 7408 );
			Item skull = new BaseItem( 6882 );
			if ( m_Owner != null )
			{
				brain.Name = "brain of " + m_Owner.Name;
				skull.Name = "skull of " + m_Owner.Name;
			}

			if ( !(Parent is Container) )
			{
				brain.MoveToWorld( GetWorldLocation(), Map );
				skull.MoveToWorld( GetWorldLocation(), Map );
			}
			else
			{
				Container cont = (Container)Parent;
				cont.DropItem( brain );
				cont.DropItem( skull );
			}

			Delete();
		}
		#endregion

		private Mobile m_Owner; 
		private DateTime m_Created;
		private int m_Bounty;
	   
		[CommandProperty( AccessLevel.GameMaster )] 
		public Mobile Owner 
		{	
			get{ return m_Owner; }	
			set{ m_Owner =	value; } 
		}	

		[CommandProperty( AccessLevel.GameMaster )] 
		public int MaxBounty 
		{	
			get{ return m_Bounty; }	
			set{ m_Bounty =	value; } 
		}	

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Age 
		{
			get { return DateTime.Now - m_Created; }
		}

		[Constructable] 
		public Head() : base( 0x1DA0 ) 
		{
			m_Created = DateTime.Now;
			Weight = 1.0; 
			LastMoved = (DateTime.Now - Item.DefaultDecayTime) + TimeSpan.FromMinutes( 7.5 );
		}

		[Constructable] 
		public Head( string name ) : this()
		{	
			Name = name; 
		}	

		public Head( Mobile owner ) : this( owner == null ? "a head" : String.Format( "head of {0}", owner.Name ) )
		{
			m_Owner = owner;
			if ( m_Owner != null && !m_Owner.Deleted && m_Owner is PlayerMobile )
				m_Bounty = ((PlayerMobile)m_Owner).Bounty;
		}

		public Head( Serial serial ) : base( serial )	
		{	
		}	

		public override void Serialize( GenericWriter writer ) 
		{	
			base.Serialize( writer ); 
			writer.Write( (int) 1 ); // version 

			writer.Write( m_Bounty );

			writer.Write( m_Owner ); 
			writer.Write( m_Created );	
		}	

		public override void Deserialize( GenericReader reader ) 
		{	
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 

			switch ( version )
			{
				case 1:
				{
					m_Bounty = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_Owner = reader.ReadMobile();	
					m_Created = reader.ReadDateTime();	

					break;
				}
			}

			if ( version == 0 && m_Owner is PlayerMobile )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( UpdateBounty ) );
		}	

		private void UpdateBounty()
		{
			if ( m_Owner is PlayerMobile && !m_Owner.Deleted )
				m_Bounty = ((PlayerMobile)m_Owner).Bounty;
		}
	}
}

