using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.First
{
	public class HealSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Heal", "In Mani",
				SpellCircle.First,
				224,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.SpidersSilk
			);

		public HealSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( m is BaseCreature && ((BaseCreature)m).IsAnimatedDead )
			{
				Caster.SendLocalizedMessage( 1061654 ); // You cannot heal that which is not alive.
			}
			else if ( Server.Items.MortalStrike.IsWounded( m ) )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x22, (Caster == m) ? 1005000 : 1010398 );
			}
			/* // idea... dont allow lesser heal to heal through poison (but allow it on gheal)
			else if ( m.Poisoned || Server.Items.MortalStrike.IsWounded( m ) )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x22, (Caster == m) ? 1005000 : 1010398 ); // You cannot heal that target/yourself in their/your current state.
			}*/
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				int toHeal = 1 + (int)( Utility.Random( 15 ) * Caster.Skills[SkillName.Magery].Value / 100.0 );
				if ( Caster != m && Caster.NetState != null )
					Caster.NetState.Send( new MessageLocalizedAffix( Serial.MinusOne, -1, MessageType.Label, 0x3B2, 3, 1008158, "", AffixType.Append | AffixType.System, (m.Hits+toHeal > m.HitsMax ? m.HitsMax - m.Hits : toHeal).ToString(), "" ) );
				m.Heal( toHeal );

				m.FixedParticles( 0x376A, 9, 32, 5005, EffectLayer.Waist );
				m.PlaySound( 0x1F2 );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private HealSpell m_Owner;

			public InternalTarget( HealSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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