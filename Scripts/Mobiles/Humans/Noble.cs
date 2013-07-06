using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class Noble : BaseEscortable
	{
		[Constructable]
		public Noble()
		{
			Title = "the noble";
			Job = JobFragment.noble;

			SetStr( 31, 45 );
			SetDex( 41, 55 );
			SetInt( 51, 65 );
			Karma = Utility.RandomMinMax( 0, -60 );

			
			SetSkill( SkillName.Tactics, 25, 47.5 );
			SetSkill( SkillName.MagicResist, 25, 47.5 );
			SetSkill( SkillName.Parry, 25, 47.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );

			PackGold( 15, 50 );
		}

		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } } // Do not display 'the noble' when single-clicking

		private static int GetRandomHue()
		{
			switch ( Utility.Random( 6 ) )
			{
				default:
				case 0: return 0;
				case 1: return Utility.RandomBlueHue();
				case 2: return Utility.RandomGreenHue();
				case 3: return Utility.RandomRedHue();
				case 4: return Utility.RandomYellowHue();
				case 5: return Utility.RandomNeutralHue();
			}
		}

		public override void InitOutfit()
		{
			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new FancyShirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new LongPants();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Cloak();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new BodySash();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new Longsword();
				AddItem( item );
				PackGold( 100, 150 );
			} 
			else 
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new FancyDress();
				item.Hue = Utility.RandomNeutralHue();
				AddItem( item );
				item = new Cloak();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new BodySash();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new Longsword();
				AddItem( item );
				PackGold( 100, 150 );
			}
		}

		public Noble( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}