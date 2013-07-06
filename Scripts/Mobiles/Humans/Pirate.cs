using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Pirate : BaseCreature
	{
		[Constructable]
		public Pirate() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the pirate";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 86, 100 );
			SetDex( 86, 100 );
			SetInt( 71, 85 );
			Karma = 2;

			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Swords, 55, 77.5 );
			SetSkill( SkillName.Macing, 25, 47.5 );
			SetSkill( SkillName.Fencing, 25, 47.5 );
			SetSkill( SkillName.Wrestling, 45, 67.5 );


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
				item = new Cutlass();
				AddItem( item );
				if ( Utility.RandomBool() )
				{
					item = new Bandana();
				}
				else
				{
					item = new SkullCap();
				}
				item.Hue = Utility.RandomRedHue();
				AddItem( item );
				item = new ThighBoots();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new ShortPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Cutlass();
				AddItem( item );
				if ( Utility.RandomBool() )
				{
					item = new Bandana();
				}
				else
				{
					item = new SkullCap();
				}
				item.Hue = Utility.RandomRedHue();
				AddItem( item );
				item = new ThighBoots();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		public override bool AlwaysAttackable { get{ return true; } }

		public Pirate( Serial serial ) : base( serial )
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

