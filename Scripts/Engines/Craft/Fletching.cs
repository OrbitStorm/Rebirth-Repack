using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Targeting;
using System.Reflection;

namespace Server.Engines.Craft
{
	public class FletchingSystem : CraftSystem
	{
		private Shaft m_Shafts;
		private Feather m_Feathers;

		public override bool NeedTarget { get { return false; } }

		public FletchingSystem( Feather feather, Shaft shaft )
		{
			m_Shafts = shaft;
			m_Feathers = feather;
		}

		public override bool Begin( Mobile from, BaseTool tool )
		{
			if ( m_Shafts != null && m_Feathers != null )
			{
				if ( !base.Begin( from, tool ) )
					return false;

				int total = m_Shafts.Amount;
				if ( total > m_Feathers.Amount )
					total = m_Feathers.Amount;

				ArrayList list = new ArrayList( 4 );
				list.Add( new CraftMenuCallback( new CraftCallback( MakeArrow ), "Make 1 Arrow", 0xF3F ) );
				list.Add( new CraftMenuCallback( new CraftCallback( MakeBolt ), "Make 1 Bolt", 0x1BFB ) );
				if ( total > 1 )
				{
					list.Add( new CraftMenuCallback( new CraftCallback( MakeAllArrows ), String.Format( "Make {0} Arrows", total ), 0xF41 ) );
					list.Add( new CraftMenuCallback( new CraftCallback( MakeAllBolts ), String.Format( "Make {0} Bolts", total ), 0x1BFD ) );
				}
                
				if ( !ShowMenu( (ItemListEntry[])list.ToArray( typeof( ItemListEntry ) ) ) )
				{
					End();
					return false;
				}
				else
				{
					return true;
				}
			}

			return false;
		}

		private void MakeArrow( CraftSystem sys, Mobile from )
		{
			Make( from, 1, typeof( Arrow ) );
		}

		private void MakeBolt( CraftSystem sys, Mobile from )
		{
			Make( from, 1, typeof( Bolt ) );
		}

		private void MakeAllArrows( CraftSystem sys, Mobile from )
		{
			int total = m_Shafts.Amount;
			if ( total > m_Feathers.Amount )
				total = m_Feathers.Amount;
			Make( from, total, typeof( Arrow ) );
		}

		private void MakeAllBolts( CraftSystem sys, Mobile from )
		{
			int total = m_Shafts.Amount;
			if ( total > m_Feathers.Amount )
				total = m_Feathers.Amount;
			Make( from, total, typeof( Bolt ) );
		}

		private static Type[] ctorTypes = new Type[1]{ typeof( int ) };
		private static object[] ctorArgs = new object[1];
		private void Make( Mobile from, int amount, Type type )
		{
			End();

			if ( from.Deleted || !from.Alive )
				return;

			if ( from.Backpack == null || !m_Shafts.IsChildOf( from ) || !m_Feathers.IsChildOf( from ) )
			{
				from.SendAsciiMessage( "Those resources do not belong to you." );
				return;
			}

			int resCost = amount;
			if ( from.CheckSkill( SkillName.Fletching, -10.0, 50.0 ) )
			{
				Item item = null;
				try
				{
					ConstructorInfo ctor = type.GetConstructor( ctorTypes );
					if ( ctor != null )
					{
						ctorArgs[0] = amount;
						item = ctor.Invoke( ctorArgs ) as Item;
					}
				}
				catch
				{
				}

				if ( item != null )
				{
					from.AddToBackpack( item );
					from.SendAsciiMessage( "You create the item{0} and place {1} in your pack.", amount > 1 ? "s" : "", amount > 1 ? "them" : "it" );
				}
				else
				{
					from.SendAsciiMessage( "Unable to create that." );
					resCost = 0;
				}
			}
			else
			{
				from.SendAsciiMessage( "You failed to create the item{0}.", amount > 1 ? "s" : "" );
				resCost = (int)( amount * ( 100.0 - from.Skills[SkillName.Fletching].Value ) / 100.0 );
				if ( resCost < amount * 0.05 )
					resCost = (int)(amount * 0.05);
				if ( resCost < 1 )
					resCost = 1;
			}

			m_Shafts.Amount -= resCost;
			m_Feathers.Amount -= resCost;

			if ( m_Shafts.Amount <= 0 )
				m_Shafts.Delete();
			if ( m_Feathers.Amount <= 0 )
				m_Feathers.Delete();
		}

		protected override bool HasResources(int count, Type toMake)
		{
			return true;
		}

		protected override bool ConsumeResources(int count, Type toMake)
		{
			return false;
		}
	}

