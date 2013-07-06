using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[TypeAlias( "Server.Mobiles.HireBeggar" )]
	public class Beggar : BaseHire
	{
		[Constructable]
		public Beggar()
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the beggar";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 26, 40 );
			SetDex( 21, 35 );
			SetInt( 36, 50 );
			Karma = Utility.RandomMinMax( 4, -5 );

			
			SetSkill( SkillName.Tactics, 15, 37.5 );
			SetSkill( SkillName.MagicResist, 15, 37.5 );
			SetSkill( SkillName.Parry, 15, 37.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Begging, 55, 77.5 );
			SetSkill( SkillName.Snooping, 25, 47.5 );
			SetSkill( SkillName.Stealing, 15, 37.5 );


			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new LongPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				if ( Utility.RandomBool() )
				{
					item = new Sandals();
					AddItem( item );
				}
				LootPack.Poor.Generate( this );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				if ( Utility.RandomBool() )
				{
					item = new Sandals();
					AddItem( item );
				}
				LootPack.Poor.Generate( this );
			}
		}

		public Beggar( Serial serial ) : base( serial )
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

