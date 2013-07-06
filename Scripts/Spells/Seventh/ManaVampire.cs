using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Seventh
{
	public class ManaVampireSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mana Vampire", "Ort Sanct",
				SpellCircle.Seventh,
				245,
				9032,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public ManaVampireSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m ) ;

				m.Paralyzed = false;

				int toDrain = 0;
				if ( CheckResistedEasy( m ) )
					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				else
					toDrain = m.Mana;

				if ( toDrain + Caster.Mana > Caster.ManaMax )
					toDrain = Caster.ManaMax - Caster.Mana;

				if ( m.Spell is Spells.Spell && toDrain > 0 )
					((Spells.Spell)m.Spell).OnCasterHurt( toDrain );

				m.Mana -= toDrain;
				if ( m != Caster )
					Caster.Mana += toDrain;

				m.FixedParticles( 0x374A, 10, 15, 5054, EffectLayer.Head );
				m.PlaySound( 0x1F9 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ManaVampireSpell m_Owner;

			public InternalTarget( ManaVampireSpell owner ) : base( 12, false, TargetFlags.Harmful )
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