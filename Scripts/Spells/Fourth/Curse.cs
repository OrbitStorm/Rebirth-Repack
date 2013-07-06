using System;
using System.Collections; using System.Collections.Generic;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fourth
{
	public class CurseSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Curse", "Des Sanct",
				SpellCircle.Fourth,
				227,
				9031,
				Reagent.Nightshade,
				Reagent.Garlic,
				Reagent.SulfurousAsh
			);

		public CurseSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public static readonly Hashtable UnderEffect = new Hashtable();

		public static void RemoveEffect( object state )
		{
			Mobile m = (Mobile)state;

			UnderEffect.Remove( m );

			m.UpdateResistances();
		}

		public static bool IsUnderEffect( Mobile m )
		{
			return UnderEffect.Contains( m );
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

				SpellHelper.AddStatCurse( Caster, m, StatType.Str ); SpellHelper.DisableSkillCheck = true;
				SpellHelper.AddStatCurse( Caster, m, StatType.Dex );
				SpellHelper.AddStatCurse( Caster, m, StatType.Int ); SpellHelper.DisableSkillCheck = false;

				Timer t = (Timer)UnderEffect[m];

				if ( Caster.Player && m.Player && t == null )
				{
					TimeSpan duration = SpellHelper.GetDuration( Caster, m );
					UnderEffect[m] = t = Timer.DelayCall( duration, new TimerStateCallback( RemoveEffect ), m );
					m.UpdateResistances();
				}

				m.Paralyzed = false;

				m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
				m.PlaySound( 0x1EA );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private CurseSpell m_Owner;

			public InternalTarget( CurseSpell owner ) : base( 12, false, TargetFlags.Harmful )
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