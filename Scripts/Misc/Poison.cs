using System;
using Server;
using Server.Network;
using Server.Spells;

namespace Server
{
	public class PoisonImpl : Poison
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			//                             Name, Level, Scalar, Interval, 
			Register( new PoisonImpl( "Lesser",		0, 0.050, 15 ) );
			Register( new PoisonImpl( "Regular",	1, 0.067, 10 ) );
			Register( new PoisonImpl( "Greater",	2, 0.125, 10 ) );
			Register( new PoisonImpl( "Deadly",		3, 0.250, 5 ) );
			Register( new PoisonImpl( "Lethal",		4, 0.500, 5 ) );
		}

		public static Poison IncreaseLevel( Poison oldPoison )
		{
			return oldPoison;
			//Poison newPoison = ( oldPoison == null ? null : GetPoison( oldPoison.Level + 1 ) );
			//return ( newPoison == null ? oldPoison : newPoison );
		}

		// Info
		private string m_Name;
		private int m_Level;

		// Damage
		private double m_Scalar;

		// Timers
		private TimeSpan m_Interval;

		public PoisonImpl( string name, int level, double scalar, double interval )
		{
			m_Name = name;
			m_Level = level;
			m_Scalar = scalar;
			m_Interval = TimeSpan.FromSeconds( interval * 0.75 );
			//m_Scalar /= 2.0; // are the t2a values are too harsh?
		}

		public override string Name{ get{ return m_Name; } }
		public override int Level{ get{ return m_Level; } }

		private class PoisonTimer : Timer
		{
			private PoisonImpl m_Poison;
			private Mobile m_Mobile;
			private int m_LastDamage;
			private int m_Index;
			private int m_Count;

			public PoisonTimer( Mobile m, PoisonImpl p ) : base( p.m_Interval, p.m_Interval )
			{
				Priority = TimerPriority.FiftyMS;
				m_Mobile = m;
				m_Poison = p;

				m_Index = 0;
				// 4 to 15 intervals 
				m_Count = ( Utility.Random( 5, 15 ) ) * ( m_Poison.m_Level + 1 );
				
				CalcDamage();
				//m_Mobile.OnPoisoned( m_Mobile, m_Poison, m_Poison );
			}

			private void CalcDamage()
			{
				m_LastDamage = (int)(m_Mobile.Hits * m_Poison.m_Scalar);
				if ( m_LastDamage < 1 )
					m_LastDamage = 1;
				else if ( m_LastDamage > 50 )
					m_LastDamage = 50;
			}

			protected override void OnTick()
			{
				if ( m_Index++ >= m_Count )
				{
					m_Mobile.SendLocalizedMessage( 502136 ); // The poison seems to have worn off.
					m_Mobile.Poison = null;

					Stop();
					return;
				}

				m_Mobile.Damage( m_LastDamage + 2, m_Mobile );
				if ( Utility.RandomBool() )
				{
					m_Mobile.OnPoisoned( m_Mobile, m_Poison, m_Poison );
					CalcDamage();
				}
			}
		}

		public override Timer ConstructTimer( Mobile m )
		{
			return new PoisonTimer( m, this );
		}
	}
}

