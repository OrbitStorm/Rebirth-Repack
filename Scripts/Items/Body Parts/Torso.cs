using System;
using Server;

namespace Server.Items
{
	public class Torso : BaseItem, ICarvable
	{
		#region ICarvable Members
		public void Carve(Mobile from, Item item)
		{
			Item ribcage = new BaseItem( 6935 );
			Item heart = new BaseItem( 7405 );
			Item liver = new BaseItem( 7406 );
			Item entrails = new BaseItem( 7407 );

			if ( m_Owner != null )
			{
				ribcage.Name = "ribcage of " + m_Owner.Name;
				liver.Name = "liver of " + m_Owner.Name;
				heart.Name = "heart of " + m_Owner.Name;
				entrails.Name = "entrails of " + m_Owner.Name;
			}

			if ( !(Parent is Container) )
			{
				ribcage.MoveToWorld( GetWorldLocation(), Map );
				liver.MoveToWorld( GetWorldLocation(), Map );
				heart.MoveToWorld( GetWorldLocation(), Map );
				entrails.MoveToWorld( GetWorldLocation(), Map );
			}
			else
			{
				Container cont = (Container)Parent;
				cont.DropItem( ribcage );
				cont.DropItem( liver );
				cont.DropItem( heart );
				cont.DropItem( entrails );
			}

			Delete();
		}
		#endregion

		private Mobile m_Owner;

		[Constructable]
		public Torso() : this( null )
		{
		}

		public Torso( Mobile owner ) : base( 0x1D9F )
		{
			m_Owner = owner;
			if ( m_Owner != null )
				Name = String.Format( "torso of {0}", m_Owner.Name );
			LastMoved = (DateTime.Now - Item.DefaultDecayTime) + TimeSpan.FromMinutes( 7.5 );
			Weight = 2.0;
		}

		public Torso( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Owner = reader.ReadMobile();
					break;
				}
			}
		}
	}
}
