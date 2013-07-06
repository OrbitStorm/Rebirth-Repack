using System;
using System.Collections; using System.Collections.Generic;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fourth
{
	public class ManaDrainSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mana Drain", "Ort Rel",
				SpellCircle.Fourth,
				215,
				9031,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public ManaDrainSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private Hashtable m_Table = new Hashtable();

		private void AosDelay_Callback( object state )
		{
			object[] states = (object[])state;

			Mobile m = (Mobile)states[0];
			int mana = (int)states[1];

			if ( m.Alive && !m.IsDeadBondedPet )
			{
				m.Mana += mana;

				m.FixedEffect( 0x3779, 10, 25 );
				m.PlaySound( 0x28E );
			}

			m_Table.Remove( m );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

				m.Paralyzed = false;

				if ( CheckResistedEasy( m ) )
				{
					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}
				else 
				{
					if ( m.Spell is Spells.Spell )
						((Spells.Spell)m.Spell).OnCasterHurt( m.Mana );
					m.Mana = 0;
				}

				m.FixedParticles( 0x374A, 10, 15, 5032, EffectLayer.Head );
				m.PlaySound( 0x1F8 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ManaDrainSpell m_Owner;

			public InternalTarget( ManaDrainSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}