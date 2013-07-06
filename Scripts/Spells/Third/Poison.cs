using System;
using Server.Targeting;
using Server.Network;
using Server.Regions;

namespace Server.Spells.Third
{
	public class PoisonSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Poison", "In Nox",
				SpellCircle.Third,
				203,
				9051,
				Reagent.Nightshade
			);

		public PoisonSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

				m.Paralyzed = false;

				if ( CheckResistedEasy( m ) )
				{
					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}
				else
				{
					if ( Caster.Skills[SkillName.Magery].Value > Utility.Random( 1, 150 ) )
						m.ApplyPoison( Caster, Poison.Greater );
					else
						m.ApplyPoison( Caster, Poison.Regular );
					if ( m.Spell is Spell )
						((Spell)m.Spell).OnCasterHurt( 1 );
				}

				m.FixedParticles( 0x374A, 10, 15, 5021, EffectLayer.Waist );
				m.PlaySound( 0x474 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private PoisonSpell m_Owner;

			public InternalTarget( PoisonSpell owner ) : base( 12, false, TargetFlags.Harmful )
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