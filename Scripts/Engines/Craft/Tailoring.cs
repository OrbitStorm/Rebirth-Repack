using System;
using System.Collections; using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Network;

namespace Server.Engines.Craft
{
	public class TailoringSystem : CraftSystem
	{
		private static ItemListEntry[] m_Shirts = new ItemListEntry[]
		{
			new CraftSystemItem( "Shirt (8 cloth)",			0x1517, SkillName.Tailoring, 0, 100, 8, typeof( Shirt ) ),
			new CraftSystemItem( "Fancy Shirt (8 cloth)",	0x1efd, SkillName.Tailoring, 0, 100, 8, typeof( FancyShirt ) ),
			new CraftSystemItem( "Fancy Dress (12 cloth)",	0x1eff, SkillName.Tailoring, 0, 100, 12, typeof( FancyDress ) ),
			new CraftSystemItem( "Plain Dress (10 cloth)",	0x1f01, SkillName.Tailoring, 0, 100, 10, typeof( PlainDress ) ),
			new CraftSystemItem( "Cloak (14 cloth)",		0x1515,	SkillName.Tailoring, 0, 100, 14, typeof( Cloak ) ),
			new CraftSystemItem( "Robe (16 cloth)",			0x1F03, SkillName.Tailoring, 0, 100, 16, typeof( Robe ) ),
			new CraftSystemItem( "Jester Suit (24 cloth)",	0x1f9f, SkillName.Tailoring, 0, 100, 24, typeof( JesterSuit ) ),
			new CraftSystemItem( "Surcoat (26 cloth)",		0x1ffd, SkillName.Tailoring, 0, 100, 26, typeof( Surcoat ) ),
		};

		private static ItemListEntry[] m_Pants = new ItemListEntry[]
		{
			new CraftSystemItem( "Fancy Pants (8 cloth)",	0x1539, SkillName.Tailoring, 0, 100, 8, typeof( LongPants ) ),
			new CraftSystemItem( "Kilt (8 cloth)",			0x1537, SkillName.Tailoring, 0, 100, 8, typeof( Kilt ) ),
			new CraftSystemItem( "Skirt (12 cloth)",		0x1516, SkillName.Tailoring, 0, 100, 12, typeof( Skirt ) ),
		};

		private static ItemListEntry[] m_Misc = new ItemListEntry[]
		{
			new CraftSystemItem( "Skullcap (2 cloth)",		0x1544, SkillName.Tailoring, 0, 100, 2, typeof( SkullCap ) ),
			new CraftSystemItem( "Bandana (2 cloth)",		0x1540, SkillName.Tailoring, 0, 100, 2, typeof( Bandana ) ),
			new CraftSystemItem( "Body Sash (4 cloth)",		0x1541, SkillName.Tailoring, 0, 100, 4, typeof( BodySash ) ),
			new CraftSystemItem( "Half Apron (6 cloth)",	0x153b, SkillName.Tailoring, 0, 100, 6, typeof( HalfApron ) ),
			new CraftSystemItem( "Full Apron (10 cloth)",	0x153d, SkillName.Tailoring, 0, 100, 10, typeof( FullApron ) ),
			new CraftSystemItem( "Straw hat (10 Cloth)",	0x1717,	SkillName.Tailoring, 0, 100, 10, typeof( StrawHat ) ),
			new CraftSystemItem( "Bonnet (11 Cloth)",		0x1719,	SkillName.Tailoring, 0, 100, 11, typeof( Bonnet ) ),
			new CraftSystemItem( "Floppy hat (11 Cloth)",	0x1713,	SkillName.Tailoring, 0, 100, 11, typeof( FloppyHat ) ),
			new CraftSystemItem( "Cap (11 Cloth)",			0x1715,	SkillName.Tailoring, 0, 100, 11, typeof( Cap ) ),
			new CraftSystemItem( "Feathered hat (12 Cloth)",0x171a,	SkillName.Tailoring, 0, 100, 12, typeof( FeatheredHat ) ),
			new CraftSystemItem( "Tall hat (12 Cloth)",		0x1716,	SkillName.Tailoring, 0, 100, 12, typeof( TallStrawHat ) ),
			new CraftSystemItem( "Tricorne (12 Cloth)",		0x171B,	SkillName.Tailoring, 0, 100, 12, typeof( TricorneHat ) ),
			new CraftSystemItem( "Wide hat (12 Cloth)",		0x1714,	SkillName.Tailoring, 0, 100, 12, typeof( WideBrimHat ) ),
			new CraftSystemItem( "Jester hat (15 Cloth)",	0x171C,	SkillName.Tailoring, 0, 100, 15, typeof( JesterHat ) ),
			new CraftSystemItem( "Wizard hat (15 Cloth)",	0x1718,	SkillName.Tailoring, 0, 100, 15, typeof( WizardsHat ) ),
		};

