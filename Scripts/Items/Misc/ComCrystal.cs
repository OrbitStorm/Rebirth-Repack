using System;
using System.Collections; using System.Collections.Generic;
using Server;
using System.Text;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
	public class ComCrystal : BaseItem, ISellPrice
	{
		public const int Range = 6;
		private int m_Charges;
		private ArrayList m_Links;
		private const int MaxCharges = 200;

		[Constructable]
		public ComCrystal() : base( 0x1ECD )
		{
			Light = LightType.Circle150;
			m_Charges = 50;
			Weight = 1.0;
			m_Links = new ArrayList();
		}

		public ComCrystal( Serial s ) : base(s) {}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_Links = reader.ReadItemList();
					m_Charges = reader.ReadInt();
					break;
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( (int)0 );
			writer.WriteItemList( m_Links );
			writer.Write( m_Charges );
		}

		public override void OnSingleClick(Mobile from)
		{
			StringBuilder sb = new StringBuilder();

			if ( Active )
				sb.Append( "an active crystal of communication" );
			else
				sb.Append( "an inactive crystal of communication" );
			sb.AppendFormat( " with {0} charge{1}", m_Charges <= MaxCharges ? m_Charges.ToString() : "infinite", m_Charges != 1 ? "s" : "" );
			
			if ( m_Links.Count > 0 )
				sb.AppendFormat( " and {0} link{1}", m_Links.Count, m_Links.Count != 1 ? "s" : "" );

			LabelTo( from, true, sb.ToString() );
		}

		public override bool HandlesOnSpeech { get { return Active && m_Charges > 0; } }

		public override void OnSpeech(SpeechEventArgs e)
		{
			base.OnSpeech (e);
			Comm( e.Mobile, e.Hue, e.Speech );
		}

		public virtual void Comm( Mobile from, int hue, string speech )
		{
			if ( !Active || m_Charges <= 0 || !from.Alive  )
				return;
			else if ( from.Hidden && from.AccessLevel > AccessLevel.Player )
				return;
			else if ( !from.InRange( GetWorldLocation(), Range ) ) // || ( RootParent is Mobile && from != RootParent ) )
				return;
			object cont = RootParent;
			if ( cont is Mobile && ( from != cont || !IsChildOf( ((Mobile)cont).Backpack, false ) ) )
				return;

			string p_str = null;
			string str = String.Format( "{0} says {1}", from.Name, speech );

			ArrayList done = new ArrayList();
			for(int i=0;i<m_Links.Count && m_Charges > 0;i++)
			{
				ComCrystal link = m_Links[i] as ComCrystal;
				if ( link == null || link.Deleted )
					continue;

				if ( link.Parent == null )
				{
					bool msg = true;
					for(int d=0;d<done.Count;d++)
					{
						if ( Utility.InRange( ((Item)done[d]).GetWorldLocation(), link.GetWorldLocation(), Range ) )
						{
							msg = false;
							break;
						}
					}
					if ( msg )
					{
						--Charges;
						link.PublicLOSMessage( MessageType.Regular, hue, true, str );
						done.Add( link );
					}
				}
				else
				{
					object root = link.RootParent;
					if ( root is PlayerMobile )
					{	
						Mobile r = (Mobile)root;

						if ( r.NetState != null && link.IsChildOf( r.Backpack, false ) )
						{
							if ( p_str == null )
								p_str = String.Format( "Crystal: {0}", str );
							--Charges;
							r.SendAsciiMessage( hue, p_str );
						}
					}
					else if ( root is Item )
					{
						bool msg = true;
						for(int d=0;d<done.Count;d++)
						{
							if ( Utility.InRange( ((Item)done[d]).GetWorldLocation(), link.GetWorldLocation(), Range ) )
							{
								msg = false;
								break;
							}
						}
						if ( msg )
						{
							--Charges;
							if ( root is BaseItem )
								((BaseItem)root).PublicLOSMessage( MessageType.Regular, hue, true, str );
							else
								((Item)root).PublicOverheadMessage( MessageType.Regular, hue, true, str );
							done.Add( root );
						}
					}
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get 
			{
				return m_Charges;
			}
			set
			{
				if ( value != m_Charges )
				{
					if ( m_Charges > 0 && value <= 0 && Active )
					{
						Active = false;
						if ( RootParent is PlayerMobile )
							((PlayerMobile)RootParent).SendAsciiMessage( "This crystal is out of charges." );
					}
					m_Charges = value;
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( Movable )
				from.BeginTarget( Range, false, TargetFlags.None, new TargetCallback( OnTarget ) );
		}

		public bool Active
		{
			get 
			{ 
				return ItemID == 0x1ED0; 
			}
			set 
			{
				if ( value && m_Charges > 0 )
					ItemID = 0x1ED0;
				else
					ItemID = 0x1ECD;
			}
		}

		public void OnTarget( Mobile from, object targeted )
		{
			if ( targeted == this )
			{
				if ( !Active )
				{
					if ( m_Charges <= 0 )
					{
						from.SendAsciiMessage( "This crystal is out of charges." );
					}
					else
					{
						from.SendAsciiMessage( "You activate the crystal." );
						Active = true;
					}
				}
				else
				{
					Active = false;
					from.SendAsciiMessage( "You deactivate the crystal." );
				}
			}
			else if ( targeted is ComCrystal )
			{
				if ( m_Links.Count >= 20 )
				{
					from.SendAsciiMessage( "This crystal cannot support any more links." );
				}
				else if ( m_Links.Contains( targeted ) )
				{
					m_Links.Remove( targeted );
					from.SendAsciiMessage( "That crystal has been UNLINKED from this one." );
				}
				else
				{
					m_Links.Add( targeted );
					from.SendAsciiMessage( "That crystal has been LINKED to this one." );
				}
			}
			else 
			{
				int power = -1;
				if ( targeted is Diamond )
					power = 200;
				else if ( targeted is StarSapphire )
					power = 125;
				else if ( targeted is Amethyst || targeted is Emerald || targeted is Sapphire )
					power = 100;
				else if ( targeted is Ruby || targeted is Tourmaline )
					power = 75;
				else if ( targeted is Amber || targeted is Citrine )
					power = 50;

				if ( power > 0 )
				{
					((Item)targeted).Consume();
					if ( m_Charges + power >= MaxCharges )
					{
						from.SendAsciiMessage( "You fully recharge the crystal." );
						m_Charges = MaxCharges;
					}
					else
					{
						from.SendAsciiMessage( "You recharge the crystal." );
						m_Charges += power;
					}
				}
				else
				{
					from.SendAsciiMessage( "You cannot use a communication crystal on that." );
				}
			}
		}

		public int GetSellPrice()
		{
			return (Utility.Random( 10 ) + 65 + Charges*10)/4+1;
		}
	}
}

