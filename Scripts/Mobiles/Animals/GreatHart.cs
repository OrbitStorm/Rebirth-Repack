using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a deer corpse" )]
	[TypeAlias( "Server.Mobiles.Greathart" )]
	public class GreatHart : BaseCreature
	{
		[Constructable]
		public GreatHart() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.45, 0.8 )
		{
			Name = "a great hart";
			Body = 0xEA;

			SetStr( 41, 71 );
			SetDex( 47, 77 );
			SetInt( 27, 57 );
			SetMana( 0 );

			SetDamage( 4, 10 );

			SetSkill( SkillName.MagicResist, 26.8, 44.5 );
			SetSkill( SkillName.Tactics, 29.8, 47.5 );
			SetSkill( SkillName.Wrestling, 29.8, 47.5 );

			VirtualArmor = 12;

			Tamable = true;
			MinTameSkill = 70;
		}

		public override int Meat{ get{ return 6; } }
		public override int Hides{ get{ return 9; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public GreatHart(Serial serial) : base(serial)
		{
		}

		public override int GetAttackSound() 
		{ 
			return 0x82; 
		} 

		public override int GetHurtSound() 
		{ 
			return 0x83; 
		} 

		public override int GetDeathSound() 
		{ 
			return 0x84; 
		} 

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
