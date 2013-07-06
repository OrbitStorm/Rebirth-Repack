using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1439, 0x1438 )]
	public class WarHammer : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.CrushingBlow; } }

		public override int AosStrengthReq{ get{ return 95; } }
		public override int AosMinDamage{ get{ return 17; } }
		public override int AosMaxDamage{ get{ return 18; } }
		public override int AosSpeed{ get{ return 28; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int OldSpeed{ get{ return 20; } }

		public override int NumDice { get { return 4; } }
		public override int NumSides { get { return 9; } }
		public override int DiceBonus { get { return 0; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash2H; } }

		[Constructable]
		public WarHammer() : base( 0x1439 )
		{
			Weight = 10.0;
			Layer = Layer.TwoHanded;
		}

		public WarHammer( Serial serial ) : base( serial )
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