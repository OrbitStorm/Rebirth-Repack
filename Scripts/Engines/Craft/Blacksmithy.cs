using System;
using System.Collections; using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Targeting;

namespace Server.Engines.Craft
{
	public class ForgeAttribute : Attribute
	{
		public ForgeAttribute()
		{
		}
	}

	public class AnvilAttribute : Attribute
	{
		public AnvilAttribute()
		{
		}
	}

	public class BlacksmithSystem : CraftSystem
	{
		private static ItemListEntry[] m_ShieldsMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Buckler (6 ingots)",			0x1B73, SkillName.Blacksmith, 0.0, 25.0, 6, typeof( Buckler ) ),
				new CraftSystemItem( "Bronze Shield (10 ingots)",	0x1B72, SkillName.Blacksmith, 1.0, 30.0, 10, typeof( BronzeShield ) ),
				new CraftSystemItem( "Metal Shield (14 ingots)",	0x1B7B, SkillName.Blacksmith, 1.0, 35.0, 14, typeof( MetalShield ) ),
				new CraftSystemItem( "Kite Shield (16 ingots)",		0x1B74, SkillName.Blacksmith, 15.0, 45.0, 16, typeof( MetalKiteShield ) ),
				new CraftSystemItem( "Heater Shield (18 ingots)",	0x1B76, SkillName.Blacksmith, 40.0, 65.0, 18, typeof( HeaterShield ) ),
			};

