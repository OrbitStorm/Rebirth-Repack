using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class BigFish : BaseItem, ICarvable
	{
		public void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), 100, false );
		}

		public override int LabelNumber{ get{ return 1041112; } } // a big fish

		[Constructable]
		public BigFish() : this( 1 )
		{
		}

		[Constructable]
		public BigFish( int amount ) : base( 0x09CC )
		{
			Stackable = true;
			Weight = 100.0;
			Amount = amount;
			Hue = 0x847;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BigFish( amount ), amount );
		}

		public BigFish( Serial serial ) : base( serial )
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
