using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Regions;
using Server.Mobiles;
using Server.Targeting;
//using Server.Engines.PartySystem;
using Server.Misc;

namespace Server.Spells
{
	public enum TravelType
	{
		Recall,
		Mark,
		Gate
	}

	public class SpellHelper
	{
		private static TimeSpan AosDamageDelay = TimeSpan.FromSeconds( 1.0 );
		private static TimeSpan OldDamageDelay = TimeSpan.FromSeconds( 1.0 );

		public static TimeSpan GetDamageDelayForSpell( Spell sp )
		{
			if ( !sp.DelayedDamage )
				return TimeSpan.Zero;

			return ( Core.AOS ? AosDamageDelay : OldDamageDelay );
		}

		public static bool CheckMulti( Point3D p, Map map )
		{
			return CheckMulti( p, map, true );
		}

		public static bool CheckMulti( Point3D p, Map map, bool houses )
		{
			if ( map == null || map == Map.Internal )
				return false;

			Sector sector = map.GetSector( p.X, p.Y );

			for ( int i = 0; i < sector.Multis.Count; ++i )
			{
				BaseMulti multi = (BaseMulti)sector.Multis[i];

				if ( multi is BaseHouse )
				{
					if ( houses && ((BaseHouse)multi).IsInside( p, 16 ) )
						return true;
				}
				else if ( multi.Contains( p ) )
				{
					return true;
				}
			}

			return false;
		}

		public static void Turn( Mobile from, object to )
		{
			IPoint3D target = to as IPoint3D;

			if ( target == null )
				return;

			if ( target is Item )
			{
				Item item = (Item)target;

				if ( item.RootParent != from )
					from.Direction = from.GetDirectionTo( item.GetWorldLocation() );
			}
			else if ( from != target )
			{
				from.Direction = from.GetDirectionTo( target );
			}
		}

		private static TimeSpan CombatHeatDelay = TimeSpan.FromSeconds( 30.0 );
		private static bool RestrictTravelCombat = true;

		public static bool CheckCombat( Mobile m )
		{
			if ( !RestrictTravelCombat )
				return false;

			for ( int i = 0; i < m.Aggressed.Count; ++i )
			{
				AggressorInfo info = (AggressorInfo)m.Aggressed[i];

				if ( info.Defender.Player && (DateTime.Now - info.LastCombatTime) < CombatHeatDelay )
					return true;
			}

			return false;
		}

		public static bool AdjustField( ref Point3D p, Map map, int height, bool mobsBlock )
		{
			if ( map == null )
				return false;

			for ( int offset = 0; offset < 10; ++offset )
			{
				Point3D loc = new Point3D( p.X, p.Y, p.Z - offset );

				if ( map.CanFit( loc, height, true, mobsBlock ) )
				{
					p = loc;
					return true;
				}
			}

			return false;
		}

		public static Point3D GetSurfaceTop( object o )
		{
			if ( o is Item )
			{
				return ((Item)o).GetSurfaceTop();
			}
			else if ( o is StaticTarget )
			{
				StaticTarget t = (StaticTarget)o;
				int z = t.Z;

				if ( (t.Flags & TileFlag.Surface) == 0 )
					z -= TileData.ItemTable[t.ItemID & 0x3FFF].CalcHeight;

				return new Point3D( t.X, t.Y, z );
			}
			else
			{
				return new Point3D( (IPoint3D)o );
			}
		}

		public static void GetSurfaceTop( ref IPoint3D p )
		{
			if ( p is Item )
			{
				p = ((Item)p).GetSurfaceTop();
			}
			else if ( p is StaticTarget )
			{
				StaticTarget t = (StaticTarget)p;
				int z = t.Z;

				if ( (t.Flags & TileFlag.Surface) == 0 )
					z -= TileData.ItemTable[t.ItemID & 0x3FFF].CalcHeight;

				p = new Point3D( t.X, t.Y, z );
			}
		}

		public static bool AddStatOffset( Mobile m, StatType type, int offset, TimeSpan duration )
		{
			if ( offset > 0 )
				return AddStatBonus( m, m, type, offset, duration );
			else if ( offset < 0 )
				return AddStatCurse( m, m, type, -offset, duration );

			return true;
		}

