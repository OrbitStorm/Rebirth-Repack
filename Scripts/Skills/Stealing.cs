using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Regions;
using System.Text;

namespace Server.SkillHandlers
{
	public class Stealing
	{
		public static void Initialize()
		{
			SkillInfo.Table[33].Callback = new SkillUseCallback( OnUse );
		}

		public static readonly bool ClassicMode = true;
		public static readonly bool SuspendOnMurder = false;

		public static bool IsInGuild( Mobile m )
		{
			return ( m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild );
		}

		private class StealingTarget : Target
		{
			private PlayerMobile m_Thief;

			public StealingTarget( PlayerMobile thief ) : base ( 1, false, TargetFlags.None )
			{
				m_Thief = thief;
				BeginTimeout( thief, TimeSpan.FromSeconds( 60.0 ) );
				AllowNonlocal = true;
			}

			private Item TryStealItem( Item toSteal, object root, int difficulty, ref bool ok, ref bool caught )
			{
				Item stolen = null;

				//if ( toSteal is KeyRing )
				//	toSteal.Weight = toSteal.TotalWeight = 1;

				if ( root is BaseVendor || root is PlayerVendor )
				{
					m_Thief.SendLocalizedMessage( 1005598 ); // You can't steal from shopkeepers.
				}
				else if ( !m_Thief.CanSee( toSteal ) || ( root != null && !m_Thief.CanSee( root ) ) )
				{
					m_Thief.SendLocalizedMessage( 500237 ); // Target can not be seen.
				}
				else if ( !toSteal.Movable || toSteal.LootType == LootType.Newbied || toSteal.CheckBlessed( root ) )
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( !m_Thief.InRange( toSteal.GetWorldLocation(), 1 ) )
				{
					m_Thief.SendLocalizedMessage( 502703 ); // You must be standing next to an item to steal it.
				}
				else if ( toSteal.Parent is Mobile )
				{
					m_Thief.SendLocalizedMessage( 1005585 ); // You cannot steal items which are equiped.
				}
				else if ( root == m_Thief )
				{
					m_Thief.SendLocalizedMessage( 502704 ); // You catch yourself red-handed.
				}
				else if ( root is Mobile && ( ((Mobile)root).AccessLevel > AccessLevel.Player || !m_Thief.CanBeHarmful( (Mobile)root ) ) )
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else
				{
					for(Item p = toSteal.Parent as Item;p != null;p=p.Parent as Item)
					{
						if ( p is LockableContainer && ((LockableContainer)p).Locked )
						{
							m_Thief.SendAsciiMessage( "That is not accessable." );
							return null;
						}
					}
					
					if ( toSteal.Weight + toSteal.TotalWeight > 10 )
					{
						m_Thief.SendAsciiMessage( "That is too heavy to steal from someone's backpack." );
					}
					else
					{
						ok = true;

						double w = toSteal.PileWeight + toSteal.TotalWeight;
						double check;
						if ( w >= 10 )
						{
							check = 10 * 3.0 * difficulty + 10.0;
							caught = CheckDetect( ( 10 * 5.0 * difficulty ) / ( m_Thief.Skills.Stealing.Value + 100.0 ), root as Mobile );
						}
						else
						{
							check = w * 3.0 * difficulty + 10.0;
							if ( toSteal is Key || toSteal is Multis.Deeds.HouseDeed || toSteal is KeyRing )
								w += 5;
							caught = CheckDetect( ( w * 5.0 * difficulty ) / ( m_Thief.Skills.Stealing.Value + 100.0 ), root as Mobile );
						}

						if ( m_Thief.CheckSkill( SkillName.Stealing, check-25, check+25 ) )
						{
							m_Thief.SendLocalizedMessage( 502724 ); // You succesfully steal the item.
							if ( toSteal.Stackable && toSteal.Amount > 1 )
							{
								int amount;
								/*int maxAmount = (int)( (m_Thief.Skills[SkillName.Stealing].Value / 10.0) / toSteal.Weight );

								if ( maxAmount < 1 )
									maxAmount = 1;
								else if ( maxAmount > toSteal.Amount )
									maxAmount = toSteal.Amount;
								amount = Utility.Random( maxAmount ) + 1;*/

								amount = Utility.Random( 10 ) + 1;
						
								if ( amount > w )
									amount = toSteal.Amount;
								else
									amount = (toSteal.Amount * amount) / ( toSteal.PileWeight + toSteal.TotalWeight );
						
								if ( amount < 1 )
									amount = 1;

								if ( amount >= toSteal.Amount )
								{
									stolen = toSteal;
								}
								else
								{
									stolen = toSteal.Dupe( amount );
									toSteal.Amount -= amount;
								}
							}
							else
							{
								stolen = toSteal;
							}
						}
						else
						{
							m_Thief.SendLocalizedMessage( 502723 ); // You fail to steal the item.
						}
					}
				}

				return stolen;
			}
			
