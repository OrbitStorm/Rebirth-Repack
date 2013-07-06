using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class SeekerOfAdventure : BaseEscortable
	{
		private static string[] m_Dungeons = new string[]{ "a dungeon" }; // { "Covetous", "Deceit", "Despise", "Destard", "Hythloth", "Shame", "Wrong" };

		public override string[] GetPossibleDestinations()
		{
			return m_Dungeons;
		}

		public override bool IsAtDest()
		{
			return this.Region is Regions.DungeonRegion;
		}

		[Constructable]
		public SeekerOfAdventure()
		{
			Title = "the seeker of adventure";

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
		}

		public override void InitOutfit()
		{
			if ( Female )
			{
				Item item = null;
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
			}
			else
			{
				Item item = null;
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
			}
		}

		public SeekerOfAdventure( Serial serial ) : base( serial )
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
