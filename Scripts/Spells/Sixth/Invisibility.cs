using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Sixth
{
	public class InvisibilitySpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Invisibility", "An Lor Xen",
				SpellCircle.Sixth,
				206,
				9002,
				Reagent.Bloodmoss,
				Reagent.Nightshade
			);

		public InvisibilitySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Effects.SendLocationParticles( EffectItem.Create( new Point3D( m.X, m.Y, m.Z + 16 ), Caster.Map, EffectItem.DefaultDuration ), 0x376A, 10, 15, 5045 );
				m.PlaySound( 0x3C4 );

				m.Hidden = true;

				RemoveTimer( m );

				TimeSpan duration = TimeSpan.FromSeconds( Caster.Skills[SkillName.Magery].Value * 1.2 ); // 120% of magery

				Timer t = new InternalTimer( m, duration );

				m_Table[m] = t;

				t.Start();
			}

			FinishSequence();
		}

		public static Hashtable m_Table = new Hashtable();

		public static void RemoveTimer( Mobile m )
		{
			Timer t = (Timer)m_Table[m];

			if ( t != null )
			{
				t.Stop();
				m_Table.Remove( m );
			}
		}

		public class InternalTimer : Timer
		{
			private Mobile m_Mobile;

			public InternalTimer( Mobile m, TimeSpan duration ) : base( duration )
			{
				Priority = TimerPriority.OneSecond;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.RevealingAction();
				RemoveTimer( m_Mobile );
			}
		}

		public class InternalTarget : Target
		{
			private InvisibilitySpell m_Owner;

			public InternalTarget( InvisibilitySpell owner ) : base( 12, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}