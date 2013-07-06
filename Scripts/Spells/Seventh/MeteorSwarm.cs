using System;
using System.Collections; using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Seventh
{
	public class MeteorSwarmSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Meteor Swarm", "Flam Kal Des Ylem",
				SpellCircle.Seventh,
				245,
				9042,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh,
				Reagent.SpidersSilk
			);

		public MeteorSwarmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();
				ArrayList targets = new ArrayList();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 2 );

					foreach ( Mobile m in eable )
					{
						if ( SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
							targets.Add( m );
					}

					eable.Free();
				}

				if ( targets.Count > 0 )
				{
					Effects.PlaySound( p, Caster.Map, 0x160 );

					double damage = GetPreUORDamage();
					if ( targets.Count > 1 )
						damage *= 2;
					damage /= targets.Count;

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = (Mobile)targets[i];

						SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

						double toDeal = damage;

						if ( CheckResisted( m, toDeal ) )
						{
							toDeal *= 0.5;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

						int range = (int)Caster.GetDistanceToSqrt( m );
						Caster.DoHarmful( m );
						SpellHelper.Damage( TimeSpan.FromSeconds( range > 6 ? 2 : 1 ), m, toDeal );
						Caster.MovingParticles( m, 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100 );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MeteorSwarmSpell m_Owner;

			public InternalTarget( MeteorSwarmSpell owner ) : base( 12, true, TargetFlags.None )
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