		public static bool AddStatBonus( Mobile caster, Mobile target, StatType type )
		{
			return AddStatBonus( caster, target, type, GetOffset( caster, target, type, false ), GetDuration( caster, target ) );
		}

		public static bool AddStatBonus( Mobile caster, Mobile target, StatType type, int bonus, TimeSpan duration )
		{
			int offset = bonus;
			string name = String.Format( "[Magic] {0} Offset", type );

			StatMod mod = target.GetStatMod( name );

			if ( mod != null && mod.Offset < 0 )
			{
				target.AddStatMod( new StatMod( type, name, mod.Offset + offset, duration ) );
				return true;
			}
			else if ( mod == null || mod.Offset < offset )
			{
				target.AddStatMod( new StatMod( type, name, offset, duration ) );
				return true;
			}

			return false;
		}

		public static bool AddStatCurse( Mobile caster, Mobile target, StatType type )
		{
			return AddStatCurse( caster, target, type, GetOffset( caster, target, type, true ), GetDuration( caster, target ) );
		}

		public static bool AddStatCurse( Mobile caster, Mobile target, StatType type, int curse, TimeSpan duration )
		{
			int offset = -curse;
			string name = String.Format( "[Magic] {0} Offset", type );

			StatMod mod = target.GetStatMod( name );

			if ( mod != null && mod.Offset > 0 )
			{
				target.AddStatMod( new StatMod( type, name, mod.Offset + offset, duration ) );
				return true;
			}
			else if ( mod == null || mod.Offset > offset )
			{
				target.AddStatMod( new StatMod( type, name, offset, duration ) );
				return true;
			}

			return false;
		}

		public static bool HasStatEffect( Mobile target, StatType type )
		{
			if ( type == StatType.All )
			{
				return HasStatEffect( target, StatType.Dex ) && HasStatEffect( target, StatType.Int ) && HasStatEffect( target, StatType.Str ) ;
			}
			else
			{
				StatMod mod = target.GetStatMod( String.Format( "[Magic] {0} Offset", type ) );

				return mod != null;
			}
		}

		public static TimeSpan GetDuration( Mobile caster, Mobile target )
		{
			if ( caster != null )
				return TimeSpan.FromSeconds( 6 * ( caster.Skills[SkillName.Magery].Value / 5 + 1 ) );
			else
				return TimeSpan.FromSeconds( 60 + Utility.Random( 30 ) );
		}

		private static bool m_DisableSkillCheck;

		public static bool DisableSkillCheck
		{
			get{ return m_DisableSkillCheck; }
			set{ m_DisableSkillCheck = value; }
		}

		public static int GetOffset( Mobile caster, Mobile target, StatType type, bool curse )
		{
			if ( caster != null )
				return 1 + (int)(caster.Skills[SkillName.Magery].Value * 0.1);
			else
				return 5 + Utility.Random( 5 );
		}

		public static Guild GetGuildFor( Mobile m )
		{
			Guild g = m.Guild as Guild;

			if ( g == null && m is BaseCreature )
			{
				BaseCreature c = (BaseCreature)m;
				m = c.ControlMaster;

				if ( m != null )
					g = m.Guild as Guild;

				if ( g == null )
				{
					m = c.SummonMaster;

					if ( m != null )
						g = m.Guild as Guild;
				}
			}

			return g;
		}

		public static bool ValidIndirectTarget( Mobile from, Mobile to )
		{
			return !to.Blessed;//from.CanBeHarmful( to );
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				0, -1,
				0,  1,
				1, -1,
				1,  0,
				1,  1
			};

