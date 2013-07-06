using System;

namespace Server.Items
{
	public class PowerCrystal : BaseItem
	{
		[Constructable]
		public PowerCrystal() : base( 0x1F1C )
		{
			Weight = 1.0;
			Name = "power crystal";
		}

		public PowerCrystal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}