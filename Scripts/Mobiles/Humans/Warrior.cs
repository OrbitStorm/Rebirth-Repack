using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Warrior : BaseCreature
	{
		[Constructable]
		public Warrior() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the warrior";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 18, 88 );
			SetDex( 18, 88 );
			SetInt( 13, 49 );
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
				item = new RingmailChest();
				AddItem( item );
				item = new RingmailLegs();
				AddItem( item );
				item = new RingmailArms();
				AddItem( item );
				item = new RingmailGloves();
				AddItem( item );
				item = new ChainCoif();
				AddItem( item );
				item = new PlateGorget();
				AddItem( item );
				item = new WoodenShield();
				AddItem( item );
				item = new Broadsword();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new RingmailChest();
				AddItem( item );
				item = new RingmailLegs();
				AddItem( item );
				item = new RingmailArms();
				AddItem( item );
				item = new RingmailGloves();
				AddItem( item );
				item = new ChainCoif();
				AddItem( item );
				item = new PlateGorget();
				AddItem( item );
				item = new WoodenShield();
				AddItem( item );
				item = new Broadsword();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Warrior( Serial serial ) : base( serial )
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

