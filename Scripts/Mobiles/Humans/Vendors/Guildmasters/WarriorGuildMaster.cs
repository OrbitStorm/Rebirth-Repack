using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class WarriorGuildMaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.WarriorsGuild; } }

		[Constructable]
		public WarriorGuildMaster() : base( "warrior's" )
		{
			Job = JobFragment.fighter;
			Karma = Utility.RandomMinMax( 15, -25 );

			
			SetSkill( SkillName.Tactics, 75, 97.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 75, 97.5 );
			SetSkill( SkillName.Swords, 65, 87.5 );
			SetSkill( SkillName.Macing, 65, 87.5 );
			SetSkill( SkillName.Fencing, 55, 77.5 );
			SetSkill( SkillName.Wrestling, 55, 77.5 );
			SetSkill( SkillName.ArmsLore, 55, 77.5 );

		}

		public override void InitBody()
		{
			SetStr( 96, 110 );
			SetDex( 91, 105 );
			SetInt( 81, 95 );
			Hue = Utility.RandomSkinHue();
			SpeechHue = Utility.RandomDyedHue();

			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
		}

		public override void InitOutfit()
		{
			Item item = null;
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
			item = new Tunic();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = new PlateGorget();
			AddItem( item );
			item = new DoubleAxe();
			AddItem( item );
			PackGold( 15, 100 );
		}

		public WarriorGuildMaster( Serial serial ) : base( serial )
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

