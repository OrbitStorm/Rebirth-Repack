using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class HireFighter : BaseHire
	{
		[Constructable]
		public HireFighter()
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the fighter";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 11, 88 );
			SetDex( 11, 88 );
			SetInt( 7, 49 );
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
				item = new StuddedChest();
				AddItem( item );
				item = new StuddedLegs();
				AddItem( item );
				item = new StuddedArms();
				AddItem( item );
				item = new StuddedGloves();
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
				item = new StuddedGorget();
				AddItem( item );
				item = Loot.RandomWeapon();
				AddItem( item );
				if ( item.Layer == Layer.OneHanded )
				{
					item = new WoodenShield();
					AddItem( item );
				}
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new StuddedChest();
				AddItem( item );
				item = new StuddedLegs();
				AddItem( item );
				item = new StuddedArms();
				AddItem( item );
				item = new StuddedGloves();
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
				item = new StuddedGorget();
				AddItem( item );
				item = Loot.RandomWeapon();
				AddItem( item );
				if ( item.Layer == Layer.OneHanded )
				{
					item = new WoodenShield();
					AddItem( item );
				}
			}
		}

		public HireFighter( Serial serial ) : base( serial )
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