			private ArrayList m_Saw = null;
			private bool CheckDetect( double chance, Mobile mroot )
			{
				m_Saw = new ArrayList();
				int range = 10 - (int)(m_Thief.Skills[SkillName.Stealing].Value / 20.0);
				
				if ( range < 5 ) range = 5;

				IPooledEnumerable eable = m_Thief.GetMobilesInRange( range );
				foreach ( Mobile m in eable )
				{
					int dir = Math.Abs( (int)(m.Direction&Direction.Mask) - (int)m.GetDirectionTo( m_Thief ) );
					if ( dir > 4 )
						dir = Math.Abs( dir - 8 );

					if ( m != m_Thief && ( m.Player || dir <= 2 ) && Utility.RandomDouble() <= chance )
						m_Saw.Add( m );
				}
				eable.Free();
				
				return m_Saw.Count > 0;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				from.RevealingAction();
				from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 10.0 );

				Item stolen = null;
				Item attempt = null;
				object root = null;
				bool ok = false;
				bool caught = false;

				Mobile mobRoot = null;

				if ( target is Item )
				{
					attempt = (Item)target;
					root = attempt.RootParent;
					mobRoot = root as Mobile;
					stolen = TryStealItem( attempt, root, 3, ref ok, ref caught );
				} 
				else if ( target is Mobile )
				{
					Container pack = ((Mobile)target).Backpack;

					if ( pack != null && pack.Items.Count > 0 )
					{
						from.SendAsciiMessage( "You reach into {0}'s backpack and try to take something...", ((Mobile)target).Name );	
						
						int randomIndex = Utility.Random( pack.Items.Count );
						root = target;
						mobRoot = root as Mobile;
						attempt = pack.Items[randomIndex] as Item;
						if ( attempt != null )
							stolen = TryStealItem( attempt, root, 1, ref ok, ref caught );
					}
					else
					{
						from.SendAsciiMessage( "You reach into {0}'s backpack...  But find it's empty.", ((Mobile)target).Name );
					}
				} 
				else 
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				
				if ( !ok )
					return;

				if ( stolen != null )
				{
					from.AddToBackpack( stolen );
					if ( mobRoot != null )
					{
						Misc.Titles.AlterNotoriety( from, -1, NotoCap.Dishonorable );

						ArrayList list = m_Table[m_Thief] as ArrayList;
						if ( list == null )
							m_Table[m_Thief] = list = new ArrayList( 1 );
						list.Add( mobRoot );
						new ThiefExpireTimer( mobRoot, m_Thief ).Start();
					}
				}

