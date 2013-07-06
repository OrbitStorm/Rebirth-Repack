using System;
using Server;

namespace Server.Items
{
	public class BedRoll : BaseItem
	{
		[Constructable]
		public BedRoll() : base( 0xA58 )
		{
		}

		public BedRoll( Serial s ) : base( s )
		{
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);
			int version = reader.ReadInt(); 
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( (int) 0 ); // version
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( RootParent == from || ( from.InRange( GetWorldLocation(), 3 ) && from.InLOS( GetWorldLocation() ) ) )
			{
				if ( !Unrolled )
				{
					Unroll();
					MoveToWorld( from.Location, from.Map );
				}
				else if ( ItemID == 0x0A55 )
				{
					if ( Parent == null )
					{
						IPooledEnumerable eable = from.GetItemsInRange( 7 );
						Campfire fire = null;
						foreach ( Item item in eable )
						{
							if ( item is Campfire )
							{
								fire = (Campfire)item;
								break;
							}
						}
						eable.Free();

						if ( fire != null )
						{
							if ( fire.CanLogout( from ) )
								new BedRollLogoutMenu().SendTo( from.NetState );
							else
								from.SendAsciiMessage( "Your camp is not yet secure." );
						}
						else
						{
							Roll();
							from.AddToBackpack( this );
						}
					}
					else 
					{
						// is in a container (not on ground)
						Roll();
						from.AddToBackpack( this );
					}
				}
			}
			else
			{
				from.SendAsciiMessage( "You must be closer to use that." );
			}
		}

		public bool Unrolled { get { return ItemID == 0x0A55; } }

		public void Unroll()
		{
			ItemID = 0x0A55;
		}

		public void Roll()
		{
			ItemID = 0x0A58;
		}

		private class BedRollLogoutMenu : Server.Menus.Questions.QuestionMenu
		{
			public BedRollLogoutMenu() : base( "Using a bedroll in the safety of a camp will log you out of the game safely. If this is what you wish to do, click in the box at left, and then select Continue. Otherwise, hit the Cancel button to avoid logging out.", new string[]{ "Continue", "Cancel" } )
			{
			}

			public override void OnResponse(Server.Network.NetState state, int index)
			{
				if ( index == 0 && state != null && state.Mobile != null && state.Mobile.Alive )
				{
					state.Mobile.SendAsciiMessage( "Logging out...." );
					state.Dispose();
				}
			}
		}
	}
}
