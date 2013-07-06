using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Gypsy : BaseCreature
	{
		[Constructable]
		public Gypsy() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the gypsy";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 41, 55 );
			SetDex( 51, 65 );
			SetInt( 61, 75 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 35, 57.5 );
			SetSkill( SkillName.MagicResist, 35, 57.5 );
			SetSkill( SkillName.Parry, 35, 57.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.ItemID, 45, 67.5 );
			SetSkill( SkillName.Hiding, 45, 67.5 );
			SetSkill( SkillName.Stealing, 45, 67.5 );
			SetSkill( SkillName.Snooping, 45, 67.5 );
			SetSkill( SkillName.Lockpicking, 45, 67.5 );
			SetSkill( SkillName.Camping, 45, 67.5 );
			SetSkill( SkillName.Begging, 45, 67.5 );


			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new LongPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Bandana();
				item.Hue = Utility.RandomBlueHue();
				AddItem( item );
				switch ( Utility.Random( 4 ) )
				{
					case 0: item = new Boots(); break;
					case 1: item = new ThighBoots(); break;
					case 2: item = new Shoes(); break;
					case 3: default: item = new Sandals(); break;
				}
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
				item = new Bandana();
				item.Hue = Utility.RandomBlueHue();
				AddItem( item );
				switch ( Utility.Random( 4 ) )
				{
					case 0: item = new Boots(); break;
					case 1: item = new ThighBoots(); break;
					case 2: item = new Shoes(); break;
					case 3: default: item = new Sandals(); break;
				}
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public override bool AlwaysAttackable
		{
			get
			{
				return true;
			}
		}


		public Gypsy( Serial serial ) : base( serial )
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

