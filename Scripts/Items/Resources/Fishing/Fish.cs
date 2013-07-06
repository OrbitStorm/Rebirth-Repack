using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class Fish : BaseItem, ICarvable
	{
		public void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), 4 );
		}

		[Constructable]
		public Fish() : base( Utility.Random( 0x09CC, 4 ) )
		{
			Weight = 1.0;
		}

		public Fish( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
