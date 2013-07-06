using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a grizzly bear corpse" )]
	public class GrizzlyBear : BaseCreature
	{
		[Constructable]
		public GrizzlyBear() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 212;
			Name = "a grizzly bear";
			SetStr( 126, 155 );
			SetHits( 126, 155 );
			SetDex( 81, 105 );
			SetStam( 81, 105 );
			SetInt( 16, 40 );
			SetMana( 0 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 70;
			BaseSoundID = 163;
			SetSkill( SkillName.Tactics, 70.1, 100 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );
			SetSkill( SkillName.Parry, 70.1, 85 );
			SetSkill( SkillName.Wrestling, 50.1, 65 );

			VirtualArmor = 12;
			SetDamage( 6, 15 );
		}

		public override int Meat{ get{ return 2; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public override int GenerateFurs(Corpse c)
		{
			Item i = new LightFur();
			i.Amount = 2;
			c.DropItem( i );
			return 2;
		}

		public override bool AlwaysMurderer
		{
			get
			{
				return true;
			}
		}

		public GrizzlyBear( Serial serial ) : base( serial )
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

