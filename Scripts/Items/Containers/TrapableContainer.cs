using System;
using Server.Network;
using Server.Mobiles;
using System.Collections; using System.Collections.Generic;

namespace Server.Items
{
	public enum TrapType
	{
		None,
		MagicTrap,
		ExplosionTrap,
		PoisonTrap,
		DartTrap,
	}

	public abstract class TrapableContainer : BaseContainer, ITelekinesisable
	{
		private TrapType m_TrapType;
		private int m_TrapPower;
		private Mobile m_Trapper;
		private bool m_Trapped;

		[CommandProperty( AccessLevel.GameMaster )]
		public TrapType TrapType
		{
			get
			{
				return m_TrapType;
			}
			set
			{
				m_TrapType = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TrapPower
		{
			get
			{
				return m_TrapPower;
			}
			set
			{
				m_TrapPower = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Trapped
		{
			get{ return m_Trapped && m_TrapType != TrapType.None; }
			set{ m_Trapped = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Trapper
		{
			get
			{
				return m_Trapper;
			}
			set
			{
				m_Trapper = value;
			}
		}

		public TrapableContainer( int itemID ) : base( itemID )
		{
		}

		public TrapableContainer( Serial serial ) : base( serial )
		{
		}

		public virtual bool ExecuteTrap( Mobile from )
		{
			if ( m_TrapType != TrapType.None && m_Trapped )
			{
				if ( from.AccessLevel != AccessLevel.Player )
				{
					if ( from.AccessLevel >= AccessLevel.GameMaster )
						from.SendAsciiMessage( "Note: This container is trapped." );
					return false;
				}

				if ( m_Trapper != null && !m_Trapper.Deleted )
					m_Trapper.Attack( from );
				//	from.AggressiveAction( m_Trapper, false );
				// should m_Trapper be flagged for their trap being opened?

				switch ( m_TrapType )
				{
					case TrapType.DartTrap:
					{
						from.SendLocalizedMessage( 502999 ); // You set off a trap!
						from.SendAsciiMessage( "A dart embeds itself in your flesh!" );

						if ( from != m_Trapper && m_Trapper != null && !m_Trapper.Deleted )
							from.AggressiveAction( m_Trapper );//AggressiveActionNoRegion( m_Trapper, from, Notoriety.Compute( m_Trapper, from ) == Notoriety.Innocent );

						AOS.Damage( from, Utility.Random( 1, 15 ) + m_TrapPower, 100, 0, 0, 0, 0 );
						Effects.SendMovingEffect( this, from, 0x1BFE, 18, 1, false, false );
						break;
					}
					case TrapType.PoisonTrap:
					{
						from.SendLocalizedMessage( 502999 ); // You set off a trap!
						
						if ( from.InRange( GetWorldLocation(), 2 ) )
						{
							from.Poison = Poison.GetPoison( m_TrapPower - 1 );
							from.SendAsciiMessage( "A cloud of green gas engulfs your body!" );

							if ( from != m_Trapper && m_Trapper != null && !m_Trapper.Deleted )
								from.AggressiveAction( m_Trapper );
						}

						Point3D loc = GetWorldLocation();
						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y + 1, loc.Z ), Map, 0x11a6, 15 );
						break;
					}
					case TrapType.ExplosionTrap:
					{
						from.SendLocalizedMessage( 502999 ); // You set off a trap!

						int damage = 20*( m_TrapPower - 1 ) + Utility.Random( 25 );
						if ( this.RootParent is Mobile && this.RootParent != from )
						{
							Mobile mob = (Mobile)this.RootParent;
							AOS.Damage( mob, damage / 3, 0, 100, 0, 0, 0 );
							damage -= damage/3;
							mob.SendLocalizedMessage( 503000 ); // Your skin blisters from the heat!

							if ( mob != m_Trapper && m_Trapper != null && !m_Trapper.Deleted )
								from.AggressiveAction( m_Trapper );
						}

						if ( from.InRange( GetWorldLocation(), 2 ) )
						{
							AOS.Damage( from, damage, 0, 100, 0, 0, 0 );
							from.SendLocalizedMessage( 503000 ); // Your skin blisters from the heat!

							if ( from != m_Trapper && m_Trapper != null && !m_Trapper.Deleted )
								from.AggressiveAction( m_Trapper );
						}

						Point3D loc = GetWorldLocation();
						Effects.PlaySound( loc, Map, 0x307 );
						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y + 1, loc.Z - 11 ), Map, 0x36BD, 15 );
						break;
					}
					case TrapType.MagicTrap:
					{
						if ( from.InRange( GetWorldLocation(), 1 ) )
						{
							double damage = Spells.Spell.GetPreUORDamage( Spells.SpellCircle.Second ) * 0.75;
							if ( from.CheckSkill( SkillName.MagicResist, damage * 2.5 - 25.0, damage * 2.5 + 25.0 ) )
							{
								damage *= 0.5;
								from.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
							}
							if ( damage < 1 )
								damage = 1;
							from.Damage( (int)damage );
							//AOS.Damage( from, m_TrapPower, 0, 100, 0, 0, 0 );
						}

						Point3D loc = GetWorldLocation();

						Effects.PlaySound( loc, Map, 0x307 );

						Effects.SendLocationEffect( new Point3D( loc.X - 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );
						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );

						Effects.SendLocationEffect( new Point3D( loc.X, loc.Y - 1, loc.Z ), Map, 0x36BD, 15 );
						Effects.SendLocationEffect( new Point3D( loc.X, loc.Y + 1, loc.Z ), Map, 0x36BD, 15 );

						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y + 1, loc.Z + 11 ), Map, 0x36BD, 15 );

						break;
					}
				}

				m_Trapped = false;
				return true;
			}

			return false;
		}

		public virtual void OnTelekinesis( Mobile from )
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
			Effects.PlaySound( Location, Map, 0x1F5 );

			if ( !ExecuteTrap( from ) )
				base.DisplayTo( from );
		}

		public override void DisplayTo(Mobile to)
		{
			if ( !ExecuteTrap( to ) )
				base.DisplayTo (to);
		}

		public override void OnDoubleClickSecureTrade( Mobile from )
		{
			if ( Trapped )
				from.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, 502503, "", "" ) ); // That is locked.
			else
				base.OnDoubleClickSecureTrade( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			writer.Write( (bool)m_Trapped );
			writer.Write( (int)(m_Trapper == null || m_Trapper.Deleted ? Serial.Zero : m_Trapper.Serial) );
			writer.Write( (int) m_TrapPower );
			writer.Write( (int) m_TrapType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				{
					m_Trapped = reader.ReadBool();

					goto case 2;
				}
				case 2:
				{
					m_Trapper = World.FindMobile( (Serial)reader.ReadInt() );

					goto case 1;
				}
				case 1:
				{
					m_TrapPower = reader.ReadInt();

					goto case 0;
				}

				case 0:
				{
					m_TrapType = (TrapType)reader.ReadInt();

					break;
				}
			}

			if ( version < 3 )
				m_Trapped = m_TrapType != TrapType.None;
		}
	}
}
