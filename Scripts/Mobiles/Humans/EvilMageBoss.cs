using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class EvilMageLord : BaseCreature
	{
		[Constructable]
		public EvilMageLord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 400;
			Title = "the mage";
			Name = "Lord " + NameList.RandomName( "evil mage" );
			Hue = Utility.RandomSkinHue();
			SetStr( 81, 105 );
			SetDex( 91, 115 );
			SetInt( 126, 150 );
			Karma = -125;
			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 75, 97.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Magery, 95.1, 100 );
			SetSkill( SkillName.Wrestling, 20.2, 60 );

			VirtualArmor = 18;
			SetDamage( 3, 12 );

			Item item = null;
			item = AddRandomHair();
			item.Hue = Utility.RandomHairHue();
			item = AddRandomFacialHair( item.Hue );
			item = new Robe();
			item.Hue = 1106+Utility.Random( 4 ); // 1106 to 1109
			AddItem( item );
			item = new Sandals();
			AddItem( item );
			LootPack.FilthyRich.Generate( this );
			LootPack.HighScrolls.Generate( this );
			LootPack.HighScrolls.Generate( this );
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public override bool AlwaysMurderer{ get{ return true; } }

		public EvilMageLord( Serial serial ) : base( serial )
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

