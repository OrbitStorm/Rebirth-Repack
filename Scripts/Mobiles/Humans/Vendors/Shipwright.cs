using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Shipwright : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBShipwright() );
		}

		[Constructable]
		public Shipwright() : base( "the shipwright" )
		{
			Job = JobFragment.shipwright;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 45, 67.5 );
			SetSkill( SkillName.MagicResist, 45, 67.5 );
			SetSkill( SkillName.Parry, 45, 67.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );

			BankAccount = BankRestockAmount = 0x40000000;
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
				item = new Shoes();
				item.Hue = Utility.RandomNeutralHue();
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
				PackGold( 15, 100 );
			}
		}

		public Shipwright( Serial serial ) : base( serial )
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

		public override double GetBuyDiscountFor( Mobile from )
		{
			return 1.0;
		}
		
		public override double GetSellDiscountFor( Mobile from )
		{
			return 1.0;
		}
	}
}

