using System;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Guilds;

namespace Server
{
	public enum NotoCap
	{
		None = 256,

		DreadLordLady = -125,
		EvilLordLady = -110,
		DarkLordLady = -90,
		Dastardly = -70,
		Dishonorable = -50,
		LowNeutral = -39,
		Neutral = 0,
		HighNeutral = 39,
		Honorable = 50,
		Noble = 70,
		LordLady = 90,
		NobleLordLady = 110,
		GreatLordLady = 125,
	}

	public enum Noto
	{
		Dread = -120,
		Evil = -100,
		Dark = -80,
		Dastardly = -60,
		Dishonorable = -40,
		LowNeutral = -39,
		Neutral = 0,
		HighNeutral = 39,
		Honorable = 40,
		Noble = 60,
		LordLady = 80,
		NobleLordLady = 100,
		Great = 120,
	}
}

namespace Server.Misc
{
	public class Titles
	{
		public const int MinFame = 0;
		public const int MaxFame = 15000;

		private static string[] m_NotoStrings = new string[]
			{
				"The Dread {1} {0}",
				"The Evil {1} {0}", 
				"The Dark {1} {0}", 
				"The Dastardly {0}",
				"The Dishonorable {0}", 
				"{0}", 
				"The Honorable {0}", 
				"The Noble {0}", 
				"The {1} {0}", 
				"The Noble {1} {0}", 
				"The Great {1} {0}", 
			};

		public const int MinKarma = -127;
		public const int MaxKarma =  127;

		public static void Configure()
		{
			SkillInfo.Table[(int)SkillName.DetectHidden].Title = "Ranger";
			SkillInfo.Table[(int)SkillName.Snooping].Title = "Rogue";
			SkillInfo.Table[(int)SkillName.AnimalTaming].Title = "Ranger";
			SkillInfo.Table[(int)SkillName.Parry].Title = "Shieldsfighter";
			SkillInfo.Table[(int)SkillName.Veterinary].Title = "Healer";
		}

		// old school noto system uses karma only
		public static void AlterNotoriety( Mobile m, int amount )
		{
			AlterNotoriety( m, amount, (int)NotoCap.None );
		}

		public static void AlterNotoriety( Mobile m, int amount, NotoCap cap )
		{
			AlterNotoriety( m, amount, (int)cap );
		}

		public static void AlterNotoriety( Mobile m, int amount, int cap )
		{
			PlayerMobile pm = m as PlayerMobile;
			if ( pm == null || amount == 0 )
				return;

			if ( amount > 1 )
				amount = 1;

			int newKarma = m.Karma + amount;
			if ( newKarma > MaxKarma )
				newKarma = MaxKarma;
			else if ( newKarma < MinKarma )
				newKarma = MinKarma;
			
			if ( amount > 0 )
			{
				if ( pm.NextNotoUp > DateTime.Now )
					return;
				pm.NextNotoUp = DateTime.Now + TimeSpan.FromMinutes( 15 );
			}

			if ( cap != (int)NotoCap.None )
			{
				if ( amount > 0 )
				{
					if ( newKarma > cap )
					{
						if ( m.Karma >= cap )
							return;
						newKarma = cap;
					}
				}
				else if ( amount < 0 )
				{
					if ( newKarma < cap )
					{
						if ( m.Karma <= cap )
							return;
						newKarma = cap;
					}
				}
			}

			m.Karma = newKarma;
		}

		public static Noto GetNotoLevel( int karma )
		{
			if ( karma <= -120 )
				return Noto.Dread;
			else if ( karma <= -100 )
				return Noto.Evil;
			else if ( karma <= -80 )
				return Noto.Dark;
			else if ( karma <= -60 )
				return Noto.Dastardly;
			else if ( karma <= -40 )
				return Noto.Dishonorable;
			else if ( karma <= 39 )
				return Noto.Neutral;
			else if ( karma <= 59 )
				return Noto.Honorable;
			else if ( karma <= 79 )
				return Noto.Noble;
			else if ( karma <= 99 )
				return Noto.LordLady;
			else if ( karma <= 119 )
				return Noto.NobleLordLady;
			else //if ( karma <= 127 )
				return Noto.Great;
		}

		public static string GetNotoTitle( Mobile beheld )
		{
			if ( beheld.ShowFameTitle && beheld.Player )
			{
				int karma = beheld.Karma;
				int t;

				if ( karma <= (int)Noto.Dread )
					t = 0;
				else if ( karma <= (int)Noto.Evil )
					t = 1;
				else if ( karma <= (int)Noto.Dark )
					t = 2;
				else if ( karma <= (int)Noto.Dastardly )
					t = 3;
				else if ( karma <= (int)Noto.Dishonorable )
					t = 4;
				else if ( karma < (int)Noto.Honorable )
					t = 5;
				else if ( karma < (int)Noto.Noble )
					t = 6;
				else if ( karma < (int)Noto.LordLady )
					t = 7;
				else if ( karma < (int)Noto.NobleLordLady )
					t = 8;
				else if ( karma < (int)Noto.Great )
					t = 9;
				else //if ( karma >= (int)Noto.Great )
					t = 10;

				return String.Format( m_NotoStrings[t], beheld.Name, beheld.Female ? "Lady" : "Lord" );
			}
			else
			{
				return beheld.Name;
			}
		}

		public static string ComputeTitle( Mobile beholder, Mobile beheld )
		{
			StringBuilder title = new StringBuilder( GetNotoTitle( beheld ) );
			string customTitle = beheld.Title;

			if ( customTitle != null && (customTitle = customTitle.Trim()).Length > 0 )
			{
				title.AppendFormat( " {0}", customTitle );
			}
			else if ( beheld.Player && beheld.ShowFameTitle && ( beholder == beheld || Math.Abs( beheld.Karma ) >= (int)Noto.NobleLordLady || ( beholder != null && beholder.AccessLevel > beheld.AccessLevel ) ) ) 
			{
				Skill highest = GetHighestSkill( beheld );// beheld.Skills.Highest;

				if ( highest != null )
				{
					string skillTitle = highest.Info.Title;
					string skillLevel;
					if ( highest.BaseFixedPoint >= 300 )
						skillLevel = (string)Utility.GetArrayCap( m_Levels, (highest.BaseFixedPoint - 300) / 100 );
					else
						skillLevel = m_Levels[0];

					if ( beheld.Female )
					{
						if ( skillTitle.EndsWith( "man" ) )
							skillTitle = skillTitle.Substring( 0, skillTitle.Length - 3 ) + "woman";
					}

					title.AppendFormat( ", {0} {1}", skillLevel, skillTitle );
				}
			}

			return title.ToString();
		}

		private static string[] m_Levels = new string[]
			{
				"Neophyte",
				"Novice",
				"Apprentice",
				"Journeyman",
				"Expert",
				"Adept",
				"Master",
				"Grandmaster",
				"Elder",
				"Legendary"
			};

		private static Skill GetHighestSkill( Mobile m )
		{
			Skills skills = m.Skills;

			if ( !Core.AOS )
				return skills.Highest;

			Skill highest = null;

			for ( int i = 0; i < m.Skills.Length; ++i )
			{
				Skill check = m.Skills[i];

				if ( highest == null || check.BaseFixedPoint > highest.BaseFixedPoint )
					highest = check;
				//else if ( highest != null && check.BaseFixedPoint == highest.BaseFixedPoint )
				//	highest = check;
			}

			return highest;
		}
	}
}
