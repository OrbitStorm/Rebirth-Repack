using System;
using Server.Network;
using Server.Prompts;
using Server.Items;

namespace Server.Items
{
	public class HolidayTreeDeed : BaseItem
	{
		public override int LabelNumber{ get{ return 1041116; } } // a deed for a holiday tree

		[Constructable]
		public HolidayTreeDeed() : base( 0x14F0 )
		{
			Hue = 0x488;
			Weight = 1.0;
		}

		public HolidayTreeDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if ( DateTime.Now.Month == 12 || from.AccessLevel > AccessLevel.Player )
				{
					this.Delete();

					new HolidayTree( from );
				}
				else
				{
					from.SendAsciiMessage( 0x35, "You cannot create a holiday tree out of season!" );
				}
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}
	}
}