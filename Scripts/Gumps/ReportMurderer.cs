using System; 
using System.Collections; using System.Collections.Generic; 
using Server; 
using Server.Gumps; 
using Server.Network; 
using Server.Mobiles; 
using Server.Items; 
using Server.ContextMenus; 

namespace Server.Gumps 
{ 
	public class ReportMurderer
	{
		public static void Configure() 
		{ 
			PacketHandlers.Register( 0xAC, 0, true, new OnPacketReceive( BountyEntryResponse ) );
		}

		private static Hashtable m_Killers = new Hashtable( 1 );

		public static void SetKillers( Mobile m, ArrayList killers )
		{
			m_Killers[m.Serial] = killers;
		}

		public static void SendNext( Mobile to )
		{
			if ( to.NetState == null )
			{
				m_Killers.Remove( to.Serial );
				return;
			}

			ArrayList list = m_Killers[to.Serial] as ArrayList;
			if ( list == null || list.Count <= 0 )
			{
				m_Killers.Remove( to.Serial );
				if ( to is PlayerMobile && !((PlayerMobile)to).AssumePlayAsGhost )
					new ResNowOption( to ).SendTo( to.NetState );
				return;
			}

			Mobile killer = (Mobile)list[0];
			list.RemoveAt( 0 );
			if ( killer == null || killer.Deleted || !killer.Player )
			{
				SendNext( to );
				return;
			}

			Item[] gold = to.BankBox.FindItemsByType( typeof( Gold ), true );
			int total = 0;
			for(int i=0;i<gold.Length && total < 5000;i++)
				total += gold[i].Amount;
			if ( total > 5000 )
				total = 5000;
			to.Send( new BountyEntry( killer, total ) );
		}

		private class BountyEntry : Packet
		{
			public BountyEntry( Mobile killer, int maxGold ) : base( 0xAB )
			{
				string prompt = String.Format( "Do you wish to place a bounty on the head of {0}?", killer.Name );
				string subText = String.Format( "({0}gp max.)", maxGold );

				EnsureCapacity( 1+2+4+1+1+2+prompt.Length+1+1+1+4+2+subText.Length+1 );

				m_Stream.Write( (int) killer.Serial ); // id
				m_Stream.Write( (byte) 0 ); // 'typeid'
				m_Stream.Write( (byte) 0 ); // 'index'

				m_Stream.Write( (short)(prompt.Length+1) ); // textlen 
				m_Stream.WriteAsciiNull( prompt );

				m_Stream.Write( (bool) true ); // enable cancel btn
				m_Stream.Write( (byte) 2 ); // style, 0=disable, 1=normal, 2=numeric
				m_Stream.Write( (int) maxGold ); // 'format' when style=1 format=maxlen, style=2 format=max # val
				
				m_Stream.Write( (short)(subText.Length+1) );
				m_Stream.WriteAsciiNull( subText );
			}
		}

		private static int GetPriceFor( Item item )
		{
			int price = 0;

			if ( item is Gold )
			{
				price = item.Amount;
			}
			else if ( item is BaseArmor )
			{
				BaseArmor armor = (BaseArmor)item;

				if ( armor.Quality == CraftQuality.Low )
					price = (int)( price * 0.75 );
				else if ( armor.Quality == CraftQuality.Exceptional )
					price = (int)( price * 1.25 );

				price += 100 * (int)armor.Durability;

				price += 100 * (int)armor.ProtectionLevel;
			}
			else if ( item is BaseWeapon )
			{
				BaseWeapon weapon = (BaseWeapon)item;

				if ( weapon.Quality == CraftQuality.Low )
					price = (int)( price * 0.60 );
				else if ( weapon.Quality == CraftQuality.Exceptional )
					price = (int)( price * 1.25 );

				price += 100 * (int)weapon.DurabilityLevel;

				price += 100 * (int)weapon.DamageLevel;
			}
			else if ( item is BaseBeverage )
			{
				int price1 = price, price2 = price;

				if ( item is Pitcher )
				{ price1 = 3; price2 = 5; }
				else if ( item is BeverageBottle )
				{ price1 = 3; price2 = 3; }
				else if ( item is Jug )
				{ price1 = 6; price2 = 6; }

				BaseBeverage bev = (BaseBeverage)item;

				if ( bev.IsEmpty || bev.Content == BeverageType.Milk )
					price = price1;
				else
					price = price2;
			}
			else
			{
				price = Utility.RandomMinMax( 10, 50 );
			}

			if ( price < 1 )
				price = 1;

			return price;
		}

		private static int EmptyAndGetGold( List<Item> items )
		{
			int gold = 0;
			ArrayList myList = new ArrayList( items );
			for (int i=0;i<myList.Count;i++)
			{
				Item item = (Item)myList[i];
				if ( item.Items.Count > 0 )
					gold += EmptyAndGetGold( item.Items );
				gold += GetPriceFor( item );
				item.Delete();
			}

			return gold;
		}

		private static void BountyEntryResponse( NetState ns, PacketReader pvSrc )
		{
			Mobile from = ns.Mobile;
			if ( from == null )
				return;
			Mobile killer = World.FindMobile( (Serial)pvSrc.ReadInt32() );
			byte typeid = pvSrc.ReadByte();
			byte index = pvSrc.ReadByte();
			bool cancel = pvSrc.ReadByte() == 0;
			short responseLen = pvSrc.ReadInt16();
			string resp = pvSrc.ReadString();

			if ( killer != null && !cancel )
			{
				int bounty = Utility.ToInt32( resp );
				if ( bounty > 5000 )
					bounty = 5000;
				bounty = from.BankBox.ConsumeUpTo( typeof( Gold ), bounty, true );

				killer.Kills++;
				if ( killer is PlayerMobile && bounty > 0 )
				{
					PlayerMobile kpm = (PlayerMobile)killer;
					kpm.Bounty += bounty;
					killer.SendAsciiMessage( "{0} has placed a bounty of {1}gp on your head!", from.Name, bounty );
					if ( kpm.Bounty >= 5000 && kpm.Kills > 1 && kpm.BankBox.Items.Count > 0 && kpm.Karma <= (int)Noto.Dark )
					{
						killer.SayTo( killer, true, "A bounty hath been issued for thee, and thy worldly goods are hereby confiscated!" );
						kpm.Bounty += EmptyAndGetGold( killer.BankBox.Items );
					}
				}
			}

			SendNext( from );
		}
	} 
}
