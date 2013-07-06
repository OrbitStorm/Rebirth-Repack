using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Jailor : BaseCreature
	{
		[Constructable]
		public Jailor() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the jailor";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 96, 110 );
			SetDex( 96, 110 );
			SetInt( 71, 85 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 75, 97.5 );
			SetSkill( SkillName.MagicResist, 75, 97.5 );
			SetSkill( SkillName.Parry, 75, 97.5 );
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
				item = new ChainChest();
				AddItem( item );
				item = new ChainLegs();
				AddItem( item );
				item = new BronzeShield();
				AddItem( item );
				item = new Boots();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				item = new VikingSword();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new ChainChest();
				AddItem( item );
				item = new ChainLegs();
				AddItem( item );
				item = new BronzeShield();
				AddItem( item );
				item = new Boots();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				item = new VikingSword();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Jailor( Serial serial ) : base( serial )
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

