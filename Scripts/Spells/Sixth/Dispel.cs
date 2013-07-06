using System;
using Server.Misc;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Sixth
{
	public class DispelSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Dispel", "An Ort",
				SpellCircle.Sixth,
				218,
				9002,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public DispelSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			Type t = m.GetType();
			bool dispellable = false;

			if ( m is BaseCreature )
				dispellable = ((BaseCreature)m).Summoned && !((BaseCreature)m).IsAnimatedDead;

			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( !dispellable )
			{
				Caster.SendLocalizedMessage( 1005049 ); // That cannot be dispelled.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				int diff;
				if ( m is Daemon )
					diff = 95;
				else if ( m is EnergyVortex )
					diff = 80;
				else if ( m is FireElemental || m is WaterElemental || m is AirElemental || m is EarthElemental )
					diff = 75;
				else if ( m is BladeSpirit )
					diff = 50;
				else
					diff = 0;

				if ( Caster.Skills[SkillName.Magery].Value >= Utility.Random( 41 )-20+diff )
				{
					Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
					Effects.PlaySound( m, m.Map, 0x201 );

					m.Delete();
				}
				else
				{
					m.Animate( 14201, 10, 20, true, false, 0 );
					Caster.SendAsciiMessage( "{0} resisted the attempt to dispell it!", m.Name );
				}
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private DispelSpell m_Owner;

			public InternalTarget( DispelSpell owner ) : base( 12, false, TargetFlags.Harmful )
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