				if ( caught )
				{
					bool crime;
					string fromStr = String.Empty;
					int noto = Notoriety.CanBeAttacked;

					if ( root is Corpse )
					{
						crime = ((Corpse)root).IsCriminalAction( m_Thief );
					}
					else if ( mobRoot != null )
					{
						if ( m_Saw != null && m_Saw.Count > 0 )
							crime = Notoriety.Compute( from, mobRoot ) == Notoriety.Innocent;	
						else
							crime = false;

						fromStr = String.Format( " from {0}", mobRoot.Name );
					}
					else 
					{
						crime = true;
					}

					if ( crime )
						m_Thief.CriminalAction( false ); // calls guards
					else if ( noto != Notoriety.Enemy && noto != Notoriety.Ally ) // dont go criminal in guild situations
						m_Thief.Criminal = true; // doesnt call guards
					
					if ( m_Saw != null && m_Saw.Count > 0 )
					{
						string message = null;
						foreach ( Mobile m in m_Saw )
						{
							if ( m.NetState != null )
							{
								if ( root is Mobile && m == root )
								{
									StringBuilder sb = new StringBuilder( "You notice " );
									sb.Append( m_Thief.Name );
									sb.Append( " trying to steal " );

									if ( attempt.Amount > 1 )
										sb.Append( "some " );
									else
										sb.Append( "a " );

									if ( attempt is BaseItem )
										((BaseItem)attempt).AppendClickName( sb );
									else if ( attempt.Name != null && attempt.Name != "" )
										sb.Append( attempt.Name );
									else
										sb.Append( attempt.ItemData.Name );

									sb.Append( " from you!" );
									m.SendAsciiMessage( sb.ToString() );
								}
								else 
								{
									if ( message == null )
									{
										if ( stolen != null )
											attempt = stolen;
										if ( attempt != null )
										{
											StringBuilder sb = new StringBuilder( "You notice " );
											sb.Append( m_Thief.Name );
											sb.Append( " trying to steal " );

											if ( attempt.Amount > 1 )
												sb.Append( "some " );
											else
												sb.Append( "a " );

											if ( attempt is BaseItem )
												((BaseItem)attempt).AppendClickName( sb );
											else if ( attempt.Name != null && attempt.Name != "" )
												sb.Append( attempt.Name );
											else
												sb.Append( attempt.ItemData.Name );

											sb.Append( fromStr );
											sb.Append( '!' );
											message = sb.ToString();
										}
										else
										{
											message = String.Format( "You notice {0} trying to something steal something{1}!", m_Thief.Name, fromStr );
										}
									}
									
									m.SendAsciiMessage( message );
								}
							}
							else if ( m is BaseCreature )
							{
								if ( m.Body.IsHuman )
								{
									if ( !(m is BaseGuard) && !(m is PlayerVendor) )
										m.Say( Utility.RandomList( 1007037, 501603, 1013037, 1013038, 1013039, 1013041, 1013042, 1013043, 1013052 ) );
									if ( crime )
									{
										GuardedRegion reg = m.Region as GuardedRegion;
										if ( reg != null && !reg.IsDisabled() )
											reg.CallGuards( m.Location );
									}
								}
									
								if ( root == m && Utility.RandomBool() )
								{
									BaseCreature bc = (BaseCreature)m;
									if ( !bc.Controled && ( bc.AlwaysMurderer || ( bc.AlwaysAttackable && !bc.IsHarmfulCriminal( m_Thief ) ) ) )
										bc.Attack( m_Thief );
								}
							}
						}// foreach m_Saw
					}
				}
			}
		}

		private class ThiefExpireTimer : Timer
		{
			private Mobile m_V, m_T;

			public ThiefExpireTimer( Mobile vic, Mobile t ) : base( TimeSpan.FromSeconds( 30.0 ) )
			{
				m_V = vic; m_T = t;
			}

			protected override void OnTick()
			{
				ArrayList list = m_Table[m_T] as ArrayList;
				if ( list != null )
				{
					list.Remove( m_V );

					if ( list.Count <= 0 )
						m_Table.Remove( m_T );
				}
			}
		}

		public static bool AttackOK( Mobile from, Mobile to )
		{
			ArrayList list = m_Table[to] as ArrayList;
			return ( list != null && list.Contains( from ) );
		}

		public static void ClearFor( Mobile thief )
		{
			m_Table.Remove( thief );
		}

		private static Hashtable m_Table = new Hashtable();

		public static TimeSpan OnUse( Mobile m )
		{
			if ( !(m is PlayerMobile) )
				return TimeSpan.Zero;

			m.Target = new Stealing.StealingTarget( (PlayerMobile)m );
			m.RevealingAction();
			m.SendLocalizedMessage( 502698 ); // Which item do you want to steal?
			return TimeSpan.Zero;
		}
	}
}