	public class BowcraftSystem : CraftSystem
	{
		private static ItemListEntry[] m_WoodMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Bow (7 wood)",			5042, SkillName.Fletching, 5.0, 55.0, 7, typeof( Bow ) ),
			new CraftSystemItem( "Crossbow (7 wood)",		3919, SkillName.Fletching, 35.0, 85.0, 7, typeof( Crossbow ) ),
			new CraftSystemItem( "Heavy Crossbow (10 wood)",5117, SkillName.Fletching, 65.0, 115.0, 10, typeof( HeavyCrossbow ) ),
			new CraftSystemItem( "Shafts (Using ALL wood)", 7124, SkillName.Fletching, -25.0, 25.0, 1, typeof( Shaft ) ),
		};

		private Item m_Wood;
		public BowcraftSystem( Item wood )
		{
			m_Wood = wood;
		}

		public override bool NeedTarget { get { return false; } }
		public override TimeSpan CraftDelay { get { return TimeSpan.FromSeconds( 3.0 ); } }

		public override bool Begin( Mobile from, BaseTool tool )
		{
			if ( from.Deleted || !from.Alive || m_Wood == null || m_Wood.Deleted )
				return false;

			if ( !base.Begin( from, tool ) )
				return false;

			if ( m_Wood.IsChildOf( from ) )
			{
				if ( ShowMenu( m_WoodMenu ) )
				{
					return true;
				}
				else
				{
					End();
					return false;
				}
			}
			else
			{
				from.SendAsciiMessage( "That wood belongs to someone else." );
				End();
				return false;
			}
		}

		public override void OnItemSelected(CraftSystemItem item)
		{
			base.OnItemSelected (item);
			m_Mobile.Emote( "*{0} begins carving.*", m_Mobile.Name );
			m_Mobile.PlaySound( 85 );
		}

		public override void Finish( CraftSystemItem item )
		{
			End();

			if ( m_Mobile.Deleted || !m_Mobile.Alive || m_Wood == null || m_Wood.Deleted )
				return;
			
			if ( !CheckTool() )
				return;

			if ( item.CreateType == typeof( Shaft ) )
			{ 
				if ( HasResources( m_Wood.Amount, typeof( Shaft ) ) )
				{
					if ( m_Mobile.CheckSkill( item.Skill, item.MinSkill, item.MaxSkill ) )
					{
						Item created = new Shaft( m_Wood.Amount );
						m_Wood.Delete();
						OnSuccess( created );
					}
					else
					{
						OnFailure();
					}
				}
				else
				{
					OnNotEnoughResources();
				}
			}
			else
			{
				base.Finish(item);
			}
		}

		protected override void OnSuccess(Item made)
		{
			m_Mobile.Emote( "*{0} places the carving into {1} pack.*", m_Mobile.Name, m_Mobile.Body.IsHuman ? ( m_Mobile.Female ? "her" : "his" ) : "its" );
			base.OnSuccess (made);

			int diff;
			if ( made is Bow )
				diff = 90 + Utility.Random( 11 );
			else if ( made is Crossbow )
				diff = 95 + Utility.Random( 11 );
			else if ( made is HeavyCrossbow )
				diff = 99 + Utility.Random( 6 );
			else
				diff = 0;

			if ( diff != 0 && diff <= m_Mobile.Skills[SkillName.Fletching].Value )
			{
				((BaseWeapon)made).Quality = CraftQuality.Exceptional;
				if ( m_Mobile.Skills[SkillName.Fletching].Value >= 99.0 )
					((BaseWeapon)made).Crafter = m_Mobile;
				m_Mobile.SendAsciiMessage( "Due to your exceptional skill, it's quality is higher than average." );
			}
		}

		protected override bool HasResources(int count, Type toMake)
		{
			return count <= m_Wood.Amount && m_Wood.IsChildOf( m_Mobile ) && m_Mobile.Alive;
		}

		protected override bool ConsumeResources(int count, Type toMake)
		{
			if ( HasResources( count, toMake ) )
			{
				m_Wood.Amount -= count;
				if ( m_Wood.Amount <= 0 )
					m_Wood.Delete();
				return true;
			}
			else
			{
				return false;
			}
		}

		protected override void OnFailure()
		{
			m_Mobile.Emote( "*{0} stops carving, but is left with nothing useful.*", m_Mobile.Name );
			End();
		}

		protected override void OnNotEnoughResources()
		{
			m_Mobile.SendAsciiMessage( "You do not have enough wood." );
			End();
		}
	}
}
