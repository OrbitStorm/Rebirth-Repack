using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
	public class MindBlastSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mind Blast", "Por Corp Wis",
				SpellCircle.Fifth,
				218,
				Core.AOS ? 9002 : 9032,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.Nightshade,
				Reagent.SulfurousAsh
			);

		public MindBlastSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
			if ( Core.AOS )
				m_Info.LeftHandEffect = m_Info.RightHandEffect = 9002;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private void AosDelay_Callback( object state )
		{
			object[] states = (object[])state;
			Mobile caster = (Mobile)states[0];
			Mobile target = (Mobile)states[1];
			Mobile defender = (Mobile)states[2];
			int damage = (int)states[3];

			if ( caster.HarmfulCheck( defender ) )
			{
				SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 0, 100, 0, 0 );

				target.FixedParticles( 0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head );
				target.PlaySound( 0x213 );
			}
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile from = Caster, target = m;
				SpellHelper.Turn( from, target );

				bool reflect = false;
				int damage = (from.Int - target.Int);
				if ( SpellHelper.CheckReflect( (int)this.Circle, ref from, ref target ) )
					reflect = true;
				if ( damage <= 0 )
				{
					from.FixedParticles( 0x374A, 10, 15, 2038, EffectLayer.Head );

					damage *= -1;
					target = from;
				}

				damage /= 4;
				if ( CheckResisted( target, damage ) )
				{
					damage /= 2;
					target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				target.FixedParticles( 0x374A, 10, 15, 5038, EffectLayer.Head );
				target.PlaySound( 0x213 );

				// if its reflected, then we should hurt the caster, otherwise the "target" (which could still be the caster)
				SpellHelper.Damage( this, reflect ? Caster : target, damage, 0, 0, 100, 0, 0 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MindBlastSpell m_Owner;

			public InternalTarget( MindBlastSpell owner ) : base( 12, false, TargetFlags.Harmful )
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