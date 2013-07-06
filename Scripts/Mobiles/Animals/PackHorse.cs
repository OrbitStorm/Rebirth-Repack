using System;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName( "a horse corpse" )]
	public class PackHorse : BaseCreature
	{
		[Constructable]
		public PackHorse() : base( AIType.AI_Animal, FightMode.Agressor, 10, 1, 0.3, 0.8 )
		{
			Body = 291;
			Name = "a pack horse";
			SetStr( 44, 120 );
			SetHits( 44, 120 );
			SetDex( 36, 55 );
			SetStam( 36, 55 );
			SetInt( 6, 10 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 45;
			BaseSoundID = 168;
			SetSkill( SkillName.Tactics, 29.3, 44 );
			SetSkill( SkillName.Wrestling, 29.3, 44 );
			SetSkill( SkillName.MagicResist, 25.1, 30 );
			SetSkill( SkillName.Parry, 35.1, 45 );

			VirtualArmor = 9;
			SetDamage( 4, 12 );

			Container pack = Backpack;

			if ( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public PackHorse( Serial serial ) : base( serial )
		{
		}

		#region Pack Animal Methods
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
		#endregion

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

	public class PackAnimalBackpackEntry : ContextMenuEntry
	{
		private BaseCreature m_Animal;
		private Mobile m_From;

		public PackAnimalBackpackEntry( BaseCreature animal, Mobile from ) : base( 6145, 3 )
		{
			m_Animal = animal;
			m_From = from;

			if ( animal.IsDeadPet )
				Enabled = false;
		}

		public override void OnClick()
		{
			PackAnimal.TryPackOpen( m_Animal, m_From );
		}
	}

	public class PackAnimal
	{
        public static void GetContextMenuEntries(BaseCreature animal, Mobile from, System.Collections.Generic.List<ContextMenus.ContextMenuEntry> list)
		{
			if ( CheckAccess( animal, from ) )
				list.Add( new PackAnimalBackpackEntry( animal, from ) );
		}

		public static bool CheckAccess( BaseCreature animal, Mobile from )
		{
			if ( from == animal || from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			if ( from.Alive && animal.Controled && (from == animal.ControlMaster || from == animal.SummonMaster) )
				return true;

			return false;
		}

		public static void CombineBackpacks( BaseCreature animal )
		{
			if ( animal.IsBonded || animal.IsDeadPet )
				return;

			Container pack = animal.Backpack;

			if ( pack != null )
			{
				Container newPack = new Backpack();

				for ( int i = pack.Items.Count - 1; i >= 0; --i )
				{
					if ( i >= pack.Items.Count )
						continue;

					newPack.DropItem( (Item)pack.Items[i] );
				}

				pack.DropItem( newPack );
			}
		}

		public static void TryPackOpen( BaseCreature animal, Mobile from )
		{
			if ( animal.IsDeadPet )
				return;

			Container item = animal.Backpack;

			if ( item != null )
				from.Use( item );
		}
	}
}
