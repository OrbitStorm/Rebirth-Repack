using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Bowyer : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.RangersGuild; } }
		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBRangedWeapon() );
			m_SBInfos.Add( new SBBowyer() );
		}

		[Constructable]
		public Bowyer() : base( "the bowyer" )
		{
			Job = JobFragment.bowyer;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 45, 67.5 );
			SetSkill( SkillName.MagicResist, 45, 67.5 );
			SetSkill( SkillName.Parry, 45, 67.5 );
			SetSkill( SkillName.Swords, 25, 47.5 );
			SetSkill( SkillName.Macing, 25, 47.5 );
			SetSkill( SkillName.Fencing, 25, 47.5 );
			SetSkill( SkillName.Wrestling, 25, 47.5 );
			SetSkill( SkillName.Archery, 65, 87.5 );
			SetSkill( SkillName.Fletching, 65, 87.5 );

		}

		public override void InitBody()
		{
			SetStr( 66, 80 );
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
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new ShortPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new LeatherGorget();
				item.Hue = 443;
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new LeatherGorget();
				item.Hue = 443;
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Bowyer( Serial serial ) : base( serial )
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

