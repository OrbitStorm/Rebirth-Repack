using System;
using System.Collections; using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class Minter : Banker
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.MerchantsGuild; } }

		[Constructable]
		public Minter()
		{
			Job = JobFragment.minter;
			Title = "the minter";

			SetStr( 71, 85 );
			SetDex( 66, 80 );
			SetInt( 66, 80 );
			Karma = Utility.RandomMinMax( 13, -45 );

			
			SetSkill( SkillName.Tactics, 45, 67.5 );
			SetSkill( SkillName.MagicResist, 45, 67.5 );
			SetSkill( SkillName.Parry, 45, 67.5 );
			SetSkill( SkillName.Swords, 15, 37.5 );
			SetSkill( SkillName.Macing, 15, 37.5 );
			SetSkill( SkillName.Fencing, 15, 37.5 );
			SetSkill( SkillName.Wrestling, 15, 37.5 );
		}

		public Minter( Serial serial ) : base( serial )
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