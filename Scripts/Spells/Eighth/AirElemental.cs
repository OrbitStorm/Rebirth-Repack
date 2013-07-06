using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class AirElementalSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Air Elemental", "Kal Vas Xen Hur",
				SpellCircle.Eighth,
				//269,
				//9010,
				230,
				9022,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public AirElementalSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromSeconds( 11.25 );
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
				SpellHelper.Summon( new AirElemental(), Caster, 0x215, TimeSpan.FromSeconds( 4.0 * Caster.Skills[SkillName.Magery].Value ), false, false );
			FinishSequence();
		}
	}
}