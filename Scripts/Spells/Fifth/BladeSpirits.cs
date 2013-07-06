using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Fifth
{
	public class BladeSpiritsSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Blade Spirits", "In Jux Hur Ylem", 
				SpellCircle.Fifth,
				266,
				9040,
				false,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.Nightshade
			);

		public BladeSpiritsSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromSeconds( 6.25 );
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			Map map = Caster.Map;

			Point3D pt = SpellHelper.GetSurfaceTop( p );

			if ( !(p is Mobile) && ( map == null || !map.CanFit( pt, 16 ) ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( SpellHelper.CheckTown( pt, Caster ) && CheckSequence() )
			{
				BaseCreature.Summon( new BladeSpirit(), false, Caster, pt, 0x212, TimeSpan.FromSeconds( Utility.Random( 80, 40 ) ) );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private BladeSpiritsSpell m_Owner;

			public InternalTarget( BladeSpiritsSpell owner ) : base( 12, true, TargetFlags.None )
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