		public static void Summon( BaseCreature creature, Mobile caster, int sound, TimeSpan duration, bool scaleDuration, bool scaleStats )
		{
			Map map = caster.Map;

			if ( map == null )
				return;

			double scale = 1.0 + ((caster.Skills[SkillName.Magery].Value - 100.0) / 200.0);

			if ( scaleDuration )
				duration = TimeSpan.FromSeconds( duration.TotalSeconds * scale );

			if ( scaleStats )
			{
				creature.RawStr = (int)(creature.RawStr * scale);
				creature.Hits = creature.HitsMax;

				creature.RawDex = (int)(creature.RawDex * scale);
				creature.Stam = creature.StamMax;

				creature.RawInt = (int)(creature.RawInt * scale);
				creature.Mana = creature.ManaMax;
			}

			int offset = Utility.Random( 8 ) * 2;

			for ( int i = 0; i < m_Offsets.Length; i += 2 )
			{
				int x = caster.X + m_Offsets[(offset + i) % m_Offsets.Length];
				int y = caster.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

				if ( map.CanFit( x, y, caster.Z, 16 ) )
				{
					BaseCreature.Summon( creature, caster, new Point3D( x, y, caster.Z ), sound, duration );
					return;
				}
				else
				{
					int z = map.GetAverageZ( x, y );

					if ( map.CanFit( x, y, z, 16 ) )
					{
						BaseCreature.Summon( creature, caster, new Point3D( x, y, z ), sound, duration );
						return;
					}
				}
			}

			creature.Delete();
			caster.SendLocalizedMessage( 501942 ); // That location is blocked.
		}

		public static bool CheckTravel( Mobile caster, Point3D to, Map map, TravelType type )
		{
			if ( IsInvalid( map, to ) )
				return false;

            DungeonRegion fdc = caster.Region as DungeonRegion;
            DungeonRegion fdt = Region.Find(to, map) as DungeonRegion;

			switch ( type )
			{
				case TravelType.Recall:
				{
					if ( ( fdc != null && !fdc.CanRecall ) || ( fdt != null && !fdt.CanRecall ) )
					{
						caster.SendAsciiMessage( "You cannot seem to get a fix on your destination." );
						return false;
					}
					else if ( IsWindLoc( to ) && caster.Skills[SkillName.Magery].Value < 72.0 )
					{
						caster.SendAsciiMessage( "You are not worthy of entrance to the city of Wind." );
						return false;
					}
					else
					{
						return true;
					}
				}

				case TravelType.Gate:
				{
					if ( ( fdc != null && !fdc.CanGate ) || ( fdt != null && !fdt.CanGate ) )
					{
						caster.SendAsciiMessage( "You cannot seem to get a fix on your destination." );
						return false;
					}
					else if ( IsWindLoc( to ) || IsWindLoc( caster.Location ) )
					{
						caster.SendAsciiMessage( "You cannot Gate Travel to that location." );
						return false;
					}
					else
					{
						return true;
					}
				}

				case TravelType.Mark:
				{
					if ( fdc != null && !fdc.CanRecall && !fdc.CanGate )
					{
						caster.SendAsciiMessage( "The spell seems to have no effect." );
						return false;
					}
					else
					{
						return true;
					}
				}

				default:
				{
					return true;
				}
			}
		}

		public static bool IsWindLoc( Point3D loc )
		{
			int x = loc.X, y = loc.Y;

			return ( x >= 5120 && y >= 0 && x < 5376 && y < 256 );
		}

		public static bool IsInvalid( Map map, Point3D loc )
		{
			if ( map == null || map == Map.Internal )
				return true;

			int x = loc.X, y = loc.Y;

			return ( x < 0 || y < 0 || x >= map.Width || y >= map.Height );
		}

		//towns
		public static bool IsTown( IPoint3D loc, Mobile caster )
		{
			if ( loc is Item )
				loc = ((Item)loc).GetWorldLocation();

			return IsTown( new Point3D( loc ), caster );
		}

		public static bool IsTown( Point3D loc, Mobile caster )
		{
			Map map = caster.Map;

			if ( map == null )
				return false;

			GuardedRegion reg = Region.Find( loc, map ) as GuardedRegion;

			return ( reg != null && !reg.IsDisabled() );
		}

		public static bool CheckTown( IPoint3D loc, Mobile caster )
		{
			if ( loc is Item )
				loc = ((Item)loc).GetWorldLocation();

			return CheckTown( new Point3D( loc ), caster );
		}

		public static bool CheckTown( Point3D loc, Mobile caster )
		{
			/*if ( IsTown( loc, caster ) )
			{
				caster.SendLocalizedMessage( 500946 ); // You cannot cast this in town!
				return false;
			}*/

			return true;
		}


		//magic reflection
		public static bool CheckReflect( int circle, Mobile caster, ref Mobile target )
		{
			return CheckReflect( circle, ref caster, ref target );
		}

