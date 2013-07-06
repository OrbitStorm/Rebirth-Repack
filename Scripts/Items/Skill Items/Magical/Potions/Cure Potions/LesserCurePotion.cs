using System;
using Server;

namespace Server.Items
{
	public class LesserCurePotion : BaseCurePotion
	{
		private static CureLevelInfo[] m_LevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  1.00 ), 
				new CureLevelInfo( Poison.Regular, 0.9875 ), 
				new CureLevelInfo( Poison.Greater, 0.8125 ), 
				new CureLevelInfo( Poison.Deadly, 0.6375 ),
				new CureLevelInfo( Poison.Lethal, 0.4625 ) 
			};

		public override CureLevelInfo[] LevelInfo{ get{ return m_LevelInfo; } }

		[Constructable]
		public LesserCurePotion() : base( PotionEffect.CureLesser )
		{
		}

		public LesserCurePotion( Serial serial ) : base( serial )
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