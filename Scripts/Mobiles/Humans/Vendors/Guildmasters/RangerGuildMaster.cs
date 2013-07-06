using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class RangerGuildMaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.RangersGuild; } }

		[Constructable]
		public RangerGuildMaster() : base( "ranger" )
		{
			Job = JobFragment.ranger;
			Karma = Utility.RandomMinMax( 15, -25 );

			
			SetSkill( SkillName.Tactics, 75, 97.5 );
			SetSkill( SkillName.MagicResist, 75, 97.5 );
			SetSkill( SkillName.Parry, 75, 97.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Archery, 75, 97.5 );
			SetSkill( SkillName.Hiding, 65, 87.5 );
			SetSkill( SkillName.Camping, 65, 87.5 );
			SetSkill( SkillName.AnimalLore, 65, 87.5 );
			SetSkill( SkillName.Herding, 55, 77.5 );
			SetSkill( SkillName.Tracking, 65, 87.5 );

		}

		public override void InitBody()
		{
			SetStr( 91, 105 );
			SetDex( 96, 110 );
			SetInt( 81, 95 );
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
				item.Hue = Utility.RandomGreenHue();
				AddItem( item );
				item = new LongPants();
				item.Hue = 443;
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new FancyShirt();
				item.Hue = Utility.RandomGreenHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = 443;
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public RangerGuildMaster( Serial serial ) : base( serial )
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

