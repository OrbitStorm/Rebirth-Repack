using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Blacksmith : BaseVendor
	{
		private System.Collections.ArrayList m_SBInfos = new System.Collections.ArrayList();
		protected override System.Collections.ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.BlacksmithsGuild; } }
		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBAxeWeapon() );
			m_SBInfos.Add( new SBKnifeWeapon() );
			m_SBInfos.Add( new SBMaceWeapon() );
			m_SBInfos.Add( new SBPoleArmWeapon() );
			m_SBInfos.Add( new SBRangedWeapon() );
			m_SBInfos.Add( new SBSpearForkWeapon() );
			m_SBInfos.Add( new SBStavesWeapon() );
			m_SBInfos.Add( new SBSwordWeapon() );

			m_SBInfos.Add( new SBPlateArmor() );
			m_SBInfos.Add( new SBLeatherArmor() );
			m_SBInfos.Add( new SBStuddedArmor() );
			m_SBInfos.Add( new SBChainmailArmor() );
			m_SBInfos.Add( new SBHelmetArmor() );
			m_SBInfos.Add( new SBMetalShields() );
			m_SBInfos.Add( new SBRingmailArmor() );
			m_SBInfos.Add( new SBWoodenShields() );
		}

		[Constructable]
		public Blacksmith() : base( "the blacksmith" )
		{
			Job = JobFragment.blacksmith;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 55, 77.5 );
			SetSkill( SkillName.MagicResist, 55, 77.5 );
			SetSkill( SkillName.Parry, 55, 77.5 );
			SetSkill( SkillName.Swords, 45, 67.5 );
			SetSkill( SkillName.Macing, 45, 67.5 );
			SetSkill( SkillName.Fencing, 45, 67.5 );
			SetSkill( SkillName.Wrestling, 45, 67.5 );
			SetSkill( SkillName.ArmsLore, 45, 67.5 );
			SetSkill( SkillName.Blacksmith, 65, 87.5 );

		}

		public override void InitBody()
		{
			SetStr( 86, 100 );
			SetDex( 66, 80 );
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
				item = new FullApron();
				item.Hue = 2305;
				AddItem( item );
				item = Loot.RandomWeapon();
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
				item = new FullApron();
				item.Hue = 2305;
				AddItem( item );
				item = Loot.RandomWeapon();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Blacksmith( Serial serial ) : base( serial )
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

