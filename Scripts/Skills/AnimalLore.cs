using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System.Collections; using System.Collections.Generic;

namespace Server.SkillHandlers
{
	public class AnimalLore
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.AnimalLore].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 500328 ); // What animal should I look at?

			return TimeSpan.FromSeconds( 10.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( !from.Alive )
				{
					from.SendLocalizedMessage( 500331 ); // The spirits of the dead are not the province of animal lore.
				}
				else if ( targeted is BaseCreature )
				{
					BaseCreature c = (BaseCreature)targeted;

					if ( !c.IsDeadPet )
					{
						if ( c.Body.IsAnimal || c.Body.IsMonster || c.Body.IsSea )
						{
							if ( (!c.Controled || !c.Tamable) && from.Skills[SkillName.AnimalLore].Base < 100.0 )
							{
								from.SendLocalizedMessage( 1049674 ); // At your skill level, you can only lore tamed creatures.
							}
							else if ( !c.Tamable && from.Skills[SkillName.AnimalLore].Base < 110.0 )
							{
								from.SendLocalizedMessage( 1049675 ); // At your skill level, you can only lore tamed or tameable creatures.
							}
							else if ( !from.CheckTargetSkill( SkillName.AnimalLore, c, 0.0, 120.0 ) )
							{
								from.SendLocalizedMessage( 500334 ); // You can't think of anything you know offhand.
							}
							else
							{
								c.SayTo( from, (!c.Controled || c.Loyalty == PetLoyalty.None) ? 1061643 : 1049594 + (int)c.Loyalty );
								c.SayTo( from, 1049563 );
								ArrayList food = new ArrayList();
								if ( (c.FavoriteFood & FoodType.FruitsAndVegies) != 0 )
									food.Add( 1049565 ); // Fruits and Vegetables
								if ( (c.FavoriteFood & FoodType.GrainsAndHay) != 0 )
									food.Add( 1049566 ); // Grains and Hay
								if ( (c.FavoriteFood & FoodType.Fish) != 0 )
									food.Add( 1049568 ); // Fish
								if ( (c.FavoriteFood & FoodType.Meat) != 0 )
									food.Add( 1049564 ); // Meat

								if ( food.Count <= 0 )
								{
									c.SayTo( from, 3000340 );
								}
								else
								{
									foreach ( int i in food )
										c.SayTo( from, i );
								}
							}
						}
						else
						{
							from.SendLocalizedMessage( 500329 ); // That's not an animal!
						}
					}
					else
					{
						from.SendLocalizedMessage( 500331 ); // The spirits of the dead are not the province of animal lore.
					}
				}
				else
				{
					from.SendLocalizedMessage( 500329 ); // That's not an animal!
				}
			}
		}
	}
}