using System;
using System.Collections; using System.Collections.Generic;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Second
{
	public class ProtectionSpell : Spell
	{
		private static Hashtable m_Registry = new Hashtable();
		public static Hashtable Registry { get { return m_Registry; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Protection", "Uus Sanct",
				SpellCircle.Second,
				236,
				9011,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.SulfurousAsh
			);

		public ProtectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Core.AOS )
				return true;

			if ( m_Registry.ContainsKey( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}

			return true;
		}

		private static Hashtable m_Table = new Hashtable();

		public static void Toggle( Mobile caster, Mobile target )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( Registry.ContainsKey( m ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
			}
			else if ( CheckSequence() )
			{
				int val = (int)(Caster.Skills[SkillName.Magery].Value / 10.0) + 1;
				m.VirtualArmorMod += val;
				Registry.Add( m, val );
				new InternalTimer( TimeSpan.FromSeconds( 6 * (Caster.Skills[SkillName.Magery].Value / 5) ), m, val ).Start();

				m.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );
				m.PlaySound( 0x1ED );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ProtectionSpell m_Owner;

			public InternalTarget( ProtectionSpell owner ) : base( 12, false, TargetFlags.Harmful )
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

		public class InternalTimer : Timer
		{
			private Mobile m_Targ;
			private int m_Val;

			public InternalTimer( TimeSpan duration, Mobile targ, int val ) : base( duration )
			{
				Priority = TimerPriority.OneSecond;
				m_Targ = targ;
				m_Val = val;
			}

			protected override void OnTick()
			{
				Registry.Remove( m_Targ );
				m_Targ.VirtualArmorMod -= m_Val;
			}
		}
	}
}
