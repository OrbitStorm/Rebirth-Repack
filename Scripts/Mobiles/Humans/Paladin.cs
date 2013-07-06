using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Paladin : BaseCreature
	{
		[Constructable]
		public Paladin() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the paladin";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 64, 92 );
			SetDex( 46, 88 );
			SetInt( 37, 49 );
			Karma = Utility.RandomMinMax( 33, -25 );

			
			SetSkill( SkillName.Tactics, 55, 77.5 );
			SetSkill( SkillName.MagicResist, 55, 77.5 );
			SetSkill( SkillName.Parry, 55, 77.5 );
			SetSkill( SkillName.Swords, 55, 77.5 );
			SetSkill( SkillName.Macing, 55, 77.5 );
			SetSkill( SkillName.Fencing, 55, 77.5 );
			SetSkill( SkillName.Wrestling, 55, 77.5 );
			SetSkill( SkillName.ArmsLore, 52.5, 75 );


			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new PlateChest();
				AddItem( item );
				item = new PlateLegs();
				AddItem( item );
				item = new PlateArms();
				AddItem( item );
				item = new PlateGloves();
				AddItem( item );
				item = new PlateGorget();
				AddItem( item );
				switch ( Utility.Random( 5 ) )
				{
					case 0: item = new PlateHelm(); break;
					case 1: item = new Helmet(); break;
					case 2: item = new CloseHelm(); break;
					case 3: item = new Bascinet(); break;
					case 4: default: item = new NorseHelm(); break;
				}
				AddItem( item );
				item = new Tunic();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new HeaterShield();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new VikingSword();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new PlateChest();
				AddItem( item );
				item = new PlateLegs();
				AddItem( item );
				item = new PlateArms();
				AddItem( item );
				item = new PlateGloves();
				AddItem( item );
				item = new PlateGorget();
				AddItem( item );
				switch ( Utility.Random( 5 ) )
				{
					case 0: item = new PlateHelm(); break;
					case 1: item = new Helmet(); break;
					case 2: item = new CloseHelm(); break;
					case 3: item = new Bascinet(); break;
					case 4: default: item = new NorseHelm(); break;
				}
				AddItem( item );
				item = new Tunic();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new HeaterShield();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new VikingSword();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Paladin( Serial serial ) : base( serial )
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

