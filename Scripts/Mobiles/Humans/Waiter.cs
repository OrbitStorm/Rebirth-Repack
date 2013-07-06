using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Waiter : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBWaiter() );
		}

		[Constructable]
		public Waiter() : base( "the waiter" )
		{
			Job = JobFragment.waiter;
			Karma = Utility.RandomMinMax( 13, -45 );

			SetSkill( SkillName.Tactics, 15, 37.5 );
			SetSkill( SkillName.MagicResist, 15, 37.5 );
			SetSkill( SkillName.Parry, 15, 37.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
		}

		public override void InitBody()
		{
			SetStr( 36, 50 );
			SetDex( 36, 50 );
			SetInt( 21, 35 );
			Hue = Utility.RandomSkinHue();
			SpeechHue = Utility.RandomDyedHue();
			Body = 400;
			Name = NameList.RandomName( "male" );
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
			item = new HalfApron();
			item.Hue = 2301;
			AddItem( item );
		}

		public Waiter( Serial serial ) : base( serial )
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

