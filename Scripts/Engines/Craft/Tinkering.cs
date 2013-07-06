using System;
using Server;
using Server.Items;
using Server.Menus;
using Server.Spells;
using Server.Menus.ItemLists;
using Server.Network;
using Server.Targeting;

namespace Server.Engines.Craft
{
	public class TinkeringSystem : CraftSystem
	{
		private static ItemListEntry[] m_IngotMenu = new ItemListEntry[]
		{
			new CraftMenuCallback( new CraftCallback( OnDartTrap ), "Dart Trap", 4397 ),
			new CraftMenuCallback( new CraftCallback( OnExpTrap ), "Explosion Trap", 4344 ),
			new CraftMenuCallback( new CraftCallback( OnPoisonTrap ), "Poison Trap", 4424 ),
			new CraftSystemItem( "Sewing Kit", 3997, SkillName.Tinkering, 0, 100, 1, typeof( SewingKit ) ),
			new CraftSystemItem( "Draw Knife", 4324, SkillName.Tinkering, 0, 100, 1, typeof( DrawKnife ) ),
			new CraftSystemItem( "Lockpick", 0x14FC, SkillName.Tinkering, 0, 100, 1, typeof( Lockpick ) ),
			new CraftSystemItem( "Blank Key", 0x100E, SkillName.Tinkering, 0, 100, 1, typeof( Key ) ),
			new CraftSystemItem( "Key Ring", 0x1011, SkillName.Tinkering, 0, 100, 1, typeof( KeyRing ) ),
			new CraftSystemItem( "Froe", 4325, SkillName.Tinkering, 0, 100, 1, typeof( Froe ) ),
			new CraftSystemItem( "In Shave", 4326, SkillName.Tinkering, 0, 100, 1, typeof( Inshave ) ),
			new CraftSystemItem( "Scissors", 3998, SkillName.Tinkering, 0, 100, 1, typeof( Scissors ) ),
			new CraftSystemItem( "Tongs", 4028, SkillName.Tinkering, 0, 100, 1, typeof( Tongs ) ),
			new CraftSystemItem( "Smith Hammer", 5092, SkillName.Tinkering, 0, 100, 1, typeof( SmithHammer ) ),
			new CraftSystemItem( "Butcher Knife", 5110, SkillName.Tinkering, 0, 100, 2, typeof( ButcherKnife ) ),
			new CraftSystemItem( "Dovetail Saw", 4136, SkillName.Tinkering, 0, 100, 2, typeof( DovetailSaw ) ),
			new CraftSystemItem( "Saw", 4148, SkillName.Tinkering, 0, 100, 2, typeof( Saw ) ),
			new CraftSystemItem( "Shovel", 3898, SkillName.Tinkering, 0, 100, 2, typeof( Shovel ) ),
			new CraftSystemItem( "Tinker's Tools", 7868, SkillName.Tinkering, 0, 100, 2, typeof( TinkerTools ) ),
			new CraftSystemItem( "Hammer", 4138, SkillName.Tinkering, 0, 100, 4, typeof( Hammer ) ),
		};

		private static void OnDartTrap( CraftSystem sys, Mobile from )
		{
			from.SendAsciiMessage( "What would you like to set the trap on?" );
			from.Target = new TrapTarget( TrapType.DartTrap, (TinkeringSystem)sys );			
		}

		private static void OnExpTrap( CraftSystem sys, Mobile from )
		{
			from.SendAsciiMessage( "What would you like to set the trap on?" );
			from.Target = new TrapTarget( TrapType.ExplosionTrap, (TinkeringSystem)sys );
		}

		private static void OnPoisonTrap( CraftSystem sys, Mobile from )
		{
			from.SendAsciiMessage( "What would you like to set the trap on?" );
			from.Target = new TrapTarget( TrapType.PoisonTrap, (TinkeringSystem)sys );
		}

		private class TrapTarget : Target
		{
			private TinkeringSystem m_Sys;
			private bool m_End;
			private TrapType m_Type;

			public TrapTarget( TrapType type, TinkeringSystem sys ) : base( 10, false, TargetFlags.None )
			{
				m_Sys = sys;
				m_Type = type;
				m_End = true;
			}

			protected override void OnTarget(Mobile from, object target)
			{
				m_End = false;
				m_Sys.OnTrapTarget( m_Type, target );
			}

			protected override void OnTargetFinish(Mobile from)
			{
				if ( m_End )
					m_Sys.End();
			}
		}

