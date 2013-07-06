using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Guilds;

namespace Server.Mobiles
{
	public class ChaosGuard : BaseShieldGuard
	{
		public override int Keyword{ get{ return 0x22; } } // *chaos shield*
		public override BaseShield Shield{ get{ return new ChaosShield(); } }
		public override string SignupString { get { return "If thou art interested, say \"chaos shield\" to join our ranks."; } }
		public override GuildType Type{ get{ return GuildType.Chaos; } }

		[Constructable]
		public ChaosGuard()
		{
		}

		public ChaosGuard( Serial serial ) : base( serial )
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
