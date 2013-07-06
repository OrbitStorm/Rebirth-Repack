using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class WaterElementalSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Water Elemental", "Kal Vas Xen An Flam",
				SpellCircle.Eighth,
				230,
				9022,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromSeconds( 11.25 );
		}

		public WaterElementalSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				SpellHelper.Summon( new WaterElemental(), Caster, 0x217, TimeSpan.FromSeconds( 4.0 * Caster.Skills[SkillName.Magery].Value ), false, false );
			}

			FinishSequence();
		}
	}
}