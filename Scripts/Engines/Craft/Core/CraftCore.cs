using System;
using System.Collections; using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;
using Server.Menus;
using System.Reflection;
using Server.Menus.ItemLists;
using Server.Network;
using Server.Targeting;

namespace Server.Engines.Craft
{
	public class CraftSystemItem : ItemListEntry
	{
		private int m_Cost;
		private Type m_ToCreate;
		private double m_MinSkill, m_MaxSkill;
		private SkillName m_Skill;

		public CraftSystemItem( string name, int itemid, SkillName skill, double minSkill, double maxSkill, int resCost, Type toCreate ) : this( name, itemid, 0, skill, minSkill, maxSkill, resCost, toCreate )
		{
		}

		public CraftSystemItem( string name, int itemid, int hue, SkillName skill, double minSkill, double maxSkill, int resCost, Type toCreate ) : base( name, itemid, hue )
		{
			m_MinSkill = minSkill;
			m_MaxSkill = maxSkill;
			m_Cost = resCost;
			m_ToCreate = toCreate;
			m_Skill = skill;
		}

		public int ResourceCost { get { return m_Cost; } }
		public Type CreateType { get { return m_ToCreate; } }
		public double MinSkill { get { return m_MinSkill; } }
		public double MaxSkill { get { return m_MaxSkill; } }
		public SkillName Skill { get { return m_Skill; } }

		public virtual object Create()
		{
			try
			{
				ConstructorInfo ctor = m_ToCreate.GetConstructor( Type.EmptyTypes );
				return ctor == null ? null : ctor.Invoke( null );
			}
			catch
			{
				return null;
			}
		}
	}

	public class CraftSubMenu : ItemListEntry
	{
		private ItemListEntry[] m_SubItems;

		public CraftSubMenu( string name, int itemid, int hue, ItemListEntry[] subItems ) : base(name, itemid, hue)
		{
			m_SubItems = subItems;
		}

		public CraftSubMenu( string name, int itemid, ItemListEntry[] subItems ) : this(name,itemid,0,subItems)
		{ 
		}

		public ItemListEntry[] SubItems 
		{ 
			get { return m_SubItems; } 
			set { m_SubItems = value; }
		}
	}

	public delegate void CraftCallback( CraftSystem sys, Mobile from );
	public class CraftMenuCallback : ItemListEntry
	{
		private CraftCallback m_Callback;

		public CraftMenuCallback( CraftCallback callback, string name, int itemid ) : this( callback, name, itemid, 0 )
		{
		}

		public CraftMenuCallback( CraftCallback callback, string name, int itemid, int hue ) : base( name, itemid, hue )
		{
			m_Callback = callback;
		}

		public CraftCallback Callback { get { return m_Callback; } }
	}

	public class CraftMenu : ItemListMenu
	{
		private CraftSystem m_System;

		public CraftMenu( CraftSystem system, ItemListEntry[] entries ) : base( "What would you like to make?", entries )
		{
			m_System = system;
		}

		public override void OnCancel( NetState state )
		{
			m_System.MenuCanceled();
		}

		public override void OnResponse( NetState state, int index )
		{
			if ( index < 0 || index >= Entries.Length || state.Mobile.Deleted || !state.Mobile.Alive )
			{
				m_System.End();
				return;
			}
			
			ItemListEntry entry = Entries[index];
			if ( entry is CraftSubMenu )
				m_System.ShowMenu( ((CraftSubMenu)entry).SubItems );
			else if ( entry is CraftSystemItem )
				m_System.OnItemSelected( (CraftSystemItem)entry );
			else if ( entry is CraftMenuCallback )
                ((CraftMenuCallback)entry).Callback( m_System, state.Mobile );
			else
				m_System.End();
		}
	}

	public abstract class CraftSystem
	{
		private static TimerStateCallback m_EndCraftingCallback = new TimerStateCallback( EndCraftingCallback );

		public static void Initialize()
		{
			EventSink.Disconnected += new DisconnectedEventHandler( OnDisconnected );
		}

