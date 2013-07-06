using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Carpenter : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBCarpenter() );
		}

		[Constructable]
		public Carpenter() : base( "the carpenter" )
		{
			Job = JobFragment.carpenter;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 45, 67.5 );
			SetSkill( SkillName.MagicResist, 45, 67.5 );
			SetSkill( SkillName.Parry, 45, 67.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Carpentry, 55, 77.5 );
			SetSkill( SkillName.Lumberjacking, 45, 67.5 );

		}

		public override void InitBody()
		{
			SetStr( 71, 85 );
			SetDex( 66, 80 );
			SetInt( 51, 65 );
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
				item = new Shoes();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				item = new HalfApron();
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
				item = new Shoes();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				item = new HalfApron();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Carpenter( Serial serial ) : base( serial )
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

