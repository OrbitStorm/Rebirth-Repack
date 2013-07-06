using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Shepherd : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBStavesWeapon() );
		}

		[Constructable]
		public Shepherd() : base( "the shepherd" )
		{
			Job = JobFragment.shepherd;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 25, 47.5 );
			SetSkill( SkillName.MagicResist, 25, 47.5 );
			SetSkill( SkillName.Parry, 25, 47.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Camping, 55, 77.5 );
			SetSkill( SkillName.Herding, 55, 77.5 );

		}

		public override void InitBody()
		{
			SetStr( 51, 65 );
			SetDex( 41, 55 );
			SetInt( 31, 45 );
			Hue = Utility.RandomSkinHue();
			SpeechHue = Utility.RandomDyedHue();

			Female = Utility.RandomBool();
			if ( Female )
			{
				Body = 401;
				Name = NameList.RandomName( "female" );
				Title = "the shepherdess";
			}
			else
			{
				Body = Female ? 401 : 400;
				Name = NameList.RandomName( "male" );
			}
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
			item = new LongPants();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = new StrawHat();
			item.Hue = Utility.RandomNeutralHue();
			AddItem( item );
			item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
			AddItem( item );
			item = new ShepherdsCrook();
			AddItem( item );
			PackGold( 15, 100 );
		}

		public Shepherd( Serial serial ) : base( serial )
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

