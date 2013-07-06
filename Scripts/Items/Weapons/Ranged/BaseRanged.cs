using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseRanged : BaseMeleeWeapon
	{
		public abstract int EffectID{ get; }
		public abstract Type AmmoType{ get; }
		public abstract Item Ammo{ get; }

		public override int DefHitSound{ get{ return 0x234; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Archery; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootXBow; } }

		protected override SkillName AccuracyMod { get{ return DefSkill; } }

		public BaseRanged( int itemID ) : base( itemID )
		{
		}

		public BaseRanged( Serial serial ) : base( serial )
		{
		}

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			// Make sure we've been standing still for one second
			if ( attacker.HarmfulCheck( defender ) )
			{
				attacker.DisruptiveAction();
				attacker.Send( new Swing( 0, attacker, defender ) );

				if ( OnFired( attacker, defender ) )
				{
					if ( CheckHit( attacker, defender ) )
						OnHit( attacker, defender );
					else
						OnMiss( attacker, defender );
				}
			}

			return GetDelay( attacker );
		}

		public override bool CheckHit( Mobile attacker, Mobile defender )
		{
			BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
			BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

			Skill atkSkill = attacker.Skills[atkWeapon.Skill];
			Skill defSkill = defender.Skills[defWeapon.Skill];

			double atkValue = atkWeapon.GetAttackSkillValue( attacker, defender );
			double defValue = defWeapon.GetDefendSkillValue( attacker, defender );
			if ( defValue == -50.0 )
				defValue = -49.9;
			
			// chance = ( attacker_ability + 50 ) / ( ( defender_ability + 50 ) * 2 )
			double chance = (atkValue + 50.0) / ((defValue + 50.0) * 2.0);
			
			chance *= 1.0 + ((double)GetHitChanceBonus() / 100.0);

			WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );
			if ( ability != null )
				chance *= ability.AccuracyScalar;

			if ( attacker.LastMoveTime+TimeSpan.FromSeconds( 0.4 ) >= DateTime.Now )
			{
				// walking -5%, running -10%
				chance -= 0.05;
				if ( (attacker.Direction&Direction.Running) != 0 )
					chance -= 0.05;
			}

			// moving target?
			//if ( (defender.Direction&Direction.Running) != 0 )
			//	chance -= 0.05;
			
			attacker.CheckSkill( atkSkill.SkillName, defSkill.Value-30.0, defSkill.Value+25.0 );
			return chance >= Utility.RandomDouble();//;attacker.CheckSkill( atkSkill.SkillName, chance );
		}

		public override void OnHit( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player && !defender.Player && defender.Backpack != null && (defender.Body.IsAnimal || defender.Body.IsMonster) && Utility.Random( 3 ) == 0 )
				defender.AddToBackpack( Ammo );

			base.OnHit( attacker, defender );
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player && Utility.Random( 3 ) == 0 )
				Ammo.MoveToWorld( new Point3D( defender.X + Utility.RandomMinMax( -1, 1 ), defender.Y + Utility.RandomMinMax( -1, 1 ), defender.Z ), defender.Map );

			base.OnMiss( attacker, defender );
		}

		public virtual bool OnFired( Mobile attacker, Mobile defender )
		{
			Container pack = attacker.Backpack;

			if ( attacker.Player && (pack == null || !pack.ConsumeTotal( AmmoType, 1 )) )
				return false;

			attacker.MovingEffect( defender, EffectID, 18, 1, false, false );

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				case 1:
				{
					break;
				}
				case 0:
				{
					/*m_EffectID =*/ reader.ReadInt();
					break;
				}
			}

			if ( version < 2 )
			{
				WeaponAttributes.MageWeapon = 0;
				WeaponAttributes.UseBestSkill = 0;
			}
		}
	}
}
