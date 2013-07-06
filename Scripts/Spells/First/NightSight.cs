using System;
using Server.Targeting;
using Server.Network;
using Server;

namespace Server.Spells.First
{
	public class NightSightSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Night Sight", "In Lor",
				SpellCircle.First,
				236,
				9031,
				Reagent.SulfurousAsh,
				Reagent.SpidersSilk
			);

		public NightSightSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new NightSightTarget( this );
		}

		private class NightSightTarget : Target
		{
			private Spell m_Spell;

			public NightSightTarget( Spell spell ) : base( 10, false, TargetFlags.None )
			{
				m_Spell = spell;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile && m_Spell.CheckSequence() )
				{
					Mobile targ = (Mobile)targeted;

					SpellHelper.Turn( m_Spell.Caster, targ );

					if ( targ.BeginAction( typeof( LightCycle ) ) )
					{
						new LightCycle.NightSightTimer( targ ).Start();
						int level = (int)Math.Abs( LightCycle.DungeonLevel * ( m_Spell.Caster.Skills[SkillName.Magery].Base / 100 ) );

						if ( level > 25 || level < 0 )
							level = 25;

						targ.LightLevel = level;
					}
					targ.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
					targ.PlaySound( 0x1E3 );
				}

				m_Spell.FinishSequence();
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Spell.FinishSequence();
			}
		}
	}
}
