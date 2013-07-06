using System;
using Server;

namespace Server.Items
{
	public class GreaterCurePotion : BaseCurePotion
	{
		private static CureLevelInfo[] m_LevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  1.00 ), // 100% chance to cure lesser poison
				new CureLevelInfo( Poison.Regular, 1.00 ), // 100% chance to cure regular poison
				new CureLevelInfo( Poison.Greater, 1.00 ), // 100% chance to cure greater poison
				new CureLevelInfo( Poison.Deadly,  1.00 ), // 100% chance to cure deadly poison
				new CureLevelInfo( Poison.Lethal,  0.875 )  // 87.5% chance to cure lethal poison
			};

		public override CureLevelInfo[] LevelInfo{ get{ return m_LevelInfo; } }

		[Constructable]
		public GreaterCurePotion() : base( PotionEffect.CureGreater )
		{
		}

		public GreaterCurePotion( Serial serial ) : base( serial )
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