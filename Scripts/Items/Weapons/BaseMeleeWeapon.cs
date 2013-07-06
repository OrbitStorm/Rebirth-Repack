using System;
using Server;
using Server.Items;
using Server.Engines.Harvest;

namespace Server.Items
{
	public abstract class BaseMeleeWeapon : BaseWeapon
	{
		public BaseMeleeWeapon( int itemID ) : base( itemID )
		{
		}

		public BaseMeleeWeapon( Serial serial ) : base( serial )
		{
		}

		public override int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
		{
			damage = base.AbsorbDamage( attacker, defender, damage );

			if ( defender.MeleeDamageAbsorb > 0 && attacker.GetDistanceToSqrt( defender ) <= 1 )
			{
				double absorb = (double)(damage * defender.MeleeDamageAbsorb) / 100.0;
				if ( absorb > damage )
					absorb = damage;
				
				attacker.PlaySound( 0x1F1 );
				attacker.FixedEffect( 0x374A, 10, 16 );

				if ( absorb >= 1 )
				{
					attacker.Damage( ((int)absorb + 1) / 2 ); // since damage is havled before its applied... halve it here too
					damage -= (int)absorb;
				}
			}

			return damage;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}