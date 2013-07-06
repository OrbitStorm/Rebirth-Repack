using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Guilds;

namespace Server.Mobiles
{
	public class OrderGuard : BaseShieldGuard
	{
		public override int Keyword{ get{ return 0x21; } } // *order shield*
		public override BaseShield Shield{ get{ return new OrderShield(); } }
		public override string SignupString { get { return "If thou art interested, say \"order shield\" to join our ranks."; } }
		public override GuildType Type{ get{ return GuildType.Order; } }

		[Constructable]
		public OrderGuard()
		{
		}

		public OrderGuard( Serial serial ) : base( serial )
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