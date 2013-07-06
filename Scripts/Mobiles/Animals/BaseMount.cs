using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public abstract class BaseMount : BaseCreature, IMount
	{
		private Mobile m_Rider;
		private Item m_InternalItem;

		public virtual bool AllowMaleRider{ get{ return true; } }
		public virtual bool AllowFemaleRider{ get{ return true; } }

		public BaseMount( string name, int bodyID, int itemID, AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed ) : base ( aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed )
		{
			Name = name;
			Body = bodyID;

			m_InternalItem = new MountItem( this, itemID );
		}

		public BaseMount( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Rider );
			writer.Write( m_InternalItem );
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get
			{
				return base.Hue;
			}
			set
			{
				base.Hue = value;

				if ( m_InternalItem != null )
					m_InternalItem.Hue = value;
			}
		}

		private byte m_Steps;
		private byte m_Accel;

		public virtual void OnRunningStep( Mobile rider )
		{
			m_Steps += 2;
			if ( m_Steps % 30 == 0 )
			{
				m_Steps = 0;
				Stam--;
			}
		}

		public virtual void OnWalkingStep( Mobile rider )
		{
			m_Steps ++;
			if ( m_Steps % 30 == 0 )
			{
				m_Steps = 0;
				Stam--;
			}
		}

		private const double LowSpeedSteps = 20.0;
		private const double AccelSteps = 15.0;
		private const int MaxSteps = (int)( LowSpeedSteps + AccelSteps );
		public virtual double GetSpeedScalar( Mobile rider, Direction newDir )
		{
			bool isRunning = (newDir&Direction.Running) != 0;

			if ( (rider.Direction&Direction.Running) == 0 && isRunning )
				m_Accel = (byte)((LowSpeedSteps+AccelSteps)/2); // have to re-accellerate when going from walking to running

			if ( (rider.Direction&Direction.Mask) == (newDir&Direction.Mask) )
			{// going strait
				if ( rider.LastMoveTime != DateTime.MinValue && DateTime.Now - rider.LastMoveTime < TimeSpan.FromSeconds( 0.5 ) )
				{//continuing on...
					if ( m_Accel < MaxSteps )
					{
						m_Accel ++;
						if ( m_Accel < MaxSteps && !isRunning )
							m_Accel++;
					}
				}
				else
				{//paused too long
					m_Accel = 0;
				}

				if ( m_Accel < LowSpeedSteps )
					return 2.0;
				else
					return 1.0 + ( (AccelSteps - (m_Accel - LowSpeedSteps)) / AccelSteps );
			}
			else
			{
				if ( m_Accel > 2 )
					m_Accel -= 2;
				else
					m_Accel = 0;

				return 1.0;
			}
		}

		public override bool OnBeforeDeath()
		{
			Rider = null;

			return base.OnBeforeDeath();
		}

		public override void OnAfterDelete()
		{
			if ( m_InternalItem != null )
				m_InternalItem.Delete();

			m_InternalItem = null;

			base.OnAfterDelete();
		}

		public override void OnDelete()
		{
			Rider = null;

			base.OnDelete();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Rider = reader.ReadMobile();
					m_InternalItem = reader.ReadItem();

					if ( m_InternalItem == null )
						Delete();

					break;
				}
			}

			if ( !Controled && Rider != null )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( DismountNow ) );
		}

		private void DismountNow()
		{
			if ( Rider != null && Rider.AccessLevel == AccessLevel.Player  )
				Rider = null;
		}

		public virtual void OnDisallowedRider( Mobile m )
		{
			m.SendAsciiMessage( "You may not ride this creature." );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsDeadPet )
				return; // TODO: Should there be a message here?

			if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				if ( Core.AOS ) // You cannot ride a mount in your current form.
					PrivateOverheadMessage( Network.MessageType.Regular, 0x3B2, 1062061, from.NetState );
				else
					from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.

				return;
			}

			if ( !from.CanBeginAction( typeof( BaseMount ) ) )
			{
				from.SendLocalizedMessage( 1040024 ); // You are still too dazed from being knocked off your mount to ride!
				return;
			}

			if ( from.Mounted )
			{
				from.SendLocalizedMessage( 1005583 ); // Please dismount first.
				return;
			}

			if ( from.Female ? !AllowFemaleRider : !AllowMaleRider )
			{
				OnDisallowedRider( from );
				return;
			}

			if ( this.Combatant != null && this.Combatant != this && !this.Combatant.Deleted && this.Combatant.Alive && this.InRange( this.Combatant, 3 ) )
			{
				from.SendAsciiMessage( "Your mount is too angry to ride right now." );
				return;
			}

			foreach ( Mobile m in this.GetMobilesInRange( 2 ) )
			{
				if ( m.Combatant == this )
				{
					from.SendAsciiMessage( "Your mount is too angry to ride right now." );
					return;
				}
			}

			if ( from.InRange( this, 1 ) )
			{
				if ( ( Controled && ControlMaster == from ) || ( Summoned && SummonMaster == from) || from.AccessLevel >= AccessLevel.GameMaster )
				{
					Rider = from;
				}
				else if ( !Controled && !Summoned )
				{
					from.SendLocalizedMessage( 501263 ); // That mount does not look broken! You would have to tame it to ride it.
				}
				else
				{
					from.SendLocalizedMessage( 501264 ); // This isn't your mount; it refuses to let you ride.
				}
			}
			else
			{
				from.SendLocalizedMessage( 500206 ); // That is too far away to ride.
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ItemID
		{
			get
			{
				if ( m_InternalItem != null )
					return m_InternalItem.ItemID;
				else
					return 0;
			}
			set
			{
				if ( m_InternalItem != null )
					m_InternalItem.ItemID = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual Mobile Rider
		{
			get
			{
				return m_Rider;
			}
			set
			{
				if ( m_Rider != value )
				{
					m_Accel = 0;

					if ( value == null )
					{
						Point3D loc = m_Rider.Location;
						Map map = m_Rider.Map;

						if ( map == null || map == Map.Internal )
						{
							loc = m_Rider.LogoutLocation;
							map = m_Rider.LogoutMap;
						}

						Direction = m_Rider.Direction;
						Location = loc;
						Map = map;
						if ( m_InternalItem != null )
							m_InternalItem.Internalize();
						m_Rider = null;
					}
					else
					{
						if ( m_Rider != null )
							Dismount( m_Rider );
						Dismount( value );

						m_Rider = value;
						if ( m_InternalItem != null )
							m_Rider.AddItem( m_InternalItem );
						m_Rider.Direction = this.Direction;
						Internalize();
					}
				}
			}
		}

		public static void Dismount( Mobile m )
		{
			IMount mount = m.Mount;

			if ( mount != null )
				mount.Rider = null;
		}

        public virtual void OnRiderDamaged(int amount, Mobile from, bool willKill)
        {
        }
	}

	public class MountItem : BaseItem, IMountItem
	{
		private BaseMount m_Mount;

		public MountItem( BaseMount mount, int itemID ) : base( itemID )
		{
			Layer = Layer.Mount;
			Movable = false;

			m_Mount = mount;
		}

		public MountItem( Serial serial ) : base( serial )
		{
		}

		public override void OnAfterDelete()
		{
			if ( m_Mount != null )
				m_Mount.Delete();

			m_Mount = null;

			base.OnAfterDelete();
		}

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			if ( m_Mount != null )
				m_Mount.Rider = null;

			return DeathMoveResult.RemainEquiped;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Mount );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Mount = reader.ReadMobile() as BaseMount;

					if ( m_Mount == null )
						Delete();

					break;
				}
			}
		}

		public IMount Mount
		{
			get
			{
				return m_Mount;
			}
		}
	}
}
