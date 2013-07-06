using System;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class LockableContainer : TrapableContainer, ILockable, ILockpickable//, ITelekinesisable
	{
		private bool m_Locked;
		private int m_LockLevel, m_MaxLockLevel, m_RequiredSkill;
		private uint m_KeyValue;
		private Mobile m_Picker;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool MustStealItems
		{
			get
			{
				return !Movable && !Locked && MaxLockLevel == 0 && KeyValue == 0xFFFFFFFF;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Picker
		{
			get
			{
				return m_Picker;
			}
			set
			{
				m_Picker = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxLockLevel
		{
			get
			{
				return m_MaxLockLevel;
			}
			set
			{
				m_MaxLockLevel = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int LockLevel
		{
			get
			{
				return m_LockLevel;
			}
			set
			{
				m_LockLevel = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RequiredSkill
		{
			get
			{
				return m_RequiredSkill;
			}
			set
			{
				m_RequiredSkill = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual bool Locked
		{
			get
			{
				return m_Locked;
			}
			set
			{
				m_Locked = value;

				if ( m_Locked )
					m_Picker = null;

				Delta( ItemDelta.Update );

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public uint KeyValue
		{
			get
			{
				return m_KeyValue;
			}
			set
			{
				m_KeyValue = value;
			}
		}

		public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			if ( item != this )
			{
				if ( CheckLocked( from ) )
				{
					return false;
				}
				else
				{
					if ( MustStealItems )
					{
						bool mustSteal = false;
						foreach ( Item i in this.Items )
						{
							if ( i is Spawner )
							{
								if ( ((Spawner)i).SpawnedObject( item ) )
								{
									mustSteal = true;
									break;
								}
							}
						}

						if ( mustSteal )
						{
							from.SendAsciiMessage( "That item does not belong to you.  You'll have to steal it." );
							return from.AccessLevel >= AccessLevel.GameMaster;
						}
					}
				}
			}

			return base.CheckLift(from, item, ref reject);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if ( dropped != this && CheckLocked( from ) )
				return false;

			return base.OnDragDrop (from, dropped);
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if ( item != this && CheckLocked( from ) )
				return false;

			return base.OnDragDropInto (from, item, p);
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 4 ); // version

			writer.Write( (int) m_RequiredSkill );

			writer.Write( (int) m_MaxLockLevel );

			writer.Write( m_KeyValue );
			writer.Write( (int) m_LockLevel );
			writer.Write( (bool) m_Locked );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 4:
				{
					m_RequiredSkill = reader.ReadInt();

					goto case 3;
				}
				case 3:
				{
					m_MaxLockLevel = reader.ReadInt();

					goto case 2;
				}
				case 2:
				{
					m_KeyValue = reader.ReadUInt();

					goto case 1;
				}
				case 1:
				{
					m_LockLevel = reader.ReadInt();

					goto case 0;
				}
				case 0:
				{
					if ( version < 3 )
						m_MaxLockLevel = 50;

					if ( version < 4 )
					{
						if ( (m_MaxLockLevel - m_LockLevel) == 40 )
						{
							m_RequiredSkill = m_LockLevel + 6;
							m_LockLevel = m_RequiredSkill - 10;
							m_LockLevel = m_RequiredSkill + 39;
						}
						else
						{
							m_RequiredSkill = m_LockLevel;
						}
					}

					m_Locked = reader.ReadBool();

					break;
				}
			}
		}

		public LockableContainer( int itemID ) : base( itemID )
		{
			m_MaxLockLevel = 50;
			m_LockLevel = 1;
		}

		public LockableContainer( Serial serial ) : base( serial )
		{
		}

		public override bool CheckContentDisplay( Mobile from )
		{
			return !m_Locked && base.CheckContentDisplay( from );
		}

		public override bool DisplaysContent{ get{ return !m_Locked; } }

		public virtual bool CheckLocked( Mobile from )
		{
			if ( m_Locked )
			{
				if ( from.AccessLevel >= AccessLevel.GameMaster )
				{
					from.Send( new AsciiMessage( Serial, ItemID, MessageType.Regular, 0x3B2, 3, "", "That is locked, but your powers allow you access." ) );
					return false;
				}
				else
				{
					from.Send( new AsciiMessage( Serial, ItemID, MessageType.Regular, 0x3B2, 3, "", "That is locked." ) );
					return true;
				}
			}
			else 
			{
				return false;
			}
		}

		public override void OnTelekinesis( Mobile from )
		{
			if ( CheckLocked( from ) )
			{
				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
				Effects.PlaySound( Location, Map, 0x1F5 );
				return;
			}

			base.OnTelekinesis( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( CheckLocked( from ) )
				return;

			base.OnDoubleClick( from );
		}

		public override void OnSnoop( Mobile from )
		{
			if ( CheckLocked( from ) )
				return;

			base.OnSnoop( from );
		}

		public virtual void LockPick( Mobile from )
		{
			Locked = false;
			Picker = from;
		}
	}
}