		private void OnTrapTarget( TrapType type, object target )
		{
			if ( m_Mobile.Deleted || !m_Mobile.Alive || !CheckTool() )
			{
				End();
				return;
			}

			if ( !HasResources( 1, typeof( TrapableContainer ) ) )
			{
				OnNotEnoughResources();
				End();
				return;
			}

			if ( target is TrapableContainer && !(target is Pouch) )
			{
				TrapableContainer cont = (TrapableContainer)target;
				if ( cont.RootParent != m_Mobile && cont.RootParent != null)
				{
					End();
					m_Mobile.SendAsciiMessage( "That does not belong to you." );
					return;
				}
				else if ( !m_Mobile.InRange( cont.GetWorldLocation(), 1 ) )
				{
					End();
					m_Mobile.SendAsciiMessage( "That is too far away." );
					return;
				}
				else if ( cont.Trapped )
				{
					End();
					m_Mobile.SendAsciiMessage( "You can only place one trap at a time." );
					return;
				}
				else if ( cont is LockableContainer && ((LockableContainer)cont).Locked )
				{
					End();
					m_Mobile.SendAsciiMessage( "You cannot trap a locked container." );
					return;
				}

				Container pack = m_Mobile.Backpack;
				if ( pack == null )
				{
					End();
					m_Mobile.SendAsciiMessage( "You need additional resources to create the trap." );
					return;
				}

				Item xres = null;
				switch ( type )
				{
					case TrapType.DartTrap:
						xres = pack.FindItemByType( typeof( Bolt ) );
						if ( xres == null )
							m_Mobile.SendAsciiMessage( "You need a crossbow bolt to make that trap." );
						else
							cont.TrapPower = (int)(m_Mobile.Skills[SkillName.Tinkering].Value / 10.0);
						break;
					case TrapType.PoisonTrap:
						xres = pack.FindItemByType( typeof( BasePoisonPotion ) );
						if ( xres == null )
							m_Mobile.SendAsciiMessage( "You need a green potion to make that trap." );
						else
							cont.TrapPower = ((BasePoisonPotion)xres).Poison.Level + 1;
						break;
					case TrapType.ExplosionTrap:
						xres = pack.FindItemByType( typeof( BaseExplosionPotion ) );
						if ( xres == null )
						{
							m_Mobile.SendAsciiMessage( "You need a purple potion to make that trap." );
						}
						else
						{
							//BaseExplosionPotion pot = (BaseExplosionPotion)xres;
							//cont.TrapPower = (int)(Utility.RandomMinMax( pot.MinDamage, pot.MaxDamage ) * ( m_Mobile.Skills[SkillName.Tinkering].Value / 10.0 ));
							if ( xres is ExplosionPotion )
								cont.TrapPower = 3;
							else if ( xres is GreaterExplosionPotion )
								cont.TrapPower = 5;
							else //if ( xres is LesserExplosionPotion )
								cont.TrapPower = 1;
						}
						break;
				}

				if ( xres == null || xres.Amount < 1 || !ConsumeResources( 1, typeof( TrapableContainer ) ) )
				{
					OnNotEnoughResources();
					End();
					return;
				}
				xres.Consume( 1 );

				if ( m_Mobile.CheckSkill( SkillName.Tinkering, 0, 100 ) )
				{
					cont.TrapType = type;
					cont.Trapped = true;
					cont.Trapper = m_Mobile;
					m_Mobile.SendAsciiMessage( "You carefully set a trap on the container..." );
				}
				else
				{
					cont.TrapPower = 0;
					m_Mobile.SendAsciiMessage( "You failed to set the trap." );
				}
			}
			else
			{
				m_Mobile.SendAsciiMessage( "You can't trap that." );
			}
			End();
		}

		public override string TargetPrompt { get { return "What would you like to use this on?"; } }

		private Item m_Res;
		protected override bool OnTarget( Item target )
		{
			if ( target is IronIngot )
			{
				m_Res = target;
				return ShowMenu( m_IngotMenu );
			}
			else
			{
				if ( target is Board || target is Log )// TODO sexants and clocks and worthless bullshit
					m_Mobile.SendAsciiMessage( "Why would you waste your time on something so useless?" );
				return false;
			}
		}

		protected override bool HasResources(int count, Type toMake)
		{
			return count <= m_Res.Amount && m_Res.IsChildOf( m_Mobile ) && m_Mobile.Alive && !m_Mobile.Deleted;
		}

		protected override bool ConsumeResources(int count, Type toMake)
		{
			if ( HasResources( count, toMake ) )
			{
				m_Res.Amount -= count;
				if ( m_Res.Amount <= 0 )
					m_Res.Delete();
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
