using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class ThiefGuildMaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.ThievesGuild; } }

		[Constructable]
		public ThiefGuildMaster() : base( "thief" )
		{
			Job = JobFragment.thief;
			Karma = Utility.RandomMinMax( 15, -25 );

			
			SetSkill( SkillName.Tactics, 75, 97.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 75, 97.5 );
			SetSkill( SkillName.Swords, 55, 77.5 );
			SetSkill( SkillName.Macing, 45, 67.5 );
			SetSkill( SkillName.Fencing, 75, 97.5 );
			SetSkill( SkillName.Wrestling, 55, 77.5 );
			SetSkill( SkillName.Poisoning, 65, 87.5 );
			SetSkill( SkillName.Lockpicking, 65, 87.5 );
			SetSkill( SkillName.Hiding, 65, 87.5 );
			SetSkill( SkillName.DetectHidden, 55, 77.5 );
			SetSkill( SkillName.Snooping, 65, 87.5 );
			SetSkill( SkillName.Stealing, 65, 87.5 );

		}

		public override void InitBody()
		{
			SetStr( 91, 105 );
			SetDex( 91, 105 );
			SetInt( 91, 105 );
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
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new LongPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Dagger();
				AddItem( item );
				PackGold( 15, 100 );
				item = new Lockpick( Utility.RandomMinMax( 1, 5 ) );
				PackItem( item );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new FancyShirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Dagger();
				AddItem( item );
				PackGold( 15, 100 );
				item = new Lockpick( Utility.RandomMinMax( 1, 5 ) );
				PackItem( item );
			}
		}

		public override bool CheckCustomReqs( PlayerMobile pm )
		{
			if ( pm.Skills[SkillName.Stealing].Base < 60.0 )
			{
				SayTo( pm, 501051 ); // You must be at least a journeyman pickpocket to join this elite organization.
				return false;
			}

			return true;
		}

		public override void SayWelcomeTo( Mobile m )
		{
			SayTo( m, 1008053 ); // Welcome to the guild! Stay to the shadows, friend.
		}

		public ThiefGuildMaster( Serial serial ) : base( serial )
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

