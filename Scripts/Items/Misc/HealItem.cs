using System;
using Server;
using Server.Mobiles;
using System.Collections; using System.Collections.Generic;

namespace Server.Items
{
	public class StatRefreshItem : BaseItem
	{
		[Constructable]
		public StatRefreshItem( int itemID ) : base( itemID )
		{
		}

		public StatRefreshItem( Serial s ) : base( s )
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
			writer.Write( (int)0 ); // version
		}

		private static Hashtable m_Table = new Hashtable();

		public override void OnDoubleClick(Mobile from)
		{
			if ( from.Player && from.InRange( this, 1 ) && from.InLOS( this ) )
			{
				object o = m_Table[from];
				if ( o == null || ( o is DateTime && ((DateTime)o)+TimeSpan.FromHours( 6 ) < DateTime.Now ) )
				{
					from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
					from.PlaySound( 0x202 );

					from.SendAsciiMessage( "You feel a magical energy surround you." );

					from.Hits = from.HitsMax;
					from.Mana = from.ManaMax;
					from.Stam = from.StamMax;
					from.CurePoison( from );

					m_Table[from] = DateTime.Now;
				}
			}
		}
	}
}
