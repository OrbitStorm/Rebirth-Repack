using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class EvilMage : BaseCreature
	{
		[Constructable]
		public EvilMage() : base( AIType.AI_Mage, FightMode.Closest,  9, 9, 0.4, 0.75 )
		{
			Female = false;
			Body = 400;
			Title = "the mage";
			Name = NameList.RandomName( "evil mage" );
			Hue = Utility.RandomSkinHue();
			SetStr( 81, 105 );
			SetDex( 91, 115 );
			SetInt( 96, 120 );
			Karma = -125;

			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 75, 97.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.EvalInt, 55, 77.5 );
			SetSkill( SkillName.Magery, 85.1, 100 );
			SetSkill( SkillName.Inscribe, 75.1, 90 );
			SetSkill( SkillName.Wrestling, 20.2, 60 );

			VirtualArmor = 8;
			SetDamage( 3, 12 );

			Item item = null;

			item = AddRandomHair();
			item.Hue = Utility.RandomHairHue();
			AddRandomFacialHair( item.Hue );

			item = new Robe();
			item.Hue = Utility.RandomRedHue();
			AddItem( item );
			item = new Sandals();
			AddItem( item );
			PackGold( 15, 100 );
			LootPack.HighScrolls.Generate( this );
			LootPack.HighScrolls.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public EvilMage( Serial serial ) : base( serial )
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

