using System;
using Server.Items;

namespace Server.Items
{
	public class Shaft : BaseItem, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} shaft" : "{0} shafts", Amount );
			}
		}

		[Constructable]
		public Shaft() : this( 1 )
		{
		}

		[Constructable]
		public Shaft( int amount ) : base( 0x1BD4 )
		{
			Stackable = true;
			Weight = 0.1;
			Amount = amount;
		}

		public Shaft( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Shaft( amount ), amount );
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

		public override void OnDoubleClick(Mobile from)
		{
			from.BeginTarget( 10, false, Targeting.TargetFlags.None, new TargetCallback( OnTargFeathers ) );
			from.SendAsciiMessage( "Select the feathers you wish to use this on." );
		}

		private void OnTargFeathers( Mobile from, object target )
		{
			if ( target is Feather )
			{
				Feather feather = (Feather)target;
				if ( feather.IsChildOf( from.Backpack ) )
					new Engines.Craft.FletchingSystem( feather, this ).Begin( from, null );
				else
					from.SendAsciiMessage( "That belongs to someone else." );
			}
			else
			{
				from.SendAsciiMessage( "You can't use shafts on that." );
			}
		}
	}
}

