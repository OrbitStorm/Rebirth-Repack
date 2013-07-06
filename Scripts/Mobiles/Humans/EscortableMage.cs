using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class EscortableMage : BaseEscortable
	{
		[Constructable]
		public EscortableMage()
		{
			Title = "the mage";
			Job = JobFragment.mage;

			SetStr( 61, 75 );
			SetDex( 61, 75 );
			SetInt( 61, 75 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 55, 77.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 55, 77.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Magery, 65, 87.5 );
			SetSkill( SkillName.Inscribe, 50.1, 65 );

			PackGold( 15, 100 );
		}

		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } } // Do not display 'the mage' when single-clicking

		private static int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public override void InitOutfit()
		{
			AddItem( new Robe( GetRandomHue() ) );

			int lowHue = GetRandomHue();
			AddItem( new ShortPants( lowHue ) );

			if ( Female )
				AddItem( new ThighBoots( lowHue ) );
			else
				AddItem( new Boots( lowHue ) );

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new ShortHair( Utility.RandomHairHue() ) ); break;
				case 1: AddItem( new TwoPigTails( Utility.RandomHairHue() ) ); break;
				case 2: AddItem( new ReceedingHair( Utility.RandomHairHue() ) ); break;
				case 3: AddItem( new KrisnaHair( Utility.RandomHairHue() ) ); break;
			}
		}

		public EscortableMage( Serial serial ) : base( serial )
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