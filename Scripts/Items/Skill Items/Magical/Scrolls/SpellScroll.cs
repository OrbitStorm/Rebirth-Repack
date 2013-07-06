using System;
using System.Collections; using System.Collections.Generic;
using Server.Spells;

namespace Server.Items
{
	public class SpellScroll : BaseItem
	{
		private int m_SpellID;

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpellID
		{
			get
			{
				return m_SpellID;
			}
		}

		public SpellScroll( Serial serial ) : base( serial )
		{
		}

		[Constructable]
		public SpellScroll( int spellID, int itemID ) : this( spellID, itemID, 1 )
		{
		}

		[Constructable]
		public SpellScroll( int spellID, int itemID, int amount ) : base( itemID )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;

			m_SpellID = spellID;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_SpellID );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_SpellID = reader.ReadInt();

					break;
				}
			}
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenus.ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new ContextMenus.AddToSpellbookEntry() );
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new SpellScroll( m_SpellID, ItemID, amount ), amount );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}
			
			Item oneHanded = from.FindItemOnLayer( Layer.OneHanded );
			Item twoHanded = from.FindItemOnLayer( Layer.TwoHanded );
			if ( (oneHanded != null && !oneHanded.AllowEquipedCast( from )) || (twoHanded != null && !twoHanded.AllowEquipedCast( from )) )
			{
				//m_Caster.SendLocalizedMessage( 502626 ); // Your hands must be free to cast spells or meditate
				from.SendAsciiMessage( "Your hands must be free to cast spells." );
				return;
			}

			Spell spell = SpellRegistry.NewSpell( m_SpellID, from, this );

			if ( spell != null )
				spell.Cast();
			else
				from.SendLocalizedMessage( 502345 ); // This spell has been temporarily disabled.
		}
	}
}
