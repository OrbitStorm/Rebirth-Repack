using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Weaponsmith : BaseVendor
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
		}

		[Constructable]
		public Weaponsmith() : base( "the weaponsmith" )
		{
			Job = JobFragment.weaponsmith;
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
			SetDex( 86, 100 );
			SetInt( 81, 95 );
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
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new HalfApron();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public Weaponsmith( Serial serial ) : base( serial )
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