		public static bool CheckReflect( int circle, ref Mobile caster, ref Mobile target )
		{
			bool reflect = ( target.MagicDamageAbsorb > 0 );
			if ( target is BaseCreature )
				((BaseCreature)target).CheckReflect( caster, ref reflect );

			if ( reflect )
			{
				target.FixedEffect( 0x37B9, 10, 5 );

				if ( target.MagicDamageAbsorb > 0 )
					target.MagicDamageAbsorb--;

				Mobile temp = caster;
				caster = target;
				target = temp;
			}

			return reflect;
		}

		public static void Damage( Spell spell, Mobile target, double damage )
		{
			TimeSpan ts = GetDamageDelayForSpell( spell );

			Damage( ts, target, spell.Caster, damage );
		}

		public static void Damage( TimeSpan delay, Mobile target, double damage )
		{
			Damage( delay, target, null, damage );
		}

		public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage )
		{
			if ( !target.Player && !target.Body.IsHuman && !Core.AOS )
				damage *= 2.0; // Double magery damage to monsters/animals if not AOS
			damage = (damage + 1.0) / 2.0;

			if ( delay == TimeSpan.Zero )
			{
				if ( target is BaseCreature && from != null )
					((BaseCreature)target).OnDamagedBySpell( from );
				target.Damage( (int) damage, from );
			}
			else
			{
				new SpellDamageTimer( target, from, (int)damage, delay ).Start();
			}
		}

		public static void Damage( Spell spell, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			TimeSpan ts = GetDamageDelayForSpell( spell );

			Damage( ts, target, spell.Caster, damage, phys, fire, cold, pois, nrgy );
		}

		public static void Damage( Spell spell, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa )
		{
			TimeSpan ts = GetDamageDelayForSpell( spell );

			Damage( ts, target, spell.Caster, damage, phys, fire, cold, pois, nrgy, dfa );
		}

		public static void Damage( TimeSpan delay, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			Damage( delay, target, null, damage, phys, fire, cold, pois, nrgy );
		}

		public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			Damage( delay, target, from, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard );
		}

		public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa )
		{
			if ( damage <= 0 )
				return;

			if ( !target.Player && !target.Body.IsHuman && !Core.AOS )
				damage *= 2.0; // Double magery damage to monsters/animals if not AOS
			damage = (damage + 1.0) / 2.0;

			if ( delay == TimeSpan.Zero )
			{
				if ( target is BaseCreature && from != null )
					((BaseCreature)target).OnDamagedBySpell( from );
				WeightOverloading.DFA = dfa;
				AOS.Damage( target, from, (int)damage, phys, fire, cold, pois, nrgy );
				WeightOverloading.DFA = DFAlgorithm.Standard;
			}
			else
			{
				new SpellDamageTimerAOS( target, from, (int)damage, phys, fire, cold, pois, nrgy, delay, dfa ).Start();
			}
		}

		private class SpellDamageTimer : Timer
		{
			private Mobile m_Target, m_From;
			private int m_Damage;

			public SpellDamageTimer( Mobile target, Mobile from, int damage, TimeSpan delay ) : base( delay )
			{
				m_Target = target;
				m_From = from;
				m_Damage = damage;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if ( m_Target is BaseCreature && m_From != null )
					((BaseCreature)m_Target).OnDamagedBySpell( m_From );
				m_Target.Damage( m_Damage, m_From );
			}
		}

		private class SpellDamageTimerAOS : Timer
		{
			private Mobile m_Target, m_From;
			private int m_Damage;
			private int m_Phys, m_Fire, m_Cold, m_Pois, m_Nrgy;
			private DFAlgorithm m_DFA;

			public SpellDamageTimerAOS( Mobile target, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy, TimeSpan delay, DFAlgorithm dfa ) : base( delay )
			{
				m_Target = target;
				m_From = from;
				m_Damage = damage;
				m_Phys = phys;
				m_Fire = fire;
				m_Cold = cold;
				m_Pois = pois;
				m_Nrgy = nrgy;
				m_DFA = dfa;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if ( m_Target is BaseCreature && m_From != null )
					((BaseCreature)m_Target).OnDamagedBySpell( m_From );

				WeightOverloading.DFA = m_DFA;
				AOS.Damage( m_Target, m_From, m_Damage, m_Phys, m_Fire, m_Cold, m_Pois, m_Nrgy );
				WeightOverloading.DFA = DFAlgorithm.Standard;
			}
		}
	}
}