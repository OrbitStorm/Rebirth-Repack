using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a panther corpse" )]
	public class Panther : BaseCreature
	{
		[Constructable]
		public Panther() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 214;
			Name = "a panther";
			Hue = 2305;
			SetStr( 61, 85 );
			SetHits( 61, 85 );
			SetDex( 86, 105 );
			SetStam( 86, 105 );
			SetInt( 26, 50 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 65;
			BaseSoundID = 186;
			SetSkill( SkillName.Tactics, 50.1, 65 );
			SetSkill( SkillName.Wrestling, 50.1, 65 );
			SetSkill( SkillName.MagicResist, 15.1, 30 );
			SetSkill( SkillName.Parry, 55.1, 65 );

			VirtualArmor = 8;
			SetDamage( 2, 14 );
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Feline; } }


		public Panther( Serial serial ) : base( serial )
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

