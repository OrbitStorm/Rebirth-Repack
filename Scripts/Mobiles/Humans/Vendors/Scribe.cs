using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Scribe : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.MagesGuild; } }
		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBScribe() );
		}

		[Constructable]
		public Scribe() : base( "the scribe" )
		{
			Job = JobFragment.scholar;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 15, 37.5 );
			SetSkill( SkillName.MagicResist, 15, 37.5 );
			SetSkill( SkillName.Parry, 15, 37.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.EvalInt, 45, 67.5 );

		}

		public override void InitBody()
		{
			SetStr( 16, 30 );
			SetDex( 26, 40 );
			SetInt( 31, 45 );
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
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				item = new Sandals();
				AddItem( item );
				LootPack.Poor.Generate( this );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Robe();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				item = new Sandals();
				AddItem( item );
				LootPack.Poor.Generate( this );
			}
		}

		public Scribe( Serial serial ) : base( serial )
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

