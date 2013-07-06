/*using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public class BarkeepContract : BaseItem
	{
		[Constructable]
		public BarkeepContract() : base( 0x14F0 )
		{
			Name = "a barkeep contract";
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public BarkeepContract( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int)0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.AccessLevel >= AccessLevel.GameMaster )
			{
				from.SendLocalizedMessage( 503248 ); // Your godly powers allow you to place this vendor whereever you wish.

				Mobile v = new PlayerBarkeeper( from );

				v.Location = from.Location;
				v.Direction = from.Direction & Direction.Mask;
				v.Map = from.Map;

				this.Delete();
			}
			else
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house == null || !house.IsOwner( from ) )
				{
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You are not the full owner of this house." );
				}
				else
				{
					Mobile v = new PlayerBarkeeper( from );

					v.Location = from.Location;
					v.Direction = from.Direction & Direction.Mask;
					v.Map = from.Map;
						
					this.Delete();
				}
			}
		}
	}
}*/
