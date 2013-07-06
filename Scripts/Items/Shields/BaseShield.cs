using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Items
{
	public class BaseShield : BaseArmor
	{
		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public BaseShield( int itemID ) : base( itemID )
		{
		}

		public BaseShield( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override double UnscaledArmorRating
		{
			get
			{
				Mobile m = this.Parent as Mobile;
				double ar = base.UnscaledArmorRating;

				if ( m != null )
				{
					double val = ( ar * m.Skills[SkillName.Parry].Value ) / 200.0 ;
					if ( val > ar / 2.0 )
						val = ar / 2.0;
					return val;
				}
				else
				{
					return ar;
				}
			}
		}

		public override int OnHit( BaseWeapon weapon, int damage )
		{
			Mobile owner = this.Parent as Mobile;
			if ( owner == null )
				return damage;

			Mobile attacker = weapon.Parent as Mobile;
			if ( Utility.Random( 3 ) == 0 && ( attacker == owner.Combatant || attacker == null ) )
				owner.CheckSkill( SkillName.Parry, 0.0, 100.0 );

			bool ranged = weapon is BaseRanged;

			if ( Utility.Random( ranged ? 400 : 200 ) < owner.Skills.Parry.Value )
			{
				damage -= Utility.RandomMinMax( (int)UnscaledArmorRating / 2, (int)UnscaledArmorRating ) / ( ranged ? 4 : 2 );

				owner.FixedEffect( 0x37B9, 10, 16 );

				if ( Utility.Random( 5 ) == 0 )
					HitPoints --;
			}

			return damage;
		}
	}
}
