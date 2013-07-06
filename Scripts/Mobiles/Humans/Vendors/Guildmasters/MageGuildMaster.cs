using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class MageGuildMaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.MagesGuild; } }

		[Constructable]
		public MageGuildMaster() : base( "mage" )
		{
			Job = JobFragment.mage;
			Karma = Utility.RandomMinMax( 15, -25 );

			
			SetSkill( SkillName.Tactics, 75, 97.5 );
			SetSkill( SkillName.MagicResist, 75, 97.5 );
			SetSkill( SkillName.Parry, 75, 97.5 );
			SetSkill( SkillName.Swords, 75, 97.5 );
			SetSkill( SkillName.Macing, 75, 97.5 );
			SetSkill( SkillName.Fencing, 75, 97.5 );
			SetSkill( SkillName.Wrestling, 75, 97.5 );
			SetSkill( SkillName.EvalInt, 55, 77.5 );
			SetSkill( SkillName.Magery, 85.1, 100 );
			SetSkill( SkillName.Inscribe, 75.1, 90 );

		}

		public override void InitBody()
		{
			SetStr( 91, 105 );
			SetDex( 101, 115 );
			SetInt( 116, 130 );
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
				item = new Robe();
				item.Hue = Utility.RandomBlueHue();
				AddItem( item );
				item = new Sandals();
				AddItem( item );
				PackGold( 15, 100 );
				LootPack.HighScrolls.Generate( this );
				LootPack.HighScrolls.Generate( this );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Robe();
				item.Hue = Utility.RandomBlueHue();
				AddItem( item );
				item = new Sandals();
				AddItem( item );
				PackGold( 15, 100 );
				LootPack.HighScrolls.Generate( this );
				LootPack.HighScrolls.Generate( this );
			}
		}

		public MageGuildMaster( Serial serial ) : base( serial )
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

