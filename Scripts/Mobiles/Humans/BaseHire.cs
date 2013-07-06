using System; 
using Server; 
using System.Collections; using System.Collections.Generic; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Mobiles; 
using Server.Network; 

namespace Server.Mobiles 
{ 
    public class BaseHire : BaseConvo
    { 
        private int m_Bank; 
        private Timer m_Timer; 

		public override bool CanBeRenamedBy( Mobile from )
		{
			return false;
		}
        
        public BaseHire( AIType AI ): base( AI, FightMode.Agressor, 10, 1, 0.5, 0.75 ) 
        { 
			m_Bank = 0;
        } 

        public BaseHire(): this( AIType.AI_Melee ) 
        { 
        } 

        public BaseHire( Serial serial ) : base( serial ) 
        { 
        } 

        public override void Serialize( GenericWriter writer ) 
        { 
            base.Serialize( writer ); 

            writer.Write( (int) 0 ); // version 
			
            writer.Write( (int)m_Bank ); 
        } 

        public override void Deserialize( GenericReader reader ) 
        { 
            base.Deserialize( reader ); 
            int version = reader.ReadInt(); 

            m_Bank = reader.ReadInt(); 
          
			if ( this.Controled )
			{
				m_Timer = new PayTimer( this ); 
				m_Timer.Start(); 
			}
        }

		public override bool KeepsItemsOnDeath
		{
			get
			{
				// dont let them hire us just to kill us for our stuff :-/
				return !this.Controled;
			}
		}

        public override void OnAfterDelete()
        { 
            if( m_Timer != null ) 
                m_Timer.Stop(); 
            m_Timer = null; 
        } 

        public virtual Mobile GetOwner() 
        { 
            if( !Controled || Deleted ) 
                return null; 
			Mobile owner = ControlMaster;
			if( owner == null || owner.Deleted ) 
			{
				Say( 1005653 ); // Hmmm.  I seem to have lost my master. 
				Delta( MobileDelta.Noto );
				SetControlMaster( null );
				SummonMaster = null;

				BondingBegin = DateTime.MinValue;
				OwnerAbandonTime = DateTime.MinValue;
				IsBonded = false;
				m_Bank = 0;
				return null;
			}
			else
			{
				return owner; 
			}
        }

        public virtual bool AddHire( Mobile m ) 
        { 
            Mobile owner = GetOwner(); 

            if( owner != null ) 
            { 
                m.SendLocalizedMessage( 1043283, owner.Name ); // I am following ~1_NAME~. 
                return false; 
            }
            return SetControlMaster( m );
        } 

        public virtual int CalcPay() 
        { 
			if ( this.Skills == null )
				return 1;

            int pay = (int)(Skills.Total / ( 10 + Utility.RandomMinMax( -2, 2 ) ));
			if ( pay < 10 )
				pay = 10;
			else if ( pay > 150 )
				pay  = 150;
            return pay; 
        } 

		public override bool OnDragDrop( Mobile from, Item item ) 
		{ 
			if ( from == null || item == null )
				return false;

			if ( !(item is Gold ) )
			{
				SayTo( from, 500200 ); // I have no need for that. 
				return false;
			}

			int pay = CalcPay();
			if ( this.Controled )
			{
				if ( this.ControlMaster == from )
				{
					m_Bank += item.Amount;
					item.Delete();

					int days = (int)(m_Bank / pay);
					Say( "I will work for thee for {0} more day{1}.", days, days != 1 ? "s" : "" );
				}
				else
				{
					Say( 1042495 );// I have already been hired. 
					return false;
				}
			}
			else if ( item.Amount < pay )
			{
				Say( "Thou willst have to pay me more than that!" );
				return false;
			}
			else if ( AddHire( from ) )
			{
				m_Bank = item.Amount;
				item.Delete();
				this.IsBonded = false;
				int days = (int)(m_Bank / pay);
				Say( "I shall work for thee! My salary is {0} gp per day, this is enough gold for {1} day{2}.", pay, days, days != 1 ? "s" : "" );
				if ( m_Timer == null )
					m_Timer = new PayTimer( this );
				m_Timer.Start();
			}

			return true;
		}

		public override bool HandlesOnSpeech(Mobile from)
		{
			return true;
		}

        public override void OnSpeech( SpeechEventArgs e ) 
        {    
            if( !e.Handled && e.Mobile.InRange( this, 4 ) ) 
            { 
                if( e.HasKeyword( 0x003B ) || e.HasKeyword( 0x0162 ) )
                { 
                    e.Handled = true;
					if ( this.Controled )
					{
						if ( this.ControlMaster == e.Mobile )
						{
							int days = (int)(m_Bank / CalcPay());
							Say( "Thou hast already payed me to work for thee for {0} more day{1}.", days, days != 1 ? "s" : "" );
						}
						else
						{
							Say( "I do not work for thee." );
						}
					}
					else
					{
						Say( 1043256, CalcPay().ToString() ); // "I am available for hire for ~1_AMOUNT~ gold coins a day. If thou dost give me gold, I will work for thee." 
					} 
                } 
            } 

            base.OnSpeech( e ); 
        } 

		[CommandProperty( AccessLevel.GameMaster )]
		public int Bank
		{
			get { return m_Bank; }
			set { m_Bank = value; }
		}
        
        private class PayTimer : Timer 
        { 
            private BaseHire m_Hire; 
          
            public PayTimer( BaseHire hire ) : base( TimeSpan.FromMinutes( Clock.MinutesPerUODay ), TimeSpan.FromMinutes( Clock.MinutesPerUODay ) ) 
            { 
                m_Hire = hire; 
                Priority = TimerPriority.OneMinute; 
            } 

            protected override void OnTick() 
            { 
                int pay = m_Hire.CalcPay();
                if( m_Hire.Bank < pay ) 
                { 
                    m_Hire.Say( true, "Thou hast not payed me! I quit!" );
					m_Hire.SetControlMaster( null );
					m_Hire.SummonMaster = null;
					m_Hire.Delta( MobileDelta.Noto );

					m_Hire.BondingBegin = DateTime.MinValue;
					m_Hire.OwnerAbandonTime = DateTime.MinValue;
					m_Hire.IsBonded = false;

					m_Hire.Bank = 0;

					Stop();
                } 
                else 
                { 
                    m_Hire.Bank -= pay; 
					if ( m_Hire.Bank < pay )
						m_Hire.Say( true, String.Format( "Thou must pay me my salary of {0}gp, or this will be my last day.", pay - m_Hire.Bank ) );
					else
						m_Hire.Say( true, "Ahh, a new day already!" );
                } 
            } 
        } 
    }    
} 
