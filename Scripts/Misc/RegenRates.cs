using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Mobiles;

namespace Server.Misc
{
	public class RegenRates
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			Mobile.DefaultHitsRate = TimeSpan.FromSeconds( 11.0 );
			Mobile.DefaultStamRate = TimeSpan.FromSeconds(  5.0 );
			Mobile.DefaultManaRate = TimeSpan.FromSeconds(  2.0 );

			Mobile.StamRegenRateHandler = new RegenRateHandler( Mobile_StaminaRegenRate );
			Mobile.HitsRegenRateHandler = new RegenRateHandler( Mobile_HitsRegenRate );
		}

		private static TimeSpan Mobile_StaminaRegenRate( Mobile from )
		{
			if ( from is BaseMount )
				return TimeSpan.FromSeconds( 2.5 );
			else
				return Mobile.DefaultStamRate;
		}

		private static TimeSpan Mobile_HitsRegenRate( Mobile from )
		{
			double scale = 1.0 - ( from.Hunger / 40.0 );
			if ( scale < 0.5 )
				scale = 0.5;
			else if ( scale > 1.0 )
				scale = 1.0;
			return TimeSpan.FromSeconds( Mobile.DefaultHitsRate.TotalSeconds * scale );
		}

		/*private static TimeSpan Mobile_ManaRegenRate( Mobile from )
		{
			if ( from.Skills == null )
				return Mobile.DefaultManaRate;

			if ( !from.Meditating )
				CheckBonusSkill( from, from.Mana, from.ManaMax, SkillName.Meditation );

			double rate;
			double armorPenalty = GetArmorOffset( from );
			double medPoints = (from.Int + from.Skills[SkillName.Meditation].Value) * 0.5;

			if ( medPoints <= 0 )
				rate = 7.0;
			else if ( medPoints <= 100 )
				rate = 7.0 - (239*medPoints/2400) + (19*medPoints*medPoints/48000);
			else if ( medPoints < 120 )
				rate = 1.0;
			else
				rate = 0.75;

			rate += armorPenalty;

			if ( from.Meditating )
				rate *= 0.5;

			if ( rate < 0.5 )
				rate = 0.5;
			else if ( rate > 7.0 )
				rate = 7.0;
			

			return TimeSpan.FromSeconds( rate );
		}*/
	}
}