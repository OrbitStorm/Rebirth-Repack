using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Guilds;

namespace Server.Mobiles
{
	public abstract class BaseShieldGuard : BaseConvo
	{
		public BaseShieldGuard() : base( AIType.AI_Melee, FightMode.Agressor, 14, 1, 0.8, 1.6 )
		{
			Job = JobFragment.guard;
			
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 9000 );
			SetDex( 9000 );
			SetInt( 9000 );
			Karma = 100;

			
			SetSkill( SkillName.Tactics, 90.1, 100 );
			SetSkill( SkillName.MagicResist, 90.1, 100 );
			SetSkill( SkillName.Parry, 90.1, 100 );
			SetSkill( SkillName.Swords, 90.1, 100 );
			SetSkill( SkillName.Macing, 90.1, 100 );
			SetSkill( SkillName.Fencing, 90.1, 100 );
			SetSkill( SkillName.Wrestling, 90.1, 100 );
			SetSkill( SkillName.DetectHidden, 90.1, 100 );
			SetSkill( SkillName.Forensics, 90.1, 100 );


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
				item = new PlateChest();
				AddItem( item );
				item = new PlateLegs();
				AddItem( item );
				item = new PlateArms();
				AddItem( item );
				item = new Tunic();
				item.Hue = Utility.RandomRedHue();
				AddItem( item );
			} 
			else 
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Shirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new Skirt();
				item.Hue = Utility.RandomNondyedHue();
				AddItem( item );
				item = new PlateChest();
				AddItem( item );
				item = new PlateLegs();
				AddItem( item );
				item = new PlateArms();
				AddItem( item );
				item = new Tunic();
				item.Hue = Utility.RandomRedHue();
				AddItem( item );
			}

			VikingSword weapon = new VikingSword();
			weapon.Movable = false;
			AddItem( weapon );

			BaseShield shield = Shield;
			shield.Movable = false;
			AddItem( shield );
			PackGold( 15, 100 );
		}

		public abstract int Keyword{ get; }
		public abstract BaseShield Shield{ get; }
		public virtual int SignupNumber{ get { return 0; } }
		public virtual string SignupString { get { return ""; } }
		public abstract GuildType Type{ get; }

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 2 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && e.Mobile.InRange( this.Location, 2 ) )
			{
				if ( e.HasKeyword( Keyword )  )
				{
					e.Handled = true;
	
					Mobile from = e.Mobile;
					Guild g = from.Guild as Server.Guilds.Guild;
		
					if ( from.Karma < (int)Noto.Great )
					{
						Say( "Thou art not famous enough to join our ranks." );
						return;
					}
					else if ( g != null && g.Type != GuildType.Regular && g.Type != Type )
					{
						Say( "Thou art not properly affiliated to join our ranks." );
						return;
					}
					else
					{
						Container pack = from.Backpack;
						BaseShield shield = Shield;
						Item twoHanded = from.FindItemOnLayer( Layer.TwoHanded );
	
						if ( (pack != null && pack.FindItemByType( shield.GetType() ) != null) || ( twoHanded != null && shield.GetType().IsAssignableFrom( twoHanded.GetType() ) ) )
						{
							Say( "Why dost thou ask about virtue guards when thou art one?" );
							shield.Delete();
						}
						else if ( from.PlaceInBackpack( shield ) )
						{
							Say( Utility.Random( 1007101, 5 ) );
							Say( "Welcome to our ranks.  Here is thy shield." );
							from.AddToBackpack( shield );
						}
						else
						{
							from.SendLocalizedMessage( 502868 ); // Your backpack is too full.
							shield.Delete();
						}
					}
				}
				else
				{
					if ( Utility.RandomBool() )
					{
						if ( SignupNumber > 0 )
							Say( SignupNumber );
						else if ( SignupString != "" )
							Say( SignupString );
						e.Handled = true;
					}
				}
			}

			base.OnSpeech( e );
		}

		public BaseShieldGuard( Serial serial ) : base( serial )
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
