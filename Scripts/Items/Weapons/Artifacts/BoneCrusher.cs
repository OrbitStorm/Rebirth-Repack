using System;
using Server;

namespace Server.Items
{
	public class BoneCrusher : WarMace
	{
		public override int LabelNumber{ get{ return 1061596; } } // Bone Crusher
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BoneCrusher()
		{
			Hue = 0x604;
			WeaponAttributes.HitLowerDefend = 50;
			Attributes.BonusStr = 10;
			Attributes.WeaponDamage = 75;
		}

		public BoneCrusher( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}