using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;

namespace Server.Misc
{
	public class CharacterCreation
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.CharacterCreated += new CharacterCreatedEventHandler( EventSink_CharacterCreated );
		}

		public static void AddBackpack( Mobile m )
		{
			Container pack = m.Backpack;

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Movable = false;

				m.AddItem( pack );
			}

			PackItem( new Dagger() );
		}

		private static Item MakeNewbie( Item item )
		{
			if ( !Core.AOS )
				item.LootType = LootType.Newbied;

			return item;
		}

		private static void PlaceItemIn( Container parent, int x, int y, Item item )
		{
			parent.AddItem( item );
			item.Location = new Point3D( x, y, 0 );
		}

		private static Item MakePotionKeg( PotionEffect type, int hue )
		{
			PotionKeg keg = new PotionKeg();

			keg.Held = 100;
			keg.Type = type;
			keg.Hue = hue;

			return MakeNewbie( keg );
		}

		private static void FillBankAOS( Mobile m )
		{

		}

		private static void FillBankbox( Mobile m )
		{
		}

		public static void AddShirt( Mobile m, int shirtHue )
		{
			int hue = Utility.ClipDyedHue( shirtHue & 0x3FFF );

			switch ( Utility.Random( 3 ) )
			{
				case 0: EquipItem( new Shirt( hue ), true ); break;
				case 1: EquipItem( new FancyShirt( hue ), true ); break;
				case 2: EquipItem( new Doublet( hue ), true ); break;
			}
		} 

		public static void AddPants( Mobile m, int pantsHue )
		{
			int hue = Utility.ClipDyedHue( pantsHue & 0x3FFF );

			if ( m.Female )
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: EquipItem( new Skirt( hue ), true ); break;
					case 1: EquipItem( new Kilt( hue ), true ); break;
				}
			}
			else
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: EquipItem( new LongPants( hue ), true ); break;
					case 1: EquipItem( new ShortPants( hue ), true ); break;
				}
			}
		} 

		public static void AddShoes( Mobile m )
		{
			EquipItem( new Shoes( Utility.RandomYellowHue() ), true );
		} 

		public static void AddHair( Mobile m, int itemID, int hue )
		{
			Item item;

			switch ( itemID & 0x3FFF )
			{
				case 0x2044: item = new Mohawk( hue ); break;
				case 0x2045: item = new PageboyHair( hue ); break;
				case 0x2046: item = new BunsHair( hue ); break;
				case 0x2047: item = new Afro( hue ); break;
				case 0x2048: item = new ReceedingHair( hue ); break;
				case 0x2049: item = new TwoPigTails( hue ); break;
				case 0x204A: item = new KrisnaHair( hue ); break;
				case 0x203B: item = new ShortHair( hue ); break;
				case 0x203C: item = new LongHair( hue ); break;
				case 0x203D: item = new PonyTail( hue ); break;
				default: return;
			}

			m.AddItem( item );
		}

		public static void AddBeard( Mobile m, int itemID, int hue )
		{
			if ( m.Female )
				return;

			Item item;

			switch ( itemID & 0x3FFF )
			{
				case 0x203E: item = new LongBeard( hue ); break;
				case 0x203F: item = new ShortBeard( hue ); break;
				case 0x2040: item = new Goatee( hue ); break;
				case 0x2041: item = new Mustache( hue ); break;
				case 0x204B: item = new MediumShortBeard( hue ); break;
				case 0x204C: item = new MediumLongBeard( hue ); break;
				case 0x204D: item = new Vandyke( hue ); break;
				default: return;
			}

			m.AddItem( item );
		}

		public static Mobile CreateMobile( Account a )
		{
			for ( int i = 0; i < 5; ++i )
				if ( a[i] == null )
					return (a[i] = new PlayerMobile());

			return null;
		}

		private static void EventSink_CharacterCreated( CharacterCreatedEventArgs args )
		{
			Mobile newChar = CreateMobile( args.Account as Account );

			if ( newChar == null )
			{
				Console.WriteLine( "Login: {0}: Character creation failed, account full", args.State );
				return;
			}

			args.Mobile = newChar;
			m_Mobile = newChar;
			newChar.StatCap = 225;
			newChar.Player = true;
			newChar.AccessLevel = ((Account)args.Account).AccessLevel;
			if ( newChar.AccessLevel != AccessLevel.Player )
				newChar.Hidden = true;
			newChar.Female = args.Female;
			newChar.Body = newChar.Female ? 0x191 : 0x190;
			newChar.Hue = Utility.ClipSkinHue( args.Hue & 0x3FFF ) | 0x8000;
			newChar.Hunger = 20;
			newChar.FollowersMax = 0;


			SetName( newChar, args.Name );

			AddBackpack( newChar );

			PackItem( new Gold( 100 ) );

			SetStats( newChar, args.Str, args.Dex, args.Int );
			SetSkills( newChar, args.Skills, args.Profession );

			AddHair( newChar, args.HairID, Utility.ClipHairHue( args.HairHue & 0x3FFF ) );
			AddBeard( newChar, args.BeardID, Utility.ClipHairHue( args.BeardHue & 0x3FFF ) );

			if ( args.Profession != 4 && args.Profession != 5 )
			{
				AddShirt( newChar, args.ShirtHue );
				AddPants( newChar, args.PantsHue );
				AddShoes( newChar );
			}

			//CityInfo city = new CityInfo( "Jail", "Cell 9", 5273, 1185,	0 );
			//CityInfo city = new CityInfo( "Britain", "Sweet Dreams Inn", 1496, 1628, 10 );
			//newChar.Location = city.Location;

			newChar.MoveToWorld( args.City.Location, Map.Felucca );

			//Console.WriteLine( "Login: {0}: New character being created (account={1})", args.State, ((Account)args.Account).Username );
			//Console.WriteLine( " - Character: {0} (serial={1})", newChar.Name, newChar.Serial );
			//Console.WriteLine( " - Started: {0} {1}", city.City, city.Location );
			Console.WriteLine( "Login: {0}: ({3}) New Char '{1}' (serial={2})", args.State, newChar.Name, newChar.Serial, args.Account );

			/*
			Meditation = 46,
			Stealth = 47,
			RemoveTrap = 48,
			Necromancy = 49,
			Focus = 50,
			Chivalry = 51
			*/
            for (int i = 0; i < newChar.Skills.Length; ++i)
            {
                if (i < (int)SkillName.Meditation) {
                    newChar.Skills[i].Cap = 100;
                }
                else {
                    newChar.Skills[i].Base = 0;
                    newChar.Skills[i].Cap = 0;
                }
                newChar.Skills[i].SetLockNoRelay(SkillLock.Locked);
            }
            
			newChar.IntLock = StatLockType.Locked;
			newChar.StrLock = StatLockType.Locked;
			newChar.DexLock = StatLockType.Locked;

			new WelcomeTimer( newChar ).Start();
		}

		private static void FixStats( ref int str, ref int dex, ref int intel )
		{
			int vStr = str - 10;
			int vDex = dex - 10;
			int vInt = intel - 10;

			if ( vStr < 0 )
				vStr = 0;

			if ( vDex < 0 )
				vDex = 0;

			if ( vInt < 0 )
				vInt = 0;

			int total = vStr + vDex + vInt;
			if ( total == 0 || total == 50 )
				return;

			double scalar = 50 / (double)total;

			vStr = (int)(vStr * scalar);
			vDex = (int)(vDex * scalar);
			vInt = (int)(vInt * scalar);

			FixStat( ref vStr, (vStr + vDex + vInt) - 50 );
			FixStat( ref vDex, (vStr + vDex + vInt) - 50 );
			FixStat( ref vInt, (vStr + vDex + vInt) - 50 );

			str = vStr + 10;
			dex = vDex + 10;
			intel = vInt + 10;
		}

		private static void FixStat( ref int stat, int diff )
		{
			stat += diff;

			if ( stat < 0 )
				stat = 0;
			else if ( stat > 50 )
				stat = 50;
		}

		private static void SetStats( Mobile m, int str, int dex, int intel )
		{
			FixStats( ref str, ref dex, ref intel );

			if ( str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != 80 )
			{
				str = 10;
				dex = 10;
				intel = 10;
			}

			// scale them down 25% for pret2a (where the max you could have was 45, total of 65)
			// if 2 of the stats you picked were 13, its possible to have 66 instead of 65 stats, but who cares?
			if ( str > 13 )
				str = (int)(str * 0.75);
			if ( dex > 13 )
				dex = (int)(dex * 0.75);
			if ( intel > 13 )
				intel = (int)(intel * 0.75);

			m.InitStats( str, dex, intel );
		}

		private static void SetName( Mobile m, string name )
		{
			name = name.Trim();

			if ( !NameVerification.Validate( name, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote ) )
				name = "Generic Player";

			m.Name = name;
		}

		private static bool ValidSkills( SkillNameValue[] skills )
		{
			int total = 0;

			for ( int i = 0; i < skills.Length; ++i )
			{
				if ( skills[i].Value < 0 || skills[i].Value > 50 )
					return false;

				total += skills[i].Value;

				for ( int j = i + 1; j < skills.Length; ++j )
				{
					if ( skills[j].Value > 0 && skills[j].Name == skills[i].Name )
						return false;
				}
			}

			return ( total == 100 );
		}

		public static Mobile m_Mobile;

		private static void SetSkills( Mobile m, SkillNameValue[] skills, int prof )
		{
			switch ( prof )
			{
				case 1: // Warrior
				{
					skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Healing, 25 ),
							new SkillNameValue( SkillName.Swords, 25 ),
							new SkillNameValue( SkillName.Tactics, 50 )
						};

					break;
				}
				case 2: // Magician
				{
					skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Wrestling, 25 ),
							new SkillNameValue( SkillName.Magery, 50 ),
							//new SkillNameValue( SkillName.Meditation, 25 )
						};

					break;
				}
				case 3: // Blacksmith
				{
					skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Mining, 25 ),
							new SkillNameValue( SkillName.ArmsLore, 25 ),
							new SkillNameValue( SkillName.Blacksmith, 50 )
						};

					break;
				}
				case 4: // Necromancer
				{
					if ( !Core.AOS )
						goto default;

					skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Necromancy, 50 ),
							new SkillNameValue( SkillName.Focus, 30 ),
							new SkillNameValue( SkillName.SpiritSpeak, 30 ),
							new SkillNameValue( SkillName.Swords, 30 ),
							new SkillNameValue( SkillName.Tactics, 20 )
						};

					break;
				}
				case 5: // Paladin
				{
					if ( !Core.AOS )
						goto default;

					skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Chivalry, 51 ),
							new SkillNameValue( SkillName.Swords, 49 ),
							new SkillNameValue( SkillName.Focus, 30 ),
							new SkillNameValue( SkillName.Tactics, 30 )
						};

					break;
				}
				default:
				{
					if ( !ValidSkills( skills ) )
						return;

					break;
				}
			}

			bool addSkillItems = true;

			switch ( prof )
			{
				case 1: // Warrior
				{
					EquipItem( new LeatherChest() );
					EquipItem( new LeatherLegs() );
					break;
				}
				case 4: // Necromancer
				{
					addSkillItems = false;
					break;
				}
				case 5: // Paladin
				{
					break;
				}
			}

			for ( int i = 0; i < skills.Length; ++i )
			{
				SkillNameValue snv = skills[i];

				if ( snv.Value > 0 && snv.Name != SkillName.Stealth && snv.Name != SkillName.RemoveTrap )
				{
					Skill skill = m.Skills[snv.Name];

					if ( skill != null )
					{
						skill.BaseFixedPoint = snv.Value * 10;

						if ( addSkillItems )
							AddSkillItems( snv.Name );
					}
				}
			}
		}

		private static void EquipItem( Item item )
		{
			EquipItem( item, false );
		}

		private static void EquipItem( Item item, bool mustEquip )
		{
			if ( !Core.AOS )
				item.LootType = LootType.Newbied;

			if ( m_Mobile != null && m_Mobile.EquipItem( item ) )
				return;

			Container pack = m_Mobile.Backpack;

			if ( !mustEquip && pack != null )
				pack.DropItem( item );
			else
				item.Delete();
		}

		private static void PackItem( Item item )
		{
			if ( !Core.AOS )
				item.LootType = LootType.Newbied;

			Container pack = m_Mobile.Backpack;

			if ( pack != null )
				pack.DropItem( item );
			else
				item.Delete();
		}

		private static void PackInstrument()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0: PackItem( new Drums() ); break;
				case 1: PackItem( new Harp() ); break;
				case 2: PackItem( new LapHarp() ); break;
				case 3: PackItem( new Lute() ); break;
				case 4: PackItem( new Tambourine() ); break;
				case 5: PackItem( new TambourineTassel() ); break;
			}
		}

		private static void PackScroll( int circle )
		{
			switch ( Utility.Random( 8 ) + (circle * 8) )
			{
				case  0: PackItem( new ClumsyScroll() ); break;
				case  1: PackItem( new CreateFoodScroll() ); break;
				case  2: PackItem( new FeeblemindScroll() ); break;
				case  3: PackItem( new HealScroll() ); break;
				case  4: PackItem( new MagicArrowScroll() ); break;
				case  5: PackItem( new NightSightScroll() ); break;
				case  6: PackItem( new ReactiveArmorScroll() ); break;
				case  7: PackItem( new WeakenScroll() ); break;
				case  8: PackItem( new AgilityScroll() ); break;
				case  9: PackItem( new CunningScroll() ); break;
				case 10: PackItem( new CureScroll() ); break;
				case 11: PackItem( new HarmScroll() ); break;
				case 12: PackItem( new MagicTrapScroll() ); break;
				case 13: PackItem( new MagicUnTrapScroll() ); break;
				case 14: PackItem( new ProtectionScroll() ); break;
				case 15: PackItem( new StrengthScroll() ); break;
				case 16: PackItem( new BlessScroll() ); break;
				case 17: PackItem( new FireballScroll() ); break;
				case 18: PackItem( new MagicLockScroll() ); break;
				case 19: PackItem( new PoisonScroll() ); break;
				case 20: PackItem( new TelekinisisScroll() ); break;
				case 21: PackItem( new TeleportScroll() ); break;
				case 22: PackItem( new UnlockScroll() ); break;
				case 23: PackItem( new WallOfStoneScroll() ); break;
			}
		}

		private static Item NecroHue( Item item )
		{
			item.Hue = 0x2C3;

			return item;
		}

		private static void AddSkillItems( SkillName skill )
		{
			switch ( skill )
			{
				case SkillName.Alchemy:
				{
					PackItem( new Bottle( 5 ) );
					PackItem( new MortarPestle() );
					EquipItem( new Robe( Utility.RandomRedHue() ) );

					PackItem( new SpidersSilk( Utility.Random( 3 ) + 3 ) );
					PackItem( new BlackPearl( Utility.Random( 3 ) + 3 ) );
					PackItem( new Ginseng( Utility.Random( 3 ) + 3 ) );
					PackItem( new Garlic( Utility.Random( 3 ) + 3 ) );
					break;
				}
				case SkillName.Anatomy:
				{
					EquipItem( new Robe( Utility.RandomYellowHue() ) );
					PackItem( new Bandage( 3 ) );
					break;
				}
				case SkillName.AnimalLore:
				{
					EquipItem( new ShepherdsCrook() );
					EquipItem( new Robe( Utility.RandomGreenHue() ) );
					break;
				}
				case SkillName.Archery:
				{
					PackItem( new Arrow( 25 ) );
					EquipItem( new PracticeBow() );
					break;
				}
				case SkillName.ItemID:
				{
					EquipItem( new GnarledStaff() );
					break;
				}
				case SkillName.ArmsLore:
				{
					break;
				}
				case SkillName.Begging:
				{
					EquipItem( new GnarledStaff() );
					break;
				}
				case SkillName.Blacksmith:
				{
					PackItem( new Tongs() );
					PackItem( new IronIngot( 10 ) );
					EquipItem( new HalfApron( Utility.RandomYellowHue() ) );
					break;
				}
				case SkillName.Fletching:
				{
					PackItem( new Board( 5 ) );
					PackItem( new Feather( 10 ) );
					PackItem( new Shaft( 5 ) );
					break;
				}
				case SkillName.Camping:
				{
					PackItem( new Kindling( 5 ) );
					break;
				}
				case SkillName.Carpentry:
				{
					PackItem( new Board( 10 ) );
					PackItem( new Saw() );
					EquipItem( new HalfApron( Utility.RandomYellowHue() ) );
					break;
				}
				case SkillName.Cartography:
				{
					PackItem( new BlankMap() );
					PackItem( new BlankMap() );
					PackItem( new Sextant() );
					break;
				}
				case SkillName.Cooking:
				{
					PackItem( new Kindling( 2 ) );
					PackItem( new RawChickenLeg() );
					PackItem( new RawFishSteak( 3 ) );
					break;
				}
				case SkillName.DetectHidden:
				{
					EquipItem( new Cloak( 2411 ) );
					break;
				}
				case SkillName.Discordance:
				{
					PackInstrument();
					break;
				}
				case SkillName.Fencing:
				{
					EquipItem( new PracticeKryss() );
					break;
				}
				case SkillName.Fishing:
				{
					EquipItem( new FishingPole() );
					EquipItem( new FloppyHat( Utility.RandomYellowHue() ) );
					break;
				}
				case SkillName.Healing:
				{
					PackItem( new Bandage( 10 ) );
					PackItem( new Scissors() );
					break;
				}
				case SkillName.Herding:
				{
					EquipItem( new ShepherdsCrook() );
					break;
				}
				case SkillName.Hiding:
				{
					EquipItem( new Cloak( 2411 ) );
					break;
				}
				case SkillName.Inscribe:
				{
					PackItem( new BlankScroll( 4 ) );
					PackItem( new BlueBook() );
					break;
				}
				case SkillName.Lockpicking:
				{
					PackItem( new Lockpick( 5 ) );
					break;
				}
				case SkillName.Lumberjacking:
				{
					EquipItem( new PracticeHatchet() );
					break;
				}
				case SkillName.Macing:
				{
					EquipItem( new PracticeClub() );
					break;
				}
				case SkillName.Magery:
				{
					PackItem( new Bloodmoss( Utility.Random( 3 ) + 3 ) );
					PackItem( new MandrakeRoot( Utility.Random( 3 ) + 3 ) );
					PackItem( new SpidersSilk( Utility.Random( 3 ) + 3 ) );
					PackItem( new SulfurousAsh( Utility.Random( 3 ) + 3 ) );
					PackItem( new BlackPearl( Utility.Random( 3 ) + 3 ) );
					PackItem( new Ginseng( Utility.Random( 3 ) + 3 ) );
					PackItem( new Garlic( Utility.Random( 3 ) + 3 ) );
					PackItem( new Nightshade( Utility.Random( 3 ) + 3 ) );

					for(int i=0;i<4;i++)
					{
						PackScroll( 0 );
						PackScroll( 1 );
					}

					Spellbook book = new Spellbook();

					PackItem( book );
					book.LootType = LootType.Blessed;

					EquipItem( new WizardsHat( Utility.RandomBlueHue() ) );
					break;
				}
				case SkillName.Mining:
				{
					if ( Utility.RandomBool() )
						PackItem( new Pickaxe() );
					else
						PackItem( new Shovel() );
					break;
				}
				case SkillName.Musicianship:
				{
					PackInstrument();
					break;
				}
				case SkillName.Parry:
				{
					EquipItem( new WoodenShield() );
					break;
				}
				case SkillName.Peacemaking:
				{
					PackInstrument();
					break;
				}
				case SkillName.Poisoning:
				{
					PackItem( new LesserPoisonPotion() );
					PackItem( new LesserPoisonPotion() );
					break;
				}
				case SkillName.Provocation:
				{
					PackInstrument();
					break;
				}
				case SkillName.Snooping:
				{
					PackItem( new Lockpick( 5 ) );
					break;
				}
				case SkillName.SpiritSpeak:
				{
					EquipItem( new Cloak( 2411 ) );
					break;
				}
				case SkillName.Stealing:
				{
					EquipItem( new Cloak( 2411 ) );
					break;
				}
				case SkillName.Swords:
				{
					EquipItem( new PracticeSword() );
					break;
				}
				case SkillName.Tactics:
				{
					break;
				}
				case SkillName.Tinkering:
				{
					PackItem( new TinkerTools() );
					PackItem( new IronIngot( 4 ) );
					EquipItem( new FullApron( Utility.RandomYellowHue() ) );
					break;
				}
				case SkillName.Tailoring:
				{
					PackItem( new BoltOfCloth() );
					PackItem( new SewingKit() );
					PackItem( new SewingKit() );
					break;
				}
				case SkillName.Tracking:
				{
					if ( m_Mobile != null )
					{
						Item shoes = m_Mobile.FindItemOnLayer( Layer.Shoes );

						if ( shoes != null )
							shoes.Delete();
					}

					EquipItem( new Boots( Utility.RandomYellowHue() ) );
					EquipItem( new SkinningKnife() );
					break;
				}
				case SkillName.Veterinary:
				{
					PackItem( new Bandage( 10 ) );
					PackItem( new Scissors() );
					break;
				}
				case SkillName.Wrestling:
				{
					EquipItem( new LeatherGloves() );
					break;
				}
			}
		}
	}
}

