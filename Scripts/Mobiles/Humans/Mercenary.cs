using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Mercenary : BaseCreature
	{
		[Constructable]
		public Mercenary() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the mercenary";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 25, 88 );
			SetDex( 25, 88 );
			SetInt( 37, 49 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 45, 67.5 );
			SetSkill( SkillName.MagicResist, 45, 67.5 );
			SetSkill( SkillName.Parry, 45, 67.5 );
			SetSkill( SkillName.Swords, 45, 67.5 );
			SetSkill( SkillName.Macing, 45, 67.5 );
			SetSkill( SkillName.Fencing, 45, 67.5 );
			SetSkill( SkillName.Wrestling, 45, 67.5 );
			SetSkill( SkillName.ArmsLore, 42.5, 65 );


			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				switch ( Utility.Random( 5 ) )
				{
					case 0: item = new PlateChest(); break;
					case 1: item = new ChainChest(); break;
					case 2: item = new StuddedChest(); break;
					case 3: item = new RingmailChest(); break;
					case 4: default: item = new LeatherChest(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 5 ) )
				{
					case 0: item = new PlateLegs(); break;
					case 1: item = new ChainLegs(); break;
					case 2: item = new StuddedLegs(); break;
					case 3: item = new RingmailLegs(); break;
					case 4: default: item = new LeatherLegs(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 4 ) )
				{
					case 0: item = new PlateArms(); break;
					case 1: item = new RingmailArms(); break;
					case 2: item = new StuddedArms(); break;
					case 3: default: item = new LeatherArms(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 3 ) )
				{
					case 0: item = new PlateGloves(); break;
					case 1: item = new StuddedGloves(); break;
					case 2: default: item = new LeatherGloves(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 6 ) )
				{
					case 0: item = new PlateHelm(); break;
					case 1: item = new ChainCoif(); break;
					case 2: item = new CloseHelm(); break;
					case 3: item = new Bascinet(); break;
					case 4: item = new NorseHelm(); break;
					case 5: default: item = new Helmet(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 7 ) )
				{
					case 0: item = new BronzeShield(); break;
					case 1: item = new Buckler(); break;
					case 2: item = new HeaterShield(); break;
					case 3: item = new MetalKiteShield(); break;
					case 4: item = new MetalShield(); break;
					case 5: item = new WoodenKiteShield(); break;
					case 6: default: item = new WoodenShield(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 4 ) )
				{
					case 0: item = new PlateGorget(); break;
					case 1: item = new StuddedGorget(); break;
					default: case 2: item = new LeatherGorget(); break;
				}
				AddItem( item );
				item = Loot.RandomWeapon();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				switch ( Utility.Random( 5 ) )
				{
					case 0: item = new PlateChest(); break;
					case 1: item = new ChainChest(); break;
					case 2: item = new StuddedChest(); break;
					case 3: item = new RingmailChest(); break;
					case 4: default: item = new LeatherChest(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 5 ) )
				{
					case 0: item = new PlateLegs(); break;
					case 1: item = new ChainLegs(); break;
					case 2: item = new StuddedLegs(); break;
					case 3: item = new RingmailLegs(); break;
					case 4: default: item = new LeatherLegs(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 3 ) )
				{
					case 0: item = new PlateGloves(); break;
					case 1: item = new StuddedGloves(); break;
					case 2: default: item = new LeatherGloves(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 6 ) )
				{
					case 0: item = new PlateHelm(); break;
					case 1: item = new ChainCoif(); break;
					case 2: item = new CloseHelm(); break;
					case 3: item = new Bascinet(); break;
					case 4: item = new NorseHelm(); break;
					case 5: default: item = new Helmet(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 7 ) )
				{
					case 0: item = new BronzeShield(); break;
					case 1: item = new Buckler(); break;
					case 2: item = new HeaterShield(); break;
					case 3: item = new MetalKiteShield(); break;
					case 4: item = new MetalShield(); break;
					case 5: item = new WoodenKiteShield(); break;
					case 6: default: item = new WoodenShield(); break;
				}
				AddItem( item );
				switch ( Utility.Random( 4 ) )
				{
					case 0: item = new PlateGorget(); break;
					case 1: item = new StuddedGorget(); break;
					default: case 2: item = new LeatherGorget(); break;
				}
				AddItem( item );
				item = Loot.RandomWeapon();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Mercenary( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}

