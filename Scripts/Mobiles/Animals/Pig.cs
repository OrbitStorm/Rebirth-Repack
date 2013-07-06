using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a pig corpse" )]
	public class Pig : BaseCreature
	{
		[Constructable]
		public Pig() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 203;
			Name = "a pig";
			SetStr( 22, 64 );
			SetHits( 23, 65 );
			SetDex( 22, 64 );
			SetStam( 23, 65 );
			SetInt( 26, 33 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 30;
			BaseSoundID = 196;
			SetSkill( SkillName.Tactics, 19.3, 34 );
			SetSkill( SkillName.Wrestling, 19.3, 34 );
			SetSkill( SkillName.MagicResist, 25.1, 33 );
			SetSkill( SkillName.Parry, 19.3, 34 );

			VirtualArmor = 6;
			SetDamage( 2, 6 );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Pig( Serial serial ) : base( serial )
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