		private static ItemListEntry[] m_BoltOfCloth = new ItemListEntry[]
		{
			new CraftSystemItem( "Bolt of Cloth (50 cloth)", 0xF95, SkillName.Tailoring, 0, 0, 50, typeof( BoltOfCloth ) ),
		};

		private static ItemListEntry[] m_FemaleArmor = new ItemListEntry[]
		{
			new CraftSystemItem( "Leather Brassiere (6 hides)", 0x1c0a, SkillName.Tailoring,0, 100, 6, typeof( LeatherBustierArms ) ), 
			new CraftSystemItem( "Leather Skirt (6 hides)", 0x1c08, SkillName.Tailoring,	0, 100, 6, typeof( LeatherSkirt ) ), 
			new CraftSystemItem( "Leather Shorts (8 hides)", 0x1c00, SkillName.Tailoring,	0, 100, 8, typeof( LeatherShorts ) ), 
			new CraftSystemItem( "Studded Brassiere (8 hides)", 0x1c0c, SkillName.Tailoring,0, 100, 8, typeof( StuddedBustierArms ) ), 
			new CraftSystemItem( "Leather One-Piece (8 hides)", 0x1c06, SkillName.Tailoring,0, 100, 8, typeof( FemaleLeatherChest ) ), 
			new CraftSystemItem( "Studded One-Piece (10 hides)", 0x1c02, SkillName.Tailoring,0, 100, 10, typeof( FemaleStuddedChest ) ), 
		};

		private static ItemListEntry[] m_LeatherArmor = new ItemListEntry[]
		{
			new CraftSystemItem( "Gloves (3 hides)", 0x13c6, SkillName.Tailoring, 0, 100, 3, typeof( LeatherGloves ) ), 
			new CraftSystemItem( "Gorget (4 hides)", 0x13c7, SkillName.Tailoring, 0, 100, 4, typeof( LeatherGorget ) ), 
			new CraftSystemItem( "Sleeves (4 hides)",0x13cd, SkillName.Tailoring, 0, 100, 4, typeof( LeatherArms ) ),
			new CraftSystemItem( "Leggings (12 hides)", 0x13cb, SkillName.Tailoring,0, 100, 12, typeof( LeatherLegs ) ), 
			new CraftSystemItem( "Tunic (12 hides)", 0x13cc, SkillName.Tailoring, 0, 100, 12, typeof( LeatherChest ) ), 
		};

		private static ItemListEntry[] m_StuddedArmor = new ItemListEntry[]
		{
			new CraftSystemItem( "Gloves (8 hides)", 0x13d5, SkillName.Tailoring, 0, 100, 8, typeof( StuddedGloves ) ), 
			new CraftSystemItem( "Gorget (6 hides)", 0x13d6, SkillName.Tailoring, 0, 100, 6, typeof( StuddedGorget ) ), 
			new CraftSystemItem( "Sleeves (8 hides)",0x13dc, SkillName.Tailoring, 0, 100, 8, typeof( StuddedArms ) ),
			new CraftSystemItem( "Leggings (12 hides)", 0x13da, SkillName.Tailoring,0, 100, 12, typeof( StuddedLegs ) ), 
			new CraftSystemItem( "Tunic (14 hides)", 0x13db, SkillName.Tailoring, 0, 100, 14, typeof( StuddedChest ) ), 
		};

