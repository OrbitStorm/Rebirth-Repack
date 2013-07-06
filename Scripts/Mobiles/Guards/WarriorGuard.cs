using System;
using System.Collections; using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	public class WeakWarriorGuard : WarriorGuard
	{
		[Constructable]
		public WeakWarriorGuard() : this( null )
		{
		}

		public WeakWarriorGuard( Mobile target ) : base( target )
		{
			InitStats( 100, 100, 100 );
		}

		public override void InitWeapon()
		{
			Halberd weapon = new Halberd();
			weapon.Movable = false;
			weapon.LootType = LootType.Newbied;
			weapon.Quality = CraftQuality.Exceptional;
			AddItem( weapon );
		}

		public WeakWarriorGuard( Serial serial ) : base( serial )
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

			switch ( version )
			{
				case 0:
					break;
			}
		}
	}

	public class WarriorGuard : BaseGuard
	{
		private Timer m_AttackTimer, m_IdleTimer;
		private bool m_IsAutoGuard;
		private Mobile m_Focus;

		[Constructable]
		public WarriorGuard() : this( null )
		{
		}

		public WarriorGuard( Mobile target ) : base( target )
		{
			m_IsAutoGuard = target != null;
			
			InitStats( 2500, 1000, 1000 );

			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );

				switch( Utility.Random( 2 ) )
				{
					case 0: AddItem( new LeatherSkirt() ); break;
					case 1: AddItem( new LeatherShorts() ); break;
				}

				switch( Utility.Random( 5 ) )
				{
					case 0: AddItem( new FemaleLeatherChest() ); break;
					case 1: AddItem( new FemaleStuddedChest() ); break;
					case 2: AddItem( new LeatherBustierArms() ); break;
					case 3: AddItem( new StuddedBustierArms() ); break;
					case 4: AddItem( new FemalePlateChest() ); break;
				}
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );

				AddItem( new PlateChest() );
				AddItem( new PlateArms() );
				AddItem( new PlateGorget() );
				AddItem( new PlateLegs() );

				switch( Utility.Random( 3 ) )
				{
					case 0: AddItem( new Doublet( Utility.RandomNondyedHue() ) ); break;
					case 1: AddItem( new Tunic( Utility.RandomNondyedHue() ) ); break;
					case 2: AddItem( new BodySash( Utility.RandomNondyedHue() ) ); break;
				}
			}

			Item hair = new Item( Utility.RandomList( 0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A ) );
			hair.Hue = Utility.RandomHairHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );

			if( Utility.RandomBool() && !this.Female )
			{
				Item beard = new Item( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ) );

				beard.Hue = hair.Hue;
				beard.Layer = Layer.FacialHair;
				beard.Movable = false;

				AddItem( beard );
			}

			InitWeapon();

			Container pack = new Backpack();
			pack.Movable = false;
			pack.DropItem( new Gold( 10, 50 ) );
			AddItem( pack );

			Skills[SkillName.Tactics].Base = 100.0;
			Skills[SkillName.Swords].Base = 100.0;
			Skills[SkillName.MagicResist].Base = 100.0;

			this.NextCombatTime = DateTime.Now + TimeSpan.FromSeconds( 0.25 );
			this.Focus = target;
		}

		public virtual void InitWeapon()
		{
			Halberd weapon = new Halberd();
			weapon.Movable = false;
			weapon.LootType = LootType.Newbied;
			weapon.Quality = CraftQuality.Exceptional;
			weapon.MinDamage = 100;
			weapon.MaxDamage = 500;
			weapon.Speed = 100;
			AddItem( weapon );
		}

		public WarriorGuard( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override Mobile Focus
		{
			get
			{
				return m_Focus;
			}
			set
			{
				if ( Deleted )
					return;

				Mobile oldFocus = m_Focus;

				if ( oldFocus != value )
				{
					m_Focus = value;

					if ( value != null )
						this.AggressiveAction( value );

					FocusMob = Combatant = value;

					if ( oldFocus != null && !oldFocus.Alive )
					{
						Say( "Thou hast suffered thy punishment, scoundrel." );
						this.AIObject.Action = ActionType.Wander;
					}

					if ( value != null )
					{
						Say( 500131 ); // Thou wilt regret thine actions, swine!
						if ( this.AIObject != null )
							this.AIObject.Action = ActionType.Combat;
					}

					if ( m_AttackTimer != null )
					{
						m_AttackTimer.Stop();
						m_AttackTimer = null;
					}

					if ( m_IdleTimer != null )
					{
						m_IdleTimer.Stop();
						m_IdleTimer = null;
					}

					if ( m_Focus != null )
					{
						m_AttackTimer = new AttackTimer( this );
						m_AttackTimer.Start();
						((AttackTimer)m_AttackTimer).DoOnTick();
					}
					else if ( m_IsAutoGuard )
					{
						m_IdleTimer = new IdleTimer( this );
						m_IdleTimer.Start();
					}
				}
				else if ( m_Focus == null && m_IdleTimer == null && m_IsAutoGuard )
				{
					m_IdleTimer = new IdleTimer( this );
					m_IdleTimer.Start();
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			
			writer.Write( m_IsAutoGuard );

			writer.Write( m_Focus );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_IsAutoGuard = reader.ReadBool();
					goto case 0;
				}	
				case 0:
				{
					if ( version < 1 )
						m_IsAutoGuard = true;
					m_Focus = reader.ReadMobile();

					if ( m_Focus != null )
					{
						m_AttackTimer = new AttackTimer( this );
						m_AttackTimer.Start();
					}
					else if ( m_IsAutoGuard )
					{
						m_IdleTimer = new IdleTimer( this );
						m_IdleTimer.Start();
					}

					break;
				}
			}
		}

		protected override bool OnMove( Direction d )
		{
			if ( !(Region is Regions.GuardedRegion) )
			{
				if ( m_IdleTimer == null )
					m_IdleTimer = new IdleTimer( this );
				if ( !m_IdleTimer.Running )
					m_IdleTimer.Start();
			}

			return base.OnMove (d);
		}


		public override void OnAfterDelete()
		{
			if ( m_AttackTimer != null )
			{
				m_AttackTimer.Stop();
				m_AttackTimer = null;
			}

			if ( m_IdleTimer != null )
			{
				m_IdleTimer.Stop();
				m_IdleTimer = null;
			}

			base.OnAfterDelete();
		}

		private class AttackTimer : Timer
		{
			private WarriorGuard m_Owner;

			public AttackTimer( WarriorGuard owner ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 0.1 ) )
			{
				m_Owner = owner;
				Priority = TimerPriority.FiftyMS;
			}

			public void DoOnTick()
			{
				OnTick();
			}

			protected override void OnTick()
			{
				if ( m_Owner.Deleted )
				{
					Stop();
					return;
				}

				m_Owner.Criminal = false;
				m_Owner.Kills = 0;
				m_Owner.Stam = m_Owner.StamMax;

				Mobile target = m_Owner.Focus;

				if ( target == null || target.Deleted || !target.Alive || !m_Owner.CanBeHarmful( target ) )	
				{
					if ( target != null && ( !target.Alive || target.Deleted ) )
					{
						if ( !target.Player && target.Corpse != null && !target.Corpse.Deleted )
							target.Corpse.Delete();

						target.RemoveAggressor( m_Owner );
						target.RemoveAggressed( m_Owner );
						m_Owner.RemoveAggressor( target );
						m_Owner.RemoveAggressed( target );
					}
					Stop();
					m_Owner.Focus = m_Owner.Combatant = null;
					m_Owner.AIObject.Action = ActionType.Wander;
					return;
				}

				if ( m_Owner.Combatant != target )
					m_Owner.Combatant = target;

				if ( !m_Owner.InRange( target, 30 ) || !m_Owner.CanSee( target ) )
				{
					m_Owner.Focus = m_Owner.Combatant = null;
					m_Owner.AIObject.Action = ActionType.Wander;
				}
				else if ( !m_Owner.InRange( target, 5 ) || !m_Owner.InLOS( target ) )
				{
					TeleportTo( target );
				}
				/*else if ( !m_Owner.InRange( target, 1 ) )
				{
					if ( !m_Owner.Move( m_Owner.GetDirectionTo( target ) | Direction.Running ) )
						TeleportTo( target );
				}*/
			}

			private void TeleportTo( Mobile target )
			{
				Point3D from = m_Owner.Location;
				Point3D to = target.Location;

				m_Owner.Location = to;

				Effects.SendLocationParticles( EffectItem.Create( from, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				Effects.SendLocationParticles( EffectItem.Create(   to, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

				m_Owner.PlaySound( 0x1FE );
			}
		}

		private class IdleTimer : Timer
		{
			private WarriorGuard m_Owner;
			private int m_Stage;

			public IdleTimer( WarriorGuard owner ) : base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.5 ) )
			{
				m_Owner = owner;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if ( m_Owner.Deleted || ( m_Owner.m_IsAutoGuard && m_Owner.Region is Regions.GuardedRegion ) )
				{
					Stop();
					return;
				}

				//if ( (m_Stage % 4) == 0 || !m_Owner.Move( m_Owner.Direction ) )
				//	m_Owner.Direction = (Direction)Utility.Random( 8 );

				if ( m_Stage++ > 16 )
				{
					Effects.SendLocationParticles( EffectItem.Create( m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
					m_Owner.PlaySound( 0x1FE );

					m_Owner.Delete();
				}
			}
		}
	}
}

