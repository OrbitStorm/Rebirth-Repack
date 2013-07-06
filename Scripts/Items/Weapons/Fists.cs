using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class Fists : BaseMeleeWeapon
	{
		public static void Initialize()
		{
			Mobile.DefaultWeapon = new Fists();
		}

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Disarm; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq{ get{ return 0; } }
		public override int AosMinDamage{ get{ return 1; } }
		public override int AosMaxDamage{ get{ return 4; } }
		public override int AosSpeed{ get{ return 50; } }

		public override int OldStrengthReq{ get{ return 0; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 8; } }
		public override int OldSpeed{ get{ return 30; } }

		public override int DefHitSound{ get{ return -1; } }
		public override int DefMissSound{ get{ return -1; } }

		public override SkillName DefSkill{ get{ return SkillName.Wrestling; } }
		public override WeaponType DefType{ get{ return WeaponType.Fists; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Wrestle; } }

		public Fists() : base( 0 )
		{
			Visible = false;
			Movable = false;
			Quality = CraftQuality.Regular;
		}

		public Fists( Serial serial ) : base( serial )
		{
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			base.PlaySwingAnimation( attacker );
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

			Delete();
		}

	}
}