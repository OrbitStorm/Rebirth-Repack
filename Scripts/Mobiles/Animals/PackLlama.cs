using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a llama corpse")]
	public class PackLlama : BaseCreature
	{
		[Constructable]
		public PackLlama() : base( AIType.AI_Animal, FightMode.Agressor, 10, 1, 0.3, 0.8 )
		{
			Body = 292;
			Name = "a pack llama";
			SetStr( 21, 49 );
			SetHits( 21, 49 );
			SetDex( 36, 55 );
			SetStam( 36, 55 );
			SetInt( 16, 30 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 50;
			BaseSoundID = 181;
			SetSkill( SkillName.Tactics, 19.2, 29 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );
			SetSkill( SkillName.Parry, 35.1, 45 );
			SetSkill( SkillName.Wrestling, 19.2, 29 );

			VirtualArmor = 8;
			SetDamage( 2, 6 );

			Container pack = Backpack;

			if ( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public override int GenerateFurs(Corpse c)
		{
			c.DropItem( new LightFur() );
			return 1;
		}

		public PackLlama( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;

			PackAnimal.CombineBackpacks( this );

			return true;
		}

		public override bool IsSnoop( Mobile from )
		{
			if ( PackAnimal.CheckAccess( this, from ) )
				return false;

			return base.IsSnoop( from );
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( CheckFeed( from, item ) )
				return true;

			if ( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}

			return base.OnDragDrop( from, item );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			PackAnimal.TryPackOpen( this, from );
		}

        public override void GetContextMenuEntries(Mobile from, System.Collections.Generic.List<ContextMenus.ContextMenuEntry> list)
		{
			base.GetContextMenuEntries( from, list );

			PackAnimal.GetContextMenuEntries( this, from, list );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
