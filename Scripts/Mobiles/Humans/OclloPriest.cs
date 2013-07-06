using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class OclloPriest : BaseCreature
	{
		[Constructable]
		public OclloPriest() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
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

			if ( !Female )
			{
				Title = "the priest";
				Item item = null;
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Robe();
				item.Hue = 946;
				AddItem( item );
				item = new Sandals();
				AddItem( item );
				PackGold( 15, 100 );
			}
			else
			{
				Title = "the priestess";
				Item item = null;
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Robe();
				item.Hue = 946;
				AddItem( item );
				item = new Sandals();
				AddItem( item );
			}
		}

		public OclloPriest( Serial serial ) : base( serial )
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

