using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a pig corpse" )]
	public class Boar : BaseCreature
	{
		[Constructable]
		public Boar() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 290;
			Name = "a boar";
			SetStr( 31, 61 );
			SetHits( 41, 59 );
			SetDex( 47, 77 );
			SetStam( 51, 63 );
			SetInt( 27, 57 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 45;
			BaseSoundID = 196;
			SetSkill( SkillName.Tactics, 21.3, 39 );
			SetSkill( SkillName.Wrestling, 21.3, 39 );
			SetSkill( SkillName.MagicResist, 17.3, 35 );
			SetSkill( SkillName.Parry, 14.8, 32.5 );

			VirtualArmor = 7;
			SetDamage( 3, 6 );
		}

		public override int Meat{ get{ return 2; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Boar( Serial serial ) : base( serial )
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

