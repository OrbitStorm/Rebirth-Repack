using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Messenger : BaseEscortable
	{
		public Messenger()
		{
			Title = "the messenger";
			Job = JobFragment.runner;
			SetStr( 36, 50 );
			SetDex( 31, 45 );
			SetInt( 36, 50 );
			Karma = Utility.RandomMinMax( -1, -10 );

			SetSkill( SkillName.Tactics, 27.5, 50 );
			SetSkill( SkillName.MagicResist, 27.5, 50 );
			SetSkill( SkillName.Parry, 27.5, 50 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.ItemID, 55, 77.5 );
		}

		public override void InitOutfit()
		{
			if ( Female )
			{
				Item item = null;
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				LootPack.Poor.Generate( this );
			}
			else
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
			}
		}

		public Messenger( Serial serial ) : base( serial )
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