		private static ItemListEntry[] m_Shoes = new ItemListEntry[]
		{
			new CraftSystemItem( "Sandals (4 hides)",		0x170d, SkillName.Tailoring, 0, 100, 8, typeof( Sandals ) ), 
			new CraftSystemItem( "Shoes (6 hides)",			0x170f, SkillName.Tailoring, 0, 100, 6, typeof( Shoes ) ), 
			new CraftSystemItem( "Boots (8 hides)",			0x170b, SkillName.Tailoring, 0, 100, 8, typeof( Boots ) ),
			new CraftSystemItem( "Thigh Boots (10 hides)",	0x1711, SkillName.Tailoring, 0, 100, 10, typeof( ThighBoots ) ), 
		};

		private static ItemListEntry[] m_LeatherMenu = new ItemListEntry[]
		{
			new CraftSubMenu( "Foot Wear",	  0x170f, m_Shoes ),
			new CraftSubMenu( "Female Armor", 0x1c06, m_FemaleArmor ),
			new CraftSubMenu( "Leather Armor",0x13cd, m_LeatherArmor ),
			new CraftSubMenu( "Studded Armor",0x13dc, m_StuddedArmor ),
		};

		private static ItemListEntry[] m_ClothMenu = new ItemListEntry[]
		{
			new CraftSubMenu( "Shirts", 0x1517, m_Shirts ),
			new CraftSubMenu( "Pants", 0x152e, m_Pants ),
			new CraftSubMenu( "Misc", 0x153d, m_Misc ),
		};

		private static ItemListEntry[] m_ClothMenuWithBolt = new ItemListEntry[]
		{
			new CraftSubMenu( "Shirts", 0x1517, m_Shirts ),
			new CraftSubMenu( "Pants", 0x152e, m_Pants ),
			new CraftSubMenu( "Misc", 0x153d, m_Misc ),
			new CraftSubMenu( "Bolt of Cloth", 0xF95, m_BoltOfCloth )
		};

		public static int GetResourcesFor( Type find )
		{
			return GetResourcesFor( find, m_ClothMenuWithBolt );
		}

		private Item m_Resource;

		public override int SoundEffect	{ get { return 0x248; } }
		public override string TargetPrompt { get { return "Select the cloth to use."; } }

		protected override bool OnTarget( Item target )
		{
			m_Resource = target;
			if ( target is Hides || target is Leather )
				return ShowMenu( m_LeatherMenu );
			else if ( target is BoltOfCloth )
				return ShowMenu( m_ClothMenu );
			else if ( target is Cloth )
				return ShowMenu( target.Amount >= 50 ? m_ClothMenuWithBolt : m_ClothMenu );
			else
				return false;
		}

		protected override void OnSuccess( Item made )
		{
			made.Hue = m_Resource.Hue;
			base.OnSuccess( made );
		}

		protected override bool ConsumeResources( int count, Type toMake )
		{
			if ( m_Resource.Deleted || !m_Resource.IsChildOf( m_Mobile ) || !m_Mobile.Alive )
				return false;

			if ( m_Resource is BoltOfCloth )
			{
				if ( count > 50 )
					return false;

				int left = 50 - count;
				if ( left > 0 )
				{
					Item cloth = new Cloth( left );
					cloth.Hue = m_Resource.Hue;
					m_Mobile.AddToBackpack( cloth );
				}

				m_Resource.Consume();
				return true;
			}
			else if ( count <= m_Resource.Amount )
			{
				m_Resource.Consume( count );
				return true;
			}
			else
			{
				return false;
			}
		}

		protected override bool HasResources( int count, Type toMake )
		{
			if ( m_Resource.Deleted || !m_Resource.IsChildOf( m_Mobile ) || !m_Mobile.Alive )
				return false;

			if ( m_Resource is BoltOfCloth )
				return count <= 50;
			else
				return count <= m_Resource.Amount;
		}
	}
}


