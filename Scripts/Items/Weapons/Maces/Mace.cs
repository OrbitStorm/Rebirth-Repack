using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF5C, 0xF5D )]
	public class Mace : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return 12; } }
		public override int AosMaxDamage{ get{ return 14; } }
		public override int AosSpeed{ get{ return 40; } }

		public override int OldStrengthReq{ get{ return 20; } }
		public override int OldSpeed{ get{ return 50; } }

		public override int NumDice { get { return 4; } }
		public override int NumSides { get { return 4; } }
		public override int DiceBonus { get { return -1; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }

		[Constructable]
		public Mace() : base( 0xF5C )
		{
			Weight = 14.0;
		}

		public Mace( Serial serial ) : base( serial )
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