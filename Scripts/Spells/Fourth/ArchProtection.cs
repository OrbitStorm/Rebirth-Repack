using System;
using System.Collections; using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;
//using Server.Engines.PartySystem;

namespace Server.Spells.Fourth
{
	public class ArchProtectionSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Arch Protection", "Vas Uus Sanct",
				SpellCircle.Fourth,
				Core.AOS ? 239 : 215,
				9011,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public static Hashtable Registry
		{
			get
			{
				return Spells.Second.ProtectionSpell.Registry;
			}
		}

		public ArchProtectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), Core.AOS ? 2 : 3 );

					foreach ( Mobile m in eable )
					{
						if ( Caster.CanBeBeneficial( m, false ) )
							targets.Add( m );
					}

					eable.Free();
				}

				Effects.PlaySound( p, Caster.Map, 0x299 );

				int val = (int)(Caster.Skills[SkillName.Magery].Value / 10.0) + 1;;
				TimeSpan duration = TimeSpan.FromSeconds( 6 * (Caster.Skills[SkillName.Magery].Value / 5) );

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = (Mobile)targets[i];

					if ( !Registry.ContainsKey( m ) )
					{
						m.VirtualArmorMod += val;
						Registry.Add( m, val );
						new Second.ProtectionSpell.InternalTimer( duration, m, val ).Start();

						m.FixedParticles( 0x375A, 9, 20, 5027, EffectLayer.Waist );
						m.PlaySound( 0x1F7 );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ArchProtectionSpell m_Owner;

			public InternalTarget( ArchProtectionSpell owner ) : base( 12, true, TargetFlags.None )
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
