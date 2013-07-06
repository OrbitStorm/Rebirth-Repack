using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1bdd, 0x1be0 )]
	public class Log : BaseItem, ICommodity, ICarvable, IChopable
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} log" : "{0} logs", Amount );
			}
		}

		[Constructable]
		public Log() : this( 1 )
		{
		}

		[Constructable]
		public Log( int amount ) : base( 0x1BDD )
		{
			Stackable = true;
			Weight = 2.0;
			Amount = amount;
		}

		public Log( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Log( amount ), amount );
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

		public void Carve( Mobile from, Item blade )
		{
			OnChop( from );
		}
		
		public void OnChop(Mobile from)
		{
			if ( this.IsChildOf( from.Backpack ) )
				new Engines.Craft.BowcraftSystem( this ).Begin( from, null );
			else
				from.SendAsciiMessage( "That belongs to someone else." );
		}
	}
}
