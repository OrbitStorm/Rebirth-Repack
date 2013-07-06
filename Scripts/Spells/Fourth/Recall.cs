using System;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Regions;

namespace Server.Spells.Fourth
{
	public class RecallSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Recall", "Kal Ort Por",
				SpellCircle.Fourth,
				239,
				9031,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public RecallSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
			if ( map == null || (!Core.AOS && Caster.Map != map) )
			{
				Caster.SendLocalizedMessage( 1005569 ); // You can not recall to another facet.
			}
			else if ( Caster.Kills >= 5 && map != Map.Felucca )
			{
				Caster.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
			}
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			}
			else if ( !map.CanFit( loc.X, loc.Y, loc.Z, 16 ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( checkMulti && SpellHelper.CheckMulti( loc, map ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( !SpellHelper.CheckTravel( Caster, loc, map, TravelType.Recall ) && Caster.AccessLevel == AccessLevel.Player )
			{
				Caster.PlaySound( 0x5C );
			}
			else if ( CheckSequence() )
			{
				BaseCreature.TeleportPets( Caster, loc, map, true );

				Caster.PlaySound( 0x1FC );
				Caster.Map = map;
				Caster.Location = loc;
				Caster.PlaySound( 0x1FC );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private RecallSpell m_Owner;

			public InternalTarget( RecallSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;

				owner.Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 501029 ); // Select Marked item.
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune)o;

					if ( rune.Marked )
						m_Owner.Effect( rune.Target, rune.TargetMap, true );
					else
						from.SendLocalizedMessage( 501805 ); // That rune is not yet marked.
				}
				/*else if ( o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat )
				{
					BaseBoat boat = ((Key)o).Link as BaseBoat;

					if ( boat != null && !boat.Deleted && boat.CheckKey( ((Key)o).KeyValue ) )
						m_Owner.Effect( boat.GetMarkedLocation(), boat.Map, false );
					else
						from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}*/
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}