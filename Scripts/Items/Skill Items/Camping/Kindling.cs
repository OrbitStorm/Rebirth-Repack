using System;
using Server.Network;

namespace Server.Items
{
	public class Kindling : BaseItem
	{
		[Constructable]
		public Kindling() : this( 1 )
		{
		}

		[Constructable]
		public Kindling( int amount ) : base( 0xDE1 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		public Kindling( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Kindling(), amount );
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
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.CheckSkill( SkillName.Camping, 0.0, 100.0 ) )
			{
				Point3D loc;

				if ( Parent == null )
					loc = Location;
				else
					loc = from.Location;

				Consume();
				new Campfire().MoveToWorld( loc, from.Map );
			}
			else
			{
				from.SendLocalizedMessage( 501696 ); // You fail to ignite the campfire.
				if ( Utility.RandomBool() )
				{
					from.SendMessage( "You lost some kindling." );
					Consume();
				}
			}
		}
	}
}
