using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Fifth
{
	public class SummonCreatureSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Summon Creature", "Kal Xen",
				SpellCircle.Fifth,
				230,
				9022,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public SummonCreatureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static Type[] m_Types = new Type[]
			{
				typeof( PolarBear ),
				typeof( BlackBear ),
				typeof( BrownBear ),
				typeof( Horse ),
				typeof( Walrus ),
				typeof( Hind ),
				typeof( GreatHart ),
				typeof( Dog ),
				typeof( Chicken ),
				typeof( Rabbit ),
				typeof( Cougar ),
				typeof( Cat ),
				typeof( Eagle ),
			};

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				try
				{
					BaseCreature creature = (BaseCreature)Activator.CreateInstance( m_Types[Utility.Random( m_Types.Length )] );

					SpellHelper.Summon( creature, Caster, 0x215, TimeSpan.FromSeconds( 4.0 * Caster.Skills[SkillName.Magery].Value ), false, false );
				}
				catch
				{
				}
			}

			FinishSequence();
		}

		public override TimeSpan GetCastDelay()
		{
			if ( Core.AOS )
				return base.GetCastDelay();

			return TimeSpan.FromSeconds( 6.25 );
		}
	}
}
