using System;
using System.Text;
using Server;
using Server.Items;
using Server.Misc;
using Server.Targeting;
using Server.Multis;

namespace Server.Mobiles
{
	public class HarborMaster : BaseGuildmaster
	{
		public override bool HandlesOnSpeech(Mobile from)
		{
			return InRange( from.Location, 3 );
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if ( !e.Handled )
			{
				if ( e.Speech.IndexOf( "dock" ) != -1 )
				{
					SayTo( e.Mobile, true, "My charges is 25 gold for docking thy ship.  Which ship do you wish to dock?" );
					e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( OnTarget ) );
					e.Handled = true;
				}
			}
			base.OnSpeech (e);
		}

		private void OnTarget( Mobile from, object targeted )
		{
			BaseBoat ship = null;
			if ( targeted is BaseBoat )
				ship = (BaseBoat)targeted;
			else if ( targeted is TillerMan )
				ship = ((TillerMan)targeted).Boat;
			else if ( targeted is Plank )
				ship = ((Plank)targeted).Boat;
			else if ( targeted is Hold )
				ship = ((Hold)targeted).Boat;

			if ( ship == null )
			{
				SayTo( from, true, "That is not a ship!" );
				return;
			}
			else if ( !InRange( ship.Location, 50 ) )
			{
				SayTo( from, true, "That is too far away!" );
				return;
			}

			BaseBoat.DryDockResult res = ship.CheckDryDock( from );
			switch ( res )
			{
				case BaseBoat.DryDockResult.Dead:
					SayTo( from, true, "Thou art dead and cannot do that." );
					break;
				case BaseBoat.DryDockResult.Decaying:
					SayTo( from, true, "I will not dock a boat that is sinking!" );
					break;
				case BaseBoat.DryDockResult.Hold:
					SayTo( from, true, "You must clear the ship's hold before you can dock it." );
					break;
				case BaseBoat.DryDockResult.Items:
					SayTo( from, true, "You must clear the ship's deck of items before docking it." );
					break;
				case BaseBoat.DryDockResult.Mobiles:
					SayTo( from, true, "You cannot dock a ship with beings on board!" );
					break;
				case BaseBoat.DryDockResult.NoKey:
					SayTo( from, true, "That ship does not belong to you." );
					break;
				case BaseBoat.DryDockResult.NotAnchored:
					SayTo( from, true, "The ship is not anchored." );
					break;
				case BaseBoat.DryDockResult.Valid:
				{
					if ( !from.BankBox.ConsumeTotal( typeof( Gold ), 25 ) )
					{
						SayTo( from, true, "You do not have 25 gold in your bank account to pay for the docking of this ship." );
						break;
					}

                    BaseDockedBoat claim = ship.DockedBoat;
					if ( claim == null )
						break;
					
					StringBuilder sb = new StringBuilder( "a ship claim ticket" );
					if ( this.Region.Name != "" )
						sb.AppendFormat( " from {0}", this.Region.Name );
					if ( claim.ShipName != null && claim.ShipName != "" )
						sb.AppendFormat( " for the {0}", claim.ShipName );
					claim.Name = sb.ToString();
					claim.DockLocation = this.Home != Point3D.Zero ? this.Home : ship.Location;

					ship.RemoveKeys( from );
					ship.Delete();

					from.AddToBackpack( claim );

					SayTo( from, true, "Here is your claim ticket.  I suggest you store it in a safe place." );
					break;
				}
			}
		}

		public override NpcGuild NpcGuild
		{
			get
			{
				return NpcGuild.FishermensGuild;
			}
		}


		[Constructable]
		public HarborMaster() : base( "" )
		{
			Job = JobFragment.shipwright;
			Karma = Utility.RandomMinMax( 13, -45 );
			
			SetSkill( SkillName.Tactics, 45, 67.5 );
			SetSkill( SkillName.MagicResist, 55, 77.5 );
			SetSkill( SkillName.Parry, 55, 77.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );

			Title = "the harbor master";
		}

		public override void InitBody()
		{
			SetStr( 86, 100 );
			SetDex( 66, 80 );
			SetInt( 71, 85 );
			Hue = Utility.RandomSkinHue();
			SpeechHue = Utility.RandomDyedHue();

			Body = 400;
			Name = NameList.RandomName( "male" );
		}

		public override void InitOutfit()
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
			item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
			AddItem( item );
			item = new QuarterStaff();
			AddItem( item );
			PackGold( 15, 100 );
			item = AddRandomHair();
			item.Hue = Utility.RandomHairHue();
			item = new Shirt();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = new ShortPants();
			item.Hue = Utility.RandomNondyedHue();
			AddItem( item );
			item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
			AddItem( item );
			item = new QuarterStaff();
			AddItem( item );
			PackGold( 15, 50 );
		}

		public HarborMaster( Serial serial ) : base( serial )
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

