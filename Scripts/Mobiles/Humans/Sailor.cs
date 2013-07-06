using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Sailor : BaseCreature
	{
		[Constructable]
		public Sailor() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the sailor";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 66, 80 );
			SetDex( 66, 80 );
			SetInt( 41, 55 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 45, 67.5 );
			SetSkill( SkillName.MagicResist, 45, 67.5 );
			SetSkill( SkillName.Parry, 45, 67.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );


			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Shirt();
				item.Hue = 2301;
				AddItem( item );
				item = new ShortPants();
				item.Hue = 2301;
				AddItem( item );
				item = new Cutlass();
				AddItem( item );
				item = new Shoes();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = 2301;
				AddItem( item );
				item = new ShortPants();
				item.Hue = 2301;
				AddItem( item );
				item = new Cutlass();
				AddItem( item );
				item = new Shoes();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Sailor( Serial serial ) : base( serial )
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

