using System;
using Server.Network;
using Server.Multis;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Regions;

namespace Server.Spells.Seventh
{
	public class GateTravelSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Gate Travel", "Vas Rel Por",
				SpellCircle.Seventh,
				245,
				9032,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);


		public GateTravelSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				Caster.SendLocalizedMessage( 1005570 ); // You can not gate to another facet.
			}
			else if ( !map.CanFit( loc.X, loc.Y, loc.Z, 16 ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( (checkMulti && SpellHelper.CheckMulti( loc, map )) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( !SpellHelper.CheckTravel( Caster, loc, map, TravelType.Gate ) && Caster.AccessLevel == AccessLevel.Player )
			{
				Caster.PlaySound( 0x5C );
			}
			else if ( CheckSequence() )
			{
				Caster.SendLocalizedMessage( 501024 ); // You open a magical gate to another location

				Effects.PlaySound( Caster.Location, Caster.Map, 0x20E );

				InternalItem firstGate = new InternalItem( loc, map );
				firstGate.MoveToWorld( Caster.Location, Caster.Map );

				Effects.PlaySound( loc, map, 0x20E );

				InternalItem secondGate = new InternalItem( Caster.Location, Caster.Map );
				secondGate.MoveToWorld( loc, map );
			}

			FinishSequence();
		}

		[DispellableField]
		private class InternalItem : Moongate
		{
			public override bool ShowFeluccaWarning{ get{ return Core.AOS; } }

			public InternalItem( Point3D target, Map map ) : base( target, map )
			{
				Map = map;

				if ( ShowFeluccaWarning && map == Map.Felucca )
					ItemID = 0xDDA;

				Dispellable = true;

				InternalTimer t = new InternalTimer( this );
				t.Start();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				Delete();
			}

			private class InternalTimer : Timer
			{
				private Item m_Item;

				public InternalTimer( Item item ) : base( TimeSpan.FromSeconds( 30.0 ) )
				{
					Priority = TimerPriority.OneSecond;
					m_Item = item;
				}

				protected override void OnTick()
				{
					m_Item.Delete();
				}
			}
		}

		private class InternalTarget : Target
		{
			private GateTravelSpell m_Owner;

			public InternalTarget( GateTravelSpell owner ) : base( 12, false, TargetFlags.None )
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
						from.SendLocalizedMessage( 501803 ); // That rune is not yet marked.
				}
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501030, from.Name, "" ) ); // I can not gate travel from that object.
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}