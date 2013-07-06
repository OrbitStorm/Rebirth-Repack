using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Miner : BaseCreature
	{
		[Constructable]
		public Miner() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the miner";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 66, 80 );
			SetDex( 51, 65 );
			SetInt( 41, 55 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Mining, 45, 67.5 );
			SetSkill( SkillName.Tactics, 35, 57.5 );
			SetSkill( SkillName.MagicResist, 35, 57.5 );
			SetSkill( SkillName.Parry, 35, 57.5 );
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
				item.Hue = 946;
				AddItem( item );
				item = new ShortPants();
				item.Hue = Utility.RandomBlueHue();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				item.Hue = 443;
				AddItem( item );
				item = new Pickaxe();
				AddItem( item );
				LootPack.Meager.Generate( this );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = 946;
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomBlueHue();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				item.Hue = 443;
				AddItem( item );
				item = new Pickaxe();
				AddItem( item );
				LootPack.Meager.Generate( this );
			}
		}

		public Miner( Serial serial ) : base( serial )
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

