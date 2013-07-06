using System;
using Server;

namespace Server.Items
{
	public enum PotionEffect
	{
		Nightsight,
		CureLesser,
		Cure,
		CureGreater,
		Agility,
		AgilityGreater,
		Strength,
		StrengthGreater,
		PoisonLesser,
		Poison,
		PoisonGreater,
		PoisonDeadly,
		Refresh,
		RefreshTotal,
		HealLesser,
		Heal,
		HealGreater,
		ExplosionLesser,
		Explosion,
		ExplosionGreater
	}

	public abstract class BasePotion : BaseItem
	{
		private PotionEffect m_PotionEffect;

		public PotionEffect PotionEffect
		{
			get
			{
				return m_PotionEffect;
			}
			set
			{
				m_PotionEffect = value;
				InvalidateProperties();
			}
		}

		public override int LabelNumber{ get{ return 1041314 + (int)m_PotionEffect; } }

		public BasePotion( int itemID, PotionEffect effect ) : base( itemID )
		{
			m_PotionEffect = effect;

			Stackable = false;
			Weight = 1.0;
		}

		public BasePotion( Serial serial ) : base( serial )
		{
		}

		public virtual bool RequireFreeHand{ get{ return true; } }

		public static bool HasFreeHand( Mobile m )
		{
			Item handOne = m.FindItemOnLayer( Layer.OneHanded );
			Item handTwo = m.FindItemOnLayer( Layer.TwoHanded );

			if ( handTwo is BaseWeapon && !(handTwo is BaseRanged)  )
				handOne = handTwo;

			return ( handOne == null || handTwo == null );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if ( !RequireFreeHand || HasFreeHand( from ) )
					Drink( from );
				else
					from.SendLocalizedMessage( 502172 ); // You must have a free hand to drink a potion.
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_PotionEffect );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_PotionEffect = (PotionEffect)reader.ReadInt();
					break;
				}
			}
		}

		public abstract void Drink( Mobile from );

		public static void PlayDrinkEffect( Mobile m )
		{
			m.RevealingAction();

			m.PlaySound( 48 + Utility.Random( 2 ) );
			//m.AddToBackpack( new Bottle() );
			new Bottle().MoveToWorld( m.Location, m.Map );

			if ( m.Body.IsHuman /*&& !m.Mounted*/ )
				m.Animate( 34, 5, 1, true, false, 0 );
		}

		public static TimeSpan Scale( Mobile m, TimeSpan v )
		{
			if ( !Core.AOS )
				return v;

			double scalar = 1.0 + (0.01 * AosAttributes.GetValue( m, AosAttribute.EnhancePotions ));

			return TimeSpan.FromSeconds( v.TotalSeconds * scalar );
		}

		public static double Scale( Mobile m, double v )
		{
			if ( !Core.AOS )
				return v;

			double scalar = 1.0 + (0.01 * AosAttributes.GetValue( m, AosAttribute.EnhancePotions ));

			return v * scalar;
		}

		public static int Scale( Mobile m, int v )
		{
			if ( !Core.AOS )
				return v;

			return AOS.Scale( v, 100 + AosAttributes.GetValue( m, AosAttribute.EnhancePotions ) );
		}
	}
}