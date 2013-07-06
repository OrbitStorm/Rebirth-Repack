using System;
using System.Threading;

namespace Server.Engines.MyRunUO
{
	public class Config
	{
		// Is MyRunUO enabled?
		public static bool Enabled = false;

		// Details required for database connection string
		public static string DatabaseDriver			= "{MySQL ODBC 3.51 Driver}";
		public static string DatabaseServer			= "localhost";
		public static string DatabaseName			= "MyRunUO";
		public static string DatabaseUserID			= "username";
		public static string DatabasePassword		= "password";

		// Should the database use transactions? This is recommended
		public static bool UseTransactions = true;

		// Database communication is done in a seperate thread. This value is the 'priority' of that thread, or, how much CPU it will try to use
		public static ThreadPriority DatabaseThreadPriority = ThreadPriority.BelowNormal;

		// Any character with an AccessLevel equal to or higher than this will not be displayed
		public static AccessLevel HiddenAccessLevel	= AccessLevel.Counselor;

		// Export character database every 30 minutes
		public static TimeSpan CharacterUpdateInterval = TimeSpan.FromMinutes( 60.0 );

		// Export online list database every 5 minutes
		public static TimeSpan StatusUpdateInterval = TimeSpan.FromMinutes( 15.0 );

		// Names of your database tables
		public const string CharsTable	= "myruo_characters";
		public const string SkillsTable	= "myruo_skills";
		public const string EquipTable	= "myruo_equip";
		public const string GuildsTable	= "myruo_guilds";
		public const string GuildWarsTable="myruo_guild_wars";
		public const string StatusTable	= "myruo_status";

		public static string CompileConnectionString()
		{
			//string connectionString = String.Format( "DRIVER={0};SERVER={1};DATABASE={2};UID={3};PASSWORD={4};",
			//	DatabaseDriver, DatabaseServer, DatabaseName, DatabaseUserID, DatabasePassword );

			return "DSN=uogr_db";//connectionString;
		}
	}
}