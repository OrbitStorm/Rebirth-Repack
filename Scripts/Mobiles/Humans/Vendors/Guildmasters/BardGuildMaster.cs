using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class BardGuildMaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.BardsGuild; } }

		[Constructable]
		public BardGuildMaster() : base( "bardic" )
		{
			Job = JobFragment.bard;
			Karma = Utility.RandomMinMax( 15, -25 );

			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Musicianship, 55, 77.5 );
			SetSkill( SkillName.Provocation, 55, 77.5 );
			SetSkill( SkillName.Discordance, 55, 77.5 );
			SetSkill( SkillName.Peacemaking, 55, 77.5 );

		}

		public override void InitBody()
		{
			SetStr( 86, 100 );
			SetDex( 71, 85 );
			SetInt( 61, 75 );
			Hue = Utility.RandomSkinHue();
			SpeechHue = Utility.RandomDyedHue();

			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
		}

		public override void InitOutfit()
		{
			Item item = null;
			item = AddRandomHair();
			item.Hue = Utility.RandomHairHue();
			item = AddRandomFacialHair( item.Hue );
			item = new Shirt();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = new ShortPants();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = new Shoes();
			item.Hue = Utility.RandomNeutralHue();
			AddItem( item );
			item = Loot.RandomInstrument();
			PackItem( item );
			PackGold( 15, 100 );
		}

		public BardGuildMaster( Serial serial ) : base( serial )
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

