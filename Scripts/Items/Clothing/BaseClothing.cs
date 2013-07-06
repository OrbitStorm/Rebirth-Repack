using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Network;
using System.Text;


namespace Server.Items
{
	public interface IArcaneEquip
	{
		bool IsArcane{ get; }
		int CurArcaneCharges{ get; set; }
		int MaxArcaneCharges{ get; set; }
	}

	public abstract class BaseClothing : BaseItem, IDyable, IScissorable, IIdentifiable
	{
		private Mobile m_Crafter;
		private CraftQuality m_Quality;
		private bool m_PlayerConstructed;

		private ArrayList m_Identified = new ArrayList();
		private SpellEffect m_Effect;
		private int m_EffectCharges;

		private Timer m_Timer = null;

		[CommandProperty( AccessLevel.GameMaster )]
		public SpellEffect SpellEffect
		{
			get{ return m_Effect; }
			set{ m_Effect = value; SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpellCharges
		{
			get{ return m_EffectCharges; }
			set{ m_EffectCharges = value; SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftQuality Quality
		{
			get{ return m_Quality; }
			set{ m_Quality = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed
		{
			get{ return m_PlayerConstructed; }
			set{ m_PlayerConstructed = value; }
		}

		public BaseClothing( int itemID, Layer layer ) : this( itemID, layer, 0 )
		{
		}

		public BaseClothing( int itemID, Layer layer, int hue ) : base( itemID )
		{
			Layer = layer;
			Hue = hue;
			m_Quality = CraftQuality.Regular;
		}

		public BaseClothing( Serial serial ) : base( serial )
		{
		}

		public override bool CheckPropertyConfliction( Mobile m )
		{
			if ( base.CheckPropertyConfliction( m ) )
				return true;

			if ( Layer == Layer.Pants )
				return ( m.FindItemOnLayer( Layer.InnerLegs ) != null );

			if ( Layer == Layer.Shirt )
				return ( m.FindItemOnLayer( Layer.InnerTorso ) != null );

			return false;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == CraftQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			base.AddResistanceProperties( list );
		}

		public bool IsMagic
		{
			get
			{
				return ( Name == null || Name.Length <= 0 ) && m_Effect != SpellEffect.None && m_EffectCharges > 0;
			}
		}

		public void OnIdentify( Mobile from )
		{
			if ( IsMagic && from.AccessLevel == AccessLevel.Player )
				m_Identified.Add( from );
		}

		private Packet m_MagicSingleClick = null;

		public override void SingleClickChanged()
		{
			base.SingleClickChanged ();
			Packet.Release( ref m_MagicSingleClick );
		}

		public override void SendSingleClickTo( Mobile from )
		{
			if ( !IsMagic || !( m_Identified.Contains( from ) || from.AccessLevel > AccessLevel.Counselor ) )
			{
				base.SendSingleClickTo( from );
			}
			else
			{
				if ( m_MagicSingleClick == null )
				{
					m_MagicSingleClick = new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", BuildMagicSingleClick() );
					m_MagicSingleClick.SetStatic();
				}

				from.NetState.Send( m_MagicSingleClick );
			}
		}

		public override bool DisplayLootType
		{
			get
			{
				return true;
			}
		}


		public override string BuildSingleClick()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			if ( AppendLootType( sb ) )
				sb.Append( ", " );
			
			if ( m_Quality == CraftQuality.Exceptional )
				sb.Append( "exceptional, " );

			if ( IsMagic )
				sb.Append( "magic, " );

			if ( sb.Length > 2 )
			{
				sb.Remove( sb.Length - 2, 2 );
				sb.Append( ' ' );
			}

			AppendClickName( sb );
			InsertNamePrefix( sb );

			if ( m_Crafter != null && !m_Crafter.Deleted )
				sb.AppendFormat( " (crafted by {0})", m_Crafter.Name );

			return sb.ToString();
		}

		public virtual string BuildMagicSingleClick()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			if ( AppendLootType( sb ) )
				sb.Append( ", " );
			
			if ( m_Quality == CraftQuality.Exceptional )
				sb.Append( "exceptional, " );

			if ( sb.Length > 2 )
			{
				sb.Remove( sb.Length - 2, 2 );
				sb.Append( ' ' );
			}

			AppendClickName( sb );
			InsertNamePrefix( sb );

			if ( IsMagic )
				sb.AppendFormat( " of {0} with {1} charge{2}", SpellCastEffect.GetName( m_Effect ), m_EffectCharges, m_EffectCharges != 1 ? "s" : "" );

			if ( m_Crafter != null && !m_Crafter.Deleted )
				sb.AppendFormat( " (crafted by {0})", m_Crafter.Name );

			return sb.ToString();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 4 ); // version

			writer.WriteMobileList( m_Identified, true );

			writer.Write( (int)m_Effect );
			writer.Write( (int)m_EffectCharges );
			//writer.Write( m_Identified );

			writer.Write( (bool) m_PlayerConstructed );

			writer.Write( (Mobile) m_Crafter );
			writer.Write( (int) m_Quality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 4:
				{
					m_Identified = reader.ReadMobileList();

					goto case 3;
				}
				case 3:
				{
					m_Effect = (SpellEffect)reader.ReadInt();
					m_EffectCharges = reader.ReadInt();
					if ( version < 4 )
						/*m_Identified = */reader.ReadBool();
					goto case 2;
				}
				case 2:
				{
					m_PlayerConstructed = reader.ReadBool();
					goto case 1;
				}
				case 1:
				{
					m_Crafter = reader.ReadMobile();
					m_Quality = (CraftQuality)reader.ReadInt();
					break;
				}
				case 0:
				{
					m_Crafter = null;
					m_Quality = CraftQuality.Regular;
					break;
				}
			}

			if ( version < 2 )
				m_PlayerConstructed = false;

			if ( Parent is Mobile && m_Effect != SpellEffect.None && m_EffectCharges > 0 && SpellCastEffect.IsRepeatingEffect( m_Effect ) )
			{
				m_Timer = new CheckTimer( this );
				m_Timer.Start();
			}
		}

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;
			else if ( RootParent is Mobile && from != RootParent )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public bool Scissor( Mobile from, Scissors scissors )
		{
			/*if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 502437 ); // Items you wish to cut must be in your backpack.
				return false;
			}

			int res = Server.Engines.Craft.TailoringSystem.GetResourcesFor( this.GetType() );
			if ( res > 0 )
			{
				Item bandage = new Bandage( res );
				bandage.Hue = this.Hue;
				from.AddToBackpack( bandage );
				this.Delete();

				from.SendAsciiMessage( "You cut the item into bandages and place them in your backpack." );
				return true;
			}
			else*/
			{
				from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
				return false;
			}
		}

		public virtual bool SpellEffectOnEquip { get { return m_Effect != SpellEffect.Teleportation; } }

		public override void OnDoubleClick(Mobile from)
		{
			if ( from.Alive && !from.Deleted && m_Effect != SpellEffect.None && SpellCharges > 0 )
			{
				if ( !SpellCastEffect.IsRepeatingEffect( m_Effect ) && SpellCastEffect.InvokeEffect( m_Effect, from, from ) )
				{
					SpellCharges--;
					if ( SpellCharges <= 0 )
					{
						m_Effect = SpellEffect.None;
						from.SendAsciiMessage( "This magic item is out of charges." );
					}
				}					
			}
			base.OnDoubleClick (from);
		}

		public override bool OnEquip(Mobile from)
		{
			if ( base.OnEquip(from) )
			{
				if ( SpellEffectOnEquip && from.Alive && !from.Deleted && m_Effect != SpellEffect.None && SpellCharges > 0 )
				{
					if ( SpellCastEffect.InvokeEffect( m_Effect, from, from ) )
					{
						SpellCharges--;
						if ( SpellCharges <= 0 )
						{
							if ( m_Timer != null )
							{
								m_Timer.Stop();
								m_Timer = null;
							}

							m_Effect = SpellEffect.None;
							from.SendAsciiMessage( "This magic item is out of charges." );
						}
					}

					if ( SpellCharges > 0 && SpellCastEffect.IsRepeatingEffect( m_Effect ) )
					{
						if ( m_Timer == null )
							m_Timer = new CheckTimer( this );
						if ( !m_Timer.Running )
							m_Timer.Start();
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public override void OnRemoved(object parent)
		{
			if ( m_Timer != null && m_Timer.Running )
				m_Timer.Stop();
			base.OnRemoved(parent);
		}

		private class CheckTimer : Timer
		{
			private BaseClothing m_Item;

			public CheckTimer( BaseClothing item ) : base( TimeSpan.FromSeconds( Utility.RandomMinMax( 15, 45 ) ), TimeSpan.FromSeconds( 15.0 ) )
			{
				m_Item = item;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				Mobile from = m_Item.Parent as Mobile;
				if ( from == null )
				{
					Stop();
					return;
				}

				if ( m_Item.SpellCharges <= 0 )
				{
					Stop();
					m_Item.m_Timer = null;
					m_Item.SpellEffect = SpellEffect.None;
					return;
				}

				if ( from.Player && from.Map == Map.Internal )
					return;

				if ( SpellCastEffect.InvokeEffect( m_Item.SpellEffect, from, from ) )
				{
					m_Item.SpellCharges--;
					if ( m_Item.SpellCharges <= 0 )
					{
						Stop();
						m_Item.m_Timer = null;
						m_Item.SpellEffect = SpellEffect.None;
						from.SendAsciiMessage( "This magic item is out of charges." );
					}
				}
			}
		}
	}
}
