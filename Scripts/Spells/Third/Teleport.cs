using System;
using Server.Targeting;
using Server.Network;
using Server.Regions;
using Server.Items;

namespace Server.Spells.Third
{
	public class TeleportSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Teleport", "Rel Por",
				SpellCircle.Third,
				215,
				9031,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public TeleportSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			IPoint3D orig = p;
			Map map = Caster.Map;

			SpellHelper.GetSurfaceTop( ref p );

			Point3D to = new Point3D( p );

			if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			}
			else if ( map == null || !map.CanFit( p.X, p.Y, p.Z, 16 ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( SpellHelper.CheckMulti( to, map ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, orig );

				Mobile m = Caster;

				Point3D from = m.Location;

				HouseRegion destRgn = Region.Find( to, m.Map ) as HouseRegion;
				if ( destRgn != null && destRgn.House != null )
				{
					if ( ( m.Region == destRgn && destRgn.House is Multis.LargePatioHouse ) || ( m.Region != destRgn && destRgn.House.IsInside( to, 15 ) ) )
					{
						Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
						return;
					}
				}

				m.Location = to;
				m.ProcessDelta();

				Effects.SendLocationParticles( EffectItem.Create( from, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				Effects.SendLocationParticles( EffectItem.Create(   to, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

				m.PlaySound( 0x1FE );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private TeleportSpell m_Owner;

			public InternalTarget( TeleportSpell owner ) : base( 12, true, TargetFlags.None )
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