using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a chicken corpse" )]
	public class Chicken : BaseCreature
	{
		[Constructable]
		public Chicken() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 208;
			Name = "a chicken";
			SetStr( 6, 10 );
			SetHits( 2, 8 );
			SetDex( 16, 25 );
			SetStam( 30, 60 );
			SetInt( 1, 5 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 20;
			BaseSoundID = 110;
			SetSkill( SkillName.Tactics, 5.1, 10 );
			SetSkill( SkillName.Wrestling, 5.1, 10 );
			SetSkill( SkillName.MagicResist, 4.1, 9 );
			SetSkill( SkillName.Parry, 15.1, 25 );

			VirtualArmor = 1;
			SetDamage( 1 );
		}

		public override int Meat{ get{ return 1; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }
		public override int Feathers{ get{ return 13; } }

		public Chicken( Serial serial ) : base( serial )
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

