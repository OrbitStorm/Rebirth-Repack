using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class FurTrader : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBFurtrader() );
		}

		[Constructable]
		public FurTrader() : base( "the furtrader" )
		{
			Job = JobFragment.furtrader;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 55, 77.5 );
			SetSkill( SkillName.MagicResist, 55, 77.5 );
			SetSkill( SkillName.Parry, 45, 67.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.AnimalLore, 65, 87.5 );
			SetSkill( SkillName.Camping, 55, 77.5 );

		}

		public override void InitBody()
		{
			SetStr( 66, 80 );
			SetDex( 51, 65 );
			SetInt( 41, 55 );
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
				item.Hue = 443;
				AddItem( item );
				item = new ShortPants();
				item.Hue = 2305;
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new SkinningKnife();
				AddItem( item );
				PackGold( 15, 100 );
				item = new SkinningKnife();
				AddItem( item );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = 443;
				AddItem( item );
				item = new Skirt();
				item.Hue = 2305;
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new SkinningKnife();
				AddItem( item );
				PackGold( 15, 100 );
				item = new SkinningKnife();
				AddItem( item );
			}
		}

		public FurTrader( Serial serial ) : base( serial )
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

