using System;
using System.Collections; using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	public abstract class BaseGuard : BaseConvo
	{
		public static void Spawn( Mobile caller, Mobile target )
		{
			Spawn( caller, target, 1, false );
		}

		public override bool ClickTitle
		{
			get
			{
				return false;
			}
		}

		public override bool BardImmune
		{
			get
			{
				return true;
			}
		}

		public static void Spawn( Mobile caller, Mobile target, int amount, bool onlyAdditional )
		{
			if ( target == null || target.Deleted )
				return;

			foreach ( Mobile m in target.GetMobilesInRange( 15 ) )
			{
				if ( m is BaseGuard )
				{
					BaseGuard g = (BaseGuard)m;

					if ( g.Focus == null ) // idling
					{
						g.Focus = target;

						--amount;
					}
					else if ( g.Focus == target && !onlyAdditional )
					{
						--amount;
					}
				}
			}

			while ( amount-- > 0 )
				caller.Region.MakeGuard( target );
		}

		public BaseGuard( Mobile target ) : base( AIType.AI_Melee, FightMode.Agressor, 14, 1, 0.2, 1.0 )
		{
			Title = "the guard";
			Job = JobFragment.guard;
			if ( target != null )
			{
				Location = target.Location;
				Map = target.Map;

				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
				Combatant = target;
			}
		}

		public BaseGuard( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );

			PlaySound( 0x1FE );

			Delete();

			return false;
		}

		public override bool OnDragDrop( Mobile m, Item item )  
		{  
			if ( item is Head ) 
			{ 
				Head head = (Head)item;
				if ( !(head.Owner is PlayerMobile) || head.Age >= TimeSpan.FromDays( 1.0 ) || head.MaxBounty <= 0 || ((PlayerMobile)head.Owner).Bounty <= 0 || head.Owner == m || ( m.Account != null && head.Owner.Account != null && head.Owner.Account == m.Account ) )
				{
					this.Say( true, "'Tis a decapitated head. How disgusting." );
				}
				else
				{ 
					PlayerMobile owner = (PlayerMobile)head.Owner;
										
					int total = owner.Bounty;
					if ( head.MaxBounty < total )
						total = head.MaxBounty;

					if ( total >= 15000 )
						Say( true, String.Format( "Thou hast brought the infamous {0} to justice!  Thy reward of {1}gp hath been deposited in thy bank account.", owner.Name, total ) ); 
					else if ( total > 100 )
						Say( true, String.Format( "Tis a minor criminal, thank thee. Thy reward of {0}gp hath been deposited in thy bank account.", total ) );
					else
						Say( true, String.Format( "Thou hast wasted thy time for {0}gp.", total ) );

					double statloss = 1.0;

					if ( total > 750000 )
						statloss = 0.90;
					else if ( total > 500000 )
						statloss = 0.91;
					else if ( total > 300000 )
						statloss = 0.92;
					else if ( total > 200000 )
						statloss = 0.93;
					else if ( total > 125000 )
						statloss = 0.94;
					else if ( total > 75000 )
						statloss = 0.95;
					else if ( total > 50000 )
						statloss = 0.96;
					else if ( total > 30000 )
						statloss = 0.97;
					else if ( total > 15000 )
						statloss = 0.98;
					else if ( total > 5000 )
						statloss = 0.99;
					
					if ( statloss < 1.0 )
					{
						if ( statloss < 0.9 )
							statloss = 0.9;

						if ( owner.RawStr * statloss >= 10 )
							owner.RawStr = (int)(owner.RawStr * statloss);
						else
							owner.RawStr = 10;

						if ( owner.RawDex * statloss >= 10 )
							owner.RawDex = (int)(owner.RawDex * statloss);
						else
							owner.RawDex = 10;

						if ( owner.RawInt * statloss >= 10 )
							owner.RawInt = (int)(owner.RawInt * statloss);
						else
							owner.RawInt = 10;

						for(int i=0;i<owner.Skills.Length;i++)
						{
							if ( owner.Skills[i].Base * statloss > 50 )
								owner.Skills[i].Base *= statloss;
						}

						owner.SendAsciiMessage( "Thy head has been turned in for a bounty of {0}gp.  Suffer thy punishment!", total );
					}

					if ( total < owner.Bounty )
						owner.Kills /= 2;
					else
						owner.Kills = 0;

					owner.Bounty -= total;
					if ( owner.Bounty < 0 )
						owner.Bounty = 0;

					while ( total > 0 )
					{
						int amount = total > 65000 ? 65000 : total;
						m.BankBox.DropItem( new Gold( amount ) ); 
						total -= amount;
					}
				} 

				return true; 
			} 
			else 
			{
				this.Say( true, "I have no use for this." ); 
				return false; 
			}
		} 

		public abstract Mobile Focus{ get; set; }

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

