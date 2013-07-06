
using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class Santa : BaseCreature
	{
		[Constructable]
		public Santa() : base( AIType.AI_Mage, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = false;
			Body = 400;
			Title = "";
			Name = "Santa";
			Hue = 0x83ea;

			SetStr( 150, 250 );
			SetDex( 91, 115 );
			SetInt( 500, 1000 );
			Karma = +125;
			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 75, 97.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Magery, 95.1, 105.0 );
			SetSkill( SkillName.Wrestling, 20.2, 60 );

			VirtualArmor = 30;
			SetDamage( 3, 12 );

			Item item = null;

			item = new ShortHair();
			item.Hue = 0x47E;
			AddItem( item );

			item = new MediumShortBeard();
			item.Hue = 0x47E;
			AddItem( item );

			item = new LeatherGloves();
			item.Hue = 0x455;
			AddItem( item );

			AddItem( new FancyShirt( 0x26 ) );
			AddItem( new LongPants( 0x26 ) );
			AddItem( new Boots() );

			LootPack.Average.Generate( this );
		}

		private static string[] m_Strings = new string[]
			{
				"Ho ho ho!", 
				"Happy holidays!", 
				"A merry season to thee!", 
				"Enjoy the holidays!",
				"Ho ho ho! Happy holidays!", 
				"May thy holidays be joyful!", 
				"Enjoy the season!", 
				"I wish thee the joy of the season!", 
				"I wish thee joy! Ho ho ho!", 
				"Naughty or nice? Hmm.", 
				"Where IS Rudolph? He's never this late.", 
				"Dancer, Prancer, don't wander off.", 
				"If I only had a sleigh...", 
				"I think I'm going to cause a worldwide shortage of coal this year.", 
				"British? Coal or a fruitcake? Hmm.", 
				"Hmm, coal, or a fruitcake for Blackthorn?", 
				"Hmm, I seem to have lost some weight."
			};

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			int oldDist = (int)GetDistanceToSqrt( oldLocation );
			int newDist = (int)GetDistanceToSqrt( m.Location );

			if ( newDist == 5 && oldDist > 5 && CanSee( m ) && m.Player )
			{
				SpeechHue = Utility.RandomList( 0x3B2, 0x3B2, 0x25, 0x26, 0x24, 0x45, 0x44, 0x43, 0x481 );
				Say( true, m_Strings[Utility.Random( m_Strings.Length )] );
				Animate( 33, 5, 1, true, false, 0 );
			}
		}

		public Santa( Serial serial ) : base( serial )
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
