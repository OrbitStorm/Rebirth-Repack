using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Regions;

namespace Server.Misc
{
	public class NotorietyHandlers
	{
		public static void Initialize()
		{
			Notoriety.Hues[Notoriety.Innocent]		= 0x59;
			Notoriety.Hues[Notoriety.Ally]			= 0x3F;
			Notoriety.Hues[Notoriety.CanBeAttacked]	= 0x3B2;
			Notoriety.Hues[Notoriety.Criminal]		= 0x3B2;
			Notoriety.Hues[Notoriety.Enemy]			= 0x90;
			Notoriety.Hues[Notoriety.Murderer]		= 0x22;
			Notoriety.Hues[Notoriety.Invulnerable]	= 0x35;

			Notoriety.Handler = new NotorietyHandler( MobileNotoriety );

			Mobile.AllowBeneficialHandler = new AllowBeneficialHandler( Mobile_AllowBeneficial );
			Mobile.AllowHarmfulHandler = new AllowHarmfulHandler( Mobile_AllowHarmful );
		}

		private enum GuildStatus{ None, Peaceful, Waring }

		private static GuildStatus GetGuildStatus( Mobile m )
		{
			if ( m.Guild == null )
				return GuildStatus.None;
			else if ( ((Guild)m.Guild).Enemies.Count == 0 && m.Guild.Type == GuildType.Regular )
				return GuildStatus.Peaceful;

			return GuildStatus.Waring;
		}

		private static bool CheckBeneficialStatus( GuildStatus from, GuildStatus target )
		{
			if ( from == GuildStatus.Waring || target == GuildStatus.Waring )
				return false;

			return true;
		}

		public static bool Mobile_AllowBeneficial( Mobile from, Mobile target )
		{
			// no restrictions in felucca/pre-uor
			return true;
		}

		public static bool Mobile_AllowHarmful( Mobile from, Mobile target )
		{
			// no restrictions in felucca/pre-uor
			return true;
		}

		public static Guild GetGuildFor( BaseGuild def, Mobile m )
		{
			Guild g = def as Guild;

			BaseCreature c = m as BaseCreature;
			if ( c != null )
			{
				if ( c.Controled && c.ControlMaster != null )
				{
					c.DisplayGuildTitle = false;

					if ( c.Map != Map.Internal )
						c.Guild = g = c.ControlMaster.Guild as Guild;
					else 
						c.Guild = g = null;
				}
				else if ( c.Summoned && c.SummonMaster != null )
				{
					c.DisplayGuildTitle = false;

					if ( c.Map != Map.Internal )
						c.Guild = g = c.SummonMaster.Guild as Guild;
					else 
						c.Guild = g = null;
				}
			}

			return g;
		}

		public static int CorpseNotoriety( Mobile source, Corpse target )
		{
			// in pre-t2a, looting was not a crime... all bodies were grey
			return Notoriety.CanBeAttacked;
		}

		public static int MobileNotoriety( Mobile source, Mobile target )
		{
			return MobileNotoriety( source, target, 2 );
		}

		private static int MobileNotoriety( Mobile source, Mobile target, int recurse )
		{
			if ( target.AccessLevel > AccessLevel.Player )
				return Notoriety.CanBeAttacked;

			if ( target is BaseCreature && !((BaseCreature)target).Commandable && ((BaseCreature)target).AlwaysMurderer )
				return Notoriety.Murderer;

			// guild check first ( should guild colors override murderer? if not, move this down )
			Guild sourceGuild = GetGuildFor( source.Guild, source );
			Guild targetGuild = GetGuildFor( target.Guild, target );
			if ( sourceGuild != null && targetGuild != null && ( source != target || ( source.Karma >= (int)Noto.LowNeutral && !source.Criminal ) ) )
			{
				if ( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
					return Notoriety.Ally;
				else if ( sourceGuild.IsEnemy( targetGuild ) )
					return Notoriety.Enemy;
			}

			// red players
			if ( target.Player && target.Karma <= (int)Noto.Dastardly )
			{
				if ( source.Karma+15 < target.Karma )
					return Notoriety.CanBeAttacked; // dreads see other reds as grey unless lower than them
				else
					return Notoriety.Murderer;
			}

			// npcs are checked for criminal later (so that red npcs (monsters, etc) are always red)
			if ( target.Player && target.Criminal )
				return Notoriety.CanBeAttacked;
			
			// all npc/pet stuff here
			if ( target is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)target;

				if ( bc.Summoned && bc.Commandable && bc.SummonMaster != null )
				{
					// summons always have owner noto
					return MobileNotoriety( source, bc.SummonMaster, recurse-1 );
				}
				else if ( bc.AlwaysMurderer )
				{
					// if not summoned, evil thigns are evil
					return Notoriety.Murderer;
				}
				else if ( bc.Controled || bc.Summoned )
				{
					//guarding  = grey
					if ( bc.ControlOrder == OrderType.Guard && bc.ControlTarget == source )
					{
						return Notoriety.CanBeAttacked;
					}
					else
					{
						// aggressor = grey, otherwise use owner noto (fall back to ban be attacked)
						if ( CheckAggressor( source.Aggressors, target ) || CheckAggressed( source.Aggressed, target ) )
							return Notoriety.CanBeAttacked;
						else if ( recurse > 0 && ( bc.ControlMaster != null || bc.SummonMaster != null ) )
							return MobileNotoriety( source, bc.ControlMaster != null ? bc.ControlMaster : bc.SummonMaster, recurse-1 );
						else
							return Notoriety.CanBeAttacked;
					}
				}
				else if ( source is PlayerMobile && ((PlayerMobile)source).EnemyOfOneType == target.GetType() )
				{
					// not used
					return Notoriety.Enemy;
				}
				else if ( bc.AlwaysAttackable )
				{
					// if all else fails...
					return Notoriety.CanBeAttacked;
				}
				else if ( target.Body.IsHuman )
				{
					// humans are usually innocenet unless criminal or aggressed
					if ( target is BaseVendor && ((Region)target.Region).Name == "Buccaneer's Den" && !bc.Controled )
						return Notoriety.Murderer; // all npcs are RED in buc's
					else if ( target.Criminal || CheckAggressor( source.Aggressors, target ) || CheckAggressed( source.Aggressed, target ) )
						return Notoriety.CanBeAttacked;
					else
						return Notoriety.Innocent; 
				}

				// if they matched nothing here they are probably an animal or monster ( handled below )
			}

			// polymorphed players and animals are grey, monsters are red
			if ( !target.Body.IsHuman && !target.Body.IsGhost )
			{
				if ( target.Player || target.Body.IsAnimal )
					return Notoriety.CanBeAttacked;
				else
					return Notoriety.Murderer;
			}

			// aggressor check
			if ( CheckAggressor( source.Aggressors, target ) || CheckAggressed( source.Aggressed, target ) )
				return Notoriety.CanBeAttacked;

			// regular old noto
			if ( target.Karma <= (int)Noto.Dishonorable )
				return Notoriety.CanBeAttacked;
			else 
				return Notoriety.Innocent;
		}

		public static bool CheckAggressor( List<AggressorInfo> list, Mobile target )
		{
			for ( int i = 0; i < list.Count; ++i )
				if ( ((AggressorInfo)list[i]).Attacker == target )
					return true;

			return false;
		}

        public static bool CheckAggressed(List<AggressorInfo> list, Mobile target)
		{
			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = (AggressorInfo)list[i];

				if ( !info.CriminalAggression && info.Defender == target )
					return true;
			}

			return false;
		}
	}
}

