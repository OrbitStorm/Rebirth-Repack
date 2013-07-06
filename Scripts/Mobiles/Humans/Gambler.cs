using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Gambler : BaseCreature
	{
		[Constructable]
		public Gambler() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the gambler";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 31, 45 );
			SetDex( 51, 65 );
			SetInt( 56, 70 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 25, 47.5 );
			SetSkill( SkillName.MagicResist, 25, 47.5 );
			SetSkill( SkillName.Parry, 25, 47.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.ItemID, 35, 57.5 );
			SetSkill( SkillName.Snooping, 55, 77.5 );
			SetSkill( SkillName.Stealing, 35, 57.5 );
			SetSkill( SkillName.EvalInt, 45, 67.5 );


			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new ShortPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				switch ( Utility.Random( 4 ) )
				{
					case 0: item = new Boots(); break;
					case 1: item = new ThighBoots(); break;
					case 2: item = new Shoes(); break;
					case 3: default: item = new Sandals(); break;
				}
				AddItem( item );
				item = new Dagger();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				switch ( Utility.Random( 4 ) )
				{
					case 0: item = new Boots(); break;
					case 1: item = new ThighBoots(); break;
					case 2: item = new Shoes(); break;
					case 3: default: item = new Sandals(); break;
				}
				AddItem( item );
				item = new Dagger();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Gambler( Serial serial ) : base( serial )
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

