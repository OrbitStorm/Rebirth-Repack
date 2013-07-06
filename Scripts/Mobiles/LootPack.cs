using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{
	public class LootPack
	{
		private LootPackEntry[] m_Entries;

		public LootPack( LootPackEntry[] entries )
		{
			m_Entries = entries;
		}

		public void Generate( BaseCreature npc )
		{
			if ( npc == null || npc.Summoned )
				return;

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				LootPackEntry entry = m_Entries[i];
				if ( !( entry.Chance > Utility.Random( 10000 ) ) )
					continue;

				Item item = entry.Construct();
				if ( item != null )
				{
					if ( npc.Backpack == null )
						npc.AddItem( new Backpack() );

					if ( !item.Stackable || !npc.Backpack.TryDropItem( npc, item, false ) )
						npc.Backpack.DropItem( item );
				}
			}
		}

		public void Generate( Container cont )
		{
			if ( cont == null )
				return;

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				LootPackEntry entry = m_Entries[i];
				if ( !( entry.Chance > Utility.Random( 10000 ) ) )
					continue;

				Item item = entry.Construct();
				if ( item != null )
					cont.DropItem( item );
			}
		}

		public void Generate( Mobile npc, Container cont )
		{
			if ( cont == null || npc == null )
				return;

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				LootPackEntry entry = m_Entries[i];
				if ( !( entry.Chance > Utility.Random( 10000 ) ) )
					continue;

				Item item = entry.Construct();
				if ( item != null )
				{
					if ( !item.Stackable || !cont.TryDropItem( npc, item, false ) )
						cont.DropItem( item );
				}
			}
		}

		private static readonly LootPackItem[] Instruments = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseInstrument ) )
			};

		private static readonly LootPackItem[] Gold = new LootPackItem[]
			{
				new LootPackItem( typeof( Gold ) )
			};

		private static readonly LootPackItem[] Armor = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseArmor ), 4 ),
				new LootPackItem( typeof( BaseShield ), 2 ),
			};

		private static readonly LootPackItem[] Weapon = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ) )
			};

		private static readonly LootPackItem[] MagicItems = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseArmor ), 4 ),
				new LootPackItem( typeof( BaseWeapon ), 4 ),
				new LootPackItem( typeof( BaseShield ), 2 ),
				new LootPackItem( typeof( MagicWand ), 1 ),
			};

		private static readonly LootPackItem[] LowScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ClumsyScroll ) )
			};

		private static readonly LootPackItem[] MedScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ArchCureScroll ) )
			};

		private static readonly LootPackItem[] HighScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( SummonAirElementalScroll ) )
			};

		private static readonly LootPackItem[] GemItems = new LootPackItem[]
			{
				new LootPackItem( typeof( Amber ) )
			};

		private static readonly LootPackItem[] BootItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ThighBoots ) ),
				new LootPackItem( typeof( Boots ) ),
			};

		private static readonly LootPackItem[] LightSources = new LootPackItem[]
			{
				new LootPackItem( typeof( Torch ) ),
				new LootPackItem( typeof( Candle ) ),
			};

		private static readonly LootPackItem[] Jewelry = new LootPackItem[]
			{
				new LootPackItem( typeof( Necklace ), 4 ),
				new LootPackItem( typeof( Beads ), 3 ),

				new LootPackItem( typeof( GoldNecklace ), 2 ),
				new LootPackItem( typeof( GoldBeadNecklace ), 1 ),
				new LootPackItem( typeof( SilverNecklace ), 3 ),
				new LootPackItem( typeof( SilverBeadNecklace ), 2 ),

				new LootPackItem( typeof( GoldRing ), 2 ),
				new LootPackItem( typeof( SilverRing ), 1 ),

				new LootPackItem( typeof( GoldEarrings ), 3 ),
				new LootPackItem( typeof( SilverEarrings ), 2 ),

				new LootPackItem( typeof( GoldBracelet ), 3 ),
				new LootPackItem( typeof( SilverBracelet ), 2 ),
			};

		private static readonly LootPackItem[] FoodItems = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseBeverage ) ),
				new LootPackItem( typeof( Food ) ),
			};

		private static readonly LootPackItem[] Reagents = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseReagent ) ),
			};

		private static readonly LootPackItem[] PotOrReag = new LootPackItem[]
			{
				new LootPackItem( typeof( BasePotion ) ),
				new LootPackItem( typeof( BaseReagent ) ),
			};

		private static readonly LootPackItem[] ClothingItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ShortPants ), 4 ),
				new LootPackItem( typeof( Shirt ), 4 ),
				new LootPackItem( typeof( StrawHat ), 4 ),
				new LootPackItem( typeof( TallStrawHat ), 4 ),

				new LootPackItem( typeof( LongPants ), 1 ),
				new LootPackItem( typeof( Tunic ), 1 ),
				new LootPackItem( typeof( Doublet ), 1 ),
				new LootPackItem( typeof( Cloak ), 1 ),
			};

		private static readonly LootPackItem[] MissleItems = new LootPackItem[]
			{
				new LootPackItem( typeof( Arrow ) ),
				new LootPackItem( typeof( Bolt ) ),
			};

		public static readonly LootPack Poor = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 5, 25 ) ),
				new LootPackEntry( FoodItems,	 50.00, 1 ),
				new LootPackEntry( ClothingItems,20.00, 1, 0, 1 ),

				new LootPackEntry( BootItems,	 25.00, 1 ),
				new LootPackEntry( LightSources, 25.00, 1 ),

				new LootPackEntry( Instruments,	  0.02, 1 ),
			} );

		public static readonly LootPack Meager = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 20, 40 ) ),
				new LootPackEntry( FoodItems,	 50.00, 1 ),
				new LootPackEntry( ClothingItems,15.00, 1, 0, 50 ),

				new LootPackEntry( PotOrReag,	 25.00, new RandMinMax( 3, 6 ) ),
				new LootPackEntry( MissleItems,  40.00, new RandMinMax( 5, 15 ) ),
				new LootPackEntry( LowScrollItems,5.00, 1 ),

				new LootPackEntry( MagicItems,	  1.00, 1, 0, 60 ),
				new LootPackEntry( MagicItems,	  0.50, 1, 0, 70 ),

				new LootPackEntry( Jewelry,		  0.50, 1, 0, 60 ),

				new LootPackEntry( Instruments,	  0.10, 1 ),
			} );

		public static readonly LootPack Average = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 40, 60 ) ),
				new LootPackEntry( FoodItems,	 50.00, new RandMinMax( 1, 2 ) ),
				new LootPackEntry( ClothingItems,10.00, 1, 1, 75 ),

				new LootPackEntry( PotOrReag,	 50.00, new RandMinMax( 3, 6 ) ),
				new LootPackEntry( MissleItems,  50.00, new RandMinMax( 5, 15 ) ),
				new LootPackEntry( MedScrollItems,25.00, 1 ),
				new LootPackEntry( GemItems,	 20.00, 1 ),
				
				new LootPackEntry( MagicItems,	  3.00, 1, 0, 65 ),
				new LootPackEntry( MagicItems,	  1.50, 1, 5, 75 ),
				new LootPackEntry( MagicItems,	  0.75, 1, 10, 85 ),

				new LootPackEntry( Jewelry,		  0.75, 1, 5, 65 ),

				new LootPackEntry( Instruments,	  0.40, 1 ),
			} );

		public static readonly LootPack Rich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 65, 85 ) ),
				new LootPackEntry( FoodItems,	 50.00, 2 ),
				new LootPackEntry( ClothingItems,10.00, 1, 10, 100 ),

				new LootPackEntry( PotOrReag,	 50.00, new RandMinMax( 3, 6 ) ),
				new LootPackEntry( MissleItems,  50.00, new RandMinMax( 5, 15 ) ),
				new LootPackEntry( HighScrollItems,25.00, 1 ),
				new LootPackEntry( GemItems,	 33.00, 1 ),

				new LootPackEntry( MagicItems,	  5.00, 1,  1, 70 ),
				new LootPackEntry( MagicItems,	  2.50, 1, 10, 80 ),
				new LootPackEntry( MagicItems,	  1.00, 1, 20, 90 ),
				new LootPackEntry( MagicItems,	  0.50, 1, 25,100 ),

				new LootPackEntry( Jewelry,		  1.50, 1, 0, 70 ),

				new LootPackEntry( Instruments,	  1.00, 1 ),
			} );

		public static readonly LootPack FilthyRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 90, 110 ) ),
				new LootPackEntry( FoodItems,	 50.00, 2 ),
				new LootPackEntry( FoodItems,	 50.00, 1 ),

				new LootPackEntry( PotOrReag,	 50.00, new RandMinMax( 3, 6 ) ),
				new LootPackEntry( MissleItems,  50.00, new RandMinMax( 5, 15 ) ),
				new LootPackEntry( HighScrollItems,33.00, 1 ),
				new LootPackEntry( GemItems,	 50.00, 1 ),
				new LootPackEntry( GemItems,	 25.00, 1 ),

				new LootPackEntry( Jewelry,		 7.50, 1, 25, 100 ),

				new LootPackEntry( ClothingItems,10.00, 1, 25, 100 ),
				
				new LootPackEntry( MagicItems,	 15.00, 1, 10,  50 ),
				new LootPackEntry( MagicItems,	 10.00, 1, 20,  75 ),
				new LootPackEntry( MagicItems,	  7.50, 1, 10, 100 ),
				new LootPackEntry( MagicItems,	  5.00, 1, 20, 100 ),
				new LootPackEntry( MagicItems,	  2.50, 1, 30, 100 ),

				new LootPackEntry( Instruments,	  1.00, 1 ),
			} );

		public static readonly LootPack ChestLvl1 = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 220, 440 ) ),
				new LootPackEntry( GemItems,	100.00, new RandMinMax( 1, 3 ) ),

				new LootPackEntry( MagicItems,	100.00, 1, 0, 1 ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 3, 5 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 3, 5 ) ),

				new LootPackEntry( LowScrollItems,100.00, new RandMinMax( 1, 2 ) ),

				new LootPackEntry( Jewelry,		100.00, 1, 0, 1 ),
				new LootPackEntry( Jewelry,		 50.00, 1, 0, 1 ),
			} );

		public static readonly LootPack ChestLvl2 = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 640, 860 ) ),
				new LootPackEntry( GemItems,	100.00, new RandMinMax( 2, 5 ) ),

				new LootPackEntry( MagicItems,	100.00, 1, 0, 1 ),
				new LootPackEntry( MagicItems,	100.00, 1, 1, 50 ),
				
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 4, 9 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 4, 9 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 4, 9 ) ),

				new LootPackEntry( LowScrollItems,	100.00, 2 ),
				new LootPackEntry( MedScrollItems,	 25.00, 1 ),
				new LootPackEntry( MedScrollItems,	 25.00, 1 ),

				new LootPackEntry( Jewelry,		100.00, 1, 0, 1 ),
				new LootPackEntry( Jewelry,		 50.00, 1, 0, 50 ),
		} );

		public static readonly LootPack ChestLvl3 = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 920, 1190 ) ),
				new LootPackEntry( GemItems,	100.00, new RandMinMax( 2, 5 ) ),

				new LootPackEntry( MagicItems,	100.00, 2, 0, 1 ),
				new LootPackEntry( MagicItems,	100.00, 2, 1, 75 ),
				
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 7, 11 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 7, 11 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 7, 11 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 7, 11 ) ),

				new LootPackEntry( LowScrollItems,	 50.00, 1 ),
				new LootPackEntry( MedScrollItems,	100.00, 1 ),
				new LootPackEntry( MedScrollItems,	 50.00, 2 ),

				new LootPackEntry( Jewelry,		100.00, 1, 0, 50 ),
				new LootPackEntry( Jewelry,		 50.00, 1, 0, 75 ),

				new LootPackEntry( FoodItems,   100.00, new RandMinMax( 2, 4 ) ),
				new LootPackEntry( ClothingItems,75.00, 1, 0, 75 ),
		} );

		public static readonly LootPack ChestLvl4 = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 1120, 1390 ) ),
				new LootPackEntry( GemItems,	100.00, new RandMinMax( 4, 6 ) ),

				new LootPackEntry( MagicItems,	100.00, 3, 0, 1 ),
				new LootPackEntry( MagicItems,	100.00, 3, 1, 75 ),
				new LootPackEntry( MagicItems,	  0.50, 1, 50, 100 ),
				
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 9, 14 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 9, 14 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 9, 14 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 9, 14 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 9, 14 ) ),

				new LootPackEntry( MedScrollItems,	100.00, new RandMinMax( 3, 6 ) ),

				new LootPackEntry( Jewelry,		100.00, 3, 0, 75 ),

				new LootPackEntry( FoodItems,   100.00, new RandMinMax( 2, 4 ) ),
				new LootPackEntry( ClothingItems,75.00, 1, 25, 100 ),
		} );

		public static readonly LootPack ChestLvl5 = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( Gold,		100.00, new RandMinMax( 1520, 1840 ) ),
				new LootPackEntry( GemItems,	100.00, new RandMinMax( 6, 12 ) ),

				new LootPackEntry( MagicItems,	100.00, 3, 0, 1 ),

				new LootPackEntry( MagicItems,	100.00, 2, 1, 50 ),
				new LootPackEntry( MagicItems,	 75.00, 2, 1, 75 ),
				new LootPackEntry( MagicItems,	 50.00, 1, 50, 100 ),
				new LootPackEntry( MagicItems,	 10.00, 1, 75, 100 ),
				new LootPackEntry( MagicItems,	  1.00, 1, 99, 100 ),
				
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 11, 16 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 11, 16 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 11, 16 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 11, 16 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 11, 16 ) ),
				new LootPackEntry( Reagents,	100.00, new RandMinMax( 11, 16 ) ),

				new LootPackEntry( MedScrollItems,	100.00, 2 ),
				new LootPackEntry( MedScrollItems,	 75.00, 1 ),
				new LootPackEntry( HighScrollItems,  75.00, 1 ),
				new LootPackEntry( HighScrollItems,  50.00, 1 ),
				new LootPackEntry( HighScrollItems,  50.00, 1 ),
				new LootPackEntry( HighScrollItems,  25.00, 1 ),

				new LootPackEntry( Jewelry,		100.00, 4, 0, 100 ),

				new LootPackEntry( FoodItems,   100.00, new RandMinMax( 4, 8 ) ),
				new LootPackEntry( ClothingItems,75.00, 2, 50, 100 ),
		} );

		public static LootPackEntry BalronSword = new LootPackEntry( new LootPackItem[1]{ new LootPackItem( typeof( Broadsword ) ) }, 100.0, 1, 50, 100 );

		public static readonly LootPack LowScrolls = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( LowScrollItems,	100.00, 1 )
			} );

		public static readonly LootPack MedScrolls = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( MedScrollItems,	100.00, 1 )
			} );

		public static readonly LootPack HighScrolls = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( HighScrollItems,	100.00, 1 )
			} );

		public static readonly LootPack Gems = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( GemItems,			100.00, 1 )
			} );

		public static readonly LootPack Food = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( FoodItems,			100.00, new RandMinMax( 1, 3 ) )
			} );
	}

	public class LootPackEntry
	{
		private int m_Chance;
		private int m_MinIntensity, m_MaxIntensity;
		private LootPackItem[] m_Items;
		private RandMinMax m_Quantity;
		private int m_TotalChance;

		public int Chance
		{
			get{ return m_Chance; }
			set{ m_Chance = value; }
		}

		public RandMinMax Quantity
		{
			get { return m_Quantity; }
		}

		public int MinIntensity
		{
			get{ return m_MinIntensity; }
			set{ m_MinIntensity = value; }
		}

		public int MaxIntensity
		{
			get{ return m_MaxIntensity; }
			set{ m_MaxIntensity = value; }
		}

		public LootPackItem[] Items
		{
			get{ return m_Items; }
			set{ m_Items = value; }
		}

		public Item Construct()
		{
			int rnd = Utility.Random( m_TotalChance );

			for ( int i = 0; i < m_Items.Length; ++i )
			{
				LootPackItem item = m_Items[i];

				if ( rnd < item.Chance )
					return Mutate( item.Construct() );

				rnd -= item.Chance;
			}

			return null;
		}

		private int GetRandomBonus()
		{
			return GetRandomBonus( m_MinIntensity, m_MaxIntensity );
		}

		public static int GetRandomBonus( int min, int max )
		{
			int rnd = Utility.Random( min, max - min );

			if ( 50 > rnd )
				return 1;
			else
				rnd -= 50;

			if ( 25 > rnd )
				return 2;
			else
				rnd -= 25;

			if ( 14 > rnd )
				return 3;
			else
				rnd -= 14;

			if ( 8 > rnd )
				return 4;

			return 5;
		}

		public Item Mutate( Item item )
		{
			if ( item == null )
				return null;
			
			int stop = 0;
			if ( item is MagicWand )
			{
				MagicWand wand = (MagicWand)item;

				while ( wand.SpellEffect == SpellEffect.None && stop < 500 )
				{
					stop++;
					double dr = Utility.RandomDouble() * 100.0;

					if ( dr < 8.0 )
					{
						if ( dr < 0.2 && m_MaxIntensity >= 80 )
							wand.SpellEffect = SpellEffect.ManaDrain;
						else if ( dr < 0.8 && m_MaxIntensity >= 80 )
							wand.SpellEffect = SpellEffect.Lightning;
						else if ( dr < 1.3 && m_MaxIntensity >= 60 )
							wand.SpellEffect = SpellEffect.Fireball;
						else if ( dr < 1.8 && m_MaxIntensity >= 60 )
							wand.SpellEffect = SpellEffect.GHeal;
						else if ( dr < 2.8 && m_MaxIntensity >= 40 )
							wand.SpellEffect = SpellEffect.Harm;
						else if ( dr < 3.8 && m_MaxIntensity >= 40 )
							wand.SpellEffect = SpellEffect.MagicArrow;
						else if ( dr < 4.8 && m_MaxIntensity >= 20 )
							wand.SpellEffect = SpellEffect.Feeblemind;
						else if ( dr < 5.8 && m_MaxIntensity >= 20 )
							wand.SpellEffect = SpellEffect.Clumsy;
						else if ( dr < 6.8 && m_MaxIntensity >= 20 )
							wand.SpellEffect = SpellEffect.MiniHeal;
						else //if ( dr < 8.0 )
							wand.SpellEffect = SpellEffect.ItemID;
					}
				}

				if ( wand.SpellEffect != SpellEffect.None )
					wand.SpellCharges = SpellCastEffect.GetChargesFor( wand.SpellEffect );
				else
					wand.Delete();
			}
			else if ( item is BaseWeapon )
			{
				BaseWeapon weapon = (BaseWeapon)item;
							
				while ( weapon.AccuracyLevel == WeaponAccuracyLevel.Regular &&
					weapon.DamageLevel == WeaponDamageLevel.Regular &&
					weapon.DurabilityLevel == DurabilityLevel.Regular &&
					weapon.Slayer == SlayerName.None &&
					weapon.SpellEffect == SpellEffect.None && stop < 500 )
				{
					stop++;

					if ( 45 > Utility.Random( 100 ) )
						weapon.AccuracyLevel = (WeaponAccuracyLevel)GetRandomBonus();
					if ( 40 > Utility.Random( 100 ) )
						weapon.DamageLevel = (WeaponDamageLevel)GetRandomBonus();
					if ( 35 > Utility.Random( 100 ) )
						weapon.DurabilityLevel = (DurabilityLevel)GetRandomBonus();
					if ( 5 > Utility.Random( 100 ) )
						weapon.Slayer = SlayerName.Silver;

					double dr = Utility.RandomDouble() * 100.0;
					if ( dr < 7.8 )
					{
						if ( dr < 0.2 && m_MaxIntensity >= 80 )
							weapon.SpellEffect = SpellEffect.Lightning;
						else if ( dr < 0.8 && m_MaxIntensity >= 80 )
							weapon.SpellEffect = SpellEffect.ManaDrain;
						else if ( dr < 1.3 && m_MaxIntensity >= 60 )
							weapon.SpellEffect = SpellEffect.Curse;
						else if ( dr < 1.8 && m_MaxIntensity >= 60 )
							weapon.SpellEffect = SpellEffect.Fireball;
						else if ( dr < 2.8 && m_MaxIntensity >= 40 )
							weapon.SpellEffect = SpellEffect.Paralyze;
						else if ( dr < 3.8 && m_MaxIntensity >= 40 )
							weapon.SpellEffect = SpellEffect.Harm;
						else if ( dr < 4.8 && m_MaxIntensity >= 20 )
							weapon.SpellEffect = SpellEffect.Weaken;
						else if ( dr < 5.8 && m_MaxIntensity >= 20 )
							weapon.SpellEffect = SpellEffect.MagicArrow;
						else if ( dr < 6.8 )
							weapon.SpellEffect = SpellEffect.Feeblemind;
						else //if ( dr < 7.8 )
							weapon.SpellEffect = SpellEffect.Clumsy;
					}
				}

				if ( weapon.SpellEffect != SpellEffect.None )
					weapon.SpellCharges = SpellCastEffect.GetChargesFor( weapon.SpellEffect );
			}
			else if ( item is BaseArmor )
			{
				BaseArmor armor = (BaseArmor)item;

				while ( armor.ProtectionLevel == ArmorProtectionLevel.Regular && armor.Durability == DurabilityLevel.Regular && stop < 500 )
				{
					stop++;

					if ( 75 > Utility.Random( 100 ) )
						armor.ProtectionLevel = (ArmorProtectionLevel)GetRandomBonus();

					if ( 35 > Utility.Random( 100 ) )
						armor.Durability = (DurabilityLevel)GetRandomBonus();
				}
			}
			else if ( item is BaseInstrument )
			{
				((BaseInstrument)item).Quality = InstrumentQuality.Regular;
			}
			else if ( item is BaseRing )
			{
				BaseRing ring = (BaseRing)item;

				if ( GetRandomBonus() >= 5 )
				{
					if ( Utility.RandomBool() )
						ring.SpellEffect = SpellEffect.Invis;
					else
						ring.SpellEffect = SpellEffect.Teleportation;
					ring.SpellCharges = SpellCastEffect.GetChargesFor( ring.SpellEffect );
				}
				else
				{
					ring.Delete();
				}
			}
			else if ( item is BaseClothing )
			{
				BaseClothing clothes = (BaseClothing)item;

				while( clothes.SpellEffect == SpellEffect.None && stop < 1 )
				{
					stop++;
					double dr = Utility.RandomDouble() * 100.0;
					if ( dr < 10.0 )
					{
						if ( dr < 0.2 && m_MaxIntensity >= 80 )
							clothes.SpellEffect = SpellEffect.Curse;
						else if ( dr < 0.8 && m_MaxIntensity >= 80 )
							clothes.SpellEffect = SpellEffect.Bless;
						else if ( dr < 1.3 && m_MaxIntensity >= 60 )
							clothes.SpellEffect = SpellEffect.Weaken;
						else if ( dr < 1.8 && m_MaxIntensity >= 60 )
							clothes.SpellEffect = SpellEffect.Clumsy;
						else if ( dr < 2.8 && m_MaxIntensity >= 60 )
							clothes.SpellEffect = SpellEffect.Feeblemind;
						else if ( dr < 3.8 && m_MaxIntensity >= 50 )
							clothes.SpellEffect = SpellEffect.Reflect;
						else if ( dr < 4.8 && m_MaxIntensity >= 50 )
							clothes.SpellEffect = SpellEffect.Invis;
						else if ( dr < 5.8 && m_MaxIntensity >= 20 )
							clothes.SpellEffect = SpellEffect.Strength;
						else if ( dr < 6.8 && m_MaxIntensity >= 20 )
							clothes.SpellEffect = SpellEffect.Cunning;
						else if ( dr < 7.8 && m_MaxIntensity >= 20 )
							clothes.SpellEffect = SpellEffect.Agility;
						else if ( dr < 8.9 )
							clothes.SpellEffect = SpellEffect.Protection;
						else
							clothes.SpellEffect = SpellEffect.NightSight;
					}
				}

				if ( clothes.SpellEffect != SpellEffect.None )
					clothes.SpellCharges = SpellCastEffect.GetChargesFor( clothes.SpellEffect );
			}

			if ( item.Stackable )
				item.Amount = m_Quantity.Generate();

			return item;
		}

		public LootPackEntry( LootPackItem[] items, double chance, int quantity ) : this( items, chance, new RandMinMax( quantity ), 0, 0 )
		{
		}

		public LootPackEntry( LootPackItem[] items, double chance, RandMinMax quantity ) : this( items, chance, quantity, 0, 0 )
		{
		}

		public LootPackEntry( LootPackItem[] items, double chance, int quantity, int minIntensity, int maxIntensity ) : this( items, chance, new RandMinMax( quantity ), minIntensity, maxIntensity )
		{
		}

		public LootPackEntry( LootPackItem[] items, double chance, RandMinMax quantity, int minIntensity, int maxIntensity )
		{
			m_Items = items;
			m_Quantity = quantity;
			m_Chance = (int)(100 * chance);
			m_MinIntensity = minIntensity;
			m_MaxIntensity = maxIntensity;

			m_TotalChance = 0;
			for ( int i = 0; i < m_Items.Length; ++i )
				m_TotalChance += m_Items[i].Chance;
		}
	}

	public class LootPackItem
	{
		private Type m_Type;
		private int m_Chance;

		public Type Type
		{
			get{ return m_Type; }
			set{ m_Type = value; }
		}

		public int Chance
		{
			get{ return m_Chance; }
		}
		
		private static int GetIntensityCircle( int cir )
		{
			switch ( cir )
			{
				case 8:
					return 100;
				case 7:
					return 80;
				case 6:
				case 5:
					return 60;
				case 4:
				case 3:
					return 40;
				case 2:
				case 1:
				default:
					return 20;
			}
		}				
		
		public static Item RandomScroll( int minCircle, int maxCircle )
		{
			if ( Utility.Random( 500 ) == 17 )
				return new Item( Utility.Random( 6 ) + 0xEf4 );

			int min = GetIntensityCircle( minCircle );
			int rnd = Utility.Random( min, GetIntensityCircle( maxCircle ) - min );
			
			if ( 50 > rnd )
				return Loot.RandomScroll( 0, 15 );
			else
				rnd -= 50;

			if ( 25 > rnd )
				return Loot.RandomScroll( 16, 31 );
			else
				rnd -= 25;

			if ( 14 > rnd )
				return Loot.RandomScroll( 32, 47 );
			else
				rnd -= 14;

			if ( 8 > rnd )
				return Loot.RandomScroll( 48, 55 );

			return Loot.RandomScroll( 56, 63 );
		}

		public Item Construct()
		{
			try
			{
				Item item;

				if ( m_Type == null )
					item = null;
				else if ( m_Type == typeof( BaseWeapon ) )
					item = Loot.RandomWeapon();
				else if ( m_Type == typeof( BaseArmor ) )
					item = Loot.RandomArmor();
				else if ( m_Type == typeof( BaseShield ) )
					item = Loot.RandomShield();
				else if ( m_Type == typeof( BaseInstrument ) )
					item = Loot.RandomInstrument();
				else if ( m_Type == typeof( Amber ) ) // gem
					item = Loot.RandomGem();
				
				else if ( m_Type == typeof( ClumsyScroll ) ) // low scroll
					item = RandomScroll( 1, 4 );
				else if ( m_Type == typeof( ArchCureScroll ) ) // med scroll
					item = RandomScroll( 5, 6 );
				else if ( m_Type == typeof( SummonAirElementalScroll ) ) // high scroll
					item = RandomScroll( 7, 8 );
				
				else if ( m_Type == typeof( BaseBeverage ) )
					item = Loot.RandomDrink();
				else if ( m_Type == typeof( Food ) )
					item = Loot.RandomFood();
				else
					item = Activator.CreateInstance( m_Type ) as Item;

				if ( item != null && item is BaseClothing )
					item.Hue = Utility.RandomNondyedHue();

				return item;
			}
			catch
			{
			}

			return null;
		}

		public LootPackItem( Type type )
		{
			m_Type = type;
			m_Chance = 1;
		}

		public LootPackItem( Type type, int chance )
		{
			m_Type = type;
			m_Chance = chance;
		}
	}

	public class RandMinMax
	{
		public int Min { get{ return m_Min; } }
		public int Max { get{ return m_Max; } }

		private int m_Min, m_Max;

		public int Generate()
		{
			if ( m_Min == m_Max )
				return m_Min;
			else
				return Utility.RandomMinMax( m_Min, m_Max );
		}

		public RandMinMax( int min, int max )
		{
			if ( max >= min )
			{
				m_Min = min;
				m_Max = max;
			}
			else
			{
				m_Min = max;
				m_Max = min;
			}
		}

		public RandMinMax( int num )
		{
			m_Min = m_Max = num;
		}
	}
}
