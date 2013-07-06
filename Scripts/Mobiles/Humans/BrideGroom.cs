using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Gumps;

namespace Server.Mobiles
{ 
	public class BrideGroom : BaseEscortable
	{
		[Constructable]
		public BrideGroom() 
		{
			LootPack.Poor.Generate( this );
		}

		public override void InitOutfit()
		{
			SetStr( 36, 50 );
			SetDex( 31, 45 );
			SetInt( 36, 50 );
			Karma = Utility.RandomMinMax( -1, -10 );
			BaseSoundID = 342;

			if ( Female )
			{
				Title = "the bride";
				Item item = null;
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
			}
			else
			{
				Title = "the groom";
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

		public BrideGroom( Serial serial ) : base( serial )
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
