using Server;
using System;
using Server.Mobiles;
using Server.Menus.Questions;
using Server.Network;

namespace Server.Gumps
{
	public class ResNowOption : QuestionMenu
	{
		private static void UnfreezeCallback( object state )
		{
			if ( state is Mobile )
				((Mobile)state).Frozen = false;
		}

		private static TimerStateCallback m_Unfreeze = new TimerStateCallback( UnfreezeCallback );

		private static string[] m_Options = new string[]
		{
			"Resurrect Now (You will be resurrected instantly, but will loose some of your stats and skills)",
			"Play as a Ghost (You will maintain your stats and skills, but must seek out a healer or mage to resurrect you)",
			"Always Play as Ghost (Never show this menu again)"
		};

		private DateTime m_Start;
		private Timer m_Timer;

		public ResNowOption( Mobile m ) : base( "Do you wish to resurrect now?", m_Options )
		{
			m_Start = DateTime.Now;

			m.Frozen = true;
			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), m_Unfreeze, m );
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

		public override void OnResponse( NetState state, int index )
		{
			PlayerMobile pm = state.Mobile as PlayerMobile;
			if ( pm == null )
				return;

			if ( m_Timer != null && m_Timer.Running )
			{
				pm.Frozen = false;
				m_Timer.Stop();
			}

			if ( m_Start + TimeSpan.FromMinutes( 10.0 ) < DateTime.Now )
			{
				if ( index == 0 )
					pm.SendAsciiMessage( "It has been too long since you died." );
				return;
			}

			switch ( index )
			{
				case 0:
				{
					if ( pm.Location != pm.DeathLocation )
					{
						pm.SendAsciiMessage( "Thou hast wandered too far from the site of thy resurrection!" );
						break;
					}
					else if ( pm.SpiritCohesion <= 0 )
					{
						pm.SendAsciiMessage( "Your spirit was too weak to return to corporeal form." );
						return;
					}

					pm.Resurrect();

					pm.SpiritCohesion --;
					switch ( pm.SpiritCohesion )
					{
						case 0:
							pm.SendAsciiMessage( "Your spirit returns to corporeal form, but is too weak to do so a gain for a while." );
							break;
						case 1:
							pm.SendAsciiMessage( "Your spirit barely manages to return to corporeal form." );
							break;
						case 2:
							pm.SendAsciiMessage( "With some effort your spirit returns to corporeal form." );
							break;
						case 3:
						default:
							pm.SendAsciiMessage( "Your spirit easily returns to corporeal form." );
							break;
					}

					for( int i=0;i<pm.Skills.Length; i++ )
					{
						if ( pm.Skills[i].Base > 25.0 )
							pm.Skills[i].Base -= Utility.Random( 5 ) + 5;
					}

					if ( pm.RawDex > 15 )
						pm.RawDex -= pm.RawDex / 15;
					if ( pm.RawStr > 15 )
						pm.RawStr -= pm.RawStr / 15;
					if ( pm.RawInt > 15 )
						pm.RawInt -= pm.RawInt / 15;

					pm.Hits = pm.HitsMax / 2;
					pm.Mana = pm.ManaMax / 5;
					break;
				}
				case 1:
				{
					break;
				}
				case 2:
				{
					pm.AssumePlayAsGhost = true;
					break;
				}
			}
		}
	}
}

