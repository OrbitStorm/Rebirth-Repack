using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class EnergyVortexSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Energy Vortex", "Vas Corp Por",
				SpellCircle.Eighth,
				230,
				9022,
				false,
				Reagent.Bloodmoss,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.Nightshade
			);

		public EnergyVortexSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			Map map = Caster.Map;

			Point3D pt = SpellHelper.GetSurfaceTop( p );

			if ( !(p is Mobile && ((Mobile)p).Alive) && ( map == null || !map.CanFit( pt, 16 ) ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( SpellHelper.CheckTown( pt, Caster ) && CheckSequence() )
			{
				BaseCreature.Summon( new EnergyVortex(), false, Caster, pt, 0x212, TimeSpan.FromSeconds( 90 ) );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private EnergyVortexSpell m_Owner;

			public InternalTarget( EnergyVortexSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetOutOfLOS( Mobile from, object o )
			{
				from.SendLocalizedMessage( 501943 ); // Target cannot be seen. Try again.
				from.Target = new InternalTarget( m_Owner );
				from.Target.BeginTimeout( from, TimeoutTime - DateTime.Now );
				m_Owner = null;
			}

			protected override void OnTargetFinish( Mobile from )
			{
				if ( m_Owner != null )
					m_Owner.FinishSequence();
			}
		}
	}
}
