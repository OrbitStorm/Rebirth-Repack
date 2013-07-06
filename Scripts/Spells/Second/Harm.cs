using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Second
{
	public class HarmSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Harm", "An Mani",
				SpellCircle.Second,
				212,
				Core.AOS ? 9001 : 9041,
				Reagent.Nightshade,
				Reagent.SpidersSilk
			);

		public override bool DelayedDamage{ get{ return false; } }

		public HarmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				double damage = GetDamage( m );

				if ( Core.AOS )
				{
					m.FixedParticles( 0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist );
					m.PlaySound( 0x0FC );
				}
				else
				{
					m.FixedParticles( 0x374A, 10, 15, 5013, EffectLayer.Waist );
					m.PlaySound( 0x1F1 );
				}

				SpellHelper.Damage( this, m, damage, 0, 0, 100, 0, 0 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private HarmSpell m_Owner;

			public InternalTarget( HarmSpell owner ) : base( 12, false, TargetFlags.Harmful )
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