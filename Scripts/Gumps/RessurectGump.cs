using System;
using Server;
using Server.Menus.Questions;
using Server.Network;
using Server.Mobiles;

namespace Server
{
	public enum ResurrectMessage
	{
		ChaosShrine = 0,
		VirtueShrine = 1,
		Healer = 2,
		Generic = 3,
	}

	public class ResurrectMenu : QuestionMenu
	{
		private static string[] m_Options = new string[] { "YES - You choose to try to come back to life now.", "NO - You prefer to remain a ghost for now." };

		private Mobile m_Healer;
		private Point3D m_Location;
		private bool m_Heal;

		private Timer m_Timer;

		public ResurrectMenu( Mobile owner ) : this( owner, owner, ResurrectMessage.Generic )
		{
		}

		public ResurrectMenu( Mobile owner, Mobile healer ) : this( owner, healer, ResurrectMessage.Generic )
		{
		}

		public ResurrectMenu( Mobile owner, ResurrectMessage msg ) : this( owner, owner, msg )
		{
		}

		private static void UnfreezeCallback( object state )
		{
			if ( state is Mobile )
				((Mobile)state).Frozen = false;
		}

		private static TimerStateCallback m_Unfreeze = new TimerStateCallback( UnfreezeCallback );

		public ResurrectMenu( Mobile owner, Mobile healer, ResurrectMessage msg ) : base( "", m_Options )
		{
			m_Location = owner.Location;
			m_Healer = healer;

			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), m_Unfreeze, owner );

			owner.Frozen = true;
			
			switch ( msg )
			{
				case ResurrectMessage.Healer:
					Question = "It is possible for you to be resurrected here by this healer. Do you wish to try?";
					break;
				case ResurrectMessage.VirtueShrine:
					m_Heal = true;
					Question = "It is possible for you to be resurrected at this Shrine to the Virtues. Do you wish to try?";
					break;
				case ResurrectMessage.ChaosShrine:
					m_Heal = true;
					Question = "It is possible for you to be resurrected at the Chaos Shrine. Do you wish to try?";
					break;
				case ResurrectMessage.Generic:
				default:
					Question = "It is possible for you to be resurrected now. Do you wish to try?";
					break;
			}
		}

		public override void OnCancel(NetState state)
		{
			base.OnCancel (state);

			if ( state.Mobile != null && m_Timer != null && m_Timer.Running )
			{
				state.Mobile.Frozen = false;
				m_Timer.Stop();
			}
		}

		public override void OnResponse(NetState state, int index)
		{
			Mobile from = state.Mobile;

			if ( from != null && m_Timer != null && m_Timer.Running )
			{
				from.Frozen = false;
				m_Timer.Stop();
			}

			if ( index == 0 && from != null )
			{
				if ( from.Location != m_Location )
				{
					from.SendAsciiMessage( "Thou hast wandered too far from the site of thy resurrection!" );
					return;
				}

				PlayerMobile pm = from as PlayerMobile;
				if ( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) || m_Location != from.Location )
				{
					from.SendAsciiMessage( "Thou can not be resurrected there!" );
					return;
				}
				else if ( pm != null && pm.SpiritCohesion <= 0 )
				{
					from.SendAsciiMessage( "Your spirit is too weak to return to corporeal form." );
					return;
				}

				from.PlaySound( 0x214 );
				from.FixedEffect( 0x376A, 10, 16 );

				from.Resurrect();

				if ( m_Heal )
				{
					from.Hits = from.HitsMax;
					from.Stam = from.StamMax;
				}

				//Regions.HouseRegion hr = from.Region as Regions.HouseRegion;
				//if ( hr != null && hr.House != null && !hr.House.Deleted )
				//	from.Location = hr.House.BanLocation;

				if ( pm != null )
				{
					pm.SpiritCohesion --;
					switch ( pm.SpiritCohesion )
					{
						case 0:
							from.SendAsciiMessage( "Your spirit returns to corporeal form, but is too weak to do so again for a while." );
							break;
						case 1:
							from.SendAsciiMessage( "Your spirit barely manages to return to corporeal form." );
							break;
						case 2:
							from.SendAsciiMessage( "With some effort your spirit returns to corporeal form." );
							break;
						case 3:
						default:
							from.SendAsciiMessage( "Your spirit easily returns to corporeal form." );
							break;
					}
				}
			}
		}
	}
}
