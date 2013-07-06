using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Harvest;
using Server.ContextMenus;

namespace Server.Items
{
	interface IUsesRemaining
	{
		int UsesRemaining{ get; set; }
		bool ShowUsesRemaining{ get; set; }
	}

	public abstract class BaseHarvestTool : BaseItem, IUsesRemaining
	{
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		public bool ShowUsesRemaining{ get{ return true; } set{} }

		public abstract HarvestSystem HarvestSystem{ get; }

		public BaseHarvestTool( int itemID ) : this( 50, itemID )
		{
		}

		public BaseHarvestTool( int usesRemaining, int itemID ) : base( itemID )
		{
			m_UsesRemaining = usesRemaining;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public virtual void DisplayDurabilityTo( Mobile m )
		{
			//LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString() ); // Durability
		}

		public override void OnSingleClick( Mobile from )
		{
			//DisplayDurabilityTo( from );

			base.OnSingleClick( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) || Parent == from )
				HarvestSystem.BeginHarvesting( from, this );
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			AddContextMenuEntries( from, this, list, HarvestSystem );
		}

        public static void AddContextMenuEntries(Mobile from, Item item, List<ContextMenus.ContextMenuEntry> list, HarvestSystem system)
		{
			if ( system != Mining.System )
				return;

			if ( !item.IsChildOf( from.Backpack ) && item.Parent != from )
				return;

			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return;

			list.Add( new ContextMenuEntry( pm.ToggleMiningStone ? 6179 : 6178 ) );
			list.Add( new ToggleMiningStoneEntry( pm, false, 6176 ) );
			list.Add( new ToggleMiningStoneEntry( pm, true, 6177 ) );
		}

		private class ToggleMiningStoneEntry : ContextMenuEntry
		{
			private PlayerMobile m_Mobile;
			private bool m_Value;

			public ToggleMiningStoneEntry( PlayerMobile mobile, bool value, int number ) : base( number )
			{
				m_Mobile = mobile;
				m_Value = value;

				Enabled = ( mobile.ToggleMiningStone != value );

				if ( value && Enabled )
					Enabled = ( mobile.StoneMining && mobile.Skills[SkillName.Mining].Base >= 100.0 );
			}

			public override void OnClick()
			{
				bool oldValue = m_Mobile.ToggleMiningStone;

				m_Mobile.ToggleMiningStone = ( m_Value && m_Mobile.StoneMining && m_Mobile.Skills[SkillName.Mining].Base >= 100.0 );

				if ( m_Mobile.ToggleMiningStone == oldValue )
				{
					if ( m_Mobile.ToggleMiningStone )
						m_Mobile.SendLocalizedMessage( 1054023 ); // You are already set to mine both ore and stone!
					else
						m_Mobile.SendLocalizedMessage( 1054021 ); // You are already set to mine only ore!
				}
				else
				{
					if ( m_Mobile.ToggleMiningStone )
						m_Mobile.SendLocalizedMessage( 1054022 ); // You are now set to mine both ore and stone.
					else
						m_Mobile.SendLocalizedMessage( 1054020 ); // You are now set to mine only ore.
				}
			}
		}

		public BaseHarvestTool( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_UsesRemaining = reader.ReadInt();
					break;
				}
			}
		}
	}
}