using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class HireThief : BaseHire
	{
		[Constructable]
		public HireThief()
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the thief";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 61, 75 );
			SetDex( 86, 100 );
			SetInt( 71, 85 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 55, 77.5 );
			SetSkill( SkillName.MagicResist, 55, 77.5 );
			SetSkill( SkillName.Parry, 55, 77.5 );
			SetSkill( SkillName.Swords, 35, 57.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 55, 77.5 );
			SetSkill( SkillName.Wrestling, 25, 47.5 );
			SetSkill( SkillName.Poisoning, 35, 57.5 );
			SetSkill( SkillName.Lockpicking, 35, 57.5 );
			SetSkill( SkillName.Hiding, 45, 67.5 );
			SetSkill( SkillName.DetectHidden, 35, 57.5 );
			SetSkill( SkillName.Snooping, 45, 67.5 );
			SetSkill( SkillName.Stealing, 45, 67.5 );


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
				item = new Lockpick( Utility.RandomMinMax( 1, 5 ) );
				PackItem( item );
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
				item = new Lockpick( Utility.RandomMinMax( 1, 5 ) );
				PackItem( item );
			}
		}

		public HireThief( Serial serial ) : base( serial )
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

