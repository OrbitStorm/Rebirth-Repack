using System;
using Server.Targeting;
using Server.Items;

namespace Server.Items
{
	public class MagicWand : BaseBashing
	{
		public override int AosStrengthReq{ get{ return 5; } }
		public override int AosMinDamage{ get{ return 9; } }
		public override int AosMaxDamage{ get{ return 11; } }
		public override int AosSpeed{ get{ return 40; } }

		public override int OldStrengthReq{ get{ return 1; } }
		public override int OldSpeed{ get{ return 35; } }

		public override int NumDice { get { return 2; } }
		public override int NumSides { get { return 3; } }
		public override int DiceBonus { get { return 0; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		public override void AppendClickName(System.Text.StringBuilder sb)
		{
			if ( Name == null || Name.Length <= 0 )
				sb.Append( "wand" );
			else
				sb.Append( Name );
		}

		[Constructable]
		public MagicWand() : base( 0xDF2 )
		{
			Weight = 1.0;
		}

		public MagicWand( Serial serial ) : base( serial )
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

		public override bool SpellEffectOnHit { get { return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( SpellEffect == SpellEffect.None )
				return;

			if ( this.Parent != from )
			{
				from.SendAsciiMessage( "You must equip this item to use it." );
				return;
			}
			else if ( this.SpellCharges <= 0 )
			{
				from.SendAsciiMessage( "This magic item is out of charges." );
				return;
			}

			from.RevealingAction();
			if ( SpellEffect == SpellEffect.ItemID )
				from.Target = new ItemIdentification.InternalTarget( false, new TargetCallback( OnTarget ) );
			else
				from.BeginTarget( 10, false, TargetFlags.None, new TargetCallback( OnTarget ) );
		}

		public virtual void OnTarget( Mobile from, object targeted )
		{
			if ( SpellEffect == SpellEffect.None || SpellCharges <= 0 )
				return;

			bool ok = false;
			if ( targeted is Mobile )
				ok = SpellCastEffect.InvokeEffect( SpellEffect, from, (Mobile)targeted );
			else if ( targeted is Item )
				ok = SpellCastEffect.InvokeEffect( SpellEffect, from, (Item)targeted );

			if ( ok )
			{
				from.NextActionTime = DateTime.Now + TimeSpan.FromSeconds( 1.5 );
				SpellCharges--;
				if ( SpellCharges == 0 )
					from.SendAsciiMessage( "This magic item is out of charges." );
			}
		}
	}
}
