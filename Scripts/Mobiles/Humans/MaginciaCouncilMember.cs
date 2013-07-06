using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class MaginciaCouncilMember : BaseCreature
	{
		[Constructable]
		public MaginciaCouncilMember() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the council member";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 41, 55 );
			SetDex( 41, 55 );
			SetInt( 41, 55 );
			Karma = Utility.RandomMinMax( 0, -60 );

			
			SetSkill( SkillName.Tactics, 25, 47.5 );
			SetSkill( SkillName.MagicResist, 25, 47.5 );
			SetSkill( SkillName.Parry, 25, 47.5 );
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
				item = new FancyShirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new LongPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new BodySash();
				item.Hue = Utility.RandomRedHue();
				AddItem( item );
				item = new Longsword();
				AddItem( item );
				LootPack.Rich.Generate( this );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new FancyShirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new BodySash();
				item.Hue = Utility.RandomRedHue();
				AddItem( item );
				item = new Longsword();
				AddItem( item );
				LootPack.Rich.Generate( this );
			}
		}

		public MaginciaCouncilMember( Serial serial ) : base( serial )
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

