using System;
using Server;
using Server.Network;

namespace Server.ContextMenus
{
	public class DisableContextMenu
	{
		public static void Configure()
		{
			PacketHandlers.RemoveExtendedHandler( 0x13 );
			PacketHandlers.RemoveExtendedHandler( 0x15 );

			OnPacketReceive handler = new OnPacketReceive( EmptyHandler );
			PacketHandlers.RegisterExtended( 0x13,  true, handler );
			PacketHandlers.RegisterExtended( 0x15,  true, handler );
		}

		public static void EmptyHandler( NetState state, PacketReader pvSrc )
		{
		}
	}
}

