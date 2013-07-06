using System;
using System.Collections; using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Fourth
{
	public class ArchCureSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Arch Cure", "Vas An Nox",
				SpellCircle.Fourth,
				215,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.MandrakeRoot
			);

		public ArchCureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				SpellHelper.GetSurfaceTop( ref p );

				ArrayList targets = new ArrayList();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 3 );

					foreach ( Mobile m in eable )
					{
						if ( Caster.CanBeBeneficial( m, false ) )
							targets.Add( m );
					}

					eable.Free();
				}

				Effects.PlaySound( p, Caster.Map, 0x299 );

				if ( targets.Count > 0 )
				{
					int cured = 0;

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = (Mobile)targets[i];

						Caster.DoBeneficial( m );

						if ( m.CurePoison( Caster ) )
							++cured;

						m.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
						m.PlaySound( 0x1E0 );
					}

					if ( cured > 0 )
						Caster.SendLocalizedMessage( 1010058 ); // You have cured the target of all poisons!
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ArchCureSpell m_Owner;

			public InternalTarget( ArchCureSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}