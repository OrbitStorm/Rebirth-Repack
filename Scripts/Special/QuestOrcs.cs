using System;
using Server;

namespace Server.Mobiles
{
	public class OrcishSpirit : Orc
	{
		[Constructable]
		public OrcishSpirit()
		{
			Hue = 0x4FFF;
			Name = "an orcish spirit";
		}

		public OrcishSpirit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class UndeadOrc : OrcishMage
	{
		[Constructable]
		public UndeadOrc()
		{
			Hue = 0x047E;
			Name = "an undead orc";
		}

		public UndeadOrc( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
