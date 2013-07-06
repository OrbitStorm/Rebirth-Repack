using System;
using System.Text;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Guilds;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Engines.MyRunUO
{
	public class MyRunUO : Timer
	{
		private static double CpuInterval = 0.1; // Processor runs every 0.1 seconds
		private static double CpuPercent = 0.25; // Processor runs for 25% of Interval, or ~25ms. This should take around 25% cpu

		public static void Initialize()
		{
			if ( Config.Enabled )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( 10.0 ), Config.CharacterUpdateInterval, new TimerCallback( Begin ) );
				Commands.CommandSystem.Register( "UpdateMyRunUO", AccessLevel.Administrator, new Server.Commands.CommandEventHandler( UpdateMyRunUO_OnCommand ) );

				Commands.CommandSystem.Register( "PublicChar", AccessLevel.Player, new Server.Commands.CommandEventHandler( PublicChar_OnCommand ) );
				Commands.CommandSystem.Register( "PrivateChar", AccessLevel.Player, new Server.Commands.CommandEventHandler( PrivateChar_OnCommand ) );
			}
		}

		[Usage( "PublicChar" )]
		[Description( "Enables showing extended character stats and skills in MyRunUO." )]
		public static void PublicChar_OnCommand( Server.Commands.CommandEventArgs e )
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;

			if ( pm != null )
			{
				if ( pm.PublicMyRunUO )
				{
					pm.SendAsciiMessage( "You have already chosen to show your skills and stats." );
				}
				else
				{
					pm.PublicMyRunUO = true;
					pm.SendAsciiMessage( "All of your skills and stats will now be shown publicly in MyRunUO." );
				}
			}
		}

		[Usage( "PublicChar" )]
		[Description( "Disables showing extended character stats and skills in MyRunUO." )]
		public static void PrivateChar_OnCommand( Server.Commands.CommandEventArgs e )
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;

			if ( pm != null )
			{
				if ( !pm.PublicMyRunUO )
				{
					pm.SendAsciiMessage( "You have already chosen to not show your skills and stats." );
				}
				else
				{
					pm.PublicMyRunUO = false;
					pm.SendAsciiMessage( "Only a general level of your top three skills will be shown in MyRunUO." );
				}
			}
		}

		[Usage( "UpdateMyRunUO" )]
		[Description( "Starts the process of updating the MyRunUO character and guild database." )]
		public static void UpdateMyRunUO_OnCommand( Server.Commands.CommandEventArgs e )
		{
			if ( m_Command != null && m_Command.HasCompleted )
				m_Command = null;

			if ( m_Timer == null && m_Command == null )
			{
				Begin();
				e.Mobile.SendAsciiMessage( "MyRunUO update process has been started." );
			}
			else
			{
				e.Mobile.SendAsciiMessage( "MyRunUO database is already being updated." );
			}
		}

		public static void Begin()
		{
			if ( m_Command != null && m_Command.HasCompleted )
				m_Command = null;

			if ( m_Timer != null || m_Command != null )
				return;

			m_Timer = new MyRunUO();
			m_Timer.Start();
		}

		private static Timer m_Timer;

		private Stage m_Stage;
		private ArrayList m_List, m_Collecting;
		private int m_Index;

		private static DatabaseCommandQueue m_Command;

		private DateTime m_StartTime;

		public MyRunUO() : base( TimeSpan.FromSeconds( CpuInterval ), TimeSpan.FromSeconds( CpuInterval ) )
		{
			m_List = new ArrayList();
			m_Collecting = new ArrayList();

			m_StartTime = DateTime.Now;
			Console.WriteLine( "MyRunUO: Updating character database" );
		}

		protected override void OnTick()
		{
			bool shouldExit = false;

			try
			{
				shouldExit = Process( DateTime.Now + TimeSpan.FromSeconds( CpuInterval * CpuPercent ) );

				if ( shouldExit )
					Console.WriteLine( "MyRunUO: Database statements compiled in {0:F2} seconds", (DateTime.Now - m_StartTime).TotalSeconds );
			}
			catch ( Exception e )
			{
				Console.WriteLine( "MyRunUO: {0}: Exception cought while processing", m_Stage );
				Console.WriteLine( e );
				shouldExit = true;
			}

			if ( shouldExit )
			{
				m_Command.Enqueue( null );

				Stop();
				m_Timer = null;
			}
		}

		private enum Stage
		{
			CollectingMobiles,
			DumpingMobiles,
			CollectingGuilds,
			DumpingGuilds,
			Complete
		}

		public bool Process( DateTime endTime )
		{
			switch ( m_Stage )
			{
				case Stage.CollectingMobiles: CollectMobiles( endTime ); break;
				case Stage.DumpingMobiles: DumpMobiles( endTime ); break;
				case Stage.CollectingGuilds: CollectGuilds( endTime ); break;
				case Stage.DumpingGuilds: DumpGuilds( endTime ); break;
			}

			return ( m_Stage == Stage.Complete );
		}

		private static ArrayList m_MobilesToUpdate = new ArrayList();

		public static void QueueMobileUpdate( Mobile m )
		{
			if ( !Config.Enabled )
				return;

			m_MobilesToUpdate.Add( m );
		}

		public void CollectMobiles( DateTime endTime )
		{
			m_List = m_MobilesToUpdate;
			m_MobilesToUpdate = new ArrayList();
			m_Stage = Stage.DumpingMobiles;
			m_Index = 0;
		}

		public void CheckConnection()
		{
			if ( m_Command == null )
			{
				m_Command = new DatabaseCommandQueue( "MyRunUO: Characeter database updated in {0:F1} seconds", "MyRunUO Character Database Thread" );

				m_Command.Enqueue( String.Format( "DELETE FROM {0}", Config.GuildsTable ) );
				m_Command.Enqueue( String.Format( "DELETE FROM {0}", Config.GuildWarsTable ) );
			}
		}

		public void ExecuteNonQuery( string text )
		{
			m_Command.Enqueue( text );
		}

		public void ExecuteNonQuery( string format, params string[] args )
		{
			ExecuteNonQuery( String.Format( format, args ) );
		}

		public void ExecuteNonQueryIfNull( string select, string insert )
		{
			m_Command.Enqueue( new string[]{ select, insert } );
		}

		private void AppendCharEntity( string input, int charIndex, ref StringBuilder sb, char c )
		{
			if ( sb == null )
			{
				if ( charIndex > 0 )
					sb = new StringBuilder( input, 0, charIndex, input.Length + 20 );
				else
					sb = new StringBuilder( input.Length + 20 );
			}

			sb.Append( "&#" );
			sb.Append( (int)c );
			sb.Append( ";" );
		}

		private void AppendEntityRef( string input, int charIndex, ref StringBuilder sb, string ent )
		{
			if ( sb == null )
			{
				if ( charIndex > 0 )
					sb = new StringBuilder( input, 0, charIndex, input.Length + 20 );
				else
					sb = new StringBuilder( input.Length + 20 );
			}

			sb.Append( ent );
		}
 
		private string SafeString( string input )
		{
			if ( input == null )
				return "";

			StringBuilder sb = null;

			for ( int i = 0; i < input.Length; ++i )
			{
				char c = input[i];

				if ( c < 0x20 || c > 0x80 )
				{
					AppendCharEntity( input, i, ref sb, c );
				}
				else
				{
					switch ( c )
					{
						case '&':	AppendEntityRef( input, i, ref sb, "&amp;" ); break;
						case '>':	AppendEntityRef( input, i, ref sb, "&gt;" ); break;
						case '<':	AppendEntityRef( input, i, ref sb, "&lt;" ); break;
						case '"':	AppendEntityRef( input, i, ref sb, "&quot;" ); break;
						case '\'':
						case ':':
						case '/':
						case '\\':	AppendCharEntity( input, i, ref sb, c ); break;
						default:
						{
							if ( sb != null )
								sb.Append( c );

							break;
						}
					}
				}
			}

			if ( sb != null )
				return sb.ToString();

			return input;
		}

		public void InsertMobile( Mobile mob )
		{
			string guildTitle = mob.GuildTitle;

			if ( guildTitle == null || (guildTitle = guildTitle.Trim()).Length == 0 )
				guildTitle = "NULL";
			else
				guildTitle = "'" + SafeString( guildTitle ) + "'";

			string notoTitle = SafeString( Titles.ComputeTitle( null, mob ) );
			string female = ( mob.Female ? "1" : "0" );
			
			bool pubBool = ( mob is PlayerMobile ) && ( ((PlayerMobile)mob).PublicMyRunUO );

			string pubString = ( pubBool ? "1" : "0" );

			string guildId = ( mob.Guild == null ? "NULL" : mob.Guild.Id.ToString() );

			ExecuteNonQuery( "INSERT INTO {12} (char_id, char_name, char_str, char_dex, char_int, char_female, char_counts, char_guild, char_guildtitle, char_nototitle, char_bodyhue, char_public ) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}, {11})", mob.Serial.Value.ToString(), SafeString( mob.Name ), mob.RawStr.ToString(), mob.RawDex.ToString(), mob.RawInt.ToString(), female, mob.Kills.ToString(), guildId, guildTitle, notoTitle, mob.Hue.ToString(), pubString, Config.CharsTable );
		}

		public void InsertSkills( Mobile mob )
		{
			Skills skills = mob.Skills;
			string serial = mob.Serial.Value.ToString();

			for ( int i = 0; i < skills.Length; ++i )
			{
				Skill skill = skills[i];

				if ( skill.BaseFixedPoint > 0 )
					ExecuteNonQuery( "INSERT INTO {3} (char_id, skill_id, skill_value) VALUES ({0}, {1}, {2})", serial, i.ToString(), skill.BaseFixedPoint.ToString(), Config.SkillsTable );
			}
		}

		public void InsertItems( Mobile mob )
		{
			ArrayList items = new ArrayList( mob.Items );
			string serial = mob.Serial.Value.ToString();

			items.Sort( LayerComparer.Instance );

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = (Item)items[i];

				if ( !LayerComparer.IsValid( item ) )
					break;

				ExecuteNonQuery( "INSERT INTO {4} (char_id, layer_id, item_id, item_hue) VALUES ({0}, {1}, {2}, {3})", serial, i.ToString(), item.ItemID.ToString(), item.Hue.ToString(), Config.EquipTable );
			}
		}

		public void DeleteMobile( Mobile mob )
		{
			ExecuteNonQuery( "DELETE FROM {1} WHERE char_id = {0}", mob.Serial.Value.ToString(), Config.CharsTable );
			ExecuteNonQuery( "DELETE FROM {1} WHERE char_id = {0}", mob.Serial.Value.ToString(), Config.SkillsTable );
			ExecuteNonQuery( "DELETE FROM {1} WHERE char_id = {0}", mob.Serial.Value.ToString(), Config.EquipTable );
		}

		public void DumpMobiles( DateTime endTime )
		{
			CheckConnection();

			for ( int i = m_Index; i < m_List.Count; ++i )
			{
				Mobile mob = (Mobile)m_List[i];

				if ( mob is PlayerMobile )
					((PlayerMobile)mob).ChangedMyRunUO = false;

				DeleteMobile( mob );

				if ( !mob.Deleted && mob.AccessLevel < Config.HiddenAccessLevel )
				{
					InsertMobile( mob );
					InsertSkills( mob );
					InsertItems( mob );
				}

				++m_Index;

				if ( DateTime.Now >= endTime )
					break;
			}

			if ( m_Index == m_List.Count )
			{
				m_List.Clear();
				m_Stage = Stage.CollectingGuilds;
				m_Index = 0;
			}
		}

		public void CollectGuilds( DateTime endTime )
		{
			m_List.AddRange( Guild.List.Values );
			m_Stage = Stage.DumpingGuilds;
			m_Index = 0;
		}

		public void InsertGuild( Guild guild )
		{
			string guildType = "Standard";

			switch ( guild.Type )
			{
				case GuildType.Chaos: guildType = "Chaos"; break;
				case GuildType.Order: guildType = "Order"; break;
			}

			ExecuteNonQuery( "INSERT INTO {9} (guild_id, guild_name, guild_abbreviation, guild_website, guild_charter, guild_type, guild_wars, guild_members, guild_master) VALUES ({0}, '{1}', {2}, {3}, {4}, '{5}', {6}, {7}, {8})", guild.Id.ToString(), SafeString( guild.Name ), guild.Abbreviation == "none" ? "NULL" : "'" + SafeString( guild.Abbreviation ) + "'", guild.Website == null ? "NULL" : "'" + SafeString( guild.Website ) + "'", guild.Charter == null ? "NULL" : "'" + SafeString( guild.Charter ) + "'", guildType, guild.Enemies.Count.ToString(), guild.Members.Count.ToString(), guild.Leader.Serial.Value.ToString(), Config.GuildsTable );
		}

		public void InsertWars( Guild guild )
		{
			ArrayList wars = guild.Enemies;

			string ourId = guild.Id.ToString();

			for ( int i = 0; i < wars.Count; ++i )
			{
				Guild them = (Guild)wars[i];
				string theirId = them.Id.ToString();

				ExecuteNonQueryIfNull(
					String.Format( "SELECT guild_1 FROM {2} WHERE (guild_1={0} AND guild_2={1}) OR (guild_1={1} AND guild_2={0})", ourId, theirId, Config.GuildWarsTable ),
					String.Format( "INSERT INTO {2} (guild_1, guild_2) VALUES ({0}, {1})", ourId, theirId, Config.GuildWarsTable ) );
			}
		}

		public void DumpGuilds( DateTime endTime )
		{
			CheckConnection();

			for ( int i = m_Index; i < m_List.Count; ++i )
			{
				Guild guild = (Guild)m_List[i];

				if ( !guild.Disbanded )
				{
					InsertGuild( guild );
					InsertWars( guild );
				}

				++m_Index;

				if ( DateTime.Now >= endTime )
					break;
			}

			if ( m_Index == m_List.Count )
			{
				m_List.Clear();
				m_Stage = Stage.Complete;
				m_Index = 0;
			}
		}
	}
}