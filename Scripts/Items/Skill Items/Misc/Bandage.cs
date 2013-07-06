using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class Bandage : BaseItem, IDyable
	{
		[Constructable]
		public Bandage() : this( 1 )
		{
		}

		[Constructable]
		public Bandage( int amount ) : base( 0xE21 )
		{
			Stackable = true;
			Weight = 0.1;
			Amount = amount;
		}

		public Bandage( Serial serial ) : base( serial )
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.NextSkillTime > DateTime.Now )
			{
				from.SendSkillMessage(); // You must wait...
			}
			else if ( from.InRange( GetWorldLocation(), 1 ) )
			{
				from.RevealingAction();

				from.SendLocalizedMessage( 500948 ); // Who will you use the bandages on?
				from.BeginTarget( 3, false, TargetFlags.Beneficial, new TargetCallback( OnTarget ) );
			}
			else
			{
				from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
			}
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Bandage(), amount );
		}

		private SkillName GetSkillFor( Mobile m )
		{
			if ( m.Body.IsHuman || m.Player || m.Body.IsGhost )
				return SkillName.Healing;
			else
				return SkillName.Veterinary;
		}

		public void OnTarget( Mobile from, object targeted )
		{
			if ( this.Deleted || this.Amount <= 0 )
				return;

			Mobile targ = targeted as Mobile;
			if ( from.NextSkillTime > DateTime.Now )
			{
				from.SendSkillMessage(); // You must wait...
			}
			else if ( targ == null || targ.Deleted || !targ.Alive || !( targ.Body.IsHuman || targ.Body.IsAnimal || targ.Body.IsSea || (targ is BaseCreature && ((BaseCreature)targ).Controled && !((BaseCreature)targ).Summoned) ) )
			{
				// you can heal anything tamable... I guess.
				from.SendLocalizedMessage( 500951 ); // You cannot heal that.
			}
			else if ( !from.InRange( targ, 3 ) )
			{
				from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
			}
			else if ( Notoriety.Compute( from, targ ) == Notoriety.Enemy ) // || Server.Misc.NotorietyHandlers.CheckAggressor( from.Aggressors, targ ) || Server.Misc.NotorietyHandlers.CheckAggressed( from.Aggressed, targ ) )
			{
				from.SendAsciiMessage( "You cannot heal them right now." );
			}
			else if ( !from.CanSee( targ ) || !from.InLOS( targ ) )
			{
				from.SendAsciiMessage( "You can't see that." );
			}
			else if ( targ.Hits >= targ.HitsMax )
			{
				from.SendLocalizedMessage( 500955 ); // That being is not damaged!
			}
			else if ( !targ.CanBeginAction( typeof( Bandage ) ) || !from.CanBeBeneficial( targ, true, true ) )
			{
				from.SendAsciiMessage( "This being cannot be newly bandaged yet." );
			}
			else
			{
				from.DoBeneficial( targ );
				from.RevealingAction();

				this.Consume();

				int damage = targ.HitsMax - targ.Hits;
				int sk = damage * 100 / targ.HitsMax;
				int healed = 1;
				SkillName skill = GetSkillFor( targ );
				if ( from.CheckSkill( skill, sk - 25.0, sk + 25.0 ) )
				{
					targ.PlaySound( 0x57 );

					double scale = ( 75.0 + from.Skills[skill].Value - sk ) / 100.0;
					if ( scale > 1.0 )
						scale = 1.0;
					else if ( scale < 0.5 )
						scale = 0.5;
					healed = (int)( damage * scale );

					if ( healed < 1 )
						healed = 1;
					else if ( healed > 100 )
						healed = 100;
				}

				targ.Hits += healed;
				if ( healed > 1 )
					from.SendAsciiMessage( "You apply the bandages." );
				else
					from.SendAsciiMessage( "You apply the bandages, but they barely help." );
				
				targ.BeginAction( typeof( Bandage ) );
				new BandageExpire( targ ).Start();
			}
		}

		private class BandageExpire : Timer
		{
			private Mobile m_Targ;

			public BandageExpire( Mobile targ ) : base( TimeSpan.FromMinutes( 3 ) )
			{
				m_Targ = targ;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Targ.EndAction( typeof( Bandage ) );
			}
		}
	}
}
