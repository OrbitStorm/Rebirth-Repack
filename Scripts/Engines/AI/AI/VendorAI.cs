using System;
using System.Collections; using System.Collections.Generic;
using Server.Targeting;
using Server.Network;

namespace Server.Mobiles
{
	public class VendorAI : MeleeAI
	{
		public VendorAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionInteract()
		{
			Mobile customer = m_Mobile.FocusMob;

			if ( customer == null || customer.Deleted || customer.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "My customer have disapeared" );
				m_Mobile.FocusMob = null;

				Action = ActionType.Wander;
			}
			else
			{
				if ( customer.InRange( m_Mobile, m_Mobile.RangeFight ) )
				{
					m_Mobile.DebugSay( "I am with {0}", customer.Name );

					m_Mobile.Direction = m_Mobile.GetDirectionTo( customer );
				}
				else
				{
					m_Mobile.DebugSay( "{0} is gone", customer.Name );
					m_Mobile.FocusMob = null;

					Action = ActionType.Wander;	
				}
			}

			return base.DoActionInteract();
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( m_Mobile, 4 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			base.OnSpeech( e );
 
			Mobile from = e.Mobile;
 
			if ( m_Mobile is BaseVendor && from.InRange( m_Mobile, 4 ) && !e.Handled )
			{
				if ( e.HasKeyword( 0x14D ) ) // *vendor sell*
				{
					((BaseVendor)m_Mobile).VendorSell( from );
					e.Handled = true;
					m_Mobile.FocusMob = from;
				}
				else if ( e.HasKeyword( 0x3C ) ) // *vendor buy*
				{
					((BaseVendor)m_Mobile).VendorBuy( from );
					e.Handled = true;
					m_Mobile.FocusMob = from;
				}
				else if ( WasNamed( e.Speech ) || Insensitive.StartsWith( e.Speech, "hi " ) )
				{
					if ( e.HasKeyword( 0x177 ) ) // *sell*
					{
						((BaseVendor)m_Mobile).VendorSell( from );
						e.Handled = true;
						m_Mobile.FocusMob = from;
					}
					else if ( e.HasKeyword( 0x171 ) ) // *buy*
					{
						((BaseVendor)m_Mobile).VendorBuy( from );
						e.Handled = true;
						m_Mobile.FocusMob = from;
					}
				}
			}
		}
	}
}
