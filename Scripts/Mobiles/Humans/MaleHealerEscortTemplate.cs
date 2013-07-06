using System;
using Server;
using Server.Items;
using Server.Misc;

/* *SPECIAL SCRIPTS* :
escort
*/

namespace Server.Mobiles
{
	public class MaleHealerEscortTemplate : BaseCreature
	{
		public MaleHealerEscortTemplate() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 71, 85 );
			SetDex( 81, 95 );
			SetInt( 86, 100 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Healing, 55, 77.5 );
			SetSkill( SkillName.Anatomy, 55, 77.5 );
			SetSkill( SkillName.SpiritSpeak, 55, 77.5 );
			SetSkill( SkillName.Forensics, 35, 57.5 );


			Item item = null;
			item = AddRandomHair();
			item.Hue = Utility.RandomHairHue();
			item = AddRandomFacialHair( item.Hue );
			item = new Robe();
			item.Hue = Utility.RandomYellowHue();
			AddItem( item );
			item = new Sandals();
			AddItem( item );
			PackGold( 15, 100 );
		}

		public MaleHealerEscortTemplate( Serial serial ) : base( serial )
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

