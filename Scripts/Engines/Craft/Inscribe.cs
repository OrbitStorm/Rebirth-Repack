using System;
using Server;
using Server.Items;
using Server.Menus;
using Server.Spells;
using Server.Menus.ItemLists;
using Server.Network;

namespace Server.Engines.Craft
{
	public class CSI_Scroll : CraftSystemItem
	{
		private const int ItemIDOffset = 0x1F2D;
		private const int IconOffset = 0x2080;
		private int m_ID;
		private int m_Cir;

		public CSI_Scroll( int circle, int spell, int id ) : base( TileData.ItemTable[IconOffset+id].Name, IconOffset+id, SkillName.Inscribe, (circle*100.0)/7.0 - 25.0, (circle*100.0)/7.0 + 25.0, 0, null )
		{
			m_ID = id;
			m_Cir = circle;
		}

		public override object Create()
		{
			/*int id = m_ID;
			if ( id <= 6 )
			{
				if ( id == 6 )
					id = 0;
				else
					id ++;
			}
			return Loot.Construct( Loot.RegularScrollTypes, id );
			*/
			return Loot.Construct( Loot.RegularScrollTypes, m_ID );
		}

		public int SpellID { get { return m_ID; } }
		public int Circle { get { return m_Cir; } }
	}

	public class InscribeSystem : CraftSystem
	{
		private static ItemListEntry[] m_MainMenu;
		private const int CircleIDOffset = 0x20C0;
		private Item m_Scroll;

		static InscribeSystem()
		{
			m_MainMenu = new ItemListEntry[8];
			for (int c=0;c<8;c++)
			{
				ItemListEntry[] entry = new ItemListEntry[8];
				for (int n=0;n<8;n++)
					entry[n] = new CSI_Scroll( c, n, c*8+n );
				m_MainMenu[c] = new CraftSubMenu( TileData.ItemTable[CircleIDOffset+c].Name, CircleIDOffset+c, entry );
			}
		}

		public InscribeSystem( Item scroll )
		{
			m_Scroll = scroll;
		}
		
		public override TimeSpan CraftDelay { get{ return TimeSpan.FromSeconds( 5.0 ); } }

		public override bool NeedTarget
		{
			get
			{
				return false;
			}
		}

		public override int SoundEffect
		{
			get
			{
				return 0x249;
			}
		}

		protected override bool HasResources(int count, Type toMake)
		{
			return m_Mobile.Alive && m_Scroll != null && m_Scroll.IsChildOf( m_Mobile, false ) && m_Scroll.Amount > 0;
		}

		protected override bool ConsumeResources(int count, Type toMake)
		{
			if ( HasResources( count, toMake ) )
			{
				m_Scroll.Consume();
				return true;
			}
			else
			{
				return false;
			}
		}

		protected override bool CanCraft(CraftSystemItem item)
		{
			return m_Mobile.Skills[SkillName.Magery].Value >= item.MinSkill && 
				m_Mobile.Skills[SkillName.Inscribe].Value >= item.MinSkill && 
				item is CSI_Scroll && 
				Spellbook.Find( m_Mobile, ((CSI_Scroll)item).SpellID ) != null ;
		}

		public override bool Begin( Mobile from, BaseTool tool )
		{
			if ( m_Scroll != null && !m_Scroll.Deleted && m_Scroll.IsChildOf( from ) && m_Scroll.Amount > 0 )
			{
				if ( base.Begin( from, tool ) )
					return ShowMenu( m_MainMenu );
				else
					return false;
			}
			else
			{
				from.SendAsciiMessage( "That scroll must be in your pack to inscribe it." );
				return false;
			}
		}

		public override void OnItemSelected(CraftSystemItem item)
		{
			if ( m_Mobile.Deleted || !m_Mobile.Alive )
			{
				End();
				return;
			}

			CSI_Scroll scr = item as CSI_Scroll;
			if ( scr == null )
			{
				End();
				return;
			}
			if ( Spellbook.Find( m_Mobile, scr.SpellID ) == null )
			{
				m_Mobile.SendAsciiMessage( "You do not have that spell!" );
				End();
				return;
			}

			Spell spell = SpellRegistry.NewSpell( scr.SpellID, m_Mobile, null );
			if ( spell == null )
			{
				End();
				return;
			}

			int mana = spell.ScaleMana( spell.GetMana() );
			if ( m_Mobile.Mana < mana )
			{
				m_Mobile.LocalOverheadMessage( MessageType.Regular, 0x22, 502625 ); // Insufficient mana for this spell.
				End();
				return;
			}
			else if ( !spell.ConsumeReagents() )
			{
				m_Mobile.LocalOverheadMessage( MessageType.Regular, 0x22, 502630 ); // More reagents are needed for this spell.
				End();
				return;
			}
			
			m_Mobile.Mana -= mana;
			
			base.OnItemSelected( item );
		}

		protected override void OnFailure()
		{
			m_Mobile.SendAsciiMessage( "You fail to inscribe the spell, and the scroll is ruined." );
		}

		protected override void OnSuccess(Item made)
		{
			if ( made is SpellScroll )
			{
				double sk = (((SpellScroll)made).SpellID / 8) * 100.0 / 7.0;
				m_Mobile.CheckSkill( SkillName.Magery, sk - 20, sk + 20 ); // passive magery gain on success
			}
			
			m_Mobile.SendAsciiMessage( "You inscribe the spell and place the scroll in your backpack." );
			m_Mobile.AddToBackpack( made );
		}
	}
}

