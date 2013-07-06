using System;
using Server;
using Server.Guilds;

namespace Server.Items
{
	public class OrderShield : VirtueShield
	{
		public override int ArmorBase{ get{ return 30; } }

		[Constructable]
		public OrderShield() : base( 0x1BC4 )
		{
			Weight = 7.0;
		}

		public OrderShield( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 6.0 )
				Weight = 7.0;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}
	}
}
