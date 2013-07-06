using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
	public class MagicReflectSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Magic Reflection", "In Jux Sanct",
			SpellCircle.Fifth,
			242,
			9012,
			Reagent.Garlic,
			Reagent.MandrakeRoot,
			Reagent.SpidersSilk
			);

		public MagicReflectSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		/*public override bool CheckCast()
		{
			if ( Core.AOS )
				return true;

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}

			return true;
		}*/

		private static Hashtable m_Table = new Hashtable();

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				if ( Caster.MagicDamageAbsorb > 0 )
				{
					DoFizzle();
				}
				else
				{
					Caster.MagicDamageAbsorb = 1;

					Caster.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );
					Caster.PlaySound( 0x1E9 );
				}
			}

			FinishSequence();
		}
	}
}
