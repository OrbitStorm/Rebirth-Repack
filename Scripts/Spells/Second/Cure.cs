using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Second
{
	public class CureSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Cure", "An Nox",
				SpellCircle.Second,
				212,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng
			);

		public CureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				if ( m.Poison != null )
				{
					int chanceToCure = (int)( 10000 + ( Caster.Skills[SkillName.Magery].Value*75 - (m.Poison.Level + 1)*1750 ) ) / 100;
					if ( chanceToCure > Utility.Random( 100 ) && m.CurePoison( Caster ) )
					{
						if ( Caster != m )
							Caster.SendLocalizedMessage( 1010058 ); // You have cured the target of all poisons!

						m.SendLocalizedMessage( 1010059 ); // You have been cured of all poisons.
					}
					else
					{
						Caster.SendAsciiMessage( "You failed to cure {0}!", m.Name );
					}
				}

				m.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
				m.PlaySound( 0x1E0 );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private CureSpell m_Owner;

			public InternalTarget( CureSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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