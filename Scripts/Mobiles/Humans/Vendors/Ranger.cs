using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Ranger : BaseCreature
	{
		[Constructable]
		public Ranger() : base( AIType.AI_Archer, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 71, 85 );
			SetDex( 76, 90 );
			SetInt( 61, 75 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Swords, 35, 57.5 );
			SetSkill( SkillName.Macing, 35, 57.5 );
			SetSkill( SkillName.Fencing, 35, 57.5 );
			SetSkill( SkillName.Wrestling, 35, 57.5 );
			SetSkill( SkillName.Archery, 55, 77.5 );
			SetSkill( SkillName.Hiding, 45, 67.5 );
			SetSkill( SkillName.Camping, 55, 77.5 );
			SetSkill( SkillName.AnimalLore, 55, 77.5 );
			SetSkill( SkillName.Herding, 45, 67.5 );
			SetSkill( SkillName.Tracking, 45, 67.5 );


			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Shirt();
				item.Hue = Utility.RandomGreenHue();
				AddItem( item );
				item = new ShortPants();
				item.Hue = 443;
				AddItem( item );
				PackGold( 15, 100 );
				item = new Bow();
				AddItem( item );
				item = new Arrow( Utility.RandomMinMax( 5, 25 ) );
				PackItem( item );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomGreenHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = 443;
				AddItem( item );
				PackGold( 15, 100 );
				item = new Bow();
				AddItem( item );
				item = new Arrow( Utility.RandomMinMax( 5, 25 ) );
				PackItem( item );
			}
		}

		public Ranger( Serial serial ) : base( serial )
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

