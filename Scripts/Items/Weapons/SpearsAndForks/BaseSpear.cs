using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public abstract class BaseSpear : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x23C; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType DefType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce2H; } }

		public BaseSpear( int itemID ) : base( itemID )
		{
		}

		public BaseSpear( Serial serial ) : base( serial )
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

		public override void OnHit( Mobile attacker, Mobile defender )
		{
			base.OnHit( attacker, defender );

			if ( !Core.AOS && Poison != null && PoisonCharges > 0 && !defender.Poisoned )
			{
				--PoisonCharges;

				if ( this is WarFork && Utility.RandomDouble() < PoisonChance ) 
				{
					defender.SayTo( defender,  true, "{0} has just poisoned you!", attacker.Name );
					defender.ApplyPoison( attacker, Poison );
				}
			}
		}
	}
}
