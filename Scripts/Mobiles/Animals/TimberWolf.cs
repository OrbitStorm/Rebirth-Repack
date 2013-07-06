using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a timber wolf corpse" )]
	public class TimberWolf : BaseCreature
	{
		[Constructable]
		public TimberWolf() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Body = 225;
			Name = "a timber wolf";
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 11, 25 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 40;
			BaseSoundID = 229;
			SetSkill( SkillName.Tactics, 30.1, 50 );
			SetSkill( SkillName.Wrestling, 40.1, 60 );
			SetSkill( SkillName.MagicResist, 27.6, 45 );
			SetSkill( SkillName.Parry, 42.6, 55 );

			VirtualArmor = 9;
			SetDamage( 4, 10 );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }
		
		public override int GenerateFurs(Corpse c)
		{
			c.DropItem( new LightFur() );
			return 1;
		}

		public TimberWolf( Serial serial ) : base( serial )
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

