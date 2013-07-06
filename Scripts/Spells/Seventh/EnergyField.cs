using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using System.Collections; using System.Collections.Generic;
using Server.Regions;
using Server.Items;

namespace Server.Spells.Seventh
{
	public class EnergyFieldSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Energy Field", "In Sanct Grav",
				SpellCircle.Seventh,
				245,
				9022,
				false,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public EnergyFieldSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				SpellHelper.GetSurfaceTop( ref p );

				int dx = Caster.Location.X - p.X;
				int dy = Caster.Location.Y - p.Y;
				int rx = (dx - dy) * 44;
				int ry = (dx + dy) * 44;

				bool eastToWest;

				if ( rx >= 0 && ry >= 0 )
				{
					eastToWest = false;
				}
				else if ( rx >= 0 )
				{
					eastToWest = true;
				}
				else if ( ry >= 0 )
				{
					eastToWest = true;
				}
				else
				{
					eastToWest = false;
				}

				Effects.PlaySound( p, Caster.Map, 0x20B );

				TimeSpan duration = TimeSpan.FromSeconds( Caster.Skills[SkillName.Magery].Value * 0.28 + 2.0 ); // (28% of magery) + 2.0 seconds
				int itemID = eastToWest ? 0x3946 : 0x3956;

				for ( int i = -2; i <= 2; ++i )
				{
					Point3D loc = new Point3D( eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z );
					bool canFit = SpellHelper.AdjustField( ref loc, Caster.Map, 12, false );

					if ( !canFit )
						continue;

					Item item = new InternalItem( loc, Caster.Map, duration, itemID, Caster );
					item.ProcessDelta();

					Effects.SendLocationParticles( EffectItem.Create( loc, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, 5051 );
				}
			}

			FinishSequence();
		}

		[DispellableField]
		private class InternalItem : BaseItem
		{
			private Timer m_Timer;
			private Mobile m_Caster;
			private DateTime m_End;

			public override bool BlocksFit{ get{ return true; } }

			public InternalItem( Point3D loc, Map map, TimeSpan duration, int itemID, Mobile caster ) : base( itemID )
			{
				m_Caster = caster;

				Visible = false;
				Movable = false;
				Light = LightType.Circle300;

				m_End = DateTime.Now + duration;

				MoveToWorld( loc, map );

				if ( caster.InLOS( this ) )
					Visible = true;
				else
					Delete();

				if ( Deleted )
					return;

				m_Timer = new InternalTimer( this, TimeSpan.FromSeconds( 1.0 ) );
				m_Timer.Start();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
				m_Timer = new InternalTimer( this, TimeSpan.FromSeconds( 5.0 ) );
				m_Timer.Start();
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

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Timer != null )
					m_Timer.Stop();
			}

			public override bool OnMoveOver( Mobile m )
			{
				if ( Visible && m_Caster != null && SpellHelper.ValidIndirectTarget( m_Caster, m ) && m_Caster.CanBeHarmful( m, false ) )
				{
					m_Caster.DoHarmful( m );

					int damage = 7;

					if ( m.Region is GuardedRegion && !((GuardedRegion)m.Region).IsDisabled() )
					{
						damage = 0;
						m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}
					else
					{
						double sk = damage * 2.5;

						// calc 4*6*2.5 +/- 20
						if ( m.CheckSkill( SkillName.MagicResist, sk - 25, sk + 25 ) )
						{
							damage /= 2;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}
					}

					SpellHelper.Damage( TimeSpan.Zero, m, m_Caster, damage, 0, 100, 0, 0, 0 );
					m.PlaySound( 0x208 );
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private InternalItem m_Item;

				private static Queue m_Queue = new Queue();

				public InternalTimer( InternalItem item, TimeSpan delay ) : base( delay, TimeSpan.FromSeconds( 1.0 ) )
				{
					m_Item = item;

					Priority = TimerPriority.FiftyMS;
				}

				protected override void OnTick()
				{
					if ( m_Item.Deleted )
						return;

					if ( !m_Item.Visible )
					{
						m_Item.Visible = true;
						
						m_Item.ProcessDelta();
						Effects.SendLocationParticles( EffectItem.Create( m_Item.Location, m_Item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, 5029 );
					}
					else if ( DateTime.Now > m_Item.m_End )
					{
						m_Item.Delete();
						Stop();
					}
					else
					{
						Map map = m_Item.Map;
						Mobile caster = m_Item.m_Caster;

						if ( map != null && caster != null )
						{
							foreach ( Mobile m in m_Item.GetMobilesInRange( 0 ) )
							{
								if ( (m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && SpellHelper.ValidIndirectTarget( caster, m ) && caster.CanBeHarmful( m, false ) )
									m_Queue.Enqueue( m );
							}

							while ( m_Queue.Count > 0 )
							{
								Mobile m = (Mobile)m_Queue.Dequeue();

								caster.DoHarmful( m );

								int damage = 3;
								if ( m.Region is GuardedRegion && !((GuardedRegion)m.Region).IsDisabled() )
								{
									damage = 0;
									m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
								}
								else
								{
									double sk = damage * 2.5;

									// calc 4*6*2.5 +/- 20
									if ( m.CheckSkill( SkillName.MagicResist, sk - 25, sk + 25 ) )
									{
										damage /= 2;

										m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
									}
								}
								SpellHelper.Damage( TimeSpan.Zero, m, caster, damage, 0, 100, 0, 0, 0 );
								m.PlaySound( 529 );
							}
						}
					}
				}
			}
		}

		private class InternalTarget : Target
		{
			private EnergyFieldSpell m_Owner;

			public InternalTarget( EnergyFieldSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}