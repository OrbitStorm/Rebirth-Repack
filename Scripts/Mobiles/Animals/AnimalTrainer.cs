using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class AnimalTrainer : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public AnimalTrainer() : base( "the animal trainer" )
		{
			Job = JobFragment.animal;
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 92.5 );
			SetSkill( SkillName.MagicResist, 92.5 );
			SetSkill( SkillName.Parry, 92.5 );
			SetSkill( SkillName.Swords, 92.5 );
			SetSkill( SkillName.Macing, 92.5 );
			SetSkill( SkillName.Fencing, 92.5 );
			SetSkill( SkillName.Wrestling, 92.5 );
			SetSkill( SkillName.Veterinary, 92.5 );
			SetSkill( SkillName.AnimalLore, 92.5 );
			SetSkill( SkillName.AnimalTaming, 92.5 );
		}

		protected override void GetConvoFragments(ArrayList list)
		{
			list.Add( JobFragment.horse );
			base.GetConvoFragments (list);
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBAnimalTrainer() );
		}

		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } }

		public override void InitBody()
		{
			SetStr( 9000 );
			SetDex( 9000 );
			SetInt( 9000 );
			Hue = Utility.RandomSkinHue();
			SpeechHue = Utility.RandomDyedHue();

			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Name = NameList.RandomName( Female ? "female" : "male" );
		}

		public override void InitOutfit()
		{
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
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new QuarterStaff();
				AddItem( item );
				PackGold( 15, 100 );
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
				item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
				AddItem( item );
				item = new QuarterStaff();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}

		private class StableEntry : ContextMenuEntry
		{
			private AnimalTrainer m_Trainer;
			private Mobile m_From;

			public StableEntry( AnimalTrainer trainer, Mobile from ) : base( 6126, 12 )
			{
				m_Trainer = trainer;
				m_From = from;
			}

			public override void OnClick()
			{
				m_Trainer.BeginStable( m_From );
			}
		}

		private class ClaimListGump : Gump
		{
			private AnimalTrainer m_Trainer;
			private Mobile m_From;
			private ArrayList m_List;

			public ClaimListGump( AnimalTrainer trainer, Mobile from, ArrayList list ) : base( 50, 50 )
			{
				m_Trainer = trainer;
				m_From = from;
				m_List = list;

				from.CloseGump( typeof( ClaimListGump ) );

				AddPage( 0 );

				AddBackground( 0, 0, 325, 50 + (list.Count * 20), 9250 );
				AddAlphaRegion( 5, 5, 315, 40 + (list.Count * 20) );

				AddHtml( 15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Select a pet to retrieve from the stables:</BASEFONT>", false, false );

				for ( int i = 0; i < list.Count; ++i )
				{
					BaseCreature pet = list[i] as BaseCreature;

					if ( pet == null || pet.Deleted )
						continue;

					AddButton( 15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0 );
					AddHtml( 32, 35 + (i * 20), 275, 18, String.Format( "<BASEFONT COLOR=#C0C0EE>{0}</BASEFONT>", pet.Name ), false, false );
				}
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				int index = info.ButtonID - 1;

				if ( index >= 0 && index < m_List.Count )
					m_Trainer.EndClaimList( m_From, m_List[index] as BaseCreature );
			}
		}

		private class ClaimAllEntry : ContextMenuEntry
		{
			private AnimalTrainer m_Trainer;
			private Mobile m_From;

			public ClaimAllEntry( AnimalTrainer trainer, Mobile from ) : base( 6127, 12 )
			{
				m_Trainer = trainer;
				m_From = from;
			}

			public override void OnClick()
			{
				m_Trainer.Claim( m_From );
			}
		}

        public override void AddCustomContextEntries(Mobile from, List<ContextMenus.ContextMenuEntry> list)
		{
			if ( from.Alive )
			{
				list.Add( new StableEntry( this, from ) );

				if ( from.Stabled.Count > 0 )
					list.Add( new ClaimAllEntry( this, from ) );
			}

			base.AddCustomContextEntries( from, list );
		}

		public static int GetMaxStabled( Mobile from )
		{
			/*double taming = from.Skills[SkillName.AnimalTaming].Value;
			double anlore = from.Skills[SkillName.AnimalLore].Value;
			double vetern = from.Skills[SkillName.Veterinary].Value;
			double sklsum = taming + anlore + vetern;

			int max;

			if ( sklsum >= 240.0 )
				max = 5;
			else if ( sklsum >= 200.0 )
				max = 4;
			else if ( sklsum >= 160.0 )
				max = 3;
			else
				max = 2;

			if ( taming >= 100.0 )
				max += (int)((taming - 90.0) / 10);

			if ( anlore >= 100.0 )
				max += (int)((anlore - 90.0) / 10);

			if ( vetern >= 100.0 )
				max += (int)((vetern - 90.0) / 10);

			return max;*/

			return 1 + (int)(from.Skills[SkillName.AnimalTaming].Value/33);
		}

		private class StableTarget : Target
		{
			private AnimalTrainer m_Trainer;

			public StableTarget( AnimalTrainer trainer ) : base( 12, false, TargetFlags.None )
			{
				m_Trainer = trainer;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is BaseCreature && !( targeted is HauntedHorse ) )
					m_Trainer.EndStable( from, (BaseCreature)targeted );
				else if ( targeted is PlayerMobile )
					m_Trainer.SayTo( from, 502672 ); // HA HA HA! Sorry, I am not an inn.
				else
					m_Trainer.SayTo( from, 1048053 ); // You can't stable that!
			}
		}

		public void BeginClaimList( Mobile from )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			ArrayList list = new ArrayList();

			for ( int i = 0; i < from.Stabled.Count; ++i )
			{
				BaseCreature pet = from.Stabled[i] as BaseCreature;

				if ( pet == null || pet.Deleted )
				{
					pet.IsStabled = false;
					from.Stabled.RemoveAt( i );
					--i;
					continue;
				}

				list.Add( pet );
			}

			if ( list.Count > 0 )
				from.SendGump( new ClaimListGump( this, from, list ) );
			else
				SayTo( from, 502671 ); // But I have no animals stabled with me at the moment!
		}

		public void EndClaimList( Mobile from, BaseCreature pet )
		{
			if ( pet == null || pet.Deleted || from.Map != this.Map || !from.InRange( this, 14 ) || !from.Stabled.Contains( pet ) || !from.CheckAlive() )
				return;

			pet.SetControlMaster( from );

			if ( pet.Summoned )
				pet.SummonMaster = from;

			if ( pet.AIObject != null )
				pet.AIObject.NumCommands = 0;
			pet.ControlTarget = from;
			pet.ControlOrder = OrderType.Follow;

			pet.Location = from.Location;
			pet.Map = from.Map;

			pet.IsStabled = false;
			from.Stabled.Remove( pet );

			SayTo( from, 1042559 ); // Here you go... and good day to you!
		}

		public void BeginStable( Mobile from )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			if ( GetNumStabled( from ) >= GetMaxStabled( from ) )
			{
				SayTo( from, 1042565 ); // You have too many pets in the stables!
			}
			else
			{
				SayTo( from, 1042558 ); /* I charge 30 gold per pet for a real week's stable time.
										 * I will withdraw it from thy bank account.
										 * Which animal wouldst thou like to stable here?
										 */

				from.Target = new StableTarget( this );
			}
		}

		public int GetStableSlotsFor( Mobile from, Mobile pet )
		{
			if ( pet is Dragon )
				return 4;
			else if ( pet is Drake )
				return 2;
			else
				return 1;
		}

		public int GetNumStabled( Mobile from )
		{
			int count = 0;

			for(int i=0;i<from.Stabled.Count;i++)
			{
				Mobile m = from.Stabled[i] as Mobile;

				if ( m != null && !m.Deleted )
					count += GetStableSlotsFor( from, m );
			}

			return count;
		}

		public void EndStable( Mobile from, BaseCreature pet )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			int maxStabled = GetMaxStabled( from );
			int newCount = GetNumStabled( from ) + GetStableSlotsFor( from, pet );

			if ( !pet.Controled || pet.ControlMaster != from )
			{
				SayTo( from, 1042562 ); // You do not own that pet!
			}
			else if ( pet.IsDeadPet )
			{
				SayTo( from, 1049668 ); // Living pets only, please.
			}
			else if ( pet.Summoned )
			{
				SayTo( from, 502673 ); // I can not stable summoned creatures.
			}
			else if ( pet.Body.IsHuman )
			{
				SayTo( from, 502672 ); // HA HA HA! Sorry, I am not an inn.
			}
			else if ( (pet is PackLlama || pet is PackHorse) && (pet.Backpack != null && pet.Backpack.Items.Count > 0) )
			{
				SayTo( from, 1042563 ); // You need to unload your pet.
			}
			else if ( pet.Combatant != null && pet.InRange( pet.Combatant, 12 ) && pet.Map == pet.Combatant.Map )
			{
				SayTo( from, 1042564 ); // I'm sorry.  Your pet seems to be busy.
			}
			else if ( newCount > maxStabled )
			{
				//SayTo( from, 1042565 ); // You have too many pets in the stables!
				SayTo( from, "There is not enough room in the stable for that!  Claim some of your other animals first." );
			}
			else
			{
				Container bank = from.BankBox;

				if ( bank != null && bank.ConsumeTotal( typeof( Gold ), 30 ) )
				{
					pet.ControlTarget = null;
					pet.ControlOrder = OrderType.Stay;
					pet.Internalize();

					pet.SetControlMaster( null );
					pet.SummonMaster = null;

					pet.IsStabled = true;
					from.Stabled.Add( pet );

					SayTo( from, 502679 ); // Very well, thy pet is stabled. Thou mayst recover it by saying 'claim' to me. In one real world week, I shall sell it off if it is not claimed!
				}
				else
				{
					SayTo( from, 502677 ); // But thou hast not the funds in thy bank account!
				}
			}
		}

		public void Claim( Mobile from )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			bool claimed = false;
			int stabled = 0;

			for ( int i = 0; i < from.Stabled.Count; ++i )
			{
				BaseCreature pet = from.Stabled[i] as BaseCreature;

				if ( pet == null || pet.Deleted )
				{
					pet.IsStabled = false;
					from.Stabled.RemoveAt( i );
					--i;
					continue;
				}

				++stabled;


				pet.SetControlMaster( from );

				if ( pet.Summoned )
					pet.SummonMaster = from;

				pet.ControlTarget = from;
				pet.ControlOrder = OrderType.Follow;

				pet.Location = from.Location;
				pet.Map = from.Map;

				pet.IsStabled = false;
				from.Stabled.RemoveAt( i );
				--i;

				claimed = true;
			}

			if ( claimed )
				SayTo( from, 1042559 ); // Here you go... and good day to you!
			else if ( stabled == 0 )
				SayTo( from, 502671 ); // But I have no animals stabled with me at the moment!
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			return true;
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && e.HasKeyword( 0x0008 ) )
			{
				e.Handled = true;
				BeginStable( e.Mobile );
			}
			else if ( !e.Handled && e.HasKeyword( 0x0009 ) )
			{
				e.Handled = true;

				if ( !Insensitive.Equals( e.Speech, "claim" ) )
					BeginClaimList( e.Mobile );
				else
					Claim( e.Mobile );
			}
			else
			{
				base.OnSpeech( e );
			}
		}

		public AnimalTrainer( Serial serial ) : base( serial )
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
