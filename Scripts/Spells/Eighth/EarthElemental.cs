using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class EarthElementalSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Earth Elemental", "Kal Vas Xen Ylem",
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

		public EarthElementalSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				SpellHelper.Summon( new EarthElemental(), Caster, 0x217, TimeSpan.FromSeconds( 4.0 * Caster.Skills[SkillName.Magery].Value ), false, false );
			}

			FinishSequence();
		}
	}
}