		public static void OnDisconnected( DisconnectedEventArgs args )
		{
			args.Mobile.EndAction( typeof( CraftSystem ) );
		}

		private static void EndCraftingCallback( object state )
		{
			if ( state is Mobile )
				((Mobile)state).EndAction( typeof( CraftSystem ) );
		}

		protected abstract bool ConsumeResources( int count, Type toMake );
		protected abstract bool HasResources( int count, Type toMake );
		
		public virtual bool NeedTarget { get { return true; } }
		public virtual TimeSpan CraftDelay { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public virtual int SoundEffect { get { return -1; } }
		public virtual string TargetPrompt { get { return "Target the resource."; } }

		protected Mobile m_Mobile;
		public Mobile Mobile { get{ return m_Mobile; } }

		protected Timer m_Timeout = null;

		public CraftSystem()
		{
		}

		public virtual bool ShowMenu( ItemListEntry[] entries )
		{
			if ( m_Mobile.Deleted || !m_Mobile.Alive )
				return false;

			ItemListEntry[] menu = BuildMenu( entries );
			if ( menu == null || menu.Length <= 0 )
			{
				OnNothingToMake();
				return false;
			}
			else
			{
				if ( m_Mobile.NetState != null )
				{
					new CraftMenu( this, menu ).SendTo( m_Mobile.NetState );
					return true;
				}
				else
				{
					End();
					return false;
				}
			}
		}
		
		public virtual void OnItemSelected( CraftSystemItem item )
		{
			if ( m_Mobile.Deleted || !m_Mobile.Alive )
			{
				End();
				return;
			}

			if ( this.SoundEffect != -1 )
				m_Mobile.PlaySound( this.SoundEffect );

			new CraftTimer( CraftDelay, item, this ).Start();
		}

		private class CraftTimer : Timer
		{
			CraftSystemItem m_Item;
			CraftSystem m_Sys;

			public CraftTimer( TimeSpan delay, CraftSystemItem item, CraftSystem sys ) : base( delay )
			{
				this.Priority = TimerPriority.TwoFiftyMS;
				m_Sys = sys;
				m_Item = item;
			}

			protected override void OnTick()
			{
				m_Sys.Finish( m_Item );
			}
		}

		private BaseTool m_Tool;
		private Timer m_TimeoutTimer;
		public BaseTool Tool { get { return m_Tool; } }

		public virtual bool Begin( Mobile from, BaseTool tool )
		{
			if ( from.Deleted || !from.Alive )
				return false;

			if ( !from.BeginAction( typeof( CraftSystem ) ) )
			{
				from.SendAsciiMessage( "You cannot craft two things at once!" );
				return false;
			}

			m_TimeoutTimer = Timer.DelayCall( TimeSpan.FromMinutes( 1.0 ), m_EndCraftingCallback, from );

			m_Mobile = from;

			m_Tool = tool;

			if ( NeedTarget )
			{
				m_Mobile.SendAsciiMessage( TargetPrompt );
				m_Mobile.Target = new CraftTarget( this );
			}

			return true;
		}

		private class CraftTarget : Target
		{
			private CraftSystem m_Sys;
			private bool m_End;

			public CraftTarget( CraftSystem sys ) : base( 0, false, TargetFlags.None )
			{
				m_End = true;
				m_Sys = sys;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				m_End = false;
				m_Sys.TargetRes_Callback( targeted );
			}

			protected override void OnTargetFinish(Mobile from)
			{
				if ( m_End )
					m_Sys.TargetCanceled();
			}
		}

		public virtual void End()
		{
			if ( m_Mobile != null )
				m_Mobile.EndAction( typeof( CraftSystem ) );
			if ( m_TimeoutTimer != null )
				m_TimeoutTimer.Stop();
		}

		public virtual bool CheckTool()
		{
			if ( m_Mobile == null || m_Mobile.Deleted || !m_Mobile.Alive )
				return false;

			if ( m_Tool != null && m_Tool.RootParent != m_Mobile )
			{
				m_Mobile.SendAsciiMessage( "You can't seem to find the tool you were working with." );
				return false;
			}
			else
			{
				return true;
			}
		}

		public virtual void Finish( CraftSystemItem item )
		{
			if ( !m_Mobile.Deleted && m_Mobile.Alive && CheckTool() )
			{
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
						OnSuccess( obj );
					else
						OnFailure();
				}
			}
			End();
		}

