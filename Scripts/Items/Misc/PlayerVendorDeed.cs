using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
	public class ContractOfEmployment : BaseItem
	{
		public override int LabelNumber{ get{ return 1041243; } } // a contract of employment

		[Constructable]
		public ContractOfEmployment() : base( 0x14F0 )
		{
			Weight = 1.0;
		}

		public ContractOfEmployment( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int)0 ); //version
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
			else if ( from.AccessLevel >= AccessLevel.GameMaster )
			{
				from.SendLocalizedMessage( 503248 );//Your godly powers allow you to place this vendor whereever you wish.
				
				Mobile v = new PlayerVendor( from );
				v.Location = from.Location;
				v.Direction = from.Direction & Direction.Mask;
				v.Map = from.Map;
				v.SayTo( from, 503246 ); // Ah! it feels good to be working again.
		
				this.Delete();
			}
			else
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );
				if ( house == null && from.Region is HouseRegion )
				{
					//allow placement in tents
					house = ((HouseRegion)from.Region).House;
					if ( house != null && !(house is Tent) )
						house = null;
				}

				if ( house == null )
				{
					from.SendLocalizedMessage( 503240 );//Vendors can only be placed in houses.	
				}
				else if ( !house.IsOwner( from ) )
				{
					from.SendLocalizedMessage( 503242 ); // You must ask the owner of this house to make you a friend in order to place this vendor here,
				}
				else
				{
					IPooledEnumerable eable = from.GetMobilesInRange( 0 );
					foreach ( Mobile m in eable )
					{
						if ( !m.Player )
						{
							from.SendAsciiMessage( "There is something blocking that location." );
							eable.Free();
							return;
						}
					}

					Mobile v = new PlayerVendor( from );
					v.Location = from.Location;
					v.Direction = from.Direction & Direction.Mask;
					v.Map = from.Map;
					v.SayTo( from, 503246 ); // Ah! it feels good to be working again.
						
					this.Delete();
				}
			}
		}
	}
}
