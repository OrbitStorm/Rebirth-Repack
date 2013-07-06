using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Gumps;

namespace Server.Mobiles
{
	public class WanderingHealer : BaseCreature
	{
		[Constructable]
		public WanderingHealer() : base( AIType.AI_Mage, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Female = Utility.RandomBool();
			Body = Female ? 401 : 400;
			Title = "the wandering healer";
			Name = NameList.RandomName( Female ? "female" : "male" );
			Hue = Utility.RandomSkinHue();
			SetStr( 71, 85 );
			SetDex( 81, 95 );
			SetInt( 86, 100 );
			Karma = -127;
			
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 65, 87.5 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.Healing, 55, 77.5 );
			SetSkill( SkillName.Anatomy, 55, 77.5 );
			SetSkill( SkillName.SpiritSpeak, 55, 77.5 );
			SetSkill( SkillName.Forensics, 35, 57.5 );
			SetSkill( SkillName.Camping, 35, 57.5 );
			SetSkill( SkillName.Fishing, 35, 57.5 );
			SetSkill( SkillName.Magery, 55, 75 );

			Item item = null;
			if ( !Female )
			{
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = AddRandomFacialHair( item.Hue );
				item = new Robe();
				item.Hue = Utility.RandomYellowHue();
				AddItem( item );
				item = new Sandals();
				AddItem( item );
				PackGold( 15, 100 );
			} else {
				item = AddRandomHair();
				item.Hue = Utility.RandomHairHue();
				item = new Robe();
				item.Hue = Utility.RandomYellowHue();
				AddItem( item );
				item = new Sandals();
				AddItem( item );
				PackGold( 15, 100 );
			}
		}
		
		public bool CheckResurrect( Mobile m )
		{
			if ( m.Karma <= (int)Noto.Dark )
			{
				Say( 501223 ); // Thou'rt not a decent and good person. I shall not resurrect thee.
				return false;
			}
			return true;
		}

		private DateTime m_NextResurrect;

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !m.Frozen && !m.Alive && DateTime.Now >= m_NextResurrect && InRange( m, 4 ) && !InRange( oldLocation, 4 ) && InLOS( m ) )
			{
				m_NextResurrect = DateTime.Now + Healer.ResurrectDelay;

				if ( m.Region is Regions.HouseRegion || m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
				{
					m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
				}
				else if ( CheckResurrect( m ) )
				{
					Direction = GetDirectionTo( m );
					Say( 501224 ); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.

					m.PlaySound( 0x214 );
					m.FixedEffect( 0x376A, 10, 16 );

					//m.CloseGump( typeof( ResurrectGump ) );
					m.SendMenu( new ResurrectMenu( m, ResurrectMessage.Healer ) );
				}
			}
		}

		public WanderingHealer( Serial serial ) : base( serial )
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

