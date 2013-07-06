using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Waitress : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBWaiter() );
		}

		[Constructable]
		public Waitress() : base( "the waitress" )
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

			Female = true;
			Body = 401;
			Name = NameList.RandomName( "female" );
		}

		public override void InitOutfit()
		{
			Item item = null;
			item = AddRandomHair();
			item.Hue = Utility.RandomHairHue();
			item = new PlainDress();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = new Shoes();
			item.Hue = Utility.RandomNeutralHue();
			AddItem( item );
			item = new HalfApron();
			item.Hue = 2301;
			AddItem( item );
		}

		public Waitress( Serial serial ) : base( serial )
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

