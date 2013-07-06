using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
	public class SkillCheck
	{
		private const bool AntiMacroCode = false;	

		public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes( 5.0 ); //How long do we remember targets/locations?
		public const int Allowance = 3;	//How many times may we use the same location/target for gain
		private const int LocationSize = 5; //The size of eeach location, make this smaller so players dont have to move as far
		private static bool[] UseAntiMacro = new bool[]
		{
			// true if this skill uses the anti-macro code, false if it does not
			false,// Alchemy = 0,
			true,// Anatomy = 1,
			true,// AnimalLore = 2,
			true,// ItemID = 3,
			true,// ArmsLore = 4,
			false,// Parry = 5,
			true,// Begging = 6,
			false,// Blacksmith = 7,
			false,// Fletching = 8,
			true,// Peacemaking = 9,
			true,// Camping = 10,
			false,// Carpentry = 11,
			false,// Cartography = 12,
			false,// Cooking = 13,
			true,// DetectHidden = 14,
			true,// Discordance = 15,
			true,// EvalInt = 16,
			true,// Healing = 17,
			true,// Fishing = 18,
			true,// Forensics = 19,
			true,// Herding = 20,
			true,// Hiding = 21,
			true,// Provocation = 22,
			false,// Inscribe = 23,
			true,// Lockpicking = 24,
			true,// Magery = 25,
			true,// MagicResist = 26,
			false,// Tactics = 27,
			true,// Snooping = 28,
			true,// Musicianship = 29,
			true,// Poisoning = 30,
			false,// Archery = 31,
			true,// SpiritSpeak = 32,
			true,// Stealing = 33,
			false,// Tailoring = 34,
			true,// AnimalTaming = 35,
			true,// TasteID = 36,
			false,// Tinkering = 37,
			true,// Tracking = 38,
			true,// Veterinary = 39,
			false,// Swords = 40,
			false,// Macing = 41,
			false,// Fencing = 42,
			false,// Wrestling = 43,
			true,// Lumberjacking = 44,
			true,// Mining = 45,
			true,// Meditation = 46,
			true,// Stealth = 47,
			true,// RemoveTrap = 48,
			true,// Necromancy = 49,
			false,// Focus = 50,
			true,// Chivalry = 51
		};

		private const int UsageHours = 4;
		private const int SkillCount = 46;

		private static int[][] m_HourlyUsage;
		private static int[] m_SkillUsage;
		private static int m_TotalUses;

		public static void Initialize() 
		{ 
			m_HourlyUsage = new int[UsageHours][];
			for(int i=0;i<UsageHours;i++)
				m_HourlyUsage[i] = new int[SkillCount];
			m_SkillUsage = new int[SkillCount];
			m_TotalUses = 0;

			Server.Commands.CommandSystem.Register( "SkillUsage", AccessLevel.GameMaster, new Server.Commands.CommandEventHandler( Cmd_SkillUsage ) );

			Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler( Mobile_SkillCheckLocation ); 
			Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler( Mobile_SkillCheckDirectLocation ); 

			Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler( Mobile_SkillCheckTarget ); 
			Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler( Mobile_SkillCheckDirectTarget ); 

			SkillInfo.Table[46].GainFactor = 0.00;// Meditation = 46 
			SkillInfo.Table[47].GainFactor = 0.00;// Stealth = 47 
			SkillInfo.Table[48].GainFactor = 0.00;// RemoveTrap = 48 
			SkillInfo.Table[49].GainFactor = 0.00;// Necromancy = 49 
			SkillInfo.Table[50].GainFactor = 0.00;// Focus = 50 
			SkillInfo.Table[51].GainFactor = 0.00;// Chivalry = 51 

			SkillInfo.Table[(int)SkillName.Alchemy].GainFactor = 0.80;
			SkillInfo.Table[(int)SkillName.AnimalTaming].GainFactor = 0.70;
			SkillInfo.Table[(int)SkillName.Blacksmith].GainFactor = 0.75;
			SkillInfo.Table[(int)SkillName.Magery].GainFactor = 0.85;
			SkillInfo.Table[(int)SkillName.MagicResist].GainFactor = 0.70;
			SkillInfo.Table[(int)SkillName.Stealing].GainFactor = 0.85;

			SkillInfo.Table[(int)SkillName.MagicResist].IntGain += 1.0;
			SkillInfo.Table[(int)SkillName.MagicResist].StrGain = SkillInfo.Table[(int)SkillName.MagicResist].DexGain = 0;
			SkillInfo.Table[(int)SkillName.Magery].IntGain += 0.75;

			PacketHandlers.Register( 0x3A, 0, true, new OnPacketReceive( ChangeSkillLock ) );
			PacketHandlers.RegisterExtended( 0x1A,  true, new OnPacketReceive( StatLockChange ) );
		} 

		public static void StatLockChange( NetState state, PacketReader pvSrc )
		{
			state.Mobile.SendAsciiMessage( "Stat locks cannot be changed.  Your stats will continue to gain normally.  Do NOT page a GM about this." );
			state.Send( new StatLockInfo( state.Mobile ) );
		}

		public static void ChangeSkillLock( NetState state, PacketReader pvSrc )
		{
			short skill = pvSrc.ReadInt16();
			if ( skill < 0 || skill >= state.Mobile.Skills.Length )
				return;
			Skill s = state.Mobile.Skills[skill];

			if ( s != null )
			{
				//s.SetLockNoRelay( (SkillLock)pvSrc.ReadByte() );
				state.Mobile.SendAsciiMessage( "Skill locks cannot be changed, your skills will continue to gain normally. Do NOT page a GM about this." );
				state.Send( new SkillChange( s ) ); // send them a skillchange packet so they see their skill is still LOCKED
			}
		}

		[Usage( "SkillUsage <name>" )]
		[Description( "View global usage statistics for this skill." )]
		private static void Cmd_SkillUsage( Server.Commands.CommandEventArgs args )
		{
			Mobile from = args.Mobile;
			SkillName skill;
			try
			{
				skill = (SkillName)Enum.Parse( typeof( SkillName ), args.GetString( 0 ), true );

				from.SendAsciiMessage( "Global Usage for {0} : ", skill );
				if ( m_TotalUses > 0 )
					from.SendAsciiMessage( "{0:F2}% ({1}/{2})", (((double)m_SkillUsage[(int)skill])/((double)m_TotalUses)) * 100.0, m_SkillUsage[(int)skill], m_TotalUses );
			}
			catch
			{
				from.SendAsciiMessage( "That is not a valid skill name." );
				return;
			}
		}

		private static int LastIDX = 0;
		private static void RecordUsage( Mobile from, SkillName sk )
		{
			int s = (int)sk;
			if ( s < 0 || s >= SkillCount )
				return;

			int idx = ((int)((DateTime.Now - Items.Clock.ServerStart).TotalHours))%UsageHours;
			if ( LastIDX != idx )
			{
				for(int i=0;i<SkillCount;i++)
				{
					m_SkillUsage[i] -= m_HourlyUsage[idx][i];
					m_TotalUses -= m_HourlyUsage[idx][i];
					m_HourlyUsage[idx][i] = 0;
				}
				LastIDX = idx;
			}

			m_HourlyUsage[idx][s]++;
			m_SkillUsage[s]++;
			m_TotalUses++;

			if ( from is PlayerMobile )
				((PlayerMobile)from).OnSkillUsed( sk );
		}

		public static bool Mobile_SkillCheckLocation( Mobile from, SkillName skillName, double minSkill, double maxSkill )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;
			
			RecordUsage( from, skillName );

			double value = skill.Value;

			if ( value < minSkill )
			{
				return false; // Too difficult
			}
			else if ( value >= maxSkill )
			{
				if ( !( value == maxSkill && value == skill.Cap && skill.Cap > 0 ) )
					return true; // No challenge
			}

			double chance = (value - minSkill) / (maxSkill - minSkill);

			Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
			return CheckSkill( from, skill, loc, chance );
		}

		public static bool Mobile_SkillCheckDirectLocation( Mobile from, SkillName skillName, double chance )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			RecordUsage( from, skillName );

			if ( chance < 0.0 )
			{
				return false; // Too difficult
			}
			else if ( chance >= 1.0 )
			{
				if ( !(chance == 1.0 && skill.Value == skill.Cap && skill.Cap > 0 ) )
					return true; // No challenge
			}

			Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
			return CheckSkill( from, skill, loc, chance );
		}

		public static bool Mobile_SkillCheckTarget( Mobile from, SkillName skillName, object target, double minSkill, double maxSkill )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			RecordUsage( from, skillName );

			double value = skill.Value;

			if ( value < minSkill )
			{
				return false; // Too difficult
			}
			else if ( value >= maxSkill )
			{
				if ( !( value == maxSkill && value == skill.Cap && skill.Cap > 0 ) )
					return true; // No challenge
			}

			double chance = (value - minSkill) / (maxSkill - minSkill);

			return CheckSkill( from, skill, target, chance );
		}

		public static bool Mobile_SkillCheckDirectTarget( Mobile from, SkillName skillName, object target, double chance )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			RecordUsage( from, skillName );

			if ( chance < 0.0 )
			{
				return false; // Too difficult
			}
			else if ( chance >= 1.0 )
			{
				if ( !(chance == 1.0 && from.Skills[skillName].Value == from.Skills[skillName].Cap && from.Skills[skillName].Cap > 0 ) )
					return true; // No challenge
			}

			return CheckSkill( from, skill, target, chance );
		}

		public static bool CheckSkill( Mobile from, Skill skill, object amObj, double chance )
		{
			if ( from.Skills.Cap == 0 )
				return false;

			// hunger bonus: completely full +2% 
			double bonus = (from.Hunger) / 1000.0;
			if ( bonus > 0.02 )
				bonus = 0.02;
			else if ( bonus < 0 )
				bonus = 0;
			chance += bonus;

			bool success = ( chance >= Utility.RandomDouble() );
			
			if ( chance > 1.0 )
				chance = 1.0;
			
			double gc = ((from.Skills.Cap - from.Skills.Total) / from.Skills.Cap);
			gc += 0.001 + (( skill.Cap - skill.Base ) / skill.Cap) * 0.999;
			// gc += ( -Math.Pow( (1.5*chance - 0.9), 2 ) + 0.85 ) * (success ? 1.0 : 0.5); // for c=0 this=0.04, for c=.6 this=0.85, for c=1, this=0.49
			gc += ( -Math.Pow( (1.75*chance - 0.83), 2 ) + 0.85 ) * (success ? 1.0 : 0.5); // for c=0 this=0.161, for c=.475 this=0.85, for c=1, this=0.0036
			//this gives a nearly even distribution cirve, osi most likely has a symetrical curve, which is gay.
			gc /= 3; // avg of the 3 (the highest this can be is (1+1+0.85)/3=0.95)

			gc *= skill.Info.GainFactor;

			// extra pentalty for high skill
			/* 
			if ( skill.Base >= 80.0 )
			{
				double chg = 0.666667 * (100.0 - skill.Base)/20 + 0.1;
				if ( chg < 0.225 )
					chg = 0.225;
				else if ( chg > 0.666667 )
					chg = 0.666667;

				gc *= chg;
			}
			*/

			gc /= 1.75; //was 2.5

			if ( skill.Base >= 80.0 )
				gc *= ((100.0 - (skill.Base - 80.0)) / 105.0);

			if ( m_TotalUses > 100 && (int)skill.SkillName < SkillCount )
			{
				/************************
				 * Skill formula scaled against the skill's global uses over the past 30 mins or so
				 * If the skill was used less than average ( 1/45 or 2.22% of the time ) then a
				 * gain is slightly more likely, otherwise its less likely.
				 * 
				 * So, if the skill was hardly used at all, we'll get numbers something like:
				 * diff = 0.02
				 * gc *= 1.0 + 0.02 * 5, so we'll increase the gain chance by about 10%
				 * 
				 * If the skill was used WAY disproportunently (capped at 10% of all uses):
				 * diff = -0.0783
				 * gc *= 1.0 + -0.0783 * 5, so we'll decrease the gain chance by about 38.89%
				**/
				
				double freq = m_SkillUsage[(int)skill.SkillName] / m_TotalUses;//m_LastUses[(int)skill.SkillName] / m_LastTotalUses;
				if ( freq > 0.1 )
					freq = 0.1;
				double diff = ( 1.0 / 45.0 ) - freq;
				gc *= 1.0 + (diff * 5.0);
			}
			else 
			{
				gc *= 0.9; 
			}
			
			if ( chance >= 1.0 && skill.Base == skill.Cap )
				gc = gc*1.25 + 0.01;

			if ( from.Alive && gc >= Utility.RandomDouble() && AllowGain( from, skill, amObj ) )
				Gain( from, skill );

			return success;
		}

		private static bool AllowGain( Mobile from, Skill skill, object obj )
		{
			if ( AntiMacroCode && from is PlayerMobile && UseAntiMacro[skill.Info.SkillID] )
				return ((PlayerMobile)from).AntiMacroCheck( skill, obj );
			return true;
		}
		
		private static bool CheckAtrophy( double total, double cap, double atrophyCoeff )
		{
			if ( total >= cap )
				return true;
			
			double atrophyStart = cap * atrophyCoeff;
			double chance = ( ( total - atrophyStart ) / ( cap - atrophyStart ) );
			if ( chance > 0.99 )
				chance = 0.99;
			return chance >= Utility.RandomDouble();
		}

		public enum Stat { Str=0, Dex=1, Int=2 }

		public static void Gain( Mobile from, Skill skill )
		{
			if ( from.Region is Regions.Jail )
				return;

			if ( skill.SkillName == SkillName.Focus && from is BaseCreature )
				return;

			if ( skill.Base < skill.Cap /*&& skill.Lock == SkillLock.Up*/ )
			{
				int toGain = 1;
				if ( skill.Base <= 10.0 )
					toGain = 2;

				Skills skills = from.Skills;
				if ( CheckAtrophy( skills.Total, skills.Cap, 0.6 ) )
				{
					SkillName toLower;
					if ( from is PlayerMobile )
						toLower = ((PlayerMobile)from).GetSkillToLower( skill.SkillName, toGain );
					else
						toLower = (SkillName)Utility.Random( skills.Length );

					if ( toLower == skill.SkillName || skills[toLower].BaseFixedPoint < toGain )
						toGain = 0;
					else
						skills[toLower].BaseFixedPoint -= toGain;
				}

				skill.BaseFixedPoint += toGain;
			}

			SkillInfo info = skill.Info;
			if ( /*from.StrLock == StatLockType.Up &&*/ (info.StrGain / 25.0) >= Utility.RandomDouble() )
				GainStat( from, Stat.Str );
			if ( /*from.DexLock == StatLockType.Up &&*/ (info.DexGain / 25.0) >= Utility.RandomDouble() )
				GainStat( from, Stat.Dex );
			if ( /*from.IntLock == StatLockType.Up &&*/ (info.IntGain / 25.0) >= Utility.RandomDouble() )
				GainStat( from, Stat.Int );
		}

		private static TimeSpan m_StatGainDelay = TimeSpan.Zero;
		public static void GainStat( Mobile from, Stat stat )
		{
			if ( (from.LastStatGain + m_StatGainDelay) > DateTime.Now )
				return;

			from.LastStatGain = DateTime.Now;

			IncreaseStat( from, stat, CheckAtrophy( from.RawStatTotal, from.StatCap, 0.90 ) );
		}

		public static bool CanLower( Mobile from, Stat stat )
		{
			switch ( stat )
			{
				case Stat.Str: return ( /*from.StrLock == StatLockType.Down &&*/ from.RawStr > 10 );
				case Stat.Dex: return ( /*from.DexLock == StatLockType.Down &&*/ from.RawDex > 10 );
				case Stat.Int: return ( /*from.IntLock == StatLockType.Down &&*/ from.RawInt > 10 );
			}

			return false;
		}

		public static bool CanRaise( Mobile from, Stat stat )
		{
			if ( !(from is BaseCreature && ((BaseCreature)from).Controled) )
			{
				if ( from.RawStatTotal >= from.StatCap )
					return false;
			}

			switch ( stat )
			{
				case Stat.Str: return ( /*from.StrLock == StatLockType.Up &&*/ from.RawStr < 100 );
				case Stat.Dex: return ( /*from.DexLock == StatLockType.Up &&*/ from.RawDex < 100 );
				case Stat.Int: return ( /*from.IntLock == StatLockType.Up &&*/ from.RawInt < 100 );
			}

			return false;
		}

		public static bool LowerFor( Mobile from, Stat stat )
		{
			if ( !( from is PlayerMobile ) )
				return false;

			PlayerMobile pm = (PlayerMobile)from;

			double str=0,dex=0,intel=0;
			for(int i=0;i<3;i++)
			{
				try
				{
					SkillInfo info = SkillInfo.Table[pm.SkillUsage[i]];
					str += info.StrScale;
					dex += info.DexScale;
					intel += info.IntScale;
				}
				catch
				{
				}
			}

			if ( stat == Stat.Str )
			{
				if ( dex < intel && CanLower( from, Stat.Dex ) )
					from.RawDex--;
				else if ( CanLower( from, Stat.Int ) )
					from.RawInt--;
				else
					return false;
			}
			else if ( stat == Stat.Dex )
			{
				if ( str < intel && CanLower( from, Stat.Str ) )
					from.RawStr--;
				else if ( CanLower( from, Stat.Int ) )
					from.RawInt--;
				else
					return false;
			}
			else // ( stat == Stat.Int )
			{
				if ( str < dex && CanLower( from, Stat.Str ) )
					from.RawStr--;
				else if ( CanLower( from, Stat.Dex ) )
					from.RawDex--;
				else
					return false;
			}

			return true;
		}

		/*public static bool LowerFor( Mobile from, Stat stat )
		{
			int count = 2;
			bool []chkd = new bool[3];
			chkd[(int)stat] = true;

			while ( count > 0 )
			{
				int s = Utility.Random( 3 );
				if ( !chkd[s] )
				{
					if ( CanLower( from, (Stat)s ) )
					{
						switch ( (Stat)s )
						{
							case Stat.Str: 
								--from.RawStr; return true;
							case Stat.Dex: 
								--from.RawDex; return true;
							case Stat.Int: 
								--from.RawInt; return true;
						}
					}
					chkd[s] = true;
					--count;
				}
			}

			return false;
		}*/

		public static void IncreaseStat( Mobile from, Stat stat, bool atrophy )
		{
			bool ok = true;
			switch ( stat )
			{
				case Stat.Str:
				{
					if ( from.RawStr < 100 )
					{
						if ( atrophy )
							ok = LowerFor( from, Stat.Str );
						if ( ok && CanRaise( from, Stat.Str ) )
							++from.RawStr;
					}
					break;
				}
				case Stat.Dex:
				{
					if ( from.RawDex < 100 )
					{
						if ( atrophy )
							ok = LowerFor( from, Stat.Dex );
						if ( ok && CanRaise( from, Stat.Dex ) )
							++from.RawDex;
					}
					break;
				}
				case Stat.Int:
				{
					if ( from.RawInt < 100 )
					{
						if ( atrophy )
							ok = LowerFor( from, Stat.Int );
						if ( ok && CanRaise( from, Stat.Int ) )
							++from.RawInt;
					}
					break;
				}
			}
		}
	}
}
