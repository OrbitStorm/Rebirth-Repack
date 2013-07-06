using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseExplosionPotion : BasePotion
	{
		public abstract int MinDamage { get; }
		public abstract int MaxDamage { get; }

		public override bool RequireFreeHand{ get{ return false; } }

		private static bool InstantExplosion = false; // Should explosion potions explode on impact?
		private const int   ExplosionRange   = 3;     // How long is the blast radius?

		public BaseExplosionPotion( PotionEffect effect ) : base( 0xF0D, effect )
		{
		}

		public BaseExplosionPotion( Serial serial ) : base( serial )
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

		public virtual object FindParent( Mobile from )
		{
			Mobile m = this.HeldBy;

			if ( m != null && m.Holding == this )
				return m;

			object obj = this.RootParent;

			if ( obj != null )
				return obj;

			if ( Map == Map.Internal )
				return from;

			return this;
		}

		private Timer m_Timer;

		private ArrayList m_Users;

		public override void Drink( Mobile from )
		{
			ThrowTarget targ = from.Target as ThrowTarget;

			if ( targ != null && targ.Potion == this )
				return;

			from.RevealingAction();

			if ( m_Users == null )
				m_Users = new ArrayList();

			if ( !m_Users.Contains( from ) )
				m_Users.Add( from );

			from.Target = new ThrowTarget( this );
			if ( m_Timer == null || !m_Timer.Running )
			{
				from.SendLocalizedMessage( 500236 ); // You should throw it now!
				int val = 3 + Utility.Random( 4 );
				m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), val+1, new TimerStateCallback( Detonate_OnTick ), new object[]{ from, val } );
			}
		}

		private void Detonate_OnTick( object state )
		{
			if ( Deleted )
				return;

			object[] states = (object[])state;
			Mobile from = (Mobile)states[0];
			int timer = (int)states[1];

			object parent = FindParent( from );

			if ( timer <= 0 )
			{
				Point3D loc = this.GetWorldLocation();
				Map map = this.Map;

				if ( parent is Item && parent != this )
				{
					Item item = (Item)parent;

					loc = item.GetWorldLocation();
					map = item.Map;
				}
				else if ( parent is Mobile )
				{
					Mobile m = (Mobile)parent;

					loc = m.Location;
					map = m.Map;
				}

				Explode( from, loc, map );
			}
			else
			{
				if ( parent is Item )
					((Item)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );
				else if ( parent is Mobile )
					((Mobile)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );

				states[1] = timer - 1;
			}
		}

		private void Reposition_OnTick( object state )
		{
			if ( Deleted )
				return;

			object[] states = (object[])state;
			Mobile from = (Mobile)states[0];
			IPoint3D p = (IPoint3D)states[1];
			Map map = (Map)states[2];

			Point3D loc = new Point3D( p );

			if ( InstantExplosion )
				Explode( from, loc, map );
			else
				MoveToWorld( loc, map );
		}

		private class ThrowTarget : Target
		{
			private BaseExplosionPotion m_Potion;

			public BaseExplosionPotion Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseExplosionPotion potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;

				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;

				Map map = from.Map;

				if ( map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );

				from.RevealingAction();

				IEntity to;
				Point3D pt = new Point3D( p );

				if ( p is Mobile )
					to = (Mobile)p;
				else
					to = new Entity( Serial.Zero, pt, map );

				Effects.SendMovingEffect( from, to, m_Potion.ItemID & 0x3FFF, 7, 0, false, false, m_Potion.Hue, 0 );

				m_Potion.Internalize();
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( m_Potion.Reposition_OnTick ), new object[]{ from, pt, map } );
			}
		}

		private static void AddPotions( Item pack, ArrayList list )
		{
			if ( pack == null )
				return;

			for(int i=0;i<pack.Items.Count;i++)
			{
				Item item = (Item)pack.Items[i];
				if ( item is BaseExplosionPotion && Utility.RandomBool() )
					list.Add( item );
				else if ( item is Container && Utility.Random( 4 ) == 0 )
					AddPotions( item, list );
			}
		}

		public void Explode( Mobile from, Point3D loc, Map map )
		{
			if ( Deleted )
				return;

			for ( int i = 0; m_Users != null && i < m_Users.Count; ++i )
			{
				Mobile m = (Mobile)m_Users[i];
				ThrowTarget targ = m.Target as ThrowTarget;

				if ( targ != null && targ.Potion == this )
					Target.Cancel( m );
			}

			if ( map == null )
				return;

			Effects.PlaySound( loc, map, 0x207 );
			Effects.SendLocationEffect( loc, map, 0x36BD, 20 );

			IPooledEnumerable eable = map.GetObjectsInRange( loc, ExplosionRange );
			ArrayList toExplode = new ArrayList();
			foreach ( object o in eable )
			{
				if ( o is Mobile )
				{
					toExplode.Add( o );
					AddPotions( ((Mobile)o).Backpack, toExplode );
				}
				else if ( o is Item )
				{
					if ( o is BaseExplosionPotion && o != this )
						toExplode.Add( o );
					else if ( ((Item)o).Items.Count > 0 )
						AddPotions( (Item)o, toExplode );
				}
			}

			eable.Free();

			int min = Scale( from, MinDamage );
			int max = Scale( from, MaxDamage );

			for ( int j = 0; j < toExplode.Count; j++ )
			{
				object o = toExplode[j];

				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;

					int dist = (int)m.GetDistanceToSqrt( loc );
					if ( dist > ExplosionRange )
						continue;

					if ( from == null || from.CanBeHarmful( m, false ) )
					{
						if ( from != null )
							from.DoHarmful( m );
						m.Damage( (int)( Utility.RandomMinMax( min, max ) * 3.0/4.0 ) );
					}
				}
				else if ( o is BaseExplosionPotion )
				{
					BaseExplosionPotion pot = (BaseExplosionPotion)o;

					//pot.Explode( from, false, pot.GetWorldLocation(), pot.Map );
					if ( ( pot.m_Timer == null || !pot.m_Timer.Running ) && !pot.Deleted )
					{
						Point3D pp = pot.GetWorldLocation();
						int x, y, z;
						double val;
						x = pp.X - loc.X;
						y = pp.Y - loc.Y;
						z = pp.Z - loc.Z;

						if ( x == 0 && y == 0 && z == 0 )
						{
							val = 0;
						}
						else
						{
							val = Math.Sqrt( x*x + y*y );
							val = Math.Sqrt( val*val + z*z );
						}

						if ( (int)val <= ExplosionRange )
						{
							val += Utility.Random( 4 );
							if ( val < 1 )
								val = 0;
					
							pot.m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), ((int)val)+1, new TimerStateCallback( pot.Detonate_OnTick ), new object[]{from, ((int)val)} );
						}
					}
				}
			}

			Delete();
		}
	}
}
