using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
	public class GuildChangeTypeGump : Gump
	{
		private Mobile m_Mobile;
		private Guild m_Guild;

		public GuildChangeTypeGump( Mobile from, Guild guild ) : base( 20, 30 )
		{
			m_Mobile = from;
			m_Guild = guild;

			Dragable = false;

			AddPage( 0 );
			AddBackground( 0, 0, 550, 400, 5054 );
			AddBackground( 10, 10, 530, 380, 3000 );

			AddHtmlLocalized( 20, 15, 510, 30, 1013062, false, false ); // <center>Change Guild Type Menu</center>

			AddHtmlLocalized( 50, 50, 450, 30, 1013066, false, false ); // Please select the type of guild you would like to change to

			AddButton( 20, 100, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 85, 100, 300, 30, 1013063, false, false ); // Standard guild

			AddButton( 20, 150, 4005, 4007, 2, GumpButtonType.Reply, 0 );
			AddItem( 50, 143, 7109 );
			AddHtmlLocalized( 85, 150, 300, 300, 1013064, false, false ); // Order guild

			AddButton( 20, 200, 4005, 4007, 3, GumpButtonType.Reply, 0 );
			AddItem( 45, 200, 7107 );
			AddHtmlLocalized( 85, 200, 300, 300, 1013065, false, false ); // Chaos guild

			AddButton( 300, 360, 4005, 4007, 4, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 335, 360, 150, 30, 1011012, false, false ); // CANCEL
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( GuildGump.BadLeader( m_Mobile, m_Guild ) )
				return;

			if ( m_Guild.TypeLastChange.AddDays( 7 ) > DateTime.Now )
			{
				m_Mobile.SendLocalizedMessage( 1005292 ); // Your guild type will be changed in one week.
			}
			else
			{
				GuildType newType;

				switch ( info.ButtonID )
				{
					default: return; // Close
					case 1: newType = GuildType.Regular; break;
					case 2: newType = GuildType.Order;   break;
					case 3: newType = GuildType.Chaos;   break;
				}

				if ( m_Guild.Type == newType )
					return;

				if ( newType != GuildType.Regular )
				{
					ArrayList remove = new ArrayList();

					for(int i=0;i<m_Guild.Members.Count;i++)
					{
						if ( ((Mobile)m_Guild.Members[i]).Karma < (int)Noto.LordLady )
							remove.Add( m_Guild.Members[i] );
					}

					if ( remove.Count >= m_Guild.Members.Count || remove.Contains( m_Guild.Leader ) )
					{
						m_Mobile.SendAsciiMessage( "The effects on your guild's membership would be far too great." );
						return;
					}
					else
					{
						for(int i=0;i<remove.Count;i++)
						{
							Mobile m = (Mobile)remove[i];
							m_Guild.RemoveMember( m );
							m.SendAsciiMessage( "You are not famous enough to stay in your guild." );
						}
					}
				}

				m_Guild.Type = newType;
				m_Guild.GuildMessage( 1018022, newType.ToString() ); // Guild Message: Your guild type has changed:
			}

			GuildGump.EnsureClosed( m_Mobile );
			m_Mobile.SendGump( new GuildmasterGump( m_Mobile, m_Guild ) );
		}
	}
}
