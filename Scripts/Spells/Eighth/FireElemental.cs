using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class FireElementalSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Fire Elemental", "Kal Vas Xen Flam",
				SpellCircle.Eighth,
				230,
				9022,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromSeconds( 11.25 );
		}

		public FireElementalSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				SpellHelper.Summon( new FireElemental(), Caster, 0x217, TimeSpan.FromSeconds( 4.0 * Caster.Skills[SkillName.Magery].Value ), false, false );
			}

			FinishSequence();
		}
	}
}