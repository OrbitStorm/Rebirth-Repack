using System;
using Server;
using Server.Items;
using Server.Misc;
using System.Collections; using System.Collections.Generic;

namespace Server.Mobiles
{
	public class QuestNPC : BaseConvo
	{
		private ArrayList m_Strings = new ArrayList();

		[CommandProperty( AccessLevel.GameMaster )]
		public ArrayList Strings
		{
			get { return m_Strings; }
			set { m_Strings = value; }
		}

		[Constructable()]
		public QuestNPC() : base(AIType.AI_Melee,FightMode.Agressor,8,1,0.4,0.8)
		{
			Title = "";

			SetStr( 36, 50 );
			SetDex( 31, 45 );
			SetInt( 36, 50 );
			Karma = Utility.RandomMinMax( 10, -10 );

			SetSkill( SkillName.Tactics, 27.5, 50 );
			SetSkill( SkillName.MagicResist, 27.5, 50 );
			SetSkill( SkillName.Parry, 27.5, 50 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
			SetSkill( SkillName.ItemID, 55, 77.5 );
		}

		public QuestNPC( Serial serial ) : base( serial )
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

