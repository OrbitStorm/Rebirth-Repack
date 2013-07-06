using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class TinkerGuildMaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.TinkersGuild; } }

		[Constructable]
		public TinkerGuildMaster() : base( "tinker" )
		{
			Job = JobFragment.tinker;
			Karma = Utility.RandomMinMax( 15, -25 );

			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 55, 77.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Tinkering, 65, 87.5 );
			SetSkill( SkillName.Lockpicking, 65, 87.5 );

		}

		public override void InitBody()
		{
			SetStr( 71, 85 );
			SetDex( 86, 100 );
			SetInt( 86, 100 );
			Hue = Utility.RandomSkinHue();
			SpeechHue = Utility.RandomDyedHue();

			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
		}

		public override void InitOutfit()
		{
			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new FancyShirt();
				item.Hue = Utility.RandomOrangeHue();
				AddItem( item );
				item = new LongPants();
				item.Hue = 2305;
				AddItem( item );
				item = new Shoes();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new FancyShirt();
				item.Hue = Utility.RandomOrangeHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = 2305;
				AddItem( item );
				item = new Shoes();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public TinkerGuildMaster( Serial serial ) : base( serial )
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

