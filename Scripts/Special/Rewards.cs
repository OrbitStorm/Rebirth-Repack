using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;
using Server.Network;

namespace Server.Gumps
{
	public class RewardGump : Gump
	{
		bool m_Abyss, m_Old;
		int m_Count;
		

		public RewardGump( PlayerMobile from, bool abyss, bool old, int count )  : base( 50, 50 )
		{
			m_Abyss = abyss;
			m_Old = old;
			m_Count = count;

			Closable = false;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddBackground(10, 10, 225, 425, 9380);

			AddPage( 1 );
			if ( m_Count > 1 )
				AddLabel( 73, 15, 1152, String.Format( "Choose {0} rewards", m_Count ) );
			else
				AddLabel( 73, 15, 1152, "Choose a reward" );

			if ( abyss )
			{
				AddCheck(40, 55 + ( 45 * 0 ), 210, 211, false, 100 );
				AddLabel(70, 55 + ( 45 * 0 ) , 0, "A check for 5000gp" );

				AddCheck(40, 55 + ( 45 * 1 ), 210, 211, false, 101 );
				AddLabel(70, 55 + ( 45 * 1 ) , 0, "100 of each reagent" );

				AddCheck(40, 55 + ( 45 * 2 ), 210, 211, false, 102 );
				AddLabel(70, 55 + ( 45 * 2 ) , 0, "Hair restyle deed" );

				if ( m_Old )
				{
					AddCheck(40, 55 + ( 45 * 3 ), 210, 211, false, 103 );
					AddLabel(70, 55 + ( 45 * 3 ) , 0, "A piece of ranger armor" );

					AddCheck(40, 55 + ( 45 * 4 ), 210, 211, false, 104 );
					AddLabel(70, 55 + ( 45 * 4 ) , 0, "A special scroll" );

					AddCheck(40, 55 + ( 45 * 5 ), 210, 211, false, 105 );
					AddLabel(70, 55 + ( 45 * 5 ) , 0, "A small dragon ship deed" );
				}
			}
			else
			{
				AddCheck(40, 55 + ( 45 * 0 ), 210, 211, false, 100 );
				AddLabel(70, 55 + ( 45 * 0 ) , 0, "A Fireworks wand" );

				AddCheck(40, 55 + ( 45 * 1 ), 210, 211, false, 101 );
				AddLabel(70, 55 + ( 45 * 1 ) , 0, "A Spyglass" );

				AddCheck(40, 55 + ( 45 * 2 ), 210, 211, false, 102 );
				AddLabel(70, 55 + ( 45 * 2 ) , 0, "Hair restyle deed" );

				if ( m_Old )
				{
					AddCheck(40, 55 + ( 45 * 3 ), 210, 211, false, 103 );
					AddLabel(70, 55 + ( 45 * 3 ) , 0, "A piece of ranger armor" );

					AddCheck(40, 55 + ( 45 * 4 ), 210, 211, false, 104 );
					AddLabel(70, 55 + ( 45 * 4 ) , 0, "Clothing bless deed" );

					AddCheck(40, 55 + ( 45 * 5 ), 210, 211, false, 105 );
					AddLabel(70, 55 + ( 45 * 5 ) , 0, "Strong Box deed" );
				}
			}

			AddButton(91, 411, 247, 248, 1, GumpButtonType.Reply, 0); // okay
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int count = m_Count;
			Mobile m = sender.Mobile;

			for ( int i = 0; i<6 && count > 0; i++ )
			{
				Item item = null;
				if ( !m_Old && i>=3 )
					break;

				if ( !info.IsSwitched( i + 100 ) )
					continue;

				count--;

				switch ( i )
				{
					case 0:
						if ( m_Abyss )
							m.AddToBackpack( new BankCheck( 5000 ) );
						else
							m.AddToBackpack( new FireworksWand() );
						break;
					case 1:
						if ( m_Abyss )
						{
							m.AddToBackpack( new BagOfReagents( 100 ) );
						}
						else
						{
							m.AddToBackpack( item=new Spyglass() );
							item.LootType = LootType.Newbied;
						}
						break;
					case 2:
						m.AddToBackpack( new HairRestylingDeed() );
						break;
					case 3:
						switch ( Utility.Random( 5 ) )
						{
							case 0:
								item = new RangerArms();
								break;
							case 1:
								item = new RangerChest();
								break;
							case 2:
								item = new RangerGloves();
								break;
							case 3:
								item = new RangerGorget();
								break;
							case 4:
								item = new RangerLegs();
								break;
						}
						
						if ( item != null )
							m.AddToBackpack( item );
						break;
					case 4:
						if ( m_Abyss )
						{
							item = new BaseItem( Utility.Random( 6 ) + 0xEf4 );
							item.Name = "I survived the Abyss!";
							item.LootType = LootType.Newbied;
							m.AddToBackpack( item );
						}
						else
						{
							m.AddToBackpack( new ClothingBlessDeed() );
						}
						break;
					case 5:
						if ( m_Abyss )
							m.AddToBackpack( new Server.Multis.SmallDragonBoatDeed() );
						else
							m.AddToBackpack( new StrongBoxDeed() );
						break;
				}
			}

			m.SendAsciiMessage( "Enjoy!" );
			if ( count > 0 )
			{
				((Account)m.Account).SetTag( "Rewards", count.ToString() );
				m.SendAsciiMessage( "You have {0} reward{1} left.", count, count == 1 ? "" : "s" );
			}
			else
			{
				((Account)m.Account).RemoveTag( "Rewards" );
				((Account)m.Account).RemoveTag( "GoodRewards" );
			}
		}
	}
}
