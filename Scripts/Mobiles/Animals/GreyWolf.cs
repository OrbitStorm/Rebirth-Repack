using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a grey wolf corpse" )]
	public class GreyWolf : BaseCreature
	{
		[Constructable]
		public GreyWolf() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 225;
			Name = "a grey wolf";
			Hue = 946;
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 31, 55 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 65;
			BaseSoundID = 229;
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 20.1, 35 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Wrestling, 45.1, 60 );

			VirtualArmor = 9;
			SetDamage( 2, 8 );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public override int GenerateFurs(Corpse c)
		{
			c.DropItem( new LightFur() );
			return 1;
		}

		public GreyWolf( Serial serial ) : base( serial )
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

