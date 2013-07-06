using System;
using Server;
using Server.Network;

namespace Server.Misc
{
	public class Paperdoll
	{
		public static void Initialize()
		{
			EventSink.PaperdollRequest += new PaperdollRequestEventHandler( EventSink_PaperdollRequest );
		}

		public static void EventSink_PaperdollRequest( PaperdollRequestEventArgs e )
		{
			e.Beholder.Send( new DisplayPaperdoll( e.Beheld, Titles.ComputeTitle( e.Beholder, e.Beheld ), e.Beheld.AllowEquipFrom( e.Beholder ) ) );
		}
	}
}