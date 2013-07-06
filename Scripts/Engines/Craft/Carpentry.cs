using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Targeting;

namespace Server.Engines.Craft
{
	public class CarpentrySystem : CraftSystem
	{
		private static ItemListEntry[] m_Chairs = new ItemListEntry[]
		{
			new CraftSystemItem( "Stool (9 wood)",		2602, SkillName.Carpentry, 0, 100, 9, typeof( Stool ) ),
			new CraftSystemItem( "Chair (13 wood)",		2906, SkillName.Carpentry, 0, 100, 13, typeof( BambooChair ) ),
			new CraftSystemItem( "Chair (13 wood)",		2902, SkillName.Carpentry, 0, 100, 13, typeof( WoodenChair ) ),
			new CraftSystemItem( "Chair (15 wood)",		2894, SkillName.Carpentry, 0, 100, 15, typeof( FancyWoodenChairCushion ) ),
			new CraftSystemItem( "Chair (15 wood)",		2898, SkillName.Carpentry, 0, 100, 15, typeof( WoodenChairCushion ) ),
			new CraftSystemItem( "Bench (17 wood)",		2860, SkillName.Carpentry, 0, 100, 17, typeof( WoodenBench ) ),
			new CraftSystemItem( "Chair (17 wood)",		2863, SkillName.Carpentry, 0, 100, 17, typeof( WoodenThrone ) ),
			new CraftSystemItem( "Throne (19 wood)",	2867, SkillName.Carpentry, 0, 100, 19, typeof( Throne ) ),
		};

		private static ItemListEntry[] m_Tables = new ItemListEntry[]
		{
			new CraftSystemItem( "Table (17 wood)",		2868, SkillName.Carpentry, 0, 100, 17, typeof( Nightstand ) ),
			new CraftSystemItem( "Writing Table (17 wood)",2890, SkillName.Carpentry, 0, 100, 17, typeof( WritingTable ) ),
			new CraftSystemItem( "Table (23 wood)",		2940, SkillName.Carpentry, 0, 100, 23, typeof( YewWoodTable ) ),
			new CraftSystemItem( "Table (27 wood)",		2941, SkillName.Carpentry, 0, 100, 27, typeof( LargeTable ) ),
		};

		private static ItemListEntry[] m_Misc = new ItemListEntry[]
		{
			new CraftSystemItem( "Wooden Shield (9 wood)",7034, SkillName.Carpentry, 0, 100, 9, typeof( WoodenShield ) ),
			new CraftSystemItem( "Box (9 wood)",		3709, SkillName.Carpentry, 0, 100, 9, typeof( WoodenBox ) ),
			new CraftSystemItem( "Small Crate (9 wood)",3710, SkillName.Carpentry, 0, 100, 9, typeof( SmallCrate ) ),
			new CraftSystemItem( "Crate (11 wood)",		3646, SkillName.Carpentry, 0, 100, 11, typeof( MediumCrate ) ),
			new CraftSystemItem( "Crate (13 wood)",		3644, SkillName.Carpentry, 0, 100, 13, typeof( LargeCrate ) ),
			new CraftSystemItem( "Chest (15 wood)",		3650, SkillName.Carpentry, 0, 100, 15, typeof( WoodenChest ) ),
			new CraftSystemItem( "Shelf (21 wood)",		2718, SkillName.Carpentry, 0, 100, 21, typeof( EmptyBookcase ) ),
			new CraftSystemItem( "Armoire (25 wood)",	2641, SkillName.Carpentry, 0, 100, 25, typeof( FancyArmoire ) ),
			new CraftSystemItem( "Armoire (25 wood)",	2643, SkillName.Carpentry, 0, 100, 25, typeof( Armoire ) ),
		};

		private static ItemListEntry[] m_MainMenu = new ItemListEntry[]
		{
			new CraftSubMenu( "Chairs", 2902, m_Chairs ),
			new CraftSubMenu( "Tables", 2940, m_Tables ),
			new CraftSubMenu( "Miscellaneous", 3650, m_Misc )
		};

		public override string TargetPrompt { get { return "Select the wood to use."; } }
		public override int SoundEffect { get { return 0x23D; } }

		private Item m_Res;
		protected override bool OnTarget( Item target )
		{
			if ( target is Log || target is Board )
			{
				m_Res = target;
				return ShowMenu( m_MainMenu );
			}
			else
			{
				return false;
			}
		}

		protected override bool HasResources(int count, Type toMake)
		{
			return m_Res.Amount >= count && m_Res.IsChildOf( m_Mobile ) && !m_Res.Deleted && CheckTool();
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

		protected override void OnNotEnoughResources()
		{
			m_Mobile.SendAsciiMessage( "You do not have enough wood to make that." );
			End();
		}

		protected override void OnFailure()
		{
			m_Mobile.SendAsciiMessage( "You failed to create the item." );
			End();
		}

		protected override void OnSuccess(Item made)
		{
			m_Mobile.SendAsciiMessage( "Put it where?" );
			m_Mobile.BeginTarget( 10, true, TargetFlags.None, new TargetStateCallback( OnTargetLoc ), made );
			End();
		}

		private void OnTargetLoc( Mobile from, object target, object item )
		{
			object loc = null;
			if ( target is LandTarget )
				loc = ((LandTarget)target).Location;
			else if ( target is StaticTarget )
				loc = ((StaticTarget)target).Location;
			else if ( target is Item )
				loc = ((Item)target).Location;
			else if ( target is Mobile )
				loc = ((Mobile)target).Location;

			if ( loc == null )
			{
				from.SendAsciiMessage( "You cannot create that there." );
				return;
			}

			Item made = (Item)item;
			if ( from.Map.CanFit( (Point3D)loc, made.ItemData.Height, true, true ) )
			{
				made.MoveToWorld( (Point3D)loc, from.Map );
				if ( made is LockableContainer )
				{
					if ( from.Skills[SkillName.Tinkering].Value >= Utility.Random( 100 ) )
					{
						LockableContainer cont = (LockableContainer)made;
						Key key = new Key( KeyType.Copper, Key.RandomValue() );
						
						cont.LockLevel = (int)(from.Skills[SkillName.Tinkering].Value/2)-15;
						if ( cont.LockLevel < 1 )
							cont.LockLevel = 1;
						cont.MaxLockLevel = cont.LockLevel + 30;
						cont.RequiredSkill = cont.LockLevel;

						cont.KeyValue = key.KeyValue;
						cont.DropItem( key );

						from.SendAsciiMessage( "Your tinker skill was sufficient to make the item lockable." );
					}
				}
			}
			else
			{
				from.SendAsciiMessage( "You cannot create that there." );
				made.Delete();
			}
		}
	}
}

