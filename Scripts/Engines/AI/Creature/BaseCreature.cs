using System;
using System.Collections; using System.Collections.Generic;
using Server;
using Server.Regions;
using Server.Guilds;
using Server.Targeting;
using Server.Network;
using Server.Spells;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Mobiles
{
	/// <summary>
	/// Summary description for MobileAI.
	/// </summary>
	/// 
	public enum FightMode
	{
		None,			// Never focus on others
		Agressor,		// Only attack agressors
		Strongest,		// Attack the strongest (str)
		Smartest,		// Attack highest int (int)
		Weakest,		// Attack the weakest (hits)
		Fastest,		// Attack the Fastest (dex)
		Closest, 		// Attack the closest
		Evil,			// Only attack aggressor -or- "red"
	}

	public enum OrderType
	{
		None,			//When no order, let's roam
		Come,			//"(All/Name) come"  Summons all or one pet to your location.  
		Drop,			//"(Name) drop"  Drops its loot to the ground (if it carries any).  
		Follow,			//"(Name) follow"  Follows targeted being.  
						//"(All/Name) follow me"  Makes all or one pet follow you.  
		Friend,			//"(Name) friend"  Allows targeted player to confirm resurrection. 
		Guard,			//"(Name) guard"  Makes the specified pet guard you. Pets can only guard their owner. 
						//"(All/Name) guard me"  Makes all or one pet guard you.  
		Attack,			//"(All/Name) kill", 
						//"(All/Name) attack"  All or the specified pet(s) currently under your control attack the target. 
		Patrol,			//"(Name) patrol"  Roves between two or more guarded targets.  
		Release,		//"(Name) release"  Releases pet back into the wild (removes "tame" status). 
		Stay,			//"(All/Name) stay" All or the specified pet(s) will stop and stay in current spot. 
		Stop,			//"(All/Name) stop Cancels any current orders to attack, guard or follow.  
		Transfert		//"(Name) transfer" Transfers complete ownership to targeted player. 
	}

	[Flags]
	public enum FoodType
	{
		Meat			= 0x0001,
		FruitsAndVegies	= 0x0002,
		GrainsAndHay	= 0x0004,
		Fish			= 0x0008,
		Eggs			= 0x0010,
		Gold			= 0x0020
	}

	[Flags]
	public enum PackInstinct
	{
		None			= 0x0000,
		Canine			= 0x0001,
		Ostard			= 0x0002,
		Feline			= 0x0004,
		Arachnid		= 0x0008,
		Daemon			= 0x0010,
		Bear			= 0x0020,
		Equine			= 0x0040,
		Bull			= 0x0080
	}

	public enum ScaleType
	{
		Red,
		Yellow,
		Black,
		Green,
		White,
		Blue,
		All
	}

	public enum MeatType
	{
		Ribs,
		Bird,
		LambLeg
	}

	public enum HideType
	{
		Regular,
		Spined,
		Horned,
		Barbed
	}

	public enum PetLoyalty
	{
		None,
		Confused,
		ExtremelyUnhappy,
		RatherUnhappy,
		Unhappy,
		SomewhatContent,
		Content,
		Happy,
		RatherHappy,
		VeryHappy,
		ExtremelyHappy,
		WonderfullyHappy,

		Minimum = None,
		Maximum = WonderfullyHappy,
	}

	public class DamageStore : IComparable
	{
		public Mobile m_Mobile;
		public int m_Damage;
		public bool m_HasRight;

		public DamageStore( Mobile m, int damage )
		{
			m_Mobile = m;
			m_Damage = damage;
		}

		public int CompareTo( object obj )
		{
			DamageStore ds = (DamageStore)obj;

			return ds.m_Damage - m_Damage;
		}
	}

	public class BaseCreature : Mobile
	{
		private BaseAI	m_AI;					// THE AI
		
		private AIType	m_CurrentAI;			// The current AI
		private AIType	m_DefaultAI;			// The default AI

		private Mobile	m_FocusMob;				// Use focus mob instead of combatant, maybe we don't whan to fight
		private FightMode m_FightMode;			// The style the mob uses

		private int		m_iRangePerception;		// The view area
		private int		m_iRangeFight;			// The fight distance
       
		private bool	m_bDebugAI;				// Show debug AI messages

		private int		m_iTeam;				// Monster Team

		private double	m_dActiveSpeed;			// Timer speed when active
		private double	m_dPassiveSpeed;		// Timer speed when not active
		private double	m_dCurrentSpeed;		// The current speed, lets say it could be changed by something;

		private Point3D m_pHome;				// The home position of the creature, used by some AI
		private int		m_iRangeHome = 10;		// The home range of the creature

		ArrayList		m_arSpellAttack;		// List of attack spell/power
		ArrayList		m_arSpellDefense;		// Liste of defensive spell/power

		private bool		m_bControled;		// Is controled
		private Mobile		m_ControlMaster;	// My master
		private Mobile		m_ControlTarget;	// My target mobile
		private Point3D		m_ControlDest;		// My target destination (patrol)
		private OrderType	m_ControlOrder;		// My order

		private PetLoyalty  m_Loyalty;

		private double	m_dMinTameSkill;
		private bool	m_bTamable;

		private bool		m_bSummoned = false;
		private DateTime	m_SummonEnd;
		private int			m_iControlSlots = 0;

		private bool		m_bBardProvoked = false;
		private bool		m_bBardPacified = false;
		private Mobile		m_bBardMaster = null;
		private Mobile		m_bBardTarget = null;
		private DateTime	m_timeBardEnd;
		private WayPoint	m_CurrentWayPoint = null;
		private Point2D		m_TargetLocation = Point2D.Zero;

		private Mobile		m_SummonMaster;

		private int			m_HitsMax = -1;
		private	int			m_StamMax = -1;
		private int			m_ManaMax = -1;
		private int			m_DamageMin = -1;
		private int			m_DamageMax = -1;

		private int			m_PhysicalResistance, m_PhysicalDamage = 100;
		private int			m_FireResistance, m_FireDamage;
		private int			m_ColdResistance, m_ColdDamage;
		private int			m_PoisonResistance, m_PoisonDamage;
		private int			m_EnergyResistance, m_EnergyDamage;

		private ArrayList	m_Owners;

		private bool		m_IsStabled;

		private bool m_Gateable = true;
		[CommandProperty( AccessLevel.GameMaster )]
		public bool CanBeGated { get { return m_Gateable; } set { m_Gateable = value; } }

		public bool IsStabled
		{
			get{ return m_IsStabled; }
			set{ m_IsStabled = value; }
		}

		public override bool ClickTitle
		{
			get
			{
				return false;
			}
		}

		public virtual InhumanSpeech SpeechType{ get{ return null; } }


		#region Bonding
		public const bool BondingEnabled = false;

		public virtual bool IsBondable{ get{ return ( BondingEnabled && !Summoned ); } }
		public virtual TimeSpan BondingDelay{ get{ return TimeSpan.FromDays( 7.0 ); } }
		public virtual TimeSpan BondingAbandonDelay{ get{ return TimeSpan.FromDays( 1.0 ); } }

		public override bool CanRegenHits{ get{ return !m_IsDeadPet && base.CanRegenHits; } }
		public override bool CanRegenStam{ get{ return !m_IsDeadPet && base.CanRegenStam; } }
		public override bool CanRegenMana{ get{ return !m_IsDeadPet && base.CanRegenMana; } }

		public override bool IsDeadBondedPet{ get{ return m_IsDeadPet; } }

		private bool m_IsBonded;
		private bool m_IsDeadPet;
		private DateTime m_BondingBegin;
		private DateTime m_OwnerAbandonTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsBonded
		{
			get{ return false; }
			set{ ; }
		}

		public bool IsDeadPet
		{
			get{ return m_IsDeadPet; }
			set{ m_IsDeadPet = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime BondingBegin
		{
			get{ return m_BondingBegin; }
			set{ m_BondingBegin = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime OwnerAbandonTime
		{
			get{ return m_OwnerAbandonTime; }
			set{ m_OwnerAbandonTime = value; }
		}
		#endregion


		public override int BasePhysicalResistance{ get{ return m_PhysicalResistance; } }
		public override int BaseFireResistance{ get{ return m_FireResistance; } }
		public override int BaseColdResistance{ get{ return m_ColdResistance; } }
		public override int BasePoisonResistance{ get{ return m_PoisonResistance; } }
		public override int BaseEnergyResistance{ get{ return m_EnergyResistance; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PhysicalResistanceSeed{ get{ return m_PhysicalResistance; } set{ m_PhysicalResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireResistSeed{ get{ return m_FireResistance; } set{ m_FireResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdResistSeed{ get{ return m_ColdResistance; } set{ m_ColdResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonResistSeed{ get{ return m_PoisonResistance; } set{ m_PoisonResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyResistSeed{ get{ return m_EnergyResistance; } set{ m_EnergyResistance = value; UpdateResistances(); } }


		[CommandProperty( AccessLevel.GameMaster )]
		public int PhysicalDamage{ get{ return m_PhysicalDamage; } set{ m_PhysicalDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireDamage{ get{ return m_FireDamage; } set{ m_FireDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdDamage{ get{ return m_ColdDamage; } set{ m_ColdDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonDamage{ get{ return m_PoisonDamage; } set{ m_PoisonDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyDamage{ get{ return m_EnergyDamage; } set{ m_EnergyDamage = value; } }

		public virtual FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public virtual PackInstinct PackInstinct{ get{ return PackInstinct.None; } }

		public ArrayList Owners{ get{ return m_Owners; } }

		public virtual bool AllowMaleTamer{ get{ return true; } }
		public virtual bool AllowFemaleTamer{ get{ return true; } }
		public virtual bool SubdueBeforeTame{ get{ return false; } }

		public virtual bool Commandable{ get{ return true; } }

		public virtual Poison HitPoison{ get{ return null; } }
		public virtual double HitPoisonChance{ get{ return 0.5; } }
		public virtual Poison PoisonImmune{ get{ return null; } }

		public override void OnPoisonImmunity( Mobile from, Poison poison )
		{
			// dont show the message if they are immune to everything
			if ( PoisonImmune != Poison.Lethal )
				base.OnPoisonImmunity( from, poison );
		}

		public virtual bool BardImmune{ get{ return MinTameSkill >= 999; } }
		public virtual bool Unprovokable{ get{ return BardImmune; } }
		public virtual bool Uncalmable{ get{ return BardImmune; } }

		private DateTime m_NextBreathTime;

		// Must be overriden in subclass to enable
		public virtual bool HasBreath{ get{ return false; } }

		// Base damage given is: CurrentHitPoints * BreathDamageScalar
		public virtual double BreathDamageScalar{ get{ return 0.05; } }

		// Min/max seconds until next breath
		public virtual double BreathMinDelay{ get{ return 5.0; } }
		public virtual double BreathMaxDelay{ get{ return 10.0; } }

		// Creature stops moving for 1.0 seconds while breathing
		public virtual double BreathStallTime{ get{ return 1.0; } }

		// Effect is sent 1.3 seconds after BreathAngerSound and BreathAngerAnimation is played
		public virtual double BreathEffectDelay{ get{ return 1.3; } }

		// Damage is given 1.0 seconds after effect is sent
		public virtual double BreathDamageDelay{ get{ return 1.0; } }

		public virtual int BreathRange{ get{ return RangePerception/2+2; } }

		// Effect details and sound
		public virtual int BreathEffectItemID{ get{ return 0x36D4; } }
		public virtual int BreathEffectSpeed{ get{ return 5; } }
		public virtual int BreathEffectDuration{ get{ return 0; } }
		public virtual bool BreathEffectExplodes{ get{ return false; } }
		public virtual bool BreathEffectFixedDir{ get{ return false; } }
		public virtual int BreathEffectHue{ get{ return 0; } }
		public virtual int BreathEffectRenderMode{ get{ return 0; } }

		public virtual int BreathEffectSound{ get{ return 0x227; } }

		// Anger sound/animations
		public virtual int BreathAngerSound{ get{ return GetAngerSound(); } }
		public virtual int BreathAngerAnimation{ get{ return 12; } }

		public virtual void BreathStart( Mobile target )
		{
			BreathStallMovement();
			BreathPlayAngerSound();
			BreathPlayAngerAnimation();

			this.Direction = this.GetDirectionTo( target );

			Timer.DelayCall( TimeSpan.FromSeconds( BreathEffectDelay ), new TimerStateCallback( BreathEffect_Callback ), target );
		}

		public virtual void BreathStallMovement()
		{
			if ( m_AI != null )
				m_AI.NextMove = DateTime.Now + TimeSpan.FromSeconds( BreathStallTime );
		}

		public virtual void BreathPlayAngerSound()
		{
			PlaySound( BreathAngerSound );
		}

		public virtual void BreathPlayAngerAnimation()
		{
			Animate( BreathAngerAnimation, 5, 1, true, false, 0 );
		}

		public virtual void BreathEffect_Callback( object state )
		{
			Mobile target = (Mobile)state;

			if ( !target.Alive || !CanBeHarmful( target ) )
				return;

			BreathPlayEffectSound();
			BreathPlayEffect( target );

			Timer.DelayCall( TimeSpan.FromSeconds( BreathDamageDelay ), new TimerStateCallback( BreathDamage_Callback ), target );
		}

		public virtual void BreathPlayEffectSound()
		{
			PlaySound( BreathEffectSound );
		}

		public virtual void BreathPlayEffect( Mobile target )
		{
			Effects.SendMovingEffect( this, target, BreathEffectItemID,
				BreathEffectSpeed, BreathEffectDuration, BreathEffectFixedDir,
				BreathEffectExplodes, BreathEffectHue, BreathEffectRenderMode );
		}

		public virtual void BreathDamage_Callback( object state )
		{
			Mobile target = (Mobile)state;

			if ( CanBeHarmful( target ) )
			{
				DoHarmful( target );
				BreathDealDamage( target );
			}
		}

		public virtual void BreathDealDamage( Mobile target )
		{
			target.Damage( BreathComputeDamage(), this );
		}

		public virtual int BreathComputeDamage()
		{
			return (int)(Hits * BreathDamageScalar);
		}

		public virtual bool CheckFlee()
		{
			/*if ( m_EndFlee == DateTime.MinValue )
				return false;

			if ( DateTime.Now >= m_EndFlee )
			{
				StopFlee();
				return false;
			}*/

			return ( !Controled && !Summoned && !( BardProvoked && BardEndTime > DateTime.Now && BardTarget != null && !BardTarget.Deleted && BardTarget.Alive && BardTarget.InRange( this, RangePerception ) ) && ( Hits <= HitsMax*0.1 && Hits < 100 ) );
		}

		public BaseAI AIObject{ get{ return m_AI; } }

		public const int MaxOwners = 10;

		public virtual bool IsFriend( Mobile m )
		{
			if ( !(m is BaseCreature) )
				return false;

			BaseCreature c = (BaseCreature)m;

			return ( m_iTeam == c.m_iTeam && ( (m_bSummoned || m_bControled) == (c.m_bSummoned || c.m_bControled) ) );
		}

		public virtual bool IsEnemy( Mobile m )
		{
			if ( m is BaseGuard )
				return false;

			if ( !(m is BaseCreature) )
				return true;

			BaseCreature c = (BaseCreature)m;

			return ( m_iTeam != c.m_iTeam || ( (m_bSummoned || m_bControled) != (c.m_bSummoned || c.m_bControled) ) );
		}

		public virtual bool CheckControlChance( Mobile m )
		{
			return CheckControlChance( m, 0.0 );
		}

		public virtual bool CheckControlChance( Mobile m, double offset )
		{
			double v = GetControlChance( m ) + offset;

			if ( v > Utility.RandomDouble() )
				return true;

			PlaySound( GetAngerSound() );

			if ( Body.IsAnimal )
				Animate( 10, 5, 1, true, false, 0 );
			else if ( Body.IsMonster )
				Animate( 18, 5, 1, true, false, 0 );

			if ( AlwaysMurderer )
			{
				double attack;
				if ( m_dMinTameSkill > 99 )
					attack = 0.33;
				else if ( m_dMinTameSkill > 90 )
					attack = 0.25;
				else if ( m_dMinTameSkill > 80 )
					attack = 0.2;
				else if ( m_dMinTameSkill > 70 )
					attack = 0.15;
				else
					attack = 0.1;

				if ( attack > Utility.RandomDouble() )
				{
					Mobile target = m;
					double dist = GetDistanceToSqrt( target );

					if ( Utility.Random( 4 ) == 0 )
					{
						IPooledEnumerable eable = GetMobilesInRange( 8 );
						foreach ( Mobile mob in eable )
						{
							double check = GetDistanceToSqrt( mob );
							if ( mob != target && mob != this && check < dist && ( mob.Body.IsHuman || ( mob is BaseCreature && ((BaseCreature)mob).Controled && !((BaseCreature)mob).AlwaysMurderer ) ) )
							{
								target = mob;
								dist = check;
							}
						}	
						eable.Free();
					}

					if ( target != null && dist <= 18 )
					{
						Attack( target );
						ControlTarget = target;
						ControlOrder = OrderType.Attack;
					}
				}
			}
			return false;
		}

		public virtual bool CanBeControlledBy( Mobile m )
		{
			return true;//( GetControlChance( m ) > 0.0 );
		}

		public virtual double GetControlChance( Mobile m )
		{
			//if ( m_dMinTameSkill <= 29.1 || m_bSummoned || m.AccessLevel >= AccessLevel.GameMaster )

			/*if ( m_dMinTameSkill >= 99.9 )
				return 0.85;
			else if ( m_dMinTameSkill >= 90 )
				return 0.90;
			else if ( m_dMinTameSkill >= 75 )
				return 0.95;
			else
				return 1.0;*/

			double chance = (95.0 - m_dMinTameSkill) / 100.0;

			chance += ((double)m_Loyalty) / ((double)PetLoyalty.Maximum);

			return chance;

			/*double dMinTameSkill = m_dMinTameSkill;

			if ( dMinTameSkill > -24.9 && Server.SkillHandlers.AnimalTaming.CheckMastery( m, this ) )
				dMinTameSkill = -24.9;

			int taming = (int)(m.Skills[SkillName.AnimalTaming].Value * 10);
			int lore = (int)(m.Skills[SkillName.AnimalLore].Value * 10);
			int difficulty = (int)(dMinTameSkill * 10);
			int weighted = ((taming * 4) + lore) / 5;
			int bonus = weighted - difficulty;
			int chance;

			if ( bonus > 0 )
				chance = 700 + (bonus * 14);
			else
				chance = 700 + (bonus * 6);

			if ( chance >= 0 && chance < 200 )
				chance = 200;
			else if ( chance > 990 )
				chance = 990;

			int loyaltyValue = 1;

			if ( m_Loyalty > PetLoyalty.Confused )
				loyaltyValue = (int)(m_Loyalty - PetLoyalty.Confused) * 10;

			chance -= (100 - loyaltyValue) * 10;

			return ( (double)chance / 10 );*/
		}

		private static Type[] m_AnimateDeadTypes = new Type[0];/*]
			{
				typeof( MoundOfMaggots ), typeof( HellSteed ), typeof( SkeletalMount ),
				typeof( WailingBanshee ), typeof( Wraith ), typeof( SkeletalDragon ),
				typeof( LichLord ), typeof( FleshGolem ), typeof( Lich ),
				typeof( SkeletalKnight ), typeof( BoneKnight ), typeof( Mummy ),
				typeof( SkeletalMage ), typeof( BoneMagi ), typeof( PatchworkSkeleton )
			};*/

		public virtual bool IsAnimatedDead
		{
			get
			{
				if ( !Summoned )
					return false;

				Type type = this.GetType();

				bool contains = false;

				for ( int i = 0; !contains && i < m_AnimateDeadTypes.Length; ++i )
					contains = ( type == m_AnimateDeadTypes[i] );

				return contains;
			}
		}

		public override void Damage( int amount, Mobile from )
		{
			if ( this.Spell is Spell )
				((Spell)this.Spell).OnCasterHurt( amount );

			int oldHits = this.Hits;
			base.Damage( amount, from );
			if ( SubdueBeforeTame && !Controled )
			{
				if ( (oldHits > (this.HitsMax / 10)) && (this.Hits <= (this.HitsMax / 10)) )
					PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* The creature has been beaten into subjugation! *" );
			}
		}

		public virtual bool DeleteCorpseOnDeath
		{
			get
			{
				return !Core.AOS && m_bSummoned;
			}
		}

		public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
		{
			if ( !Alive || IsDeadPet )
				return ApplyPoisonResult.Immune;

			return base.ApplyPoison( from, poison );
		}

		public override bool CheckPoisonImmunity( Mobile from, Poison poison )
		{
			if ( base.CheckPoisonImmunity( from, poison ) )
				return true;

			Poison p = this.PoisonImmune;

			return ( p != null && p.Level >= poison.Level );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public PetLoyalty Loyalty
		{
			get
			{
				return m_Loyalty;
			}
			set
			{
				m_Loyalty = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WayPoint CurrentWayPoint 
		{
			get
			{
				return m_CurrentWayPoint;
			}
			set
			{
				m_CurrentWayPoint = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point2D TargetLocation
		{
			get
			{
				return m_TargetLocation;
			}
			set
			{
				m_TargetLocation = value;
			}
		}

		public virtual Mobile ConstantFocus{ get{ return null; } }

		public virtual bool DisallowAllMoves
		{
			get
			{
				return false;
			}
		}

		public virtual bool InitialInnocent
		{
			get
			{
				return false;
			}
		}

		public virtual bool AlwaysMurderer
		{
			get
			{
				return false;
			}
		}

		public virtual bool AlwaysAttackable
		{
			get
			{
				return false;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int DamageMin{ get{ return m_DamageMin; } set{ m_DamageMin = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int DamageMax{ get{ return m_DamageMax; } set{ m_DamageMax = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override int HitsMax
		{
			get
			{
				if ( m_HitsMax >= 0 )
					return m_HitsMax;

				return Str;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitsMaxSeed
		{
			get{ return m_HitsMax; }
			set{ m_HitsMax = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int StamMax
		{
			get
			{
				if ( m_StamMax >= 0 )
					return m_StamMax;

				return Dex;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StamMaxSeed
		{
			get{ return m_StamMax; }
			set{ m_StamMax = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int ManaMax
		{
			get
			{
				if ( m_ManaMax >= 0 )
					return m_ManaMax;

				return Int;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ManaMaxSeed
		{
			get{ return m_ManaMax; }
			set{ m_ManaMax = value; }
		}

		public virtual bool CanOpenDoors
		{
			get
			{
				return !this.Summoned && !this.Body.IsAnimal && !this.Body.IsSea && this.Int > 20;
			}
		}

		public virtual bool CanMoveOverObstacles
		{
			get
			{
				return false;//this.Body.IsMonster;
			}
		}

		public virtual bool CanDestroyObstacles
		{
			get
			{
				// to enable breaking of furniture, 'return CanMoveOverObstacles;'
				return !this.Summoned && this.Body.IsMonster && this.Int >= 30 && this.Str >= 45;
			}
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			WeightOverloading.FatigueOnDamage( this, amount );

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && !willKill )
				speechType.OnDamage( this, amount );

			base.OnDamage( amount, from, willKill );
		}

		public virtual void OnDamagedBySpell( Mobile from )
		{
		}

		public virtual void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
		}

		public virtual void AlterDamageScalarTo( Mobile target, ref double scalar )
		{
		}

		public virtual void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			//damage *= 2;
		}

		public virtual void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
		}

		public virtual void CheckReflect( Mobile caster, ref bool reflect )
		{
		}

		public virtual int GenerateFurs( Corpse c )
		{
			return 0;
		}

		public virtual void OnCarve( Mobile from, Corpse corpse )
		{
			int feathers = Feathers;
			int wool = Wool;
			int meat = Meat;
			int hides = Hides;
			int scales = 0;//Scales; 
			int furs = GenerateFurs( corpse );

			if ( (feathers == 0 && furs == 0 && wool == 0 && meat == 0 && hides == 0 && scales == 0) || Summoned )
			{
				from.SendLocalizedMessage( 500485 ); // You see nothing useful to carve from the corpse.
			}
			else
			{
				new Blood( 0x122D ).MoveToWorld( corpse.Location, corpse.Map );

				if ( feathers != 0 )
				{
					corpse.DropItem( new Feather( feathers ) );
					//from.SendLocalizedMessage( 500479 ); // You pluck the bird. The feathers are now on the corpse.
				}

				if ( wool != 0 )
				{
					corpse.DropItem( new Wool( wool ) );
					//from.SendLocalizedMessage( 500483 ); // You shear it, and the wool is now on the corpse.
				}

				if ( meat != 0 )
				{
					if ( MeatType == MeatType.Ribs )
						corpse.DropItem( new RawRibs( meat ) );
					else if ( MeatType == MeatType.Bird )
						corpse.DropItem( new RawBird( meat ) );
					else if ( MeatType == MeatType.LambLeg )
						corpse.DropItem( new RawLambLeg( meat ) );

					//from.SendLocalizedMessage( 500467 ); // You carve some meat, which remains on the corpse.
				}

				if ( hides != 0 )
				{
					if ( HideType == HideType.Regular )
						corpse.DropItem( new Hides( hides ) );
					else if ( HideType == HideType.Spined )
						corpse.DropItem( new Hides( hides ) );
					else if ( HideType == HideType.Horned )
						corpse.DropItem( new Hides( hides ) );
					else if ( HideType == HideType.Barbed )
						corpse.DropItem( new Hides( hides ) );
				}

				//if ( hides != 0 || furs != 0 )
				//	from.SendLocalizedMessage( 500471 ); // You skin it, and the hides are now in the corpse.

				corpse.Carved = true;

				if ( corpse.IsCriminalAction( from ) )
					from.CriminalAction( true );
			}
		}

		public const int DefaultRangePerception = 16;
		public const int OldRangePerception = 10;

		public BaseCreature(AIType ai,
			FightMode mode,
			int iRangePerception,
			int iRangeFight,
			double dActiveSpeed, 
			double dPassiveSpeed)
		{
			if ( iRangePerception == OldRangePerception )
				iRangePerception = DefaultRangePerception;

			m_Loyalty = PetLoyalty.WonderfullyHappy;

			m_CurrentAI = ai;
			m_DefaultAI = ai;

			m_iRangePerception = iRangePerception;
			m_iRangeFight = iRangeFight;
			
			m_FightMode = mode;

			m_iTeam = 0;

			SpeedInfo.GetSpeeds( this, ref dActiveSpeed, ref dPassiveSpeed );

			m_dActiveSpeed = dActiveSpeed;
			m_dPassiveSpeed = dPassiveSpeed;
			m_dCurrentSpeed = dPassiveSpeed;

			m_bDebugAI = false;

			m_arSpellAttack = new ArrayList();
			m_arSpellDefense = new ArrayList();

			m_bControled = false;
			m_ControlMaster = null;
			m_ControlTarget = null;
			m_ControlOrder = OrderType.None;

			m_bTamable = false;

			m_Owners = new ArrayList();

			m_NextReaquireTime = DateTime.Now + ReaquireDelay;

			ChangeAIType(AI);

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null )
				speechType.OnConstruct( this );

			//VirtualArmorMod = -2;
		}

		public BaseCreature( Serial serial ) : base( serial )
		{
			m_arSpellAttack = new ArrayList();
			m_arSpellDefense = new ArrayList();

			m_bDebugAI = false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 10 ); // version

			writer.Write( (int)m_CurrentAI );
			writer.Write( (int)m_DefaultAI );

			writer.Write( (int)m_iRangePerception );
			writer.Write( (int)m_iRangeFight );

			writer.Write( (int)m_iTeam );

			writer.Write( (double)m_dActiveSpeed );
			writer.Write( (double)m_dPassiveSpeed );
			writer.Write( (double)m_dCurrentSpeed );

			writer.Write( (int) m_pHome.X );
			writer.Write( (int) m_pHome.Y );
			writer.Write( (int) m_pHome.Z );

			// Version 1
			writer.Write( (int) m_iRangeHome );

			int i=0;

			writer.Write( (int) m_arSpellAttack.Count );
			for ( i=0; i< m_arSpellAttack.Count; i++ )
			{
				writer.Write( m_arSpellAttack[i].ToString() );
			}

			writer.Write( (int) m_arSpellDefense.Count );
			for ( i=0; i< m_arSpellDefense.Count; i++ )
			{
				writer.Write( m_arSpellDefense[i].ToString() );
			}

			// Version 2
			writer.Write( (int) m_FightMode );

			writer.Write( (bool) m_bControled );
			writer.Write( (Mobile) m_ControlMaster );
			writer.Write( (Mobile) m_ControlTarget );
			writer.Write( (Point3D) m_ControlDest );
			writer.Write( (int) m_ControlOrder );
			writer.Write( (double) m_dMinTameSkill );
			// Removed in version 9
			//writer.Write( (double) m_dMaxTameSkill );
			writer.Write( (bool) m_bTamable );
			writer.Write( (bool) m_bSummoned );

			if ( m_bSummoned )
				writer.WriteDeltaTime( m_SummonEnd );

			writer.Write( (int) m_iControlSlots );

			// Version 3
			writer.Write( (int)m_Loyalty );

			// Version 4 
			writer.Write( m_CurrentWayPoint );

			// Verison 5
			writer.Write( m_SummonMaster );

			// Version 6
			writer.Write( (int) m_HitsMax );
			writer.Write( (int) m_StamMax );
			writer.Write( (int) m_ManaMax );
			writer.Write( (int) m_DamageMin );
			writer.Write( (int) m_DamageMax );

			// Version 7
			writer.Write( (int) m_PhysicalResistance );
			writer.Write( (int) m_PhysicalDamage );

			writer.Write( (int) m_FireResistance );
			writer.Write( (int) m_FireDamage );

			writer.Write( (int) m_ColdResistance );
			writer.Write( (int) m_ColdDamage );

			writer.Write( (int) m_PoisonResistance );
			writer.Write( (int) m_PoisonDamage );

			writer.Write( (int) m_EnergyResistance );
			writer.Write( (int) m_EnergyDamage );

			// Version 8
			if ( m_Owners != null )
			{
				for ( int j = 0; j < m_Owners.Count; )
				{
					if ( m_Owners[j] == null )
						m_Owners.RemoveAt( j );
					else
						j++;
				}
				
				writer.WriteMobileList( m_Owners, true );
			}
			else
				writer.Write( (int)0 );

			// Version 10
			writer.Write( (bool) m_IsDeadPet );
			writer.Write( (bool) m_IsBonded );
			writer.Write( (DateTime) m_BondingBegin );
			writer.Write( (DateTime) m_OwnerAbandonTime );
		}

		private static double[] m_StandardActiveSpeeds = new double[]
			{
				0.175, 0.1, 0.15, 0.2, 0.25, 0.3, 0.4, 0.5, 0.6, 0.8
			};

		private static double[] m_StandardPassiveSpeeds = new double[]
			{
				0.350, 0.2, 0.4, 0.5, 0.6, 0.8, 1.0, 1.2, 1.6, 2.0
			};

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			//VirtualArmorMod = -2;

			int version = reader.ReadInt();

			m_CurrentAI = (AIType)reader.ReadInt();
			m_DefaultAI = (AIType)reader.ReadInt();

			m_iRangePerception = reader.ReadInt();
			m_iRangeFight = reader.ReadInt();

			m_iTeam = reader.ReadInt();

			m_dActiveSpeed = reader.ReadDouble();
			m_dPassiveSpeed = reader.ReadDouble();
			m_dCurrentSpeed = reader.ReadDouble();

			double activeSpeed = m_dActiveSpeed;
			double passiveSpeed = m_dPassiveSpeed;

			SpeedInfo.GetSpeeds( this, ref activeSpeed, ref passiveSpeed );

			bool isStandardActive = false;
			for ( int i = 0; !isStandardActive && i < m_StandardActiveSpeeds.Length; ++i )
				isStandardActive = ( m_dActiveSpeed == m_StandardActiveSpeeds[i] );

			bool isStandardPassive = false;
			for ( int i = 0; !isStandardPassive && i < m_StandardPassiveSpeeds.Length; ++i )
				isStandardPassive = ( m_dPassiveSpeed == m_StandardPassiveSpeeds[i] );

			if ( isStandardActive && m_dCurrentSpeed == m_dActiveSpeed )
				m_dCurrentSpeed = activeSpeed;
			else if ( isStandardPassive && m_dCurrentSpeed == m_dPassiveSpeed )
				m_dCurrentSpeed = passiveSpeed;

			if ( isStandardActive )
				m_dActiveSpeed = activeSpeed;

			if ( isStandardPassive )
				m_dPassiveSpeed = passiveSpeed;
			
			if ( m_iRangePerception == OldRangePerception )
				m_iRangePerception = DefaultRangePerception;

			m_pHome.X = reader.ReadInt();
			m_pHome.Y = reader.ReadInt();
			m_pHome.Z = reader.ReadInt();

			if ( version >= 1 )
			{
				m_iRangeHome = reader.ReadInt();

				int i, iCount;
				
				iCount = reader.ReadInt();
				for ( i=0; i< iCount; i++ )
				{
					string str = reader.ReadString();
					Type type = Type.GetType( str );

					if ( type != null )
					{
						m_arSpellAttack.Add( type );
					}
				}

				iCount = reader.ReadInt();
				for ( i=0; i< iCount; i++ )
				{
					string str = reader.ReadString();
					Type type = Type.GetType( str );

					if ( type != null )
					{
						m_arSpellDefense.Add( type );
					}			
				}
			}
			else
			{
				m_iRangeHome = 0;
			}

			if ( version >= 2 )
			{
				m_FightMode = ( FightMode )reader.ReadInt();

				m_bControled = reader.ReadBool();
				m_ControlMaster = reader.ReadMobile();
				m_ControlTarget = reader.ReadMobile();
				m_ControlDest = reader.ReadPoint3D();
				m_ControlOrder = (OrderType) reader.ReadInt();

				m_dMinTameSkill = reader.ReadDouble();

				if ( version < 9 )
					reader.ReadDouble();

				m_bTamable = reader.ReadBool();
				m_bSummoned = reader.ReadBool();

				if ( m_bSummoned )
				{
					m_SummonEnd = reader.ReadDeltaTime();
					new UnsummonTimer( m_ControlMaster, this, m_SummonEnd - DateTime.Now ).Start();
				}

				m_iControlSlots = reader.ReadInt();
			}
			else
			{
				m_FightMode = FightMode.Closest;

				m_bControled = false;
				m_ControlMaster = null;
				m_ControlTarget = null;
				m_ControlOrder = OrderType.None;
			}

			if ( version >= 3 )
				m_Loyalty = (PetLoyalty)reader.ReadInt();
			else
				m_Loyalty = PetLoyalty.WonderfullyHappy;

			if ( version >= 4 )
				m_CurrentWayPoint = reader.ReadItem() as WayPoint;

			if ( version >= 5 )
				m_SummonMaster = reader.ReadMobile();

			if ( version >= 6 )
			{
				m_HitsMax = reader.ReadInt();
				m_StamMax = reader.ReadInt();
				m_ManaMax = reader.ReadInt();
				m_DamageMin = reader.ReadInt();
				m_DamageMax = reader.ReadInt();
			}

			if ( version >= 7 )
			{
				m_PhysicalResistance = reader.ReadInt();
				m_PhysicalDamage = reader.ReadInt();

				m_FireResistance = reader.ReadInt();
				m_FireDamage = reader.ReadInt();

				m_ColdResistance = reader.ReadInt();
				m_ColdDamage = reader.ReadInt();

				m_PoisonResistance = reader.ReadInt();
				m_PoisonDamage = reader.ReadInt();

				m_EnergyResistance = reader.ReadInt();
				m_EnergyDamage = reader.ReadInt();
			}

			if ( version >= 8 )
				m_Owners = reader.ReadMobileList();
			else
				m_Owners = new ArrayList();

			if ( version >= 10 )
			{
				m_IsDeadPet = reader.ReadBool();
				m_IsBonded = reader.ReadBool();
				m_BondingBegin = reader.ReadDateTime();
				m_OwnerAbandonTime = reader.ReadDateTime();
			}

			if ( Controled && !Owners.Contains( ControlMaster ) )
				Owners.Add( ControlMaster );

			CheckStatTimers();

			ChangeAIType(m_CurrentAI);

			AddFollowers();
		}

		public virtual bool IsHumanInTown()
		{
			return ( Body.IsHuman );//&& Region is Regions.GuardedRegion );
		}

		public virtual bool CheckGold( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
				return OnGoldGiven( from, (Gold)dropped );

			return false;
		}

		public virtual bool OnGoldGiven( Mobile from, Gold dropped )
		{
			if ( CheckTeachingMatch( from ) )
			{
				if ( Teach( m_Teaching, from, dropped.Amount, true ) )
				{
					dropped.Delete();
					return true;
				}
			}
			else if ( IsHumanInTown() )
			{
				Direction = GetDirectionTo( from );

				int oldSpeechHue = this.SpeechHue;

				this.SpeechHue = 0x23F;
				SayTo( from, "Thou art giving me gold?" );

				if ( dropped.Amount >= 400 )
					SayTo( from, "'Tis a noble gift." );
				else
					SayTo( from, "Money is always welcome." );

				this.SpeechHue = 0x3B2;
				SayTo( from, 501548 ); // I thank thee.

				this.SpeechHue = oldSpeechHue;

				if ( dropped.Amount >= 25 )
					Misc.Titles.AlterNotoriety( from, 1, NotoCap.Dastardly ); // only gets you to dastardly

				dropped.Delete();
				return true;
			}

			return false;
		}

		public override bool ShouldCheckStatTimers{ get{ return false; } }

		private static Type[] m_Eggs = new Type[]
			{
				typeof( FriedEggs ), typeof( Eggs )
			};

		private static Type[] m_Fish = new Type[]
			{
				typeof( FishSteak ), typeof( RawFishSteak )
			};

		private static Type[] m_GrainsAndHay = new Type[]
			{
				typeof( BreadLoaf ), typeof( FrenchBread )
			};

		private static Type[] m_Meat = new Type[]
			{
				/* Cooked */
				typeof( Bacon ), typeof( CookedBird ), typeof( Sausage ),
				typeof( Ham ), typeof( Ribs ), typeof( LambLeg ),
				typeof( ChickenLeg ),

				/* Uncooked */
				typeof( RawBird ), typeof( RawRibs ), typeof( RawLambLeg ),
				typeof( RawChickenLeg ),

				/* Body Parts */
				typeof( Head ), typeof( LeftArm ), typeof( LeftLeg ),
				typeof( Torso ), typeof( RightArm ), typeof( RightLeg )
			};

		private static Type[] m_FruitsAndVegies = new Type[]
			{
				typeof( HoneydewMelon ), typeof( YellowGourd ), typeof( GreenGourd ),
				typeof( Banana ), typeof( Bananas ), typeof( Lemon ), typeof( Lime ),
				typeof( Dates ), typeof( Grapes ), typeof( Peach ), typeof( Pear ),
				typeof( Apple ), typeof( Watermelon ), typeof( Squash ),
				typeof( Cantaloupe ), typeof( Carrot ), typeof( Cabbage ),
				typeof( Onion ), typeof( Lettuce ), typeof( Pumpkin )
			};

		private static Type[] m_Gold = new Type[]
			{
				// white wyrms eat gold..
				typeof( Gold )
			};

		public virtual bool CheckFoodPreference( Item f )
		{
			if ( CheckFoodPreference( f, FoodType.Eggs, m_Eggs ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Fish, m_Fish ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.GrainsAndHay, m_GrainsAndHay ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Meat, m_Meat ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.FruitsAndVegies, m_FruitsAndVegies ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Gold, m_Gold ) )
				return true;

			return false;
		}

		public virtual bool CheckFoodPreference( Item fed, FoodType type, Type[] types )
		{
			if ( (FavoriteFood & type) == 0 )
				return false;

			Type fedType = fed.GetType();
			bool contains = false;

			for ( int i = 0; !contains && i < types.Length; ++i )
				contains = ( fedType == types[i] );

			return contains;
		}

		public virtual bool CheckFeed( Mobile from, Item dropped )
		{
			if ( !IsDeadPet && Controled && ControlMaster == from && (dropped is Food || dropped is Gold || dropped is CookableFood || dropped is Head || dropped is LeftArm || dropped is LeftLeg || dropped is Torso || dropped is RightArm || dropped is RightLeg) )
			{
				Item f = dropped;

				if ( CheckFoodPreference( f ) )
				{
					int amount = f.Amount;

					if ( amount > 0 )
					{
						bool happier = false;

						int stamGain;

						if ( f is Gold )
							stamGain = amount - 50;
						else
							stamGain = (amount * 15) - 50;

						if ( stamGain > 0 )
							Stam += stamGain;

						for ( int i = 0; i < amount; ++i )
						{
							if ( m_Loyalty < PetLoyalty.WonderfullyHappy && 0.5 >= Utility.RandomDouble() )
							{
								++m_Loyalty;
								happier = true;
							}
						}

						if ( happier )
							SayTo( from, 502060 ); // Your pet looks happier.

						if ( Body.IsAnimal )
							Animate( 3, 5, 1, true, false, 0 );
						else if ( Body.IsMonster )
							Animate( 17, 5, 1, true, false, 0 );

						if ( IsBondable && !IsBonded )
						{
							Mobile master = m_ControlMaster;

							if ( master != null )
							{
								if ( m_dMinTameSkill <= 29.1 || master.Skills[SkillName.AnimalTaming].Value >= m_dMinTameSkill )
								{
									if ( BondingBegin == DateTime.MinValue )
									{
										BondingBegin = DateTime.Now;
									}
									else if ( (BondingBegin + BondingDelay) <= DateTime.Now )
									{
										IsBonded = true;
										BondingBegin = DateTime.MinValue;
										from.SendLocalizedMessage( 1049666 ); // Your pet has bonded with you!
									}
								}
							}
						}

						dropped.Delete();
						return true;
					}
				}
			}

			return false;
		}

		public virtual void OnActionWander()
		{
		}

		public virtual void OnActionCombat()
		{
		}

		public virtual void OnActionGuard()
		{
		}

		public virtual void OnActionFlee()
		{
		}

		public virtual void OnActionInteract()
		{
		}

		public virtual void OnActionBackoff()
		{
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( CheckFeed( from, dropped ) )
				return true;
			else if ( CheckGold( from, dropped ) )
				return true;

			return base.OnDragDrop( from, dropped );
		}

		public virtual void ChangeAIType(AIType NewAI)
		{
			if ( m_AI != null )
				m_AI.m_Timer.Stop();

			m_AI = null;

			switch (NewAI)
			{
				case AIType.AI_Melee:
					m_AI = new MeleeAI(this);
					break;
				case AIType.AI_Animal:
					m_AI = new AnimalAI(this);
					break;
				case AIType.AI_Berserk:
					m_AI = new BerserkAI(this);
					break;
				case AIType.AI_Archer:
					m_AI = new ArcherAI(this);
					break;
				case AIType.AI_Healer:
					m_AI = new HealerAI(this);
					break;
				case AIType.AI_Vendor:
					m_AI = new VendorAI(this);
					break;
				case AIType.AI_Mage:
					m_AI = new MageAI(this);
					break;
				case AIType.AI_Predator:
					//m_AI = new PredatorAI(this);
					m_AI = new MeleeAI(this);
					break;
				case AIType.AI_Thief:
					m_AI = new ThiefAI(this);
					break;
				case AIType.AI_TeleHideMage:
					m_AI = new TeleHideAI(this);
					break;
			}
		}

		public virtual void ChangeAIToDefault()
		{
			ChangeAIType(m_DefaultAI);
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AIType AI
		{
			get
			{
				return m_CurrentAI;
			}
			set
			{
				m_CurrentAI = value;

				if (m_CurrentAI == AIType.AI_Use_Default)
				{
					m_CurrentAI = m_DefaultAI;
				}
				
				ChangeAIType(m_CurrentAI);
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Debug
		{
			get
			{
				return m_bDebugAI;
			}
			set
			{
				m_bDebugAI = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Team
		{
			get
			{
				return m_iTeam;
			}
			set
			{
				m_iTeam = value;
				
				OnTeamChange();
			}
		}

		public virtual void OnTeamChange()
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile FocusMob
		{
			get
			{
				return m_FocusMob;
			}
			set
			{
				m_FocusMob = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public FightMode FightMode
		{
			get
			{
				return m_FightMode;
			}
			set
			{
				m_FightMode = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangePerception
		{
			get
			{
				return m_iRangePerception;
			}
			set
			{
				m_iRangePerception = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangeFight
		{
			get
			{
				return m_iRangeFight;
			}
			set
			{
				m_iRangeFight = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangeHome
		{
			get
			{
				return m_iRangeHome;
			}
			set
			{
				m_iRangeHome = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double ActiveSpeed
		{
			get
			{
				return m_dActiveSpeed;
			}
			set
			{
				m_dActiveSpeed = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double PassiveSpeed
		{
			get
			{
				return m_dPassiveSpeed;
			}
			set
			{
				m_dPassiveSpeed = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double CurrentSpeed
		{
			get
			{
				return m_dCurrentSpeed;
			}
			set
			{
				if ( m_dCurrentSpeed != value )
				{
					m_dCurrentSpeed = value;

					if (m_AI != null)
						m_AI.OnCurrentSpeedChanged();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Home
		{
			get
			{
				return m_pHome;
			}
			set
			{
				m_pHome = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Controled
		{
			get
			{
				return m_bControled;
			}
			set
			{
				if ( m_bControled == value )
					return;

				m_bControled = value;
				Delta( MobileDelta.Noto );
				InvalidateProperties();
			}
		}

		public override void RevealingAction()
		{
			Spells.Sixth.InvisibilitySpell.RemoveTimer( this );

			base.RevealingAction();
		}

		public void RemoveFollowers()
		{
			/*
			if ( m_ControlMaster != null )
				m_ControlMaster.Followers -= ControlSlots;
			else if ( m_SummonMaster != null )
				m_SummonMaster.Followers -= ControlSlots;

			if ( m_ControlMaster != null && m_ControlMaster.Followers < 0 )
				m_ControlMaster.Followers = 0;

			if ( m_SummonMaster != null && m_SummonMaster.Followers < 0 )
				m_SummonMaster.Followers = 0;
			*/
		}

		public void AddFollowers()
		{
			/*
			if ( m_ControlMaster != null )
				m_ControlMaster.Followers += ControlSlots;
			else if ( m_SummonMaster != null )
				m_SummonMaster.Followers += ControlSlots;
			*/
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ControlMaster
		{
			get
			{
				return m_ControlMaster;
			}
			set
			{
				if ( m_ControlMaster == value )
					return;

				RemoveFollowers();

				m_ControlMaster = value;
				AddFollowers();
				
				Delta( MobileDelta.Noto );
			}
		}

		public virtual void OnTamed( Mobile owner )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile SummonMaster
		{
			get
			{
				return m_SummonMaster;
			}
			set
			{
				if ( m_SummonMaster == value )
					return;

				RemoveFollowers();
				m_SummonMaster = value;
				AddFollowers();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ControlTarget
		{
			get
			{
				return m_ControlTarget;
			}
			set
			{
				m_ControlTarget = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D ControlDest
		{
			get
			{
				return m_ControlDest;
			}
			set
			{
				m_ControlDest = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public OrderType ControlOrder
		{
			get
			{
				return m_ControlOrder;
			}
			set
			{
				m_ControlOrder = value;

				if ( m_AI != null )
					m_AI.OnCurrentOrderChanged();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BardProvoked
		{
			get
			{
				return m_bBardProvoked;
			}
			set
			{
				m_bBardProvoked = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BardPacified
		{
			get
			{
				return m_bBardPacified;
			}
			set
			{
				m_bBardPacified = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile BardMaster
		{
			get
			{
				return m_bBardMaster;
			}
			set
			{
				m_bBardMaster = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile BardTarget
		{
			get
			{
				return m_bBardTarget;
			}
			set
			{
				m_bBardTarget = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime BardEndTime
		{
			get
			{
				return m_timeBardEnd;
			}
			set
			{
				m_timeBardEnd = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double MinTameSkill
		{
			get
			{
				return m_dMinTameSkill;
			}
			set
			{
				m_dMinTameSkill = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Tamable
		{
			get
			{
				return m_bTamable;
			}
			set
			{
				m_bTamable = value;
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Summoned
		{
			get
			{
				return m_bSummoned;
			}
			set
			{
				if ( m_bSummoned == value )
					return;

				m_NextReaquireTime = DateTime.Now;
				m_bSummoned = value;
				Delta( MobileDelta.Noto );
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public int ControlSlots
		{
			get
			{
				return 0;//m_iControlSlots;
			}
			set
			{
				//m_iControlSlots = value;
			}
		}

		public virtual bool NoHouseRestrictions{ get{ return false; } }
		public virtual bool IsHouseSummonable{ get{ return false; } }

		public virtual int Feathers{ get{ return 0; } }
		public virtual int Wool{ get{ return 0; } }

		public virtual MeatType MeatType{ get{ return MeatType.Ribs; } }
		public virtual int Meat{ get{ return 0; } }

		public virtual int Hides{ get{ return 0; } }
		public virtual HideType HideType{ get{ return HideType.Regular; } }

		public virtual int Scales{ get{ return 0; } }
		public virtual ScaleType ScaleType{ get{ return ScaleType.Red; } }

		public virtual bool AutoDispel{ get{ return false; } }

		public virtual bool IsScaryToPets{ get{ return false; } }
		public virtual bool IsScaredOfScaryThings{ get{ return Body.IsAnimal; } }

		public virtual bool CanRummageCorpses{ get{ return Body.IsHuman && ( AlwaysAttackable || AlwaysMurderer ); } }

		public virtual void OnGotMeleeAttack( Mobile attacker )
		{
			if ( AutoDispel && attacker is BaseCreature && ((BaseCreature)attacker).Summoned && !((BaseCreature)attacker).IsAnimatedDead )
				Dispel( attacker );
		}

		private DateTime m_LastAutoDispel;
		public virtual void Dispel( Mobile m )
		{
			if ( m_LastAutoDispel+TimeSpan.FromSeconds( 10.0 ) > DateTime.Now || Mana < 20 )
				return;

			Mana -= 20;
			if ( Mana < 0 )
				Mana = 0;
			m_LastAutoDispel = DateTime.Now;

			int diff = 0;
			if ( m is Daemon )
				diff = 95;
			else if ( m is EnergyVortex )
				diff = 80;
			else if ( m is FireElemental || m is WaterElemental || m is AirElemental || m is EarthElemental )
				diff = 75;
			else if ( m is BladeSpirit )
				diff = 50;

			Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
			if ( this.Skills[SkillName.Magery].Value >= Utility.Random( 41 )+diff )
			{
				Effects.PlaySound( m, m.Map, 0x201 );
				m.Delete();
			}
		}

		public virtual bool DeleteOnRelease{ get{ return m_bSummoned; } }

		public virtual void OnGaveMeleeAttack( Mobile defender )
		{
			Poison p = HitPoison;

			if ( p != null && HitPoisonChance >= Utility.RandomDouble() )
				defender.ApplyPoison( this, p );

			if ( AutoDispel && defender is BaseCreature && ((BaseCreature)defender).Summoned && !((BaseCreature)defender).IsAnimatedDead )
				Dispel( defender );
		}

		public override void OnAfterDelete()
		{
			if ( m_AI != null )
			{
				if ( m_AI.m_Timer != null )
					m_AI.m_Timer.Stop();

				m_AI = null;
			}

			FocusMob = null;

			base.OnAfterDelete();
		}

		public void DebugSay( string text )
		{
			if ( m_bDebugAI )
				this.PublicOverheadMessage( MessageType.Regular, 41, false, text );
		}

		public void DebugSay( string format, params object[] args )
		{
			if ( m_bDebugAI )
				this.PublicOverheadMessage( MessageType.Regular, 41, false, String.Format( format, args ) );
		}

		/*
		 * Will need to be givent a better name
		 * 
		 * This function can be overriden.. so a "Strongest" mobile, can have a different definition depending
		 * on who check for value
		 * -Could add a FightMode.Prefered
		 * 
		 */
		public virtual double GetValueFrom( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			if ( ( bPlayerOnly && m.Player ) ||  !bPlayerOnly )
			{
				switch( acqType )
				{
					case FightMode.Strongest: 
						return m.Str;

					case FightMode.Weakest:
						return -m.Hits;

					case FightMode.Fastest: 
						return m.Dex;

					case FightMode.Smartest:
						return m.Int;

					default : 
						if ( m.Player )//|| PlayerMobile.CheckAggressors( this, m ) )
							return -GetDistanceToSqrt( m ); 
						else
							return -(GetDistanceToSqrt( m )+5); // always attack players first if they are around
				}
			}
			else
			{
				return double.MinValue;
			}
		}

		// Turn, - for let, + for right
		// Basic for now, need works
		public virtual void Turn(int iTurnSteps)
		{
			int v = (int)Direction;

			Direction = (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80));
		}

		public virtual void TurnInternal(int iTurnSteps)
		{
			int v = (int)Direction;

			SetDirection( (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80)) );
		}

		public bool IsHurt()
		{
			return ( Hits != HitsMax );
		}

		public double GetHomeDistance()
		{
			return GetDistanceToSqrt( m_pHome );
		}

		public virtual int GetTeamSize(int iRange)
		{
			int iCount = 0;

			foreach ( Mobile m in this.GetMobilesInRange( iRange ) )
			{
				if (m is BaseCreature)
				{
					if ( ((BaseCreature)m).Team == Team )
					{
						if ( !m.Deleted )
						{
							if ( m != this )
							{
								if ( CanSee( m ) )
								{
									iCount++;
								}
							}
						}
					}
				}
			}
			
			return iCount;
		}

		// Do my combatant is attaking me??
		public bool IsCombatantAnAgressor()
		{
			if (Combatant != null)
			{
				if (Combatant.Combatant == this)
				{
					return true;
				}
			}
			return false;
		}

		private class TameEntry : ContextMenuEntry
		{
			private BaseCreature m_Mobile;

			public TameEntry( Mobile from, BaseCreature creature ) : base( 6130, 6 )
			{
				m_Mobile = creature;

				Enabled = Enabled && ( from.Female ? creature.AllowFemaleTamer : creature.AllowMaleTamer );
			}

			public override void OnClick()
			{
				if ( !Owner.From.CheckAlive() )
					return;

				Owner.From.TargetLocked = true;
				SkillHandlers.AnimalTaming.DisableMessage = true;

				if ( Owner.From.UseSkill( SkillName.AnimalTaming ) )
					Owner.From.Target.Invoke( Owner.From, m_Mobile );

				SkillHandlers.AnimalTaming.DisableMessage = false;
				Owner.From.TargetLocked = false;
			}
		}

		public virtual bool CanTeach{ get{ return false; } }

		public virtual bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !CanTeach )
				return false;

			if ( skill == SkillName.Stealth && from.Skills[SkillName.Hiding].Base < 80.0 )
				return false;

			if ( skill == SkillName.RemoveTrap && (from.Skills[SkillName.Lockpicking].Base < 50.0 || from.Skills[SkillName.DetectHidden].Base < 50.0) )
				return false;

			if ( !Core.AOS && (skill == SkillName.Focus || skill == SkillName.Chivalry || skill == SkillName.Necromancy) )
				return false;

			return true;
		}

		public enum TeachResult
		{
			Success,
			Failure,
			KnowsMoreThanMe,
			KnowsWhatIKnow,
			SkillNotRaisable,
			NotEnoughFreePoints
		}

		public virtual TeachResult CheckTeachSkills( SkillName skill, Mobile m, int maxPointsToLearn, ref int pointsToLearn, bool doTeach )
		{
			if ( !CheckTeach( skill, m ) || !m.CheckAlive() )
				return TeachResult.Failure;

			Skill ourSkill = Skills[skill];
			Skill theirSkill = m.Skills[skill];

			if ( ourSkill == null || theirSkill == null )
				return TeachResult.Failure;

			int baseToSet = ourSkill.BaseFixedPoint / 3;

			if ( baseToSet > 420 )
				baseToSet = 420;
			else if ( baseToSet < 200 )
				return TeachResult.Failure;

			if ( baseToSet > theirSkill.CapFixedPoint )
				baseToSet = theirSkill.CapFixedPoint;

			pointsToLearn = baseToSet - theirSkill.BaseFixedPoint;

			if ( maxPointsToLearn > 0 && pointsToLearn > maxPointsToLearn )
			{
				pointsToLearn = maxPointsToLearn;
				baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
			}

			if ( pointsToLearn < 0 )
				return TeachResult.KnowsMoreThanMe;

			if ( pointsToLearn == 0 )
				return TeachResult.KnowsWhatIKnow;

			//if ( theirSkill.Lock != SkillLock.Up )
			//	return TeachResult.SkillNotRaisable;

			int freePoints = m.Skills.Cap - m.Skills.Total;
			int freeablePoints = 0;

			if ( freePoints < 0 )
				freePoints = 0;

			for ( int i = 0; (freePoints + freeablePoints) < pointsToLearn && i < m.Skills.Length; ++i )
			{
				Skill sk = m.Skills[i];

				if ( sk == theirSkill || sk.Lock != SkillLock.Down )
					continue;

				freeablePoints += sk.BaseFixedPoint;
			}

			if ( (freePoints + freeablePoints) == 0 )
				return TeachResult.NotEnoughFreePoints;

			if ( (freePoints + freeablePoints) < pointsToLearn )
			{
				pointsToLearn = freePoints + freeablePoints;
				baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
			}

			if ( doTeach )
			{
				int need = pointsToLearn - freePoints;

				for ( int i = 0; need > 0 && i < m.Skills.Length; ++i )
				{
					Skill sk = m.Skills[i];

					if ( sk == theirSkill || sk.Lock != SkillLock.Down )
						continue;

					if ( sk.BaseFixedPoint < need )
					{
						need -= sk.BaseFixedPoint;
						sk.BaseFixedPoint = 0;
					}
					else
					{
						sk.BaseFixedPoint -= need;
						need = 0;
					}
				}

				/* Sanity check */
				if ( baseToSet > theirSkill.CapFixedPoint || (m.Skills.Total - theirSkill.BaseFixedPoint + baseToSet) > m.Skills.Cap )
					return TeachResult.NotEnoughFreePoints;

				theirSkill.BaseFixedPoint = baseToSet;
			}

			return TeachResult.Success;
		}

		public virtual bool CheckTeachingMatch( Mobile m )
		{
			if ( m_Teaching == (SkillName)(-1) )
				return false;

			if ( m is PlayerMobile )
				return ( ((PlayerMobile)m).Learning == m_Teaching );

			return true;
		}

		private SkillName m_Teaching = (SkillName)(-1);

		public virtual bool Teach( SkillName skill, Mobile m, int maxPointsToLearn, bool doTeach )
		{
			int pointsToLearn = 0;
			TeachResult res = CheckTeachSkills( skill, m, maxPointsToLearn, ref pointsToLearn, doTeach );

			switch ( res )
			{
				case TeachResult.KnowsMoreThanMe:
				{
					Say( 501508 ); // I cannot teach thee, for thou knowest more than I!
					break;
				}
				case TeachResult.KnowsWhatIKnow:
				{
					Say( 501509 ); // I cannot teach thee, for thou knowest all I can teach!
					break;
				}
				case TeachResult.NotEnoughFreePoints:
				case TeachResult.SkillNotRaisable:
				{
					// Make sure this skill is marked to raise. If you are near the skill cap (700 points) you may need to lose some points in another skill first.
					m.SendLocalizedMessage( 501510, "", 0x22 );
					break;
				}
				case TeachResult.Success:
				{
					if ( doTeach )
					{
						Say( 501539 ); // Let me show thee something of how this is done.
						m.SendLocalizedMessage( 501540 ); // Your skill level increases.

						m_Teaching = (SkillName)(-1);

						if ( m is PlayerMobile )
						{
							((PlayerMobile)m).OnSkillUsed( ((PlayerMobile)m).Learning );
							((PlayerMobile)m).Learning = (SkillName)(-1);
						}
					}
					else
					{
						// I will teach thee all I know, if paid the amount in full.  The price is:
						Say( 1019077, AffixType.Append, String.Format( " {0}", pointsToLearn ), "" );
						Say( 1043108 ); // For less I shall teach thee less.

						m_Teaching = skill;

						if ( m is PlayerMobile )
							((PlayerMobile)m).Learning = skill;
					}

					return true;
				}
			}

			return false;
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );
			
			ForceReaquire();

			if ( m_bControled || m_bSummoned )
			{
				/* // allow owner to attack someone who attacks their pet?
				if ( m_ControlMaster != null )
					m_ControlMaster.AggressiveAction( aggressor, false );
				else if ( m_SummonMaster != null )
					m_SummonMaster.AggressiveAction( aggressor, false );
				*/
				OrderType ct = m_ControlOrder;
				if ( aggressor.ChangingCombatant && (m_bControled || m_bSummoned) && (ct == OrderType.Come || ct == OrderType.Stay || ct == OrderType.Stop || ct == OrderType.None || ct == OrderType.Follow) )
				{
					ControlTarget = aggressor;
					ControlOrder = OrderType.Attack;
				}
			}
		}

        public virtual void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
		}

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries( from, list );

			if ( m_AI != null && Commandable )
				m_AI.GetContextMenuEntries( from, list );

			if ( m_bTamable && !m_bControled && from.Alive )
				list.Add( new TameEntry( from, this ) );

			AddCustomContextEntries( from, list );

			if ( CanTeach && from.Alive )
			{
				Skills ourSkills = this.Skills;
				Skills theirSkills = from.Skills;

				for ( int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i )
				{
					Skill skill = ourSkills[i];
					Skill theirSkill = theirSkills[i];

					if ( skill != null && theirSkill != null && skill.Base >= 60.0 && CheckTeach( skill.SkillName, from ) )
					{
						double toTeach = skill.Base / 3.0;

						if ( toTeach > 42.0 )
							toTeach = 42.0;

						list.Add( new TeachEntry( (SkillName)i, this, from, ( toTeach > theirSkill.Base ) ) );
					}
				}
			}
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && (speechType.Flags & IHSFlags.OnSpeech) != 0 && from.InRange( this, 3 ) )
				return true;

			return ( m_AI != null && m_AI.HandlesOnSpeech( from ) && from.InRange( this, m_iRangePerception ) );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && speechType.OnSpeech( this, e.Mobile, e.Speech ) )
				e.Handled = true;
			else if ( !e.Handled && m_AI != null && e.Mobile.InRange( this, m_iRangePerception ) )
				m_AI.OnSpeech( e );
		}

		public override bool IsHarmfulCriminal( Mobile target )
		{
			if ( (Controled && target == m_ControlMaster) || (Summoned && target == m_SummonMaster) )
				return false;

			if ( target is BaseCreature && ((BaseCreature)target).InitialInnocent )
				return false;

			if ( target is PlayerMobile )
			{
				if ( target.Criminal || target.AccessLevel > AccessLevel.Player )
					return false;// always ok to attack criminals, self, and staff

				Guild fromGuild = this.Guild as Guild;
				Guild targetGuild = target.Guild as Guild;

				if ( fromGuild != null && targetGuild != null )
				{
					if ( fromGuild == targetGuild || fromGuild.IsAlly( targetGuild ) || fromGuild.IsEnemy( targetGuild ) )
						return false; // always ok to attack guild stuffs
				}

				if ( NotorietyHandlers.CheckAggressor( this.Aggressors, target ) || NotorietyHandlers.CheckAggressed( this.Aggressed, target ) )
					return false; // always ok to attack aggressors

				if ( target.Karma <= (int)Noto.Dark )
					return false; // dark/evil/dread are always ok to attack
				else if ( target.Karma <= (int)Noto.Dishonorable ) 
					return PlayerMobile.IsGuarded( target );// its a criminal action to attack dishonorable and dastardly only while they're in town
				else // Innocent
					return true;
			}

			return base.IsHarmfulCriminal( target );
		}

		public override void CriminalAction( bool message )
		{
			base.CriminalAction( message );

			if ( (Controled || Summoned) )
			{
				if ( m_ControlMaster != null && m_ControlMaster.Player )
					m_ControlMaster.CriminalAction( false );
				else if ( m_SummonMaster != null && m_SummonMaster.Player )
					m_SummonMaster.CriminalAction( false );
			}
		}

		public override void DoHarmful( Mobile target, bool indirect )
		{
			base.DoHarmful( target, indirect );

			if ( target == this || target == m_ControlMaster || target == m_SummonMaster || (!Controled && !Summoned) )
				return;

			List<AggressorInfo> list = this.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo ai = (AggressorInfo)list[i];

				if ( ai.Attacker == target )
					return;
			}

			list = this.Aggressed;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo ai = (AggressorInfo)list[i];

				if ( ai.Defender == target )
				{
					if ( m_ControlMaster != null && m_ControlMaster.Player && m_ControlMaster.CanBeHarmful( target, false ) )
						m_ControlMaster.DoHarmful( target, true );
					else if ( m_SummonMaster != null && m_SummonMaster.Player && m_SummonMaster.CanBeHarmful( target, false ) )
						m_SummonMaster.DoHarmful( target, true );

					return;
				}
			}
		}

		public void ReleaseGuardDupeLock()
		{
		}

		public void ReleaseGuardLock()
		{
			EndAction( typeof( GuardedRegion ) );
		}

		private DateTime m_IdleReleaseTime;

		public virtual bool CheckIdle()
		{
			if ( Combatant != null )
				return false; // in combat.. not idling

			if ( m_IdleReleaseTime > DateTime.MinValue )
			{
				// idling...

				if ( DateTime.Now >= m_IdleReleaseTime )
				{
					m_IdleReleaseTime = DateTime.MinValue;
					return false; // idle is over
				}

				return true; // still idling
			}

			if ( 95 > Utility.Random( 100 ) )
				return false; // not idling, but don't want to enter idle state

			m_IdleReleaseTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 15, 25 ) );

			if ( Body.IsHuman )
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: Animate( 5, 5, 1, true,  true, 1 ); break;
					case 1: Animate( 6, 5, 1, true, false, 1 ); break;
				}	
			}
			else if ( Body.IsAnimal )
			{
				switch ( Utility.Random( 3 ) )
				{
					case 0: Animate(  3, 3, 1, true, false, 1 ); break;
					case 1: Animate(  9, 5, 1, true, false, 1 ); break;
					case 2: Animate( 10, 5, 1, true, false, 1 ); break;
				}
			}
			else if ( Body.IsMonster )
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: Animate( 17, 5, 1, true, false, 1 ); break;
					case 1: Animate( 18, 5, 1, true, false, 1 ); break;
				}
			}

			PlaySound( GetIdleSound() );
			return true; // entered idle state
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			if ( ReaquireOnMovement )
				ForceReaquire();

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null )
				speechType.OnMovement( this, m, oldLocation );

			/* Begin notice sound */
			if ( m.Player && m_FightMode != FightMode.Agressor && m_FightMode != FightMode.None && Combatant == null && !Controled && !Summoned )
			{
				// If this creature defends itself but doesn't actively attack (animal) or
				// doesn't fight at all (vendor) then no notice sounds are played..
				// So, players are only notified of agressive monsters

				// Monsters that are currently fighting are ignored

				// Controled or summoned creatures are ignored

				if ( InRange( m.Location, 18 ) && !InRange( oldLocation, 18 ) )
				{
					if ( Body.IsMonster )
						Animate( 11, 5, 1, true, false, 1 );

					PlaySound( GetAngerSound() );
				}
			}
			/* End notice sound */

			/*if ( m_NoDupeGuards == m )
				return;

			if ( !Body.IsHuman || AlwaysMurderer || AlwaysAttackable || m.Karma >= (int)Noto.LowNeutral || !m.InRange( Location, 12 ) || !m.Alive )
				return;

			Region reg = this.Region;

			if ( reg is GuardedRegion )
			{
				GuardedRegion guardedRegion = (GuardedRegion)reg;

				if ( !guardedRegion.IsDisabled() && guardedRegion.IsGuardCandidate( m ) && BeginAction( typeof( GuardedRegion ) ) )
				{
					Say( 1013037 + Utility.Random( 16 ) );
					guardedRegion.CallGuards( this.Location );

					Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( ReleaseGuardLock ) );

					m_NoDupeGuards = m;
					Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ReleaseGuardDupeLock ) );
				}
			}*/
		}


		public void AddSpellAttack( Type type )
		{
			m_arSpellAttack.Add ( type );
		}

		public void AddSpellDefense( Type type )
		{
			m_arSpellDefense.Add ( type );
		}

		public Spell GetAttackSpellRandom()
		{
			if ( m_arSpellAttack.Count > 0 )
			{
				Type type = (Type) m_arSpellAttack[Utility.Random(m_arSpellAttack.Count)];

				object[] args = {this, null};
				return Activator.CreateInstance( type, args ) as Spell;
			}
			else
			{
				return null;
			}
		}

		public Spell GetDefenseSpellRandom()
		{
			if ( m_arSpellDefense.Count > 0 )
			{
				Type type = (Type) m_arSpellDefense[Utility.Random(m_arSpellDefense.Count)];

				object[] args = {this, null};
				return Activator.CreateInstance( type, args ) as Spell;
			}
			else
			{
				return null;
			}
		}

		public Spell GetSpellSpecific( Type type )
		{
			int i;

			for ( i=0; i< m_arSpellAttack.Count; i++ )
			{
				if ( m_arSpellAttack[i] == type )
				{
					object[] args = {this, null};
					return Activator.CreateInstance( type, args ) as Spell;
				}
			}

			for ( i=0; i< m_arSpellDefense.Count; i++ )
			{
				if ( m_arSpellDefense[i] == type )
				{
					object[] args = {this, null};
					return Activator.CreateInstance( type, args ) as Spell;
				}			
			}

			return null;
		}

		public void SetDamage( int val )
		{
			m_DamageMin = val;
			m_DamageMax = val;
		}

		public void SetDamage( int min, int max )
		{
			m_DamageMin = min;
			m_DamageMax = max;
		}

		public void SetHits( int val )
		{
			if ( val < 1000 && !Core.AOS )
				val = (val * 100) / 60;

			m_HitsMax = val;
			Hits = HitsMax;
		}

		public void SetHits( int min, int max )
		{
			if ( min < 1000 && !Core.AOS )
			{
				min = (min * 100) / 60;
				max = (max * 100) / 60;
			}

			m_HitsMax = Utility.RandomMinMax( min, max );
			Hits = HitsMax;
		}

		public void SetStam( int val )
		{
			m_StamMax = val;
			Stam = StamMax;
		}

		public void SetStam( int min, int max )
		{
			m_StamMax = Utility.RandomMinMax( min, max );
			Stam = StamMax;
		}

		public void SetMana( int val )
		{
			m_ManaMax = val;
			Mana = ManaMax;
		}

		public void SetMana( int min, int max )
		{
			m_ManaMax = Utility.RandomMinMax( min, max );
			Mana = ManaMax;
		}

		public void SetStr( int val )
		{
			RawStr = val;
			Hits = HitsMax;
		}

		public void SetStr( int min, int max )
		{
			RawStr = Utility.RandomMinMax( min, max );
			Hits = HitsMax;
		}

		public void SetDex( int val )
		{
			RawDex = val;
			Stam = StamMax;
		}

		public void SetDex( int min, int max )
		{
			RawDex = Utility.RandomMinMax( min, max );
			Stam = StamMax;
		}

		public void SetInt( int val )
		{
			RawInt = val;
			Mana = ManaMax;
		}

		public void SetInt( int min, int max )
		{
			RawInt = Utility.RandomMinMax( min, max );
			Mana = ManaMax;
		}

		public void SetDamageType( ResistanceType type, int min, int max )
		{
			SetDamageType( type, Utility.RandomMinMax( min, max ) );
		}

		public void SetDamageType( ResistanceType type, int val )
		{
			switch ( type )
			{
				case ResistanceType.Physical: m_PhysicalDamage = val; break;
				case ResistanceType.Fire: m_FireDamage = val; break;
				case ResistanceType.Cold: m_ColdDamage = val; break;
				case ResistanceType.Poison: m_PoisonDamage = val; break;
				case ResistanceType.Energy: m_EnergyDamage = val; break;
			}
		}

		public void SetResistance( ResistanceType type, int min, int max )
		{
			SetResistance( type, Utility.RandomMinMax( min, max ) );
		}

		public void SetResistance( ResistanceType type, int val )
		{
			switch ( type )
			{
				case ResistanceType.Physical: m_PhysicalResistance = val; break;
				case ResistanceType.Fire: m_FireResistance = val; break;
				case ResistanceType.Cold: m_ColdResistance = val; break;
				case ResistanceType.Poison: m_PoisonResistance = val; break;
				case ResistanceType.Energy: m_EnergyResistance = val; break;
			}

			UpdateResistances();
		}

		public void SetSkill( SkillName name, double val )
		{
			Skills[name].BaseFixedPoint = (int)(val * 10);
		}

		public void SetSkill( SkillName name, double min, double max )
		{
			int minFixed = (int)(min * 10);
			int maxFixed = (int)(max * 10);

			Skills[name].BaseFixedPoint = Utility.RandomMinMax( minFixed, maxFixed );
		}

		public void SetFameLevel( int level )
		{
			switch ( level )
			{
				case 1: Fame = Utility.RandomMinMax(     0,  1249 ); break;
				case 2: Fame = Utility.RandomMinMax(  1250,  2499 ); break;
				case 3: Fame = Utility.RandomMinMax(  2500,  4999 ); break;
				case 4: Fame = Utility.RandomMinMax(  5000,  9999 ); break;
				case 5: Fame = Utility.RandomMinMax( 10000, 10000 ); break;
			}
		}

		public void SetKarmaLevel( int level )
		{
			switch ( level )
			{
				case 0: Karma = -Utility.RandomMinMax(     0,   624 ); break;
				case 1: Karma = -Utility.RandomMinMax(   625,  1249 ); break;
				case 2: Karma = -Utility.RandomMinMax(  1250,  2499 ); break;
				case 3: Karma = -Utility.RandomMinMax(  2500,  4999 ); break;
				case 4: Karma = -Utility.RandomMinMax(  5000,  9999 ); break;
				case 5: Karma = -Utility.RandomMinMax( 10000, 10000 ); break;
			}
		}

		public static void Cap( ref int val, int min, int max )
		{
			if ( val < min )
				val = min;
			else if ( val > max )
				val = max;
		}

		public void PackPotion()
		{
			PackItem( Loot.RandomPotion() );
		}

		public void PackScroll( int minCircle, int maxCircle )
		{
			PackItem( Loot.RandomScroll( minCircle, maxCircle ) );
		}

		public static void GetRandomAOSStats( int minLevel, int maxLevel, out int attributeCount, out int min, out int max )
		{
			int v = RandomMinMaxScaled( minLevel, maxLevel );

			if ( v >= 5 )
			{
				attributeCount = Utility.RandomMinMax( 2, 6 );
				min = 20; max = 70;
			}
			else if ( v == 4 )
			{
				attributeCount = Utility.RandomMinMax( 2, 4 );
				min = 20; max = 50;
			}
			else if ( v == 3 )
			{
				attributeCount = Utility.RandomMinMax( 2, 3 );
				min = 20; max = 40;
			}
			else if ( v == 2 )
			{
				attributeCount = Utility.RandomMinMax( 1, 2 );
				min = 10; max = 30;
			}
			else
			{
				attributeCount = 1;
				min = 10; max = 20;
			}
		}

		public static int RandomMinMaxScaled( int min, int max )
		{
			if ( min == max )
				return min;

			if ( min > max )
			{
				int hold = min;
				min = max;
				max = hold;
			}

			/* Example:
			 *    min: 1
			 *    max: 5
			 *  count: 5
			 * 
			 * total = (5*5) + (4*4) + (3*3) + (2*2) + (1*1) = 25 + 16 + 9 + 4 + 1 = 55
			 * 
			 * chance for min+0 : 25/55 : 45.45%
			 * chance for min+1 : 16/55 : 29.09%
			 * chance for min+2 :  9/55 : 16.36%
			 * chance for min+3 :  4/55 :  7.27%
			 * chance for min+4 :  1/55 :  1.81%
			 */

			int count = max - min + 1;
			int total = 0, toAdd = count;

			for ( int i = 0; i < count; ++i, --toAdd )
				total += toAdd*toAdd;

			int rand = Utility.Random( total );
			toAdd = count;

			int val = min;

			for ( int i = 0; i < count; ++i, --toAdd, ++val )
			{
				rand -= toAdd*toAdd;

				if ( rand < 0 )
					break;
			}

			return val;
		}

		public virtual Item AddRandomHair()
		{
			Item hair = null;
			switch ( Utility.Random( 8 ) )
			{
				case 0: AddItem( hair = new Afro() ); break;
				case 1: AddItem( hair = new KrisnaHair() ); break;
				case 2: AddItem( hair = new PageboyHair() ); break;
				case 3: AddItem( hair = new PonyTail() ); break;
				case 4: AddItem( hair = new ReceedingHair() ); break;
				case 5: AddItem( hair = new TwoPigTails() ); break;
				case 6: AddItem( hair = new ShortHair() ); break;
				case 7: AddItem( hair = new LongHair() ); break;
			}
			return hair;
		}

		public virtual Item AddRandomFacialHair( int hairHue )
		{
			Item hair = null;
			switch ( Utility.Random( 10 ) ) // 50% chance to not have hair
			{
				case 0: AddItem( hair = new LongBeard( hairHue ) ); break;
				case 1: AddItem( hair = new MediumLongBeard( hairHue ) ); break;
				case 2: AddItem( hair = new Vandyke( hairHue ) ); break;
				case 3: AddItem( hair = new Mustache( hairHue ) ); break;
				case 4: AddItem( hair = new Goatee( hairHue ) ); break;
			}
			return hair;
		}

		public bool PackSlayer()
		{
			if ( 0.10 >= Utility.RandomDouble() )
				return false;

			BaseWeapon weapon = Loot.RandomWeapon();

			if ( weapon != null )
			{
				weapon.Slayer = SlayerName.Silver;//SlayerGroup.GetLootSlayerType( GetType() );
				PackItem( weapon );
			}
			return true;
		}

		public void PackGold( int amount )
		{
			if ( amount > 0 )
				PackItem( new Gold( amount ) );
		}

		public void PackGold( int min, int max )
		{
			PackGold( Utility.RandomMinMax( min, max ) );
		}

		public void PackGem()
		{
			PackGem( 1 );
		}

		public void PackGem( int min, int max )
		{
			PackGem( Utility.RandomMinMax( min, max ) );
		}

		public void PackGem( int amount )
		{
			if ( amount <= 0 )
				return;

			Item gem = Loot.RandomGem();

			gem.Amount = amount;

			PackItem( gem );
		}

		public void PackReg( int min, int max )
		{
			PackReg( Utility.RandomMinMax( min, max ) );
		}

		public void PackReg( int amount )
		{
			if ( amount <= 0 )
				return;

			Item reg = Loot.RandomReagent();

			reg.Amount = amount;

			PackItem( reg );
		}

		public void PackItem( Item item )
		{
			if ( item == null )
			{
				return;
			}
			else if ( Summoned )
			{
				item.Delete();
				return;
			}

			Container pack = Backpack;
			if ( pack == null )
			{
				pack = new Backpack();
				pack.Movable = false;
				AddItem( pack );
			}

			if ( !item.Stackable || !pack.TryDropItem( this, item, false ) ) // try stack
				pack.DropItem( item ); // failed, drop it anyway
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster && !Body.IsHuman )
			{
				Container pack = this.Backpack;

				if ( pack != null )
					pack.DisplayTo( from );
			}

			base.OnDoubleClick( from );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			if ( Controled && Commandable )
			{
				if ( Summoned )
					list.Add( 1049646 ); // (summoned)
				else if ( IsBonded )
					list.Add( 1049608 ); // (bonded)
				else
					list.Add( 502006 ); // (tame)
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Deleted )
				return;

			if ( Mobile.GuildClickMessage )
			{
				Server.Guilds.Guild guild = this.Guild as Server.Guilds.Guild;

				if ( guild != null && this.DisplayGuildTitle )
				{
					string title = GuildTitle;
					string type;

					if ( title == null )
						title = "";
					else
						title = title.Trim();

					if ( guild.Type >= 0 && (int)guild.Type < PlayerMobile.m_GuildTypes.Length )
						type = PlayerMobile.m_GuildTypes[(int)guild.Type];
					else
						type = "";

					string text = String.Format( title.Length <= 0 ? "[{1}]{2}" : "[{0}, {1}]{2}", title, guild.Abbreviation, type );

					PrivateOverheadMessage( MessageType.Regular, SpeechHue, true, text, from.NetState );
				}
			}

			int hue;

			if ( NameHue != -1 )
				hue = NameHue;
			else if ( AccessLevel > AccessLevel.Player )
				hue = 11;
			else
				hue = Notoriety.GetHue( Notoriety.Compute( from, this ) );

			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//if ( ShowFameTitle && Body.IsHuman && ( Karma >= (int)Noto.LordLady || Karma <= (int)Noto.Dark ) )
			//	sb.Append( Female ? "Lady " : "Lord " );
			
			sb.Append( Name );

			if ( ClickTitle && Title != null && Title.Length > 0 )
			{
				sb.Append( ' ' );
				sb.Append( Title );
			}
			
			if ( Controled && Commandable && !Body.IsHuman )
				sb.Append( " (tame)" );
			if ( Frozen || Paralyzed )
				sb.Append( " (frozen)" );
			if ( Blessed )
				sb.Append( " (invulnerable)" );

			PrivateOverheadMessage( MessageType.Label, hue, Mobile.AsciiClickMessage, sb.ToString(), from.NetState );
		}

		public virtual int TreasureMapLevel{ get{ return 0; } }

		public override bool OnBeforeDeath()
		{
			/*int treasureLevel = TreasureMapLevel;

			if ( !Summoned && !NoKillAwards && !IsBonded && treasureLevel > 0 && (Map == Map.Felucca || Map == Map.Trammel) && TreasureMap.LootChance >= Utility.RandomDouble() )
				PackItem( new TreasureMap( treasureLevel, Map ) );

			if ( !Summoned && !NoKillAwards && !m_HasGeneratedLoot )
			{
				m_HasGeneratedLoot = true;
				GenerateLoot( false );
			}

			if ( !NoKillAwards && Region.Name == "Doom" )
			{
				int bones = Engines.Quests.Doom.TheSummoningQuest.GetDaemonBonesFor( this );

				if ( bones > 0 )
					PackItem( new DaemonBone( bones ) );
			}

			if ( IsAnimatedDead )
				Effects.SendLocationEffect( Location, Map, 0x3728, 13, 1, 0x461, 4 );
			*/
			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null )
				speechType.OnDeath( this );

			return base.OnBeforeDeath();
		}

		private bool m_NoKillAwards;

		public bool NoKillAwards
		{
			get{ return m_NoKillAwards; }
			set{ m_NoKillAwards = value; }
		}

		public int ComputeBonusDamage( ArrayList list, Mobile m )
		{
			int bonus = 0;

			for ( int i = list.Count - 1; i >= 0; --i )
			{
				DamageEntry de = (DamageEntry)list[i];

				if ( de.Damager == m || !(de.Damager is BaseCreature) )
					continue;

				BaseCreature bc = (BaseCreature)de.Damager;
				Mobile master = null;

				if ( bc.Controled && bc.ControlMaster != null )
					master = bc.ControlMaster;
				else if ( bc.Summoned && bc.SummonMaster != null )
					master = bc.SummonMaster;

				if ( master == m )
					bonus += de.DamageGiven;
			}

			return bonus;
		}

		private class FKEntry
		{
			public Mobile m_Mobile;
			public int m_Damage;

			public FKEntry( Mobile m, int damage )
			{
				m_Mobile = m;
				m_Damage = damage;
			}
		}

		public static ArrayList GetLootingRights( List<DamageEntry> damageEntries )
		{
			ArrayList rights = new ArrayList();

			for ( int i = damageEntries.Count - 1; i >= 0; --i )
			{
				if ( i >= damageEntries.Count )
					continue;

				DamageEntry de = (DamageEntry)damageEntries[i];

				if ( de.HasExpired )
				{
					damageEntries.RemoveAt( i );
					continue;
				}

				Mobile m = de.Damager;

				if ( m is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)m;

					if ( bc.Controled && bc.ControlMaster != null )
						m = bc.ControlMaster;
					else if ( bc.Summoned && bc.SummonMaster != null )
						m = bc.SummonMaster;
				}

				if ( m == null || m.Deleted || !m.Player )
					continue;

				int damage = de.DamageGiven;

				if ( damage <= 0 )
					continue;

				bool needNewEntry = true;

				for ( int j = 0; needNewEntry && j < rights.Count; ++j )
				{
					DamageStore ds = (DamageStore)rights[j];

					if ( ds.m_Mobile == m )
					{
						ds.m_Damage += damage;
						needNewEntry = false;
					}
				}

				if ( needNewEntry )
					rights.Add( new DamageStore( m, damage ) );
			}

			if ( rights.Count > 0 )
			{
				if ( rights.Count > 1 )
					rights.Sort();

				int topDamage = ((DamageStore)rights[0]).m_Damage;
				int minDamage = (topDamage * 70) / 100;

				for ( int i = 0; i < rights.Count; ++i )
				{
					DamageStore ds = (DamageStore)rights[i];

					ds.m_HasRight = ( ds.m_Damage >= minDamage );
				}
			}

			return rights;
		}

		public override void OnDeath( Container c )
		{
			if ( !Summoned && !m_NoKillAwards )
			{
				// note creatures with neg karma GIVE us karma, so these numbers are negated
				ArrayList list = GetLootingRights( this.DamageEntries );
				for ( int i = 0; i < list.Count; ++i )
				{
					DamageStore ds = (DamageStore)list[i];
					if ( !ds.m_HasRight )
						continue;
					Mobile mob = ds.m_Mobile;

					if ( mob is BaseCreature )
					{
						BaseCreature bc = (BaseCreature)mob;
						if ( bc.Controled && bc.ControlMaster != null )
							mob = bc.ControlMaster;
						else if ( bc.Summoned && bc.SummonMaster != null )
							mob = bc.SummonMaster;
					}

					int noto = Notoriety.Compute( mob, this );
					if ( noto == Notoriety.Innocent )
						Titles.AlterNotoriety( mob, -8 );
					else if ( noto == Notoriety.Murderer )
						Titles.AlterNotoriety( mob, 1 );
					// cap the gain/loss at the npc's karma?? (so killing a 0 karma npc only takes you to 0)
				}
			}

			base.OnDeath( c );
			if ( DeleteCorpseOnDeath )
			{
				c.Delete();
			}
			else if ( this is WaterElemental || this is FireElemental || this is AirElemental || this is PoisonElemental || this is BloodElemental )
			{
				Backpack pack = new Backpack();
				this.Corpse = pack;
				pack.MoveToWorld( c.Location, c.Map );
				while ( c.Items.Count > 0 )
					pack.DropItem( (Item)c.Items[0] );
				c.Delete();
			}
		}

		/* To save on cpu usage, RunUO creatures only reaquire creatures under the following circumstances:
		 *  - 10 seconds have elapsed since the last time it tried
		 *  - The creature was attacked
		 *  - Some creatures, like dragons, will reaquire when they see someone move
		 * 
		 * This functionality appears to be implemented on OSI as well
		 */

		private DateTime m_NextReaquireTime;

		public DateTime NextReaquireTime{ get{ return m_NextReaquireTime; } set{ m_NextReaquireTime = value; } }

		public virtual TimeSpan ReaquireDelay{ get{ return TimeSpan.FromSeconds( 5.0 ); } }
		public virtual bool ReaquireOnMovement{ get{ return false; } }

		public void ForceReaquire()
		{
			m_NextReaquireTime = DateTime.MinValue;
		}

		public override void OnDelete()
		{
			SetControlMaster( null );
			SummonMaster = null;

			base.OnDelete();
		}

		public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
		{
			if ( (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier )
			{
				if ( message )
				{
					if ( target.Title == null )
						SendAsciiMessage( "{0} the vendor cannot be harmed.", target.Name );
					else
						SendAsciiMessage( "{0} {1} cannot be harmed.", target.Name, target.Title );
				}

				return false;
			}

			return base.CanBeHarmful( target, message, ignoreOurBlessedness );
		}

		public override bool CanBeRenamedBy( Mobile from )
		{
			bool ret = base.CanBeRenamedBy( from );

			if ( Controled && from == ControlMaster )
				ret = true;

			return ret;
		}

		public bool SetControlMaster( Mobile m )
		{
			if ( AIObject != null )
				AIObject.NumCommands = 0;

			if ( m == null )
			{
				ControlMaster = null;
				Controled = false;
				ControlTarget = null;
				ControlOrder = OrderType.None;
				Guild = null;
			}
			else
			{
				/*if ( m.Followers + ControlSlots > m.FollowersMax )
				{
					m.SendLocalizedMessage( 1049607 ); // You have too many followers to control that creature.
					return false;
				}*/

				CurrentWayPoint = null;//so tamed animals don't try to go back
			
				ControlMaster = m;
				Controled = true;
				ControlTarget = null;
				ControlOrder = OrderType.Come;
				Owners.Add( m );
				Guild = null;
			}

			Delta( MobileDelta.Noto );
			return true;
		}

		private static bool m_Summoning;

		public static bool Summoning
		{
			get{ return m_Summoning; }
			set{ m_Summoning = value; }
		}

		public static bool Summon( BaseCreature creature, Mobile caster, Point3D p, int sound, TimeSpan duration )
		{
			return Summon( creature, true, caster, p, sound, duration );
		}

		public static bool Summon( BaseCreature creature, bool controled, Mobile caster, Point3D p, int sound, TimeSpan duration )
		{
			/*if ( caster.Followers + creature.ControlSlots > caster.FollowersMax )
			{
				caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				creature.Delete();
				return false;
			}*/

			m_Summoning = true;

			if ( controled )
				creature.SetControlMaster( caster );

			creature.RangeHome = 10;
			creature.Summoned = true;

			creature.SummonMaster = caster;

			Container pack = creature.Backpack;

			if ( pack != null )
			{
				for ( int i = pack.Items.Count - 1; i >= 0; --i )
				{
					if ( i >= pack.Items.Count )
						continue;

					((Item)pack.Items[i]).Delete();
				}
			}

			new UnsummonTimer( caster, creature, duration ).Start();
			creature.m_SummonEnd = DateTime.Now + duration;

			creature.MoveToWorld( p, caster.Map );

			Effects.PlaySound( p, creature.Map, sound );

			m_Summoning = false;

			return true;
		}

		private static bool EnableRummaging = true;

		private const double ChanceToRummage = 0.5; // 50%

		private const double MinutesToNextRummageMin = 1.0;
		private const double MinutesToNextRummageMax = 4.0;

		private const double MinutesToNextChanceMin = 0.25;
		private const double MinutesToNextChanceMax = 0.75;

		private DateTime m_NextRummageTime;

		public virtual void OnThink()
		{
			if ( EnableRummaging && CanRummageCorpses && !Summoned && !Controled && DateTime.Now >= m_NextRummageTime )
			{
				double min, max;

				if ( ChanceToRummage > Utility.RandomDouble() && Rummage() )
				{
					min = MinutesToNextRummageMin;
					max = MinutesToNextRummageMax;
				}
				else
				{
					min = MinutesToNextChanceMin;
					max = MinutesToNextChanceMax;
				}

				double delay = min + (Utility.RandomDouble() * (max - min));
				m_NextRummageTime = DateTime.Now + TimeSpan.FromMinutes( delay );
			}

			if ( HasBreath && !Summoned && DateTime.Now >= m_NextBreathTime ) // tested: controled dragons do breath fire, what about summoned skeletal dragons?
			{
				Mobile target = this.Combatant;

				if ( target != null && target.Alive && CanBeHarmful( target ) && target.Map == this.Map && target.InRange( this, BreathRange ) && InLOS( target ) )
					BreathStart( target );

				m_NextBreathTime = DateTime.Now + TimeSpan.FromSeconds( BreathMinDelay + (Utility.RandomDouble() * BreathMaxDelay) );
			}
		}

		public virtual bool Rummage()
		{
			Corpse toRummage = null;

			foreach ( Item item in this.GetItemsInRange( 2 ) )
			{
				if ( item is Corpse && item.Items.Count > 0 )
				{
					toRummage = (Corpse)item;
					break;
				}
			}

			if ( toRummage == null )
				return false;

			Container pack = this.Backpack;

			if ( pack == null )
				return false;

			List<Item> items = toRummage.Items;

			bool rejected;
			LRReason reason;

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = (Item)items[Utility.Random( items.Count )];

				Lift( item, item.Amount, out rejected, out reason );

				if ( !rejected && Drop( this, new Point3D( -1, -1, 0 ) ) )
				{
					// *rummages through a corpse and takes an item*
					PublicOverheadMessage( MessageType.Emote, 0x3B2, 1008086 );
					return true;
				}
			}

			return false;
		}

		public void Pacify( Mobile master, DateTime endtime )
		{
			BardPacified = true;
			BardEndTime = endtime;
			if ( AIObject != null )
				AIObject.Action = ActionType.Wander;
			NextReaquireTime = endtime + ReaquireDelay;
		}

		public override Mobile GetDamageMaster( Mobile damagee )
		{
			if ( m_bBardProvoked && damagee == m_bBardTarget )
				return m_bBardMaster;

			return base.GetDamageMaster( damagee );
		}
 
		public void Provoke( Mobile master, Mobile target, bool bSuccess )
		{
			// BardProvoked = true;

			this.PublicOverheadMessage( MessageType.Emote, EmoteHue, false, "*looks furious*" );

			PlaySound( GetAngerSound() );

			if ( bSuccess )
			{
				master.DoHarmful( this, true );
				master.DoHarmful( target, true );

				Attack( target );
				target.Attack( this );

				AIObject.Action = ActionType.Combat;
				NextReaquireTime = DateTime.Now + TimeSpan.FromSeconds( 10.0 ) + ReaquireDelay;
				BardEndTime = DateTime.Now + TimeSpan.FromSeconds( 30 );
				BardProvoked = true;
				BardMaster = master;
				BardTarget = target;

				if ( target is BaseCreature )
				{
					BaseCreature t = (BaseCreature)target;

					t.AIObject.Action = ActionType.Combat;
					t.NextReaquireTime = DateTime.Now + TimeSpan.FromSeconds( 10.0 ) + t.ReaquireDelay;
					t.BardEndTime = DateTime.Now + TimeSpan.FromSeconds( 30 );
					t.BardProvoked = true;
					t.BardMaster = master;
					t.BardTarget = this;
				}
 
				/*BardMaster = master;
				BardTarget = target;
				Combatant = target;
				BardEndTime = DateTime.Now + TimeSpan.FromMinutes( 0.5 );

				if ( target is BaseCreature )
				{
					

					t.BardProvoked = true;

					t.BardMaster = master;
					t.BardTarget = this;
					t.Combatant = this;
					t.BardEndTime = DateTime.Now + TimeSpan.FromMinutes( 0.5 );
				}*/
			}
		}

		public bool FindMyName( string str, bool bWithAll )
		{
			int i, j;

			string name = this.Name;
 
			if( name == null || str.Length < name.Length )
				return false;
 
			string[] wordsString = str.Split(' ');
			string[] wordsName = name.Split(' ');
 
			for ( j=0 ; j < wordsName.Length; j++ )
			{
				string wordName = wordsName[j];
 
				bool bFound = false;
				for ( i=0 ; i < wordsString.Length; i++ )
				{
					string word = wordsString[i];

					if ( Insensitive.Equals( word, wordName ) )
						bFound = true;
 
					if ( bWithAll && Insensitive.Equals( word, "all" ) )
						return true;
				}
 
				if ( !bFound )
					return false;
			}
 
			return true;
		}

		public static void TeleportPets( Mobile master, Point3D loc, Map map )
		{
			TeleportPets( master, loc, map, false );
		}

		public static void TeleportPets( Mobile master, Point3D loc, Map map, bool onlyBonded )
		{
			ArrayList move = new ArrayList();

			foreach ( Mobile m in master.GetMobilesInRange( 3 ) )
			{
				if ( m is BaseCreature )
				{
					BaseCreature pet = (BaseCreature)m;

					if ( pet.Controled && pet.ControlMaster == master )
					{
						if ( !onlyBonded || pet.IsBonded )
						{
							if ( pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come )
								move.Add( pet );
						}
					}
				}
			}

			foreach ( Mobile m in move )
				m.MoveToWorld( loc, map );
		}

		public virtual void ResurrectPet()
		{
			if ( !IsDeadPet )
				return;

			OnBeforeResurrect();

			Poison = null;

			Warmode = false;

			Hits = 10;
			Stam = StamMax;
			Mana = 0;

			ProcessDeltaQueue();

			IsDeadPet = false;

			Effects.SendPacket( Location, Map, new BondedStatus( 0, this.Serial, 0 ) );

			this.SendIncomingPacket();
			this.SendIncomingPacket();

			OnAfterResurrect();

			Mobile owner = this.ControlMaster;

			if ( owner == null || owner.Deleted || owner.Map != this.Map || !owner.InRange( this, 12 ) || !this.CanSee( owner ) || !this.InLOS( owner ) )
			{
				if ( this.OwnerAbandonTime == DateTime.MinValue )
					this.OwnerAbandonTime = DateTime.Now;
			}
			else
			{
				this.OwnerAbandonTime = DateTime.MinValue;
			}
		}

		public override bool CanBeDamaged()
		{
			if ( IsDeadPet )
				return false;

			return base.CanBeDamaged();
		}

		public virtual bool PlayerRangeSensitive
		{ 
			get
			{ 
				return m_CurrentWayPoint == null; 
			} 
		}

		public override void OnSectorDeactivate()
		{
			if ( PlayerRangeSensitive && m_AI != null && ( Combatant == null || Combatant.Deleted || !InRange( Combatant.Location, RangePerception ) ) )
				m_AI.Deactivate();

			base.OnSectorDeactivate();
		}

		public override void OnSectorActivate()
		{
			if ( PlayerRangeSensitive && m_AI != null )
				m_AI.Activate();

			base.OnSectorActivate();
		}

		public override void OnHarmfulAction(Mobile target, bool isCriminal)
		{
			if ( target != this && !(this is BaseGuard) && !PlayerMobile.CheckAggressors( this, target ) && !PlayerMobile.CheckAggressors( target, this ) )
			{
				IPooledEnumerable eable = GetClientsInRange( 13 );
				Packet p = null;
				foreach ( NetState ns in eable )
				{
					Mobile m = ns.Mobile;
					if ( m != null && m.CanSee( this ) && m != target && m != this )
					{
						if ( p == null )
						{
							p = new AsciiMessage( Serial.MinusOne, -1, MessageType.Regular, 0x3b2, 3, "System", String.Format( "You see {0} attacking {1}!", this.Name, target.Name ) );
							p.SetStatic();
						}
						ns.Send( p );
					}
				}
				eable.Free();
				Packet.Release( ref p );
			}

			base.OnHarmfulAction( target, isCriminal );
		}

		public override int GetDeathSound()
		{
			if ( this.Body.IsFemale )
				return Utility.Random( 4 ) + 0x150;
			else if ( this.Body.IsMale )
				return Utility.Random( 4 ) + 0x15A;
			else
				return base.GetDeathSound();
		}

		public override int GetHurtSound()
		{
			if ( this.Body.IsFemale )
				return Utility.Random( 5 ) + 0x14B;
			else if ( this.Body.IsMale )
				return Utility.Random( 5 ) + 0x154;
			else
				return base.GetHurtSound();
		}


		// used for deleting creatures in houses
		private int m_RemoveStep; 

		[CommandProperty( AccessLevel.GameMaster )] 
		public int RemoveStep { get { return m_RemoveStep; } set { m_RemoveStep = value; } }
	}

	public class LoyaltyTimer : Timer
	{
		private static TimeSpan InternalDelay = TimeSpan.FromMinutes( 15.0 );

		public static void Initialize()
		{
			new LoyaltyTimer().Start();
		}

		public LoyaltyTimer() : base( InternalDelay, InternalDelay )
		{
			m_NextCheck = DateTime.Now + TimeSpan.FromHours( 0.5 );
			Priority = TimerPriority.OneMinute;
		}

		private DateTime m_NextCheck;

		protected override void OnTick() 
		{ 
			bool hasElapsed = ( DateTime.Now >= m_NextCheck );

			if ( hasElapsed )
				m_NextCheck = DateTime.Now + TimeSpan.FromHours( 1 );

			ArrayList toRelease = new ArrayList();

			// added array for wild creatures in house regions to be removed
			ArrayList toRemove = new ArrayList();

			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature c = (BaseCreature)m;

					if ( c.IsStabled )
						continue;

					if ( c.Controled && c.Commandable && c.Loyalty > PetLoyalty.None )
					{
						Mobile owner = c.ControlMaster;

						if ( hasElapsed && c.AIObject != null )
							c.AIObject.NumCommands = 0;

						bool dec = hasElapsed || owner == null || owner.Deleted || !owner.InRange( c, 15 );
						// changed loyalty decrement
						if ( !dec && c.Map != Map.Internal && !c.CanSee( owner ) && owner.Map != Map.Internal && !(c.Region is HouseRegion) )
							dec = true;

						if ( dec && c.Map == Map.Internal && ( owner.Stabled.Contains( c ) || owner.Map == Map.Internal ) )
							dec = false;

						if ( dec )
						{
							--c.Loyalty;
							if ( c.Loyalty == PetLoyalty.Confused && c.Map != Map.Internal )
							{
								//if ( c is BaseMount )
								//	((BaseMount)c).Rider = null;
								c.Say( 1043270, c.Name ); // * ~1_NAME~ looks around desperately *
								c.PlaySound( c.GetIdleSound() );
							}
							else if ( c.Loyalty == PetLoyalty.None && !c.Body.IsHuman )
							{
								toRelease.Add( c );
							}
						}
					}

					// added lines to check if a wild creature in a house region has to be removed or not
					if ( !c.Controled && c.Region is HouseRegion && c.CanBeDamaged() && c.CurrentWayPoint == null )
					{
						c.RemoveStep++;

						if ( c.RemoveStep >= 20 )
							toRemove.Add( c );
					}
					else
					{
						c.RemoveStep = 0;
					}
				}
			}

			foreach ( BaseCreature c in toRelease )
			{
				if ( c is BaseMount )
					((BaseMount)c).Rider = null;
				c.Loyalty = PetLoyalty.WonderfullyHappy;
				c.IsBonded = false;
				c.BondingBegin = DateTime.MinValue;
				c.OwnerAbandonTime = DateTime.MinValue;
				c.ControlTarget = null;
				c.Say( 1043255, c.Name ); // ~1_NAME~ appears to have decided that is better off without a master!
				c.AIObject.DoOrderRelease();
			}

			// added code to handle removing of wild creatures in house regions
			foreach ( BaseCreature c in toRemove )
			{
				c.Delete();
			}
		}
	}
}