		protected virtual bool OnTarget( Item target )
		{
			return false;
		}

		private void TargetRes_Callback( object targeted )
		{
			if ( m_Mobile.Deleted || !m_Mobile.Alive || !CheckTool() )
			{
				End();
				return;
			}

			Item targ = targeted as Item;
			if ( targ != null )
			{
				if ( !targ.IsChildOf( m_Mobile ) )
				{
					m_Mobile.SendAsciiMessage( "That belongs to someone else." );
					End();
				}
				else if ( OnTarget( targ ) )
				{
					if ( m_Tool != null && Utility.Random( 3 ) == 1 )
					{
						m_Tool.UsesRemaining--;

						if ( m_Tool.UsesRemaining <= 0 )
						{
							m_Tool.Delete();
							m_Mobile.SendAsciiMessage( "You destroyed the {0}.", m_Tool.Name != null && m_Tool.Name.Length > 0 ? m_Tool.Name : m_Tool.ItemData.Name );
						}
					}
				}
				else
				{
					m_Mobile.SendAsciiMessage( "You cannot make anything with that." );
					End();
				}
			}
			else
			{
				m_Mobile.SendAsciiMessage( "You cannot make anything with that." );
				End();
			}
		}

		protected virtual void OnNotEnoughResources()
		{
			m_Mobile.SendAsciiMessage( "You do not have enough resources to make that." );
			End();
		}

		protected virtual void OnFailure()
		{
			m_Mobile.SendAsciiMessage( "You failed to make the item." );
			End();
		}

		protected virtual void OnNothingToMake()
		{
			m_Mobile.SendAsciiMessage( "You cannot think of anything you could make with that." );
			End();
		}

		protected virtual void OnSuccess( Item made )
		{
			m_Mobile.AddToBackpack( made );
			m_Mobile.SendAsciiMessage( "You create the item and place it in your backpack." );
			End();
		}

		public virtual void MenuCanceled()
		{
			// from.SendAsciiMessage( "You decide not to make anything." );
			End();
		}

		public virtual void TargetCanceled()
		{
			End();
		}

		protected virtual bool CanCraft( CraftSystemItem item )
		{
			return m_Mobile.Skills[item.Skill].Value >= item.MinSkill && HasResources( item.ResourceCost, item.CreateType );
		}

		private static ItemListEntry[] EmptyILE = new ItemListEntry[0];
		private ItemListEntry[] BuildMenu( ItemListEntry[] entries )
		{
			if ( m_Mobile == null || entries == null || entries.Length == 0 )
				return EmptyILE;

			ArrayList list = new ArrayList();
			for (int i=0;i<entries.Length;i++)
			{
				ItemListEntry entry = entries[i];
				if ( entry is CraftSubMenu )
				{
					if ( BuildMenu( ((CraftSubMenu)entry).SubItems ).Length > 0 )
						list.Add( entry );
				}
				else if ( entry is CraftSystemItem )
				{
					CraftSystemItem item = (CraftSystemItem)entry;

					if ( CanCraft( item ) )
						list.Add( entry );
				}
				else
				{
					list.Add( entry );
				}
			} 
			return (ItemListEntry[])list.ToArray( typeof( ItemListEntry ) );
		}

		protected static int GetResourcesFor( Type find, ItemListEntry[] entries )
		{
			for(int i=0;i<entries.Length;i++)
			{
				if ( entries[i] is CraftSystemItem )
				{
					CraftSystemItem csi = (CraftSystemItem)entries[i];
					if ( csi.CreateType.IsSubclassOf( find ) )
						return csi.ResourceCost;
				}
				else if ( entries[i] is CraftSubMenu )
				{
					int sub = GetResourcesFor( find, ((CraftSubMenu)entries[i]).SubItems );
					if ( sub != -1 )
						return sub;
				}
			}

			return -1;
		}
	}
}

