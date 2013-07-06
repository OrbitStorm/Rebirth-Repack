using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class HireMage : BaseHire
	{
		[Constructable]
		public HireMage() : base( AIType.AI_Mage )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
			Title = "the mage";
			Hue = Utility.RandomSkinHue();
			SetStr( 61, 75 );
			SetDex( 25, 50 );
			SetInt( 61, 75 );
			Karma = Utility.RandomMinMax( 13, -45 );
			
			SetSkill( SkillName.Tactics, 55, 77.5 );
			SetSkill( SkillName.MagicResist, 65, 80 );
			SetSkill( SkillName.Parry, 55, 77.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Magery, 65, 80 );
			SetSkill( SkillName.Inscribe, 50.1, 65 );

			Item item = null;
			item = AddRandomHair();
			item.Hue = Utility.RandomHairHue();
			item = new Robe();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = new BodySash();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
		}

		public HireMage( Serial serial ) : base( serial )
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