		private static ItemListEntry[] m_PlateMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Chest (25 ingots)",			0x1415, SkillName.Blacksmith, 75.0, 110.0, 25, typeof( PlateChest ) ),
				new CraftSystemItem( "Female Chest (20 ingots)",	0x1C04, SkillName.Blacksmith, 40.0, 80.0, 20, typeof( FemalePlateChest ) ),
				new CraftSystemItem( "Gorget (10 ingots)",			0x1413, SkillName.Blacksmith, 50.0, 90.0, 10, typeof( PlateGorget ) ),
				new CraftSystemItem( "Gloves (12 ingots)",			0x1414, SkillName.Blacksmith, 50.0, 90.0, 12, typeof( PlateGloves ) ),
				new CraftSystemItem( "Legs (20 ingots)",			0x1411, SkillName.Blacksmith, 65.0, 100.0, 20, typeof( PlateLegs ) ),
				new CraftSystemItem( "Arms (18 ingots)",			0x1410, SkillName.Blacksmith, 60.0, 95.0, 18, typeof( PlateArms ) ),
			};

		private static ItemListEntry[] m_ChainMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Tunic (20 ingots)",		0x13BF, SkillName.Blacksmith, 40.0, 75.0, 20, typeof( ChainChest ) ),
				new CraftSystemItem( "Legs (18 ingots)",		0x13BE, SkillName.Blacksmith, 40.0, 70.0, 18, typeof( ChainLegs ) ),
			};

		private static ItemListEntry[] m_RingMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Tunic (18 ingots)",		0x13EC, SkillName.Blacksmith, 30.0, 70.0, 18, typeof( RingmailChest ) ),
				new CraftSystemItem( "Arms (14 ingots)",		0x13EE, SkillName.Blacksmith, 20.0, 60.0, 14, typeof( RingmailArms ) ),
				new CraftSystemItem( "Legs (16 ingots)",		0x13F0, SkillName.Blacksmith, 25.0, 65.0, 16, typeof( RingmailLegs ) ),
				new CraftSystemItem( "Gloves (10 ingots)",		0x13EB, SkillName.Blacksmith, 17.5, 57.5, 10, typeof( RingmailGloves ) ),
			};

		private static ItemListEntry[] m_HelmsMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Chainman Coif (10 ingots)",		0x13BB, SkillName.Blacksmith, 27.5, 67.5, 10, typeof( ChainCoif ) ),
				new CraftSystemItem( "Bascinet (15 ingots)",			0x140C, SkillName.Blacksmith, 20.0, 55.0, 15, typeof( Bascinet ) ),
				new CraftSystemItem( "Close Helmet (15 ingots)",		0x1408, SkillName.Blacksmith, 55.0, 80.0, 15, typeof( CloseHelm ) ),
				new CraftSystemItem( "Helmet (15 ingots)",				0x140A, SkillName.Blacksmith, 55.0, 80.0, 15, typeof( Helmet ) ),
				new CraftSystemItem( "Norse Helmet (15 ingots)",		0x140E, SkillName.Blacksmith, 55.0, 80.0, 15, typeof( NorseHelm ) ),
				new CraftSystemItem( "Platemail Helmet (15 ingots)",	0x1412, SkillName.Blacksmith, 57.5, 82.5, 15, typeof( PlateHelm ) ),
			};

		private static ItemListEntry[] m_ArmorMenu = new ItemListEntry[]
			{
				new CraftSubMenu( "Plate Mail", 0x1415, m_PlateMenu ),
				new CraftSubMenu( "Chain Mail", 0x13BF, m_ChainMenu ),
				new CraftSubMenu( "Ring Mail", 0x13EC, m_RingMenu ),
				new CraftSubMenu( "Helmets", 0x1412, m_HelmsMenu ),
			};

		private static ItemListEntry[] m_SwordsMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Broadsword (10 ingots)", 0xF5E, SkillName.Blacksmith, 55.0, 80.0, 10, typeof( Broadsword ) ),
				new CraftSystemItem( "Cutlass (8 ingots)", 0x1441, SkillName.Blacksmith, 40.5, 80.0, 8, typeof( Cutlass ) ),
				new CraftSystemItem( "Dagger (3 ingots)", 0xF52, SkillName.Blacksmith, 7.7, 30.0, 3, typeof( Dagger ) ),
				new CraftSystemItem( "Katana (8 ingots)", 0x13FF, SkillName.Blacksmith, 66.8, 90.0, 8, typeof( Katana ) ),
				new CraftSystemItem( "Kryss (8 ingots)", 0x1401, SkillName.Blacksmith, 56.8, 90.0, 8, typeof( Kryss ) ),
				new CraftSystemItem( "Longsword (12 ingots)", 0xF5E, SkillName.Blacksmith, 45.4, 80.0, 12, typeof( Longsword ) ),
				new CraftSystemItem( "Scimitar (10 ingots)", 0x13B6, SkillName.Blacksmith, 50.4, 85.0, 10, typeof( Scimitar ) ),
				new CraftSystemItem( "Vikingsword (14 ingots)", 0x13B9, SkillName.Blacksmith, 40.5, 80.0, 14, typeof( VikingSword ) ),
			};

		private static ItemListEntry[] m_AxesMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Executioner's Axe (16 ingots)", 0xF45, SkillName.Blacksmith, 53.8, 95.0, 16, typeof( ExecutionersAxe ) ),
				new CraftSystemItem( "Axe (14 ingots)", 0xF49, SkillName.Blacksmith, 53.8, 95.0, 14, typeof( Axe ) ),
				new CraftSystemItem( "Double Axe (12 ingots)", 0xF4B, SkillName.Blacksmith, 47.1, 80.0, 12, typeof( DoubleAxe ) ),
				new CraftSystemItem( "Battle Axe (14 ingots)", 0xF47, SkillName.Blacksmith, 48.7, 80.0, 14, typeof( BattleAxe ) ),
				new CraftSystemItem( "Large Battle Axe (14 ingots)", 0x13FB, SkillName.Blacksmith, 45.4, 85.0, 14, typeof( LargeBattleAxe ) ),
				new CraftSystemItem( "War Axe (16 ingots)", 0x13B0, SkillName.Blacksmith, 60.2, 90.0, 16, typeof( WarAxe ) ),
				new CraftSystemItem( "Two Handed Axe (16 ingots)", 0x1443, SkillName.Blacksmith, 52.0, 90.0, 16, typeof( TwoHandedAxe ) ),
			};

		private static ItemListEntry[] m_MacesMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Mace (6 ingots)", 0xF5C, SkillName.Blacksmith, 27.5, 40.0, 6, typeof( Mace ) ),
				new CraftSystemItem( "Maul (10 ingots)", 0x143B, SkillName.Blacksmith, 33.9, 50.0, 10, typeof( Maul ) ),
				new CraftSystemItem( "Hammer Pick (6 ingots)", 0x143D, SkillName.Blacksmith, 42.2, 80.0, 6, typeof( HammerPick ) ),
				new CraftSystemItem( "War Hammer (16 ingots)", 0x1439, SkillName.Blacksmith, 53.8, 85.0, 16, typeof( WarHammer ) ),
				new CraftSystemItem( "War Mace (14 ingots)", 0x1407, SkillName.Blacksmith, 45.4, 80.0, 14, typeof( WarMace ) ),
			};

		private static ItemListEntry[] m_SpearsMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Short Spear (6 ingots)", 0x1403, SkillName.Blacksmith, 68.4, 95.0, 6, typeof ( ShortSpear ) ),
				new CraftSystemItem( "Spear (12 ingots)", 0x0F62, SkillName.Blacksmith, 73.2, 100.0, 12, typeof ( Spear ) ),
				new CraftSystemItem( "War Fork (12 ingots)", 0x1405, SkillName.Blacksmith, 58.6, 90.0, 12, typeof ( WarFork ) ),
			};

		private static ItemListEntry[] m_PolesMenu = new ItemListEntry[]
			{
				new CraftSystemItem( "Bardiche (18 ingots)", 0x0F4D, SkillName.Blacksmith, 50.4, 80.0, 18, typeof ( Bardiche ) ),
				new CraftSystemItem( "Halberd (20 ingots)", 0x143E, SkillName.Blacksmith, 60.2, 90.0, 20, typeof ( Halberd ) ),
			};
	
		private static ItemListEntry[] m_WeaponsMenu = new ItemListEntry[]
			{
				new CraftSubMenu( "Swords & Blades", 0x13B9, m_SwordsMenu ),
				new CraftSubMenu( "Axes", 0xF49, m_AxesMenu ),
				new CraftSubMenu( "Maces & Hammers", 0x1407, m_MacesMenu ),
				new CraftSubMenu( "Spears & Forks", 0x1403, m_SpearsMenu ),
				new CraftSubMenu( "Polearms", 0x0F4D, m_PolesMenu ),
			};

		private static ItemListEntry[] m_MainMenu = new ItemListEntry[]
			{
				new CraftMenuCallback( new CraftCallback( On_RepairItem ), "Repair", 0xFAF ),
				new CraftSubMenu( "Shields", 0x1B72, m_ShieldsMenu ),
				new CraftSubMenu( "Armor", 0x1415, m_ArmorMenu ),
				new CraftSubMenu( "Weapons", 0x13B9, m_WeaponsMenu ),
			};

		private static void On_RepairItem( CraftSystem sys, Mobile from )
		{
			if ( sys != null && sys is BlacksmithSystem )
			{
				if ( sys.CheckTool() )
				{
					from.BeginTarget( 1, false, TargetFlags.None, new TargetCallback( BlacksmithSystem.RepairTarget ) );
					from.SendAsciiMessage( "What would you like to repair?" );
				}
				sys.End();
			}
		}

		private static void RepairTarget( Mobile from, object target )
		{
			from.PlaySound( 42 ); // anvil sound
			if ( target is BaseArmor )
			{
				BaseArmor item = (BaseArmor)target;
				if ( item.MaterialType != ArmorMaterialType.Chainmail && item.MaterialType != ArmorMaterialType.Plate && item.MaterialType != ArmorMaterialType.Ringmail )
				{
					from.SendAsciiMessage( "You cannot repair that!" );
				}
				else if ( item.MaxHitPoints <= 0 || item.HitPoints >= item.MaxHitPoints*0.95 )
				{
					from.SendAsciiMessage( "That item is already in full repair." );
				}
				else
				{
					item.MaxHitPoints--;
					item.HitPoints--;

					if ( item.HitPoints <= 0 || item.MaxHitPoints < 3 )
					{
						from.SendAsciiMessage( "You destroyed the item." );
						item.Delete();
					}
					else
					{
						int skill = ( ( item.MaxHitPoints - item.HitPoints ) * 125 ) / ( item.MaxHitPoints );
						if ( skill < 0 )
							skill = 0;
						else if ( skill > 100 )
							skill = 100;
						if ( from.CheckSkill( SkillName.Blacksmith, skill - 25.0, skill + 25.0 ) )
						{
							item.HitPoints = item.MaxHitPoints;
							from.SendAsciiMessage( "You repair the item." );
						}
						else
						{
							from.SendAsciiMessage( "You failed to repair the item." );
						}
					}
				}
			}
			else if ( target is BaseWeapon )
			{
				BaseWeapon item = (BaseWeapon)target;
				if ( item is BaseStaff || item is BaseRanged )
				{
					from.SendAsciiMessage( "You cannot repair that." );
				}
				else if ( item.MaxHits <= 0 || item.Hits >= item.MaxHits*0.95 )
				{
					from.SendAsciiMessage( "That item is already in full repair." );
				}
				else
				{
					item.MaxHits--;
					item.Hits--;

					if ( item.Hits <= 0 || item.MaxHits < 3 )
					{
						from.SendAsciiMessage( "You destroyed the item." );
						item.Delete();
						return;
					}

					int skill = ( ( item.MaxHits - item.Hits ) * 125 ) / ( item.MaxHits );
					if ( skill < 0 )
						skill = 0;
					else if ( skill > 100 )
						skill = 100;
					if ( from.CheckSkill( SkillName.Blacksmith, skill - 25.0, skill + 25.0 ) )
					{
						item.Hits = item.MaxHits;
						from.SendAsciiMessage( "You repair the item." );
					}
					else
					{
						from.SendAsciiMessage( "You failed to repair the item." );
					}
				}
			}
			else
			{
				from.SendAsciiMessage( "You can't repair that!" );
			}
		}

		public override int SoundEffect { get { return 42; } }
		public override bool NeedTarget { get { return false; } }

		public static bool IsForge( Item item )
		{
			return item.GetType().IsDefined( typeof( ForgeAttribute ), true ) || 
				item.ItemID == 4017 || 
				( item.ItemID >= 6522 && item.ItemID <= 6569 );
		}

		public override bool Begin(Mobile from, BaseTool tool)
		{
			bool forge = false, anvil = false;
			IPooledEnumerable eable = from.GetItemsInRange( 3 );
			foreach ( Item item in eable )
			{
				if ( IsForge( item ) )
					forge = true;
				else if ( item.GetType().IsDefined( typeof( AnvilAttribute ), true ) || item.ItemID == 0xFB0 )
					anvil = true;
			}
			eable.Free();

			if ( anvil && forge )
			{
				if ( base.Begin( from, tool ) )
				{
					if ( ShowMenu( m_MainMenu ) )
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
					return false;
				}
			}
			else if ( anvil && !forge )
			{
				from.SendAsciiMessage( "You are not near a forge." );
			}
			else if ( forge && !anvil )
			{
				from.SendAsciiMessage( "You are not near an anvil." );
			}
			else // !forge && !anvil
			{
				from.SendAsciiMessage( "You are not near a forge or an anvil." );
			}

			return false;
		}

		protected override bool HasResources(int count, Type toMake)
		{
			int total = 0;
			if ( m_Mobile.Backpack != null )
			{
				Item[] ingots = m_Mobile.Backpack.FindItemsByType( typeof( BaseIngot ), true );
				for (int i=0;ingots != null && i < ingots.Length;i++)
					total += ingots[i].Amount;
			}

			return count <= total && CheckTool();
		}

		protected override bool ConsumeResources(int count, Type toMake)
		{
			return m_Mobile.Backpack == null ? false : m_Mobile.Backpack.ConsumeTotal( typeof( BaseIngot ), count, true );
		}

		public override void Finish( CraftSystemItem item )
		{
			if ( !CheckTool() )
			{
				End();
				return;
			}

			if ( !ConsumeResources( item.ResourceCost, item.CreateType ) )
			{
				OnNotEnoughResources();
			}
			else if ( !m_Mobile.CheckSkill( item.Skill, item.MinSkill, item.MaxSkill ) )
			{
				OnFailure();
			}
			else
			{
				Item obj = item.Create() as Item;
				if ( obj != null )
				{
					OnSuccess( obj );
					
					if ( m_Mobile.Skills[SkillName.Blacksmith].Value >= item.MaxSkill || ( item.MaxSkill > 100 && m_Mobile.Skills[SkillName.Blacksmith].Value > 99.0 && Utility.Random( 10 ) == 0 ) )
					{
						if ( obj is BaseArmor )
							((BaseArmor)obj).Quality = CraftQuality.Exceptional;
						else if ( obj is BaseWeapon )
							((BaseWeapon)obj).Quality = CraftQuality.Exceptional;
						if ( m_Mobile.Skills[SkillName.Blacksmith].Value >= 99.0 )
						{
							if ( obj is BaseArmor )
								((BaseArmor)obj).Crafter = m_Mobile;
							else if ( obj is BaseWeapon )
								((BaseWeapon)obj).Crafter = m_Mobile;
						}
						m_Mobile.SendAsciiMessage( "Due to your exceptional skill, it's quality is higher than average." );
					}
					else if ( m_Mobile.Skills[SkillName.Blacksmith].Value <= item.MinSkill+((item.MaxSkill-item.MinSkill)*0.25) )
					{
						if ( obj is BaseArmor )
							((BaseArmor)obj).Quality = CraftQuality.Low;
						else if ( obj is BaseWeapon )
							((BaseWeapon)obj).Quality = CraftQuality.Low;
						m_Mobile.SendAsciiMessage( "You were barely able to make this item.  It's quality is below average." );
					}
				}
				else
				{
					OnFailure();
				}
			}

			End();
		}
	}
}

