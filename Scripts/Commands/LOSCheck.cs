using System;
using Server;
using Server.Items;
using Server.Network;
using System.Collections; using System.Collections.Generic;

namespace Server.Misc
{
	public class LOSCheck
	{
		public static void Initialize()
		{
			Server.Commands.CommandSystem.Register( "CheckLos", AccessLevel.GameMaster, new Server.Commands.CommandEventHandler( CheckLos_OnCommand ) );
			Server.Commands.CommandSystem.Register( "ClearLos", AccessLevel.GameMaster, new Server.Commands.CommandEventHandler( ClearLOS_OnCommand ) );
		}

		public static void ClearLOS_OnCommand( Server.Commands.CommandEventArgs args )
		{
			for(int i=0x7FFF0000;i<0x7FFFFE00;i++)
				args.Mobile.Send( new RemoveLOSPacket( i ) ); 
		}

		public static void CheckLos_OnCommand( Server.Commands.CommandEventArgs args )
		{
			try
			{
				DateTime start = DateTime.Now;

				ClearLOS_OnCommand( args );

				LOSItemPacket.Reset();

				Mobile m = args.Mobile;
				Map map = m.Map;
				int range = 12;
				if ( args.Length > 0 )
					range = args.GetInt32( 0 );

				if ( range > 20 )
					range = 20;
				else if ( range < 1 )
					range = 1;

				m.SendMessage( "Calculating..." );
				int minx, maxx;
				int miny, maxy;

				Point3D from = map.GetPoint( m, true );

				minx = m.Location.X - range;
				maxx = m.Location.X + range;

				miny = m.Location.Y - range;
				maxy = m.Location.Y + range;

				for(int x=minx;x<=maxx;x++)
				{
					for (int y=miny;y<=maxy;y++)
                    {
                        {
                            LandTile tile = map.Tiles.GetLandTile(x, y);

                            ItemData id = TileData.ItemTable[tile.ID & 0x3FFF];

                            if (!tile.Ignored && id.Surface)
                            {
                                int z = tile.Z + id.CalcHeight;
                                Point3D loc = new Point3D(x, y, z);
                                int hue;
                                if (loc == m.Location)
                                    hue = 0x35; // yellow
                                else if (map.LineOfSight(from, loc))
                                    hue = 0x3F; // green
                                else
                                    hue = 0x26; // red
                                m.Send(new LOSItemPacket(hue, loc));
                            }
                        }

                        StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, true);

                        for (int i = 0; i < tiles.Length; i++)
                        {
                            StaticTile tile = (StaticTile)tiles[i];

                            ItemData id = TileData.ItemTable[tile.ID & 0x3FFF];

                            if (id.Surface) // land doesnt use same flags, so just allow it
                            {
                                int z = tile.Z + id.CalcHeight;
                                Point3D loc = new Point3D(x, y, z);
                                int hue;
                                if (loc == m.Location)
                                    hue = 0x35; // yellow
                                else if (map.LineOfSight(from, loc))
                                    hue = 0x3F; // green
                                else
                                    hue = 0x26; // red
                                m.Send(new LOSItemPacket(hue, loc));
                            }
                        }
                    }
				}
				m.SendAsciiMessage( "Completed LOS check in {0}s", (DateTime.Now-start).TotalSeconds );
			}
			catch ( Exception e )
			{
				args.Mobile.SendMessage( "CRASHED : {0}", e.Message );
			}
		}

		private class RemoveLOSPacket : Packet
		{
			public RemoveLOSPacket( int serial ) : base( 0x1D, 5 )
			{
				m_Stream.Write( serial );
			}
		}

		private class LOSItemPacket : Packet
		{
			private static ushort m_Serial = 0;
			public static void Reset() { m_Serial = 0; }

			public LOSItemPacket( int hue, Point3D loc ) : base( 0x1A )
			{
				this.EnsureCapacity( 14+2 );

				// 14 base length
				// +2 - Amount
				// +2 - Hue
				// +1 - Flags

				m_Stream.Write( (uint)( 0x7FFF0000 | m_Serial++ ) );
				m_Stream.Write( (short) 0x51D );

				m_Stream.Write( (short)( loc.X & 0x7FFF ) );
				m_Stream.Write( (short)( (loc.Y & 0x3FFF)|0x8000 ) );
				m_Stream.Write( (sbyte)( loc.Z ) );
				m_Stream.Write( (short)( hue ) ); 
			}
		}
	}
}
