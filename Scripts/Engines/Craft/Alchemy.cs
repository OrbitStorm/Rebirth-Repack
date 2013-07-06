using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Network;

namespace Server.Engines.Craft
{
	public class AlchemySystem : CraftSystem
	{
		private static ItemListEntry[] m_RefreshMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Refresh Potion",			0x0F0B, SkillName.Alchemy, -25.0, 25.0, 1, typeof ( RefreshPotion ) ),
			new CraftSystemItem( "Total Refresh Potion",	0x0F0B, SkillName.Alchemy, 25.0, 75.0, 5, typeof ( TotalRefreshPotion ) ),
		};

		private static ItemListEntry[] m_CureMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Lesser Cure Potion",		0xF07, SkillName.Alchemy, -10.0, 40.0, 1, typeof ( LesserCurePotion ) ),
			new CraftSystemItem( "Cure Potion",				0xF07, SkillName.Alchemy, 25.0, 75.0, 3, typeof ( CurePotion ) ),
			new CraftSystemItem( "Greater Cure Potion",		0xF07, SkillName.Alchemy, 65.0, 115.0, 6, typeof ( GreaterCurePotion ) ),
		};

		private static ItemListEntry[] m_AgilityMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Agility Potion",			0xF08, SkillName.Alchemy, 15.0, 65.0, 1, typeof ( AgilityPotion ) ),
			new CraftSystemItem( "Greater Agility Potion",	0xF08, SkillName.Alchemy, 35.0, 85.0, 3, typeof ( GreaterAgilityPotion ) ),
		};

		private static ItemListEntry[] m_ExpMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Lesser Exploison Potion", 0xF0D, SkillName.Alchemy, 5.0, 55.0, 3, typeof ( LesserExplosionPotion ) ),
			new CraftSystemItem( "Exploison Potion",		0xF0D, SkillName.Alchemy, 35.0, 85.0, 5, typeof ( ExplosionPotion ) ),
			new CraftSystemItem( "Greater Exploison Potion",0xF0D, SkillName.Alchemy, 65.0, 115.0, 10, typeof ( GreaterExplosionPotion ) ),
		};

		private static ItemListEntry[] m_HealMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Lesser Heal Potion",		0xF0C, SkillName.Alchemy, -15.0, 35.0, 1, typeof ( LesserHealPotion ) ),
			new CraftSystemItem( "Heal Potion",				0xF0C, SkillName.Alchemy, 15.0, 65.0, 3, typeof ( HealPotion ) ),
			new CraftSystemItem( "Greater Heal Potion",		0xF0C, SkillName.Alchemy, 55.0, 105.0, 7, typeof ( GreaterHealPotion ) ),
		};

		private static ItemListEntry[] m_NightSightMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Night Sight Potion",		0xF06, SkillName.Alchemy, -25.0, 25.0, 1, typeof ( NightSightPotion ) ),
		};

		private static ItemListEntry[] m_PoisonMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Lesser Poison Potion",	0xF0A, SkillName.Alchemy, -5.0, 45.0, 1, typeof ( LesserPoisonPotion ) ),
			new CraftSystemItem( "Poison Potion",			0xF0A, SkillName.Alchemy, 15.0, 65.0, 2, typeof ( PoisonPotion ) ),
			new CraftSystemItem( "Greater Poison Potion",	0xF0A, SkillName.Alchemy, 55.0, 105.0, 4, typeof ( GreaterPoisonPotion ) ),
			new CraftSystemItem( "Deadly Potion",			0xF0A, SkillName.Alchemy, 90.0, 140.0, 8, typeof ( DeadlyPoisonPotion ) ),
		};

		private static ItemListEntry[] m_StrMenu = new ItemListEntry[]
		{
			new CraftSystemItem( "Strength Potion",			0xF09, SkillName.Alchemy, 25.0, 75.0, 2, typeof ( StrengthPotion ) ),
			new CraftSystemItem( "Greater Strength Potion",	0xF09, SkillName.Alchemy, 45.0, 95.0, 5, typeof ( GreaterStrengthPotion ) ),
		};

		private Item m_Res;

		public override string TargetPrompt { get { return "What reagent would you like to make the potion out of?"; } }

		public override bool Begin( Mobile from, BaseTool tool )
		{
			if ( tool is MortarPestle )
				return base.Begin(from, tool);
			else
				return false;
		}

		protected override bool HasResources(int count, Type toMake)
		{
			return m_Res != null && m_Res.Amount >= count && m_Mobile.Alive ;
		}

		protected override bool ConsumeResources(int count, Type toMake)
		{
			if ( HasResources(count, toMake) )
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

		protected override bool OnTarget( Item target )
		{
			if ( !CheckTool() )
				return false;

			Type type = target.GetType();
			m_Res = target;
			if ( type == typeof( BlackPearl ) )
				return ShowMenu( m_RefreshMenu );
			else if ( type == typeof( Garlic ) )
				return ShowMenu( m_CureMenu );
			else if ( type == typeof( Bloodmoss ) )
				return ShowMenu( m_AgilityMenu );
			else if ( type == typeof( SulfurousAsh ) )
				return ShowMenu( m_ExpMenu );
			else if ( type == typeof( Ginseng ) )
				return ShowMenu( m_HealMenu );
			else if ( type == typeof( SpidersSilk ) )
				return ShowMenu( m_NightSightMenu );
			else if ( type == typeof( Nightshade ) )
				return ShowMenu( m_PoisonMenu );
			else if ( type == typeof( MandrakeRoot ) )
				return ShowMenu( m_StrMenu );
			else
				return false;
		}

		public override void OnItemSelected( CraftSystemItem item )
		{
			if ( !ConsumeResources( item.ResourceCost, item.CreateType ) )
				OnNotEnoughResources();
			else
				new AlchemyTimer( item, this ).Start();
		}

		protected override void OnNotEnoughResources()
		{
			m_Mobile.SendAsciiMessage( "You do not have enough to make that." );
			End();
		}

		protected override void OnFailure()
		{
			m_Mobile.PublicOverheadMessage( MessageType.Emote, 0x25, true, String.Format( "*{0} tosses out the contents of the mortar, unable to make a potion from it.*", m_Mobile.Name ) );
			End();
		}

		protected override void OnSuccess( Item made )
		{
			m_Mobile.PublicOverheadMessage( MessageType.Emote, 0x25, true, String.Format( "*{0} pours the completed potion into a bottle.*", m_Mobile.Name ) );
			m_Mobile.PlaySound( 0x240 );
			m_Mobile.AddToBackpack( made );
			End();
		}

		private class AlchemyTimer : Timer
		{
			private Mobile m_From;
			private AlchemySystem m_Sys;
			private CraftSystemItem m_Item;
			private int m_Index;

			private static string[] m_Messages = new string[]
				{
					"*{0} starts grinding some {1} in the mortar.*",
					"*{0} continues grinding.*",
					"*{0} continues grinding.*",
				};

			public AlchemyTimer( CraftSystemItem item, AlchemySystem sys ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 2.75 ) )
			{
				m_From = sys.Mobile;
				m_Sys = sys;
				m_Item = item;

				Priority = TimerPriority.TwoFiftyMS;
				m_Index = 0;
			}

			protected override void OnTick()
			{
				if ( m_From.Deleted || !m_From.Alive )
				{
					Stop();
					m_Sys.End();
					return;
				}

				if ( m_Index < m_Messages.Length )
				{
					m_Sys.Mobile.PlaySound( 0x242 ); // grinding sound
					m_Sys.Mobile.PublicOverheadMessage( MessageType.Emote, 0x25, true, String.Format( m_Messages[m_Index], m_Sys.Mobile.Name, m_Sys.m_Res.Name == null || m_Sys.m_Res.Name.Length == 0 ? m_Sys.m_Res.ItemData.Name : m_Sys.m_Res.Name ) );
					m_Index++;
				}
				else
				{
					m_Sys.Finish( m_Item );
					Stop();
				}
			}
		}

		public override void Finish( CraftSystemItem item )
		{
			if ( m_Mobile.Deleted || !m_Mobile.Alive )
			{
				End();
				return;
			}

			if ( !m_Mobile.CheckSkill( item.Skill, item.MinSkill, item.MaxSkill ) )
			{
				OnFailure();
			}
			else
			{
				Item bottle = null;
				if ( m_Mobile.Backpack != null )
					bottle = m_Mobile.Backpack.FindItemByType( typeof( Bottle ), true );

				if ( bottle != null && bottle.Amount > 0 )
				{
					Item obj = item.Create() as Item;
					if ( obj != null )
					{
						bottle.Consume();
						OnSuccess( obj );
					}
					else
					{
						OnFailure();
					}
				}
				else
				{
					((MortarPestle)Tool).PotionToMake = item.CreateType;
					Tool.OnDoubleClick( m_Mobile );
				}
			}

			End();
		}
	}
}
