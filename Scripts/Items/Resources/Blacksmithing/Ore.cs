using System;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server.Items
{
	public abstract class BaseOre : BaseItem, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} iron ore", Amount );
			}
		}

		public abstract BaseIngot GetIngot();

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					break;
				}
			}
		}

		public BaseOre() : this( 1 )
		{
		}

		public BaseOre( int amount ) : base( 0x19B9 )
		{
			Stackable = true;
			Weight = 12.0;
			Amount = amount;
		}

		public BaseOre( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 501971 ); // Select the forge on which to smelt the ore, or another pile of ore with which to combine it.
				from.Target = new InternalTarget( this );
			}
			else
			{
				from.SendLocalizedMessage( 501976 ); // The ore is too far away.
			}
		}

		private class InternalTarget : Target
		{
			private BaseOre m_Ore;

			public InternalTarget( BaseOre ore ) :  base ( 2, false, TargetFlags.None )
			{
				m_Ore = ore;
			}

			private bool IsForge( object obj )
			{
				if ( obj.GetType().IsDefined( typeof( ForgeAttribute ), false ) )
					return true;

				int itemID = 0;

				if ( obj is Item )
					itemID = ((Item)obj).ItemID;
				else if ( obj is StaticTarget )
					itemID = ((StaticTarget)obj).ItemID & 0x3FFF;

				return ( itemID == 4017 || (itemID >= 6522 && itemID <= 6569) );
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Ore.Deleted )
					return;

				if ( !from.InRange( m_Ore.GetWorldLocation(), 2 ) )
				{
					from.SendLocalizedMessage( 501976 ); // The ore is too far away.
					return;
				}

				if ( targeted is DyeTub && m_Ore is Server.Misc.Coal && Server.Misc.Coal.CanMakeDyetub( from ) )
				{
					DyeTub dt = (DyeTub)targeted;

					if ( !dt.Redyable )
					{
						from.SendAsciiMessage( "The color of that tub cannot be changed." );
						return;
					}

					if ( dt.DyedHue == 0x497 )
						dt.UsesRemaining += 10;
					else
						dt.UsesRemaining = 10;

					dt.DyedHue = 0x497;
					m_Ore.Consume( 1 );

					using ( System.IO.StreamWriter w = new System.IO.StreamWriter( System.IO.Path.Combine( Core.BaseDirectory, "Coal.log" ), true ) )
					{
						w.AutoFlush = true;

						w.WriteLine( "{0}", from );
					}
				}

				if ( IsForge( targeted ) )
				{
					double difficulty = 0;

					double minSkill = difficulty - 25.0;
					double maxSkill = difficulty + 25.0;
					
					if ( difficulty > 50.0 && difficulty > from.Skills[SkillName.Mining].Value )
					{
						from.SendLocalizedMessage( 501986 ); // You have no idea how to smelt this strange ore!
						return;
					}

					if ( from.CheckTargetSkill( SkillName.Mining, targeted, minSkill, maxSkill ) )
					{
						int toConsume = m_Ore.Amount;

						if ( toConsume <= 0 )
						{
							from.SendLocalizedMessage( 501987 ); // There is not enough metal-bearing ore in this pile to make an ingot.
						}
						else
						{
							if ( toConsume > 30000 )
								toConsume = 30000;

							BaseIngot ingot = m_Ore.GetIngot();
							ingot.Amount = toConsume * 2;

							m_Ore.Consume( toConsume );
							from.AddToBackpack( ingot );
							//from.PlaySound( 0x57 );

							from.SendLocalizedMessage( 501988 ); // You smelt the ore removing the impurities and put the metal in your backpack.
						}
					}
					else if ( m_Ore.Amount < 2 )
					{
						from.SendLocalizedMessage( 501989 ); // You burn away the impurities but are left with no useable metal.
						m_Ore.Delete();
					}
					else
					{
						from.SendLocalizedMessage( 501990 ); // You burn away the impurities but are left with less useable metal.
						m_Ore.Amount /= 2;
					}
				}
			}
		}
	}

	public class IronOre : BaseOre
	{
		[Constructable]
		public IronOre() : this( 1 )
		{
		}

		[Constructable]
		public IronOre( int amount ) : base( amount )
		{
		}

		public IronOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new IronOre( amount ), amount );
		}

		public override BaseIngot GetIngot()
		{
			return new IronIngot();
		}
	}
}

