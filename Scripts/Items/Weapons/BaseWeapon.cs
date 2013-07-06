using System;
using System.Text;
using System.Collections; using System.Collections.Generic;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseWeapon : BaseItem, IWeapon, IIdentifiable
	{
		/* Weapon internals work differently now (Mar 13 2003)
		 * 
		 * The attributes defined below default to -1.
		 * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
		 * If not, the attribute value itself is used. Here's the list:
		 *  - MinDamage
		 *  - MaxDamage
		 *  - Speed
		 *  - HitSound
		 *  - MissSound
		 *  - StrRequirement, DexRequirement, IntRequirement
		 *  - WeaponType
		 *  - WeaponAnimation
		 *  - MaxRange
		 */

		// Instance values. These values must are unique to each weapon.
		private WeaponDamageLevel m_DamageLevel;
		private WeaponAccuracyLevel m_AccuracyLevel;
		private DurabilityLevel m_DurabilityLevel;
		private CraftQuality m_Quality;
		private Mobile m_Crafter;
		private Poison m_Poison;
		private int m_PoisonCharges;
		private double m_PoisonChance;
		//private bool m_Identified;
		private ArrayList m_Identified = new ArrayList();
		private int m_Hits;
		private int m_MaxHits;
		private SlayerName m_Slayer;
		private SkillMod m_SkillMod, m_MageMod;
		private CraftResource m_Resource;
		private bool m_PlayerConstructed;

		protected virtual SkillName AccuracyMod { get{ return SkillName.Tactics; } }

		private SpellEffect m_Effect;
		private int m_EffectCharges;

		[CommandProperty( AccessLevel.GameMaster )]
		public SpellEffect SpellEffect
		{
			get { return m_Effect; }
			set { m_Effect = value; SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpellCharges
		{
			get { return m_EffectCharges; }
			set { m_EffectCharges = value; SingleClickChanged(); }
		}

		private bool m_Cursed; // Is this weapon cursed via Curse Weapon necromancer spell? Temporary; not serialized.
		private bool m_Consecrated; // Is this weapon blessed via Consecrate Weapon paladin ability? Temporary; not serialized.

		private AosAttributes m_AosAttributes;
		private AosWeaponAttributes m_AosWeaponAttributes;

		// Overridable values. These values are provided to override the defaults which get defined in the individual weapon scripts.
		private int m_StrReq, m_DexReq, m_IntReq;
		private int m_MinDamage, m_MaxDamage;
		private int m_HitSound, m_MissSound;
		private int m_Speed;
		private int m_MaxRange;
		private SkillName m_Skill;
		private WeaponType m_Type;
		private WeaponAnimation m_Animation;

		public virtual WeaponAbility PrimaryAbility{ get{ return null; } }
		public virtual WeaponAbility SecondaryAbility{ get{ return null; } }

		public virtual int DefMaxRange{ get{ return 1; } }
		public virtual int DefHitSound{ get{ return 0; } }
		public virtual int DefMissSound{ get{ return 0; } }
		public virtual SkillName DefSkill{ get{ return SkillName.Swords; } }
		public virtual WeaponType DefType{ get{ return WeaponType.Slashing; } }
		public virtual WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash1H; } }

		public virtual int AosStrengthReq{ get{ return 0; } }
		public virtual int AosDexterityReq{ get{ return 0; } }
		public virtual int AosIntelligenceReq{ get{ return 0; } }
		public virtual int AosMinDamage{ get{ return 0; } }
		public virtual int AosMaxDamage{ get{ return 0; } }
		public virtual int AosSpeed{ get{ return 0; } }
		public virtual int AosMaxRange{ get{ return DefMaxRange; } }
		public virtual int AosHitSound{ get{ return DefHitSound; } }
		public virtual int AosMissSound{ get{ return DefMissSound; } }
		public virtual SkillName AosSkill{ get{ return DefSkill; } }
		public virtual WeaponType AosType{ get{ return DefType; } }
		public virtual WeaponAnimation AosAnimation{ get{ return DefAnimation; } }

		public virtual int OldStrengthReq{ get{ return 0; } }
		public virtual int OldDexterityReq{ get{ return 0; } }
		public virtual int OldIntelligenceReq{ get{ return 0; } }
		
		public virtual int OldMinDamage
		{ 
			get
			{ 
				if ( this.NumDice > 0 )
					return this.NumDice + this.DiceBonus;
				else
					return 0; 
			} 
		}
		public virtual int OldMaxDamage
		{ 
			get
			{ 
				if ( this.NumDice > 0 )
					return this.NumDice*this.NumSides + this.DiceBonus;
				else
					return 0; 
			} 
		}

		public virtual int OldSpeed{ get{ return 0; } }
		public virtual int OldMaxRange{ get{ return DefMaxRange; } }
		public virtual int OldHitSound{ get{ return DefHitSound; } }
		public virtual int OldMissSound{ get{ return DefMissSound; } }
		public virtual SkillName OldSkill{ get{ return DefSkill; } }
		public virtual WeaponType OldType{ get{ return DefType; } }
		public virtual WeaponAnimation OldAnimation{ get{ return DefAnimation; } }

		public virtual int InitMinHits{ get{ return 0; } }
		public virtual int InitMaxHits{ get{ return 0; } }

		public override int PhysicalResistance{ get{ return m_AosWeaponAttributes.ResistPhysicalBonus; } }
		public override int FireResistance{ get{ return m_AosWeaponAttributes.ResistFireBonus; } }
		public override int ColdResistance{ get{ return m_AosWeaponAttributes.ResistColdBonus; } }
		public override int PoisonResistance{ get{ return m_AosWeaponAttributes.ResistPoisonBonus; } }
		public override int EnergyResistance{ get{ return m_AosWeaponAttributes.ResistEnergyBonus; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosWeaponAttributes WeaponAttributes
		{
			get{ return m_AosWeaponAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Cursed
		{
			get{ return m_Cursed; }
			set{ m_Cursed = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Consecrated
		{
			get{ return m_Consecrated; }
			set{ m_Consecrated = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Hits
		{
			get{ return m_Hits; }
			set
			{
				if ( m_Hits == value )
					return;

				m_Hits = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHits
		{
			get{ return m_MaxHits; }
			set{ m_MaxHits = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonCharges
		{
			get{ return m_PoisonCharges; }
			set{ m_PoisonCharges = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double PoisonChance
		{
			get{ return m_PoisonChance; }
			set{ m_PoisonChance = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get{ return m_Poison; }
			set{ m_Poison = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftQuality Quality
		{
			get{ return m_Quality; }
			set{ UnscaleDurability(); m_Quality = value; ScaleDurability(); InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer
		{
			get{ return m_Slayer; }
			set{ m_Slayer = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ UnscaleDurability(); m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); InvalidateProperties(); ScaleDurability(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponDamageLevel DamageLevel
		{
			get{ return m_DamageLevel; }
			set{ m_DamageLevel = value; InvalidateProperties(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DurabilityLevel DurabilityLevel
		{
			get{ return m_DurabilityLevel; }
			set{ UnscaleDurability(); m_DurabilityLevel = value; InvalidateProperties(); ScaleDurability(); SingleClickChanged(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed
		{
			get{ return m_PlayerConstructed; }
			set{ m_PlayerConstructed = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxRange
		{
			get{ return ( m_MaxRange == -1 ? Core.AOS ? AosMaxRange : OldMaxRange : m_MaxRange ); }
			set{ m_MaxRange = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponAnimation Animation
		{
			get{ return ( m_Animation == (WeaponAnimation)(-1) ? Core.AOS ? AosAnimation : OldAnimation : m_Animation ); } 
			set{ m_Animation = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponType Type
		{
			get{ return ( m_Type == (WeaponType)(-1) ? Core.AOS ? AosType : OldType : m_Type ); }
			set{ m_Type = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get{ return ( m_Skill == (SkillName)(-1) ? Core.AOS ? AosSkill : OldSkill : m_Skill ); }
			set{ m_Skill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitSound
		{
			get{ return ( m_HitSound == -1 ? Core.AOS ? AosHitSound : OldHitSound : m_HitSound ); }
			set{ m_HitSound = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MissSound
		{
			get{ return ( m_MissSound == -1 ? Core.AOS ? AosMissSound : OldMissSound : m_MissSound ); }
			set{ m_MissSound = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MinDamage
		{
			get{ return ( m_MinDamage == -1 ? Core.AOS ? AosMinDamage : OldMinDamage : m_MinDamage ); }
			set{ m_MinDamage = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxDamage
		{
			get{ return ( m_MaxDamage == -1 ? Core.AOS ? AosMaxDamage : OldMaxDamage : m_MaxDamage ); }
			set{ m_MaxDamage = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Speed
		{
			get{ return ( m_Speed == -1 ? Core.AOS ? AosSpeed : OldSpeed : m_Speed ); }
			set{ m_Speed = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get{ return ( m_StrReq == -1 ? Core.AOS ? AosStrengthReq : OldStrengthReq : m_StrReq ); }
			set{ m_StrReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DexRequirement
		{
			get{ return ( m_DexReq == -1 ? Core.AOS ? AosDexterityReq : OldDexterityReq : m_DexReq ); }
			set{ m_DexReq = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int IntRequirement
		{
			get{ return ( m_IntReq == -1 ? Core.AOS ? AosIntelligenceReq : OldIntelligenceReq : m_IntReq ); }
			set{ m_IntReq = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponAccuracyLevel AccuracyLevel
		{
			get
			{
				return m_AccuracyLevel;
			}
			set
			{
				if ( m_AccuracyLevel != value )
				{
					m_AccuracyLevel = value;

					if ( UseSkillMod )
					{
						if ( m_AccuracyLevel == WeaponAccuracyLevel.Regular )
						{
							if ( m_SkillMod != null )
								m_SkillMod.Remove();

							m_SkillMod = null;
						}
						else if ( m_SkillMod == null && Parent is Mobile )
						{
							m_SkillMod = new DefaultSkillMod( AccuracyMod, true, (int)m_AccuracyLevel * 5 );
							((Mobile)Parent).AddSkillMod( m_SkillMod );
						}
						else if ( m_SkillMod != null )
						{
							m_SkillMod.Value = (int)m_AccuracyLevel * 5;
						}
					}

					InvalidateProperties();
					SingleClickChanged();
				}
			}
		}

		public void UnscaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_Hits = (m_Hits * 100) / scale;
			m_MaxHits = (m_MaxHits * 100) / scale;
			InvalidateProperties();
		}

		public void ScaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_Hits = (m_Hits * scale) / 100;
			m_MaxHits = (m_MaxHits * scale) / 100;
			InvalidateProperties();
		}

		public int GetDurabilityBonus()
		{
			int bonus = 0;
			bonus = ((int)m_Quality - 1)*5;
			bonus += 10*(int)m_DurabilityLevel;
			return bonus;
		}

		public int GetLowerStatReq()
		{
			if ( !Core.AOS )
				return 0;

			int v = m_AosWeaponAttributes.LowerStatReq;

			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info != null )
			{
				CraftAttributeInfo attrInfo = info.AttributeInfo;

				if ( attrInfo != null )
					v += attrInfo.WeaponLowerRequirements;
			}

			if ( v > 100 )
				v = 100;

			return v;
		}

		public static void BlockEquip( Mobile m, TimeSpan duration )
		{
			if ( m.BeginAction( typeof( BaseWeapon ) ) )
				new ResetEquipTimer( m, duration ).Start();
		}

		private class ResetEquipTimer : Timer
		{
			private Mobile m_Mobile;

			public ResetEquipTimer( Mobile m, TimeSpan duration ) : base( duration )
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( BaseWeapon ) );
			}
		}

		public override bool CheckConflictingLayer( Mobile m, Item item, Layer layer )
		{
			if ( base.CheckConflictingLayer( m, item, layer ) )
				return true;

			if ( this.Layer == Layer.TwoHanded && layer == Layer.OneHanded )
				return true;
			else if ( this.Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight) )
				return true;

			return false;
		}

		public override bool CanEquip( Mobile from )
		{
			if ( from.Dex < DexRequirement )
			{
				from.SendAsciiMessage( "You are not nimble enough to equip that." );
				return false;
			} 
			else if ( from.Str < AOS.Scale( StrRequirement, 100 - GetLowerStatReq() ) )
			{
				from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
				return false;
			}
			else if ( from.Int < IntRequirement )
			{
				from.SendAsciiMessage( "You are not smart enough to equip that." );
				return false;
			}
			else if ( !from.CanBeginAction( typeof( BaseWeapon ) ) )
			{
				return false;
			}
			else
			{
				return base.CanEquip( from );
			}
		}

		public virtual bool UseSkillMod{ get{ return !Core.AOS; } }


		public override bool OnEquip( Mobile from )
		{
			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = from;

				string modName = this.Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			// from.NextCombatTime = DateTime.Now + GetDelay( from ); // instahit - no delay when the weapon is (first) equipped.

			if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular )
			{
				if ( m_SkillMod != null )
					m_SkillMod.Remove();

				m_SkillMod = new DefaultSkillMod( AccuracyMod, true, (int)m_AccuracyLevel * 5 );
				from.AddSkillMod( m_SkillMod );
			}

			return true;
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).CheckStatTimers();
				((Mobile)parent).Delta( MobileDelta.WeaponDamage );
			}
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile m = (Mobile)parent;
				BaseWeapon weapon = m.Weapon as BaseWeapon;

				string modName = this.Serial.ToString();

				m.RemoveStatMod( modName + "Str" );
				m.RemoveStatMod( modName + "Dex" );
				m.RemoveStatMod( modName + "Int" );

				/*if ( weapon != null )
				{
					//m.NextCombatTime = DateTime.Now + weapon.GetDelay( m );
					DateTime nextTime = DateTime.Now + weapon.GetDelay( m );
					if ( nextTime > m.NextCombatTime )
						m.NextCombatTime = nextTime;
				}*/

				if ( UseSkillMod && m_SkillMod != null )
				{
					m_SkillMod.Remove();
					m_SkillMod = null;
				}

				if ( m_MageMod != null )
				{
					m_MageMod.Remove();
					m_MageMod = null;
				}

				m.CheckStatTimers();

				m.Delta( MobileDelta.WeaponDamage );
			}
		}

		public virtual SkillName GetUsedSkill( Mobile m, bool checkSkillAttrs )
		{
			SkillName sk;

			if ( checkSkillAttrs && m_AosWeaponAttributes.UseBestSkill != 0 )
			{
				double swrd = m.Skills[SkillName.Swords].Value;
				double fenc = m.Skills[SkillName.Fencing].Value;
				double arch = m.Skills[SkillName.Archery].Value;
				double mcng = m.Skills[SkillName.Macing].Value;
				double val;

				sk = SkillName.Swords;
				val = swrd;

				if ( fenc > val ){ sk = SkillName.Fencing; val = fenc; }
				if ( arch > val ){ sk = SkillName.Archery; val = arch; }
				if ( mcng > val ){ sk = SkillName.Macing; val = mcng; }
			}
			else if ( m_AosWeaponAttributes.MageWeapon != 0 )
			{
				sk = SkillName.Magery;
			}
			else
			{
				sk = Skill;

				if ( sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman && m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value )
					sk = SkillName.Wrestling;
			}

			return sk;
		}

		public virtual double GetAttackSkillValue( Mobile attacker, Mobile defender )
		{
			return attacker.Skills[GetUsedSkill( attacker, true )].Value;
		}

		public virtual double GetDefendSkillValue( Mobile attacker, Mobile defender )
		{
			SkillName sk = GetUsedSkill( defender, false );
			if ( sk == SkillName.Archery )
			{
				double arch = defender.Skills[SkillName.Archery].Value / 2.0;
				double wrestling = defender.Skills[SkillName.Wrestling].Value * 3.0 / 4.0;

				return arch >= wrestling ? arch : wrestling;
			}
			else
			{
				return defender.Skills[sk].Value;
			}
		}

		public virtual bool CheckHit( Mobile attacker, Mobile defender )
		{
			BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
			BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

			Skill atkSkill = attacker.Skills[atkWeapon.Skill];
			Skill defSkill = defender.Skills[defWeapon.Skill];

			double atkValue = atkWeapon.GetAttackSkillValue( attacker, defender );
			double defValue = defWeapon.GetDefendSkillValue( attacker, defender );
			if ( defValue == -50.0 )
				defValue = -49.9;
			
			// chance = ( attacker_ability + 50 ) / ( ( defender_ability + 50 ) * 2 )
			double chance = (atkValue + 50.0) / ((defValue + 50.0) * 2.0);
			
			chance *= 1.0 + ((double)GetHitChanceBonus() / 100.0);

			WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );
			if ( ability != null )
				chance *= ability.AccuracyScalar;
			
			attacker.CheckSkill( atkSkill.SkillName, defSkill.Value-30.0, defSkill.Value+25.0 );
			return chance >= Utility.RandomDouble();//;attacker.CheckSkill( atkSkill.SkillName, chance );
		}

		public virtual TimeSpan GetDelay( Mobile m )
		{
			int speed = this.Speed;

			if ( speed <= 0 )
				return TimeSpan.FromSeconds( 10 );

			double delayInSeconds;

			// should be 10000? 15000?
			delayInSeconds = 12500.0 / ((double)( ( m.Dex + 100 ) * speed ));
			if ( delayInSeconds < 0.5 )
				delayInSeconds = 0.5;
			else if ( delayInSeconds > 7 )
				delayInSeconds = 7;

			return TimeSpan.FromSeconds( delayInSeconds );
		}

        public virtual void OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            /*WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);

            if (a != null && !a.OnBeforeSwing(attacker, defender))
                WeaponAbility.ClearCurrentAbility(attacker);

            SpecialMove move = SpecialMove.GetCurrentMove(attacker);

            if (move != null && !move.OnBeforeSwing(attacker, defender))
                SpecialMove.ClearCurrentMove(attacker);*/
        }

		public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			if ( attacker.HarmfulCheck( defender ) )
			{
				attacker.DisruptiveAction();

				if ( attacker.NetState != null )
					attacker.Send( new Swing( 0, attacker, defender ) );

				if ( CheckHit( attacker, defender ) )
					OnHit( attacker, defender );
				else
					OnMiss( attacker, defender );
			}

			return GetDelay( attacker );
		}

		public virtual int GetHitAttackSound( Mobile attacker, Mobile defender )
		{
			int sound = attacker.GetAttackSound();

			if ( sound == -1 )
				sound = HitSound;

			return sound;
		}

		public virtual int GetHitDefendSound( Mobile attacker, Mobile defender )
		{
			return defender.GetHurtSound();
		}

		public virtual int GetMissAttackSound( Mobile attacker, Mobile defender )
		{
			if ( attacker.GetAttackSound() == -1 )
				return MissSound;
			else
				return -1;
		}

		public virtual int GetMissDefendSound( Mobile attacker, Mobile defender )
		{
			return -1;
		}

		public virtual int AbsorbDamageAOS( Mobile attacker, Mobile defender, int damage )
		{
			return damage;
		}

		public virtual int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
		{
			double chance = Utility.RandomDouble();
			BaseArmor armor;

			if ( chance < 0.07 )
				armor = defender.NeckArmor as BaseArmor;
			else if ( chance < 0.14 )
				armor = defender.HandArmor as BaseArmor;
			else if ( chance < 0.28 )
				armor = defender.ArmsArmor as BaseArmor;
			else if ( chance < 0.43 )
				armor = defender.HeadArmor as BaseArmor;
			else if ( chance < 0.65 )
				armor = defender.LegsArmor as BaseArmor;
			else
				armor = defender.ChestArmor as BaseArmor;

			BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;
			if ( shield != null )
				damage = shield.OnHit( this, damage );

			if ( armor != null )
				damage = armor.OnHit( this, damage );

			int virtualArmor = ( defender.VirtualArmor + defender.VirtualArmorMod );
			if ( virtualArmor > 0 )
				damage -= Utility.RandomMinMax( virtualArmor / 2, virtualArmor ) / 2;

			return damage;
		}

		public virtual int GetPackInstinctBonus( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player || defender.Player )
				return 0;

			BaseCreature bc = attacker as BaseCreature;

			if ( bc == null || bc.PackInstinct == PackInstinct.None || (!bc.Controled && !bc.Summoned) )
				return 0;

			Mobile master = bc.ControlMaster;

			if ( master == null )
				master = bc.SummonMaster;

			if ( master == null )
				return 0;

			int inPack = 1;

			foreach ( Mobile m in defender.GetMobilesInRange( 1 ) )
			{
				if ( m != attacker && m is BaseCreature )
				{
					BaseCreature tc = (BaseCreature)m;

					if ( (tc.PackInstinct & bc.PackInstinct) == 0 || (!tc.Controled && !tc.Summoned) )
						continue;

					Mobile theirMaster = tc.ControlMaster;

					if ( theirMaster == null )
						theirMaster = tc.SummonMaster;

					if ( master == theirMaster && tc.Combatant == defender )
						++inPack;
				}
			}

			if ( inPack >= 5 )
				return 100;
			else if ( inPack >= 4 )
				return 75;
			else if ( inPack >= 3 )
				return 50;
			else if ( inPack >= 2 )
				return 25;

			return 0;
		}

		private static bool m_InDoubleStrike;

		public static bool InDoubleStrike
		{
			get{ return m_InDoubleStrike; }
			set{ m_InDoubleStrike = value; }
		}

		public virtual void OnHit( Mobile attacker, Mobile defender )
		{
			PlaySwingAnimation( attacker );
			PlayHurtAnimation( defender );

			attacker.PlaySound( GetHitAttackSound( attacker, defender ) );
			defender.PlaySound( GetHitDefendSound( attacker, defender ) );

			int damage = ComputeDamage( attacker, defender );

			CheckSlayerResult cs = CheckSlayers( attacker, defender );

			if ( cs != CheckSlayerResult.None )
			{
				if ( cs == CheckSlayerResult.Slayer )
					defender.FixedEffect( 0x37B9, 10, 5 );

				damage *= 2;
			}

			if ( attacker is BaseCreature )
				((BaseCreature)attacker).AlterMeleeDamageTo( defender, ref damage );

			if ( defender is BaseCreature )
				((BaseCreature)defender).AlterMeleeDamageFrom( attacker, ref damage );

			damage = AbsorbDamage( attacker, defender, damage );

			// Halve the computed damage...
			damage /= 2;
			if ( damage < 1 )
				damage = 1;

			AddBlood( attacker, defender, damage );

			defender.Damage( damage, attacker );

			if ( MaxHits > 0 && Utility.Random( 10 ) == 0 )
			{
				if ( Hits > 1 )
					--Hits;
				else
					Delete();
			}

			if ( SpellEffectOnHit && defender.Alive && !defender.Deleted && m_Effect != SpellEffect.None && m_EffectCharges > 0 )
			{
				if ( SpellCastEffect.InvokeEffect( m_Effect, attacker, defender ) )
				{
					SpellCharges--;
					if ( SpellCharges <= 0 )
					{
						m_Effect = SpellEffect.None;
						attacker.SendAsciiMessage( "This magic item is out of charges." );
					}
				}
			}

			if ( attacker is BaseCreature )
				((BaseCreature)attacker).OnGaveMeleeAttack( defender );

			if ( defender is BaseCreature )
				((BaseCreature)defender).OnGotMeleeAttack( attacker );
		}

		public virtual bool SpellEffectOnHit { get { return true; } }

		public virtual double GetAosDamage( Mobile attacker, int min, int random, double div )
		{
			double scale = 1.0;

			scale += attacker.Skills[SkillName.Inscribe].Value * 0.001;

			if ( attacker.Player )
			{
				scale += attacker.Int * 0.001;
				scale += AosAttributes.GetValue( attacker, AosAttribute.SpellDamage ) * 0.01;
			}

			int baseDamage = min + (int)(attacker.Skills[SkillName.EvalInt].Value / div);

			double damage = Utility.RandomMinMax( baseDamage, baseDamage + random );

			return damage * scale;
		}

		public virtual void DoAreaAttack( Mobile from, int sound, int hue, int phys, int fire, int cold, int pois, int nrgy )
		{
			Map map = from.Map;

			if ( map == null )
				return;

			ArrayList list = new ArrayList();

			foreach ( Mobile m in from.GetMobilesInRange( 10 ) )
			{
				if ( from != m && SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false ) && from.InLOS( m ) )
					list.Add( m );
			}

			if ( list.Count == 0 )
				return;

			Effects.PlaySound( from.Location, map, sound );

			// TODO: What is the damage calculation?

			for ( int i = 0; i < list.Count; ++i )
			{
				Mobile m = (Mobile)list[i];

				double scalar = (11 - from.GetDistanceToSqrt( m )) / 10;

				if ( scalar > 1.0 )
					scalar = 1.0;
				else if ( scalar < 0.0 )
					continue;

				from.DoHarmful( m, true );
				m.FixedEffect( 0x3779, 1, 15, hue, 0 );
				AOS.Damage( m, from, (int)(GetBaseDamage( from ) * scalar), phys, fire, cold, pois, nrgy );
			}
		}

		public virtual CheckSlayerResult CheckSlayers( Mobile attacker, Mobile defender )
		{
			BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
			SlayerEntry atkSlayer = SlayerGroup.GetEntryByName( atkWeapon.Slayer );

			if ( atkSlayer != null && atkSlayer.Slays( defender ) )
				return CheckSlayerResult.Slayer;

			BaseWeapon defWeapon = defender.Weapon as BaseWeapon;
			SlayerEntry defSlayer = SlayerGroup.GetEntryByName( defWeapon.Slayer );

			if ( defSlayer != null && defSlayer.Group != null && defSlayer.Group.Opposition != null && defSlayer.Group.Opposition.Super != null && defSlayer.Group.Opposition.Super.Slays( attacker ) )
				return CheckSlayerResult.Opposition;

			return CheckSlayerResult.None;
		}

		public virtual void AddBlood( Mobile attacker, Mobile defender, int damage )
		{
			if ( damage <= 5 )
				return;

			Direction d = defender.GetDirectionTo( attacker );

			int count;
			count = damage / 10;
			if ( count > 4 )
				count = 4;
			else if ( count < 1 )
				count = 1;
			count = Utility.Random( count )+1;
			
			for( int i = 0; i < count; ++i )
			{
				int x = defender.X;
				int y = defender.Y;

				switch( d )
				{
					case Direction.North:
						x += Utility.Random( -1, 3 );
						y += Utility.Random( 2 );
						break;
					case Direction.East:
						y += Utility.Random( -1, 3 );
						x += Utility.Random( -1, 2 );
						break;
					case Direction.West:
						y += Utility.Random( -1, 3 );
						x += Utility.Random( 2 );
						break;
					case Direction.South:
						x += Utility.Random( -1, 3 );
						y += Utility.Random( -1, 2 );
						break;
					case Direction.Up:
						x += Utility.Random( 2 );
						y += Utility.Random( 2 );
						break;
					case Direction.Down:
						x += Utility.Random( -1, 2 );
						y += Utility.Random( -1, 2 );
						break;
					case Direction.Left:
						x += Utility.Random( 2 );
						y += Utility.Random( -1, 2 );
						break;
					case Direction.Right:
						x += Utility.Random( -1, 2 );
						y += Utility.Random( 2 );
						break;
				}

				new Blood().MoveToWorld( new Point3D( x, y, defender.Z ), defender.Map );
			}
		}

		public virtual void GetDamageTypes( out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			if ( Parent is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)Parent;

				phys = bc.PhysicalDamage;
				fire = bc.FireDamage;
				cold = bc.ColdDamage;
				pois = bc.PoisonDamage;
				nrgy = bc.EnergyDamage;
			}
			else
			{
				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

				if ( resInfo != null )
				{
					CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

					if ( attrInfo != null )
					{
						fire = attrInfo.WeaponFireDamage;
						cold = attrInfo.WeaponColdDamage;
						pois = attrInfo.WeaponPoisonDamage;
						nrgy = attrInfo.WeaponEnergyDamage;
						phys = 100 - fire - cold - pois - nrgy;
						return;
					}
				}

				phys = 100;
				fire = 0;
				cold = 0;
				pois = 0;
				nrgy = 0;
			}
		}

		public virtual void OnMiss( Mobile attacker, Mobile defender )
		{
			PlaySwingAnimation( attacker );
			attacker.PlaySound( GetMissAttackSound( attacker, defender ) );
			defender.PlaySound( GetMissDefendSound( attacker, defender ) );

			WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );

			if ( ability != null )
				ability.OnMiss( attacker, defender );

			// check their parrying even on a miss (if they are holding a shield)
			// on a hit, parrying is checked in Shield.OnHit
			BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;
			if ( shield != null )
				defender.CheckSkill( SkillName.Parry, 0, 75.0 );
		}

		public virtual void GetBaseDamageRange( Mobile attacker, out int min, out int max )
		{
			if ( attacker is BaseCreature )
			{
				BaseCreature c = (BaseCreature)attacker;

				if ( c.DamageMin >= 0 )
				{
					min = c.DamageMin;
					max = c.DamageMax;
					return;
				}

				if ( this is Fists && !attacker.Body.IsHuman )
				{
					min = attacker.Str / 28;
					max = attacker.Str / 28;
					return;
				}
			}

			min = MinDamage;
			max = MaxDamage;
		}

		public virtual void GetStatusDamage( Mobile from, out int min, out int max )
		{
			int baseMin, baseMax;

			GetBaseDamageRange( from, out baseMin, out baseMax );

			min = (int)ScaleDamageOld( from, baseMin, false, false );
			max = (int)ScaleDamageOld( from, baseMax, false, false );

			if ( min < 1 )
				min = 1;
			if ( max <= min )
				max = min+1;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int NumDice { get { return 0; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int NumSides { get { return 0; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int DiceBonus { get { return 0; } }

		public virtual double GetBaseDamage( Mobile attacker )
		{
			// if theres a min damage defined, then use that, otherwise use the dice (if they are defined)
			if ( this.NumDice <= 0 || m_MinDamage != -1 )
			{
				int min, max;

				GetBaseDamageRange( attacker, out min, out max );

				return Utility.RandomMinMax( min, max );
			}
			else
			{
				return Utility.Dice( this.NumDice, this.NumSides, this.DiceBonus );
			}
		}

		public virtual double GetBonus( double value, double scalar, double threshold, double offset )
		{
			double bonus = value * scalar;

			if ( value >= threshold )
				bonus += offset;

			return bonus / 100;
		}

		public virtual int GetHitChanceBonus()
		{
			if ( !Core.AOS )
				return 0;

			int bonus = 0;

			switch ( m_AccuracyLevel )
			{
				case WeaponAccuracyLevel.Accurate:		bonus += 02; break;
				case WeaponAccuracyLevel.Surpassingly:	bonus += 04; break;
				case WeaponAccuracyLevel.Eminently:		bonus += 06; break;
				case WeaponAccuracyLevel.Exceedingly:	bonus += 08; break;
				case WeaponAccuracyLevel.Supremely:		bonus += 10; break;
			}

			return bonus;
		}

		public virtual int GetDamageBonus()
		{
			int bonus = VirtualDamageBonus;

			switch ( m_Quality )
			{
				case CraftQuality.Low:			bonus -= 20; break;
				case CraftQuality.Exceptional:	bonus += 20; break;
			}

			switch ( m_DamageLevel )
			{
				case WeaponDamageLevel.Ruin:	bonus += 15; break;
				case WeaponDamageLevel.Might:	bonus += 20; break;
				case WeaponDamageLevel.Force:	bonus += 25; break;
				case WeaponDamageLevel.Power:	bonus += 30; break;
				case WeaponDamageLevel.Vanq:	bonus += 35; break;
			}

			return bonus;
		}

		public virtual double ScaleDamageAOS( Mobile attacker, double damage, bool checkSkills, bool checkAbility )
		{
			if ( checkSkills )
			{
				attacker.CheckSkill( SkillName.Tactics, 0.0, 120.0 ); // Passively check tactics for gain
				attacker.CheckSkill( SkillName.Anatomy, 0.0, 120.0 ); // Passively check Anatomy for gain

				if ( Type == WeaponType.Axe )
					attacker.CheckSkill( SkillName.Lumberjacking, 0.0, 100.0 ); // Passively check Lumberjacking for gain
			}

			double strengthBonus = GetBonus( attacker.Str,										0.300, 100.0,  5.00 );
			double  anatomyBonus = GetBonus( attacker.Skills[SkillName.Anatomy].Value,			0.500, 100.0,  5.00 );
			double  tacticsBonus = GetBonus( attacker.Skills[SkillName.Tactics].Value,			0.625, 100.0,  6.25 );
			double   lumberBonus = GetBonus( attacker.Skills[SkillName.Lumberjacking].Value,	0.200, 100.0, 10.00 );

			if ( Type != WeaponType.Axe )
				lumberBonus = 0.0;

			double totalBonus = strengthBonus + anatomyBonus + tacticsBonus + lumberBonus + ((double)(GetDamageBonus() + AosAttributes.GetValue( attacker, AosAttribute.WeaponDamage )) / 100);
			double discordanceScalar = 0.0;

			if ( SkillHandlers.Discordance.GetScalar( attacker, ref discordanceScalar ) )
				totalBonus += discordanceScalar;

			damage += (damage * totalBonus);

			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );

			if ( checkAbility && a != null )
				damage *= a.DamageScalar;

			return damage;
		}

		public virtual int VirtualDamageBonus{ get{ return 0; } }

		public virtual int ComputeDamageAOS( Mobile attacker, Mobile defender )
		{
			return (int)ScaleDamageAOS( attacker, GetBaseDamage( attacker ), true, true );
		}

		public virtual double ScaleDamageOld( Mobile attacker, double damage, bool checkSkills, bool checkAbility )
		{
			if ( checkSkills )
				attacker.CheckSkill( SkillName.Tactics, 0.0, 100.0 ); // Passively check tactics for gain

			/* Compute tactics modifier
			 * :   0.0 = 50% loss
			 * :  50.0 = unchanged
			 * : 100.0 = 50% bonus
			 */
			double tacticsBonus = (attacker.Skills[SkillName.Tactics].Value - 50.0) / 100.0;

			/* Compute strength modifier
			 * : 1% bonus for every 2 strength
			 */
			double strBonus = (attacker.Str / 2.0) / 100.0;

			// Apply bonuses
			damage += (damage * tacticsBonus) + (damage * strBonus) + ((damage * VirtualDamageBonus) / 100.0);

			if ( m_MaxHits > 0 )
				damage *= ( ((double)m_Hits) / ((double)m_MaxHits) )*0.125 + 0.9;

			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );
			if ( checkAbility && a != null )
				damage *= a.DamageScalar;

			if ( IsMagic || m_Quality == CraftQuality.Exceptional )
				damage += 0.5;
			else if ( m_Quality == CraftQuality.Low )
				damage -= 0.5;

			damage += (int)( 1.8 * (int)m_DamageLevel );

			return (int)( damage + 0.5 );
		}

		public virtual int ComputeDamage( Mobile attacker, Mobile defender )
		{
			if ( Core.AOS )
				return ComputeDamageAOS( attacker, defender );

			return (int)ScaleDamageOld( attacker, GetBaseDamage( attacker ), true, true );
		}

		public virtual void PlayHurtAnimation( Mobile from )
		{
			int action;
			int frames;

			switch ( from.Body.Type )
			{
				case BodyType.Sea:
				case BodyType.Animal:
				{
					action = 7;
					frames = 5;
					break;
				}
				case BodyType.Monster:
				{
					action = 10;
					frames = 4;
					break;
				}
				case BodyType.Human:
				{
					action = 20;
					frames = 5;
					break;
				}
				default: return;
			}

			if ( from.Mounted )
				return;

			from.Animate( action, frames, 1, true, false, 0 );
		}

		public virtual void PlaySwingAnimation( Mobile from )
		{
			int action;

			switch ( from.Body.Type )
			{
				case BodyType.Sea:
				case BodyType.Animal:
				{
					action = Utility.Random( 5, 2 );
					break;
				}
				case BodyType.Monster:
				{
					switch ( Animation )
					{
						default:
						case WeaponAnimation.Wrestle:
						case WeaponAnimation.Bash1H:
						case WeaponAnimation.Pierce1H:
						case WeaponAnimation.Slash1H:
						case WeaponAnimation.Bash2H:
						case WeaponAnimation.Pierce2H:
						case WeaponAnimation.Slash2H: action = Utility.Random( 4, 3 ); break;
						case WeaponAnimation.ShootBow:  return; // 7
						case WeaponAnimation.ShootXBow: return; // 8
					}

					break;
				}
				case BodyType.Human:
				{
					if ( !from.Mounted )
					{
						action = (int)Animation;
					}
					else
					{
						switch ( Animation )
						{
							default:
							case WeaponAnimation.Wrestle:
							case WeaponAnimation.Bash1H:
							case WeaponAnimation.Pierce1H:
							case WeaponAnimation.Slash1H: action = 26; break;
							case WeaponAnimation.Bash2H:
							case WeaponAnimation.Pierce2H:
							case WeaponAnimation.Slash2H: action = 29; break;
							case WeaponAnimation.ShootBow: action = 27; break;
							case WeaponAnimation.ShootXBow: action = 28; break;
						}
					}

					break;
				}
				default: return;
			}

			from.Animate( action, 7, 1, true, false, 0 );
		}

		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 8 ); // version

			writer.WriteMobileList( m_Identified, true );

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.DamageLevel,		m_DamageLevel != WeaponDamageLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.AccuracyLevel,		m_AccuracyLevel != WeaponAccuracyLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.DurabilityLevel,	m_DurabilityLevel != DurabilityLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.Quality,			m_Quality != CraftQuality.Regular );
			SetSaveFlag( ref flags, SaveFlag.Hits,				m_Hits != 0 );
			SetSaveFlag( ref flags, SaveFlag.MaxHits,			m_MaxHits != 0 );
			SetSaveFlag( ref flags, SaveFlag.Slayer,			m_Slayer != SlayerName.None );
			SetSaveFlag( ref flags, SaveFlag.Poison,			m_Poison != null );
			SetSaveFlag( ref flags, SaveFlag.PoisonCharges,		m_PoisonCharges != 0 );
			SetSaveFlag( ref flags, SaveFlag.Crafter,			m_Crafter != null );
			//SetSaveFlag( ref flags, SaveFlag.Identified,		m_Identified != false );
			SetSaveFlag( ref flags, SaveFlag.StrReq,			m_StrReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.DexReq,			m_DexReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.IntReq,			m_IntReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.MinDamage,			m_MinDamage != -1 );
			SetSaveFlag( ref flags, SaveFlag.MaxDamage,			m_MaxDamage != -1 );
			SetSaveFlag( ref flags, SaveFlag.HitSound,			m_HitSound != -1 );
			SetSaveFlag( ref flags, SaveFlag.MissSound,			m_MissSound != -1 );
			SetSaveFlag( ref flags, SaveFlag.Speed,				m_Speed != -1 );
			SetSaveFlag( ref flags, SaveFlag.MaxRange,			m_MaxRange != -1 );
			SetSaveFlag( ref flags, SaveFlag.Skill,				m_Skill != (SkillName)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Type,				m_Type != (WeaponType)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Animation,			m_Animation != (WeaponAnimation)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Resource,			m_Resource != CraftResource.Iron );
			SetSaveFlag( ref flags, SaveFlag.xAttributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.xWeaponAttributes,	!m_AosWeaponAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,	m_PlayerConstructed );
			SetSaveFlag( ref flags, SaveFlag.SpellEffect,		m_Effect != SpellEffect.None && m_EffectCharges > 0 );

			writer.Write( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
				writer.Write( (int) m_DamageLevel );

			if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
				writer.Write( (int) m_AccuracyLevel );

			if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
				writer.Write( (int) m_DurabilityLevel );

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.Write( (int) m_Quality );

			if ( GetSaveFlag( flags, SaveFlag.Hits ) )
				writer.Write( (int) m_Hits );

			if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
				writer.Write( (int) m_MaxHits );

			if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
				writer.Write( (int) m_Slayer );

			if ( GetSaveFlag( flags, SaveFlag.Poison ) )
				Poison.Serialize( m_Poison, writer );

			if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
			{
				writer.Write( (int) m_PoisonCharges );
				writer.Write( (double)m_PoisonChance );
			}

			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				writer.Write( (Mobile) m_Crafter );

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				writer.Write( (int) m_StrReq );

			if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
				writer.Write( (int) m_DexReq );

			if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
				writer.Write( (int) m_IntReq );

			if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
				writer.Write( (int) m_MinDamage );

			if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
				writer.Write( (int) m_MaxDamage );

			if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
				writer.Write( (int) m_HitSound );

			if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
				writer.Write( (int) m_MissSound );

			if ( GetSaveFlag( flags, SaveFlag.Speed ) )
				writer.Write( (int) m_Speed );

			if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
				writer.Write( (int) m_MaxRange );

			if ( GetSaveFlag( flags, SaveFlag.Skill ) )
				writer.Write( (int) m_Skill );

			if ( GetSaveFlag( flags, SaveFlag.Type ) )
				writer.Write( (int) m_Type );

			if ( GetSaveFlag( flags, SaveFlag.Animation ) )
				writer.Write( (int) m_Animation );

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				writer.Write( (int) m_Resource );

			if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
				m_AosWeaponAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.SpellEffect ) )
			{
				writer.Write( (int)m_Effect );
				writer.Write( (int)m_EffectCharges );
			}
		}

		[Flags]
		private enum SaveFlag
		{
			None					= 0x00000000,
			DamageLevel				= 0x00000001,
			AccuracyLevel			= 0x00000002,
			DurabilityLevel			= 0x00000004,
			Quality					= 0x00000008,
			Hits					= 0x00000010,
			MaxHits					= 0x00000020,
			Slayer					= 0x00000040,
			Poison					= 0x00000080,
			PoisonCharges			= 0x00000100,
			Crafter					= 0x00000200,
			Identified				= 0x00000400,
			StrReq					= 0x00000800,
			DexReq					= 0x00001000,
			IntReq					= 0x00002000,
			MinDamage				= 0x00004000,
			MaxDamage				= 0x00008000,
			HitSound				= 0x00010000,
			MissSound				= 0x00020000,
			Speed					= 0x00040000,
			MaxRange				= 0x00080000,
			Skill					= 0x00100000,
			Type					= 0x00200000,
			Animation				= 0x00400000,
			Resource				= 0x00800000,
			xAttributes				= 0x01000000,
			xWeaponAttributes		= 0x02000000,
			PlayerConstructed		= 0x04000000,
			SpellEffect				= 0x10000000,
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 8:
				{
					m_Identified = reader.ReadMobileList();

					goto case 7;
				}
				case 7:
				case 6:
				case 5:
				{
					SaveFlag flags = (SaveFlag)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
						m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
						m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
						m_DurabilityLevel = (DurabilityLevel)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Quality ) )
						m_Quality = (CraftQuality)reader.ReadInt();
					else
						m_Quality = CraftQuality.Regular;

					if ( GetSaveFlag( flags, SaveFlag.Hits ) )
						m_Hits = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
						m_MaxHits = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
						m_Slayer = (SlayerName)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Poison ) )
						m_Poison = Poison.Deserialize( reader );

					if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
					{
						m_PoisonCharges = reader.ReadInt();
						if ( version >= 7 )
							m_PoisonChance = reader.ReadDouble();
						else
							m_PoisonChance = Utility.RandomDouble() * 0.25;
					}

					if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
						m_Crafter = reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.Identified ) && version < 6 )
						reader.ReadBool();
						//m_Identified = ( version >= 6 || reader.ReadBool() );

					if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
						m_StrReq = reader.ReadInt();
					else
						m_StrReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
						m_DexReq = reader.ReadInt();
					else
						m_DexReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
						m_IntReq = reader.ReadInt();
					else
						m_IntReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
						m_MinDamage = reader.ReadInt();
					else
						m_MinDamage = -1;

					if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
						m_MaxDamage = reader.ReadInt();
					else
						m_MaxDamage = -1;

					if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
						m_HitSound = reader.ReadInt();
					else
						m_HitSound = -1;

					if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
						m_MissSound = reader.ReadInt();
					else
						m_MissSound = -1;

					if ( GetSaveFlag( flags, SaveFlag.Speed ) )
						m_Speed = reader.ReadInt();
					else
						m_Speed = -1;

					if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
						m_MaxRange = reader.ReadInt();
					else
						m_MaxRange = -1;

					if ( GetSaveFlag( flags, SaveFlag.Skill ) )
						m_Skill = (SkillName)reader.ReadInt();
					else
						m_Skill = (SkillName)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Type ) )
						m_Type = (WeaponType)reader.ReadInt();
					else
						m_Type = (WeaponType)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Animation ) )
						m_Animation = (WeaponAnimation)reader.ReadInt();
					else
						m_Animation = (WeaponAnimation)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Resource ) )
						m_Resource = (CraftResource)reader.ReadInt();
					else
						m_Resource = CraftResource.Iron;

					if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
						m_AosAttributes = new AosAttributes( this, reader );
					else
						m_AosAttributes = new AosAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
						m_AosWeaponAttributes = new AosWeaponAttributes( this, reader );
					else
						m_AosWeaponAttributes = new AosWeaponAttributes( this );

					if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
					{
						m_SkillMod = new DefaultSkillMod( AccuracyMod, true, (int)m_AccuracyLevel * 5 );
						((Mobile)Parent).AddSkillMod( m_SkillMod );
					}

					if ( Core.AOS && m_AosWeaponAttributes.MageWeapon != 0 && Parent is Mobile )
					{
						m_MageMod = new DefaultSkillMod( SkillName.Magery, true, -m_AosWeaponAttributes.MageWeapon );
						((Mobile)Parent).AddSkillMod( m_MageMod );
					}

					if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
						m_PlayerConstructed = true;

					if ( GetSaveFlag( flags, SaveFlag.SpellEffect ) )
					{
						m_Effect = (SpellEffect)reader.ReadInt();
						m_EffectCharges = reader.ReadInt();
					}

					break;
				}
				case 4:
				{
					m_Slayer = (SlayerName)reader.ReadInt();

					goto case 3;
				}
				case 3:
				{
					m_StrReq = reader.ReadInt();
					m_DexReq = reader.ReadInt();
					m_IntReq = reader.ReadInt();

					goto case 2;
				}
				case 2:
				{
					/*m_Identified = */reader.ReadBool();

					goto case 1;
				}
				case 1:
				{
					m_MaxRange = reader.ReadInt();

					goto case 0;
				}
				case 0:
				{
					if ( version == 0 )
						m_MaxRange = 1; // default

					if ( version < 5 )
					{
						m_Resource = CraftResource.Iron;
						m_AosAttributes = new AosAttributes( this );
						m_AosWeaponAttributes = new AosWeaponAttributes( this );
					}

					m_MinDamage = reader.ReadInt();
					m_MaxDamage = reader.ReadInt();

					m_Speed = reader.ReadInt();

					m_HitSound = reader.ReadInt();
					m_MissSound = reader.ReadInt();

					m_Skill = (SkillName)reader.ReadInt();
					m_Type = (WeaponType)reader.ReadInt();
					m_Animation = (WeaponAnimation)reader.ReadInt();
					m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();
					m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();
					m_DurabilityLevel = (DurabilityLevel)reader.ReadInt();
					m_Quality = (CraftQuality)reader.ReadInt();

					m_Crafter = reader.ReadMobile();

					m_Poison = Poison.Deserialize( reader );
					m_PoisonCharges = reader.ReadInt();

					if ( m_StrReq == OldStrengthReq )
						m_StrReq = -1;

					if ( m_DexReq == OldDexterityReq )
						m_DexReq = -1;

					if ( m_IntReq == OldIntelligenceReq )
						m_IntReq = -1;

					if ( m_MinDamage == OldMinDamage )
						m_MinDamage = -1;

					if ( m_MaxDamage == OldMaxDamage )
						m_MaxDamage = -1;

					if ( m_HitSound == OldHitSound )
						m_HitSound = -1;

					if ( m_MissSound == OldMissSound )
						m_MissSound = -1;

					if ( m_Speed == OldSpeed )
						m_Speed = -1;

					if ( m_MaxRange == OldMaxRange )
						m_MaxRange = -1;

					if ( m_Skill == OldSkill )
						m_Skill = (SkillName)(-1);

					if ( m_Type == OldType )
						m_Type = (WeaponType)(-1);

					if ( m_Animation == OldAnimation )
						m_Animation = (WeaponAnimation)(-1);

					if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
					{
						m_SkillMod = new DefaultSkillMod( AccuracyMod, true, (int)m_AccuracyLevel * 5);
						((Mobile)Parent).AddSkillMod( m_SkillMod );
					}

					break;
				}
			}

			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( this.Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)this.Parent;

				string modName = this.Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();

			if ( m_Hits <= 0 && m_MaxHits <= 0 )
			{
				m_Hits = m_MaxHits = Utility.RandomMinMax( InitMinHits, InitMaxHits );
			}

			if ( version < 6 )
				m_PlayerConstructed = true; // we don't know, so, assume it's crafted
		}

		public BaseWeapon( int itemID ) : base( itemID )
		{
			Layer = (Layer)ItemData.Quality;

			m_Quality = CraftQuality.Regular;
			m_StrReq = -1;
			m_DexReq = -1;
			m_IntReq = -1;
			m_MinDamage = -1;
			m_MaxDamage = -1;
			m_HitSound = -1;
			m_MissSound = -1;
			m_Speed = -1;
			m_MaxRange = -1;
			m_Skill = (SkillName)(-1);
			m_Type = (WeaponType)(-1);
			m_Animation = (WeaponAnimation)(-1);

			m_Hits = m_MaxHits = Utility.RandomMinMax( InitMinHits, InitMaxHits );

			m_Resource = CraftResource.Iron;

			m_AosAttributes = new AosAttributes( this );
			m_AosWeaponAttributes = new AosWeaponAttributes( this );
		}

		public BaseWeapon( Serial serial ) : base( serial )
		{
		}

		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; InvalidateProperties(); }
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			int oreType;

			if ( Hue == 0 )
			{
				oreType = 0;
			}
			else
			{
				switch ( m_Resource )
				{
					case CraftResource.DullCopper:		oreType = 1053108; break; // dull copper
					case CraftResource.ShadowIron:		oreType = 1053107; break; // shadow iron
					case CraftResource.Copper:			oreType = 1053106; break; // copper
					case CraftResource.Bronze:			oreType = 1053105; break; // bronze
					case CraftResource.Gold:			oreType = 1053104; break; // golden
					case CraftResource.Agapite:			oreType = 1053103; break; // agapite
					case CraftResource.Verite:			oreType = 1053102; break; // verite
					case CraftResource.Valorite:		oreType = 1053101; break; // valorite
					case CraftResource.SpinedLeather:	oreType = 1061118; break; // spined
					case CraftResource.HornedLeather:	oreType = 1061117; break; // horned
					case CraftResource.BarbedLeather:	oreType = 1061116; break; // barbed
					case CraftResource.RedScales:		oreType = 1060814; break; // red
					case CraftResource.YellowScales:	oreType = 1060818; break; // yellow
					case CraftResource.BlackScales:		oreType = 1060820; break; // black
					case CraftResource.GreenScales:		oreType = 1060819; break; // green
					case CraftResource.WhiteScales:		oreType = 1060821; break; // white
					case CraftResource.BlueScales:		oreType = 1060815; break; // blue
					default: oreType = 0; break;
				}
			}

			if ( oreType != 0 )
				list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~
			else if ( Name == null )
				list.Add( LabelNumber );
			else
				list.Add( Name );
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
				return true;

			return ( m_AosAttributes.SpellChanneling != 0 );
		}

		public virtual int ArtifactRarity
		{
			get{ return 0; }
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == CraftQuality.Exceptional )
				list.Add( 1060636 ); // exceptional


			if ( ArtifactRarity > 0 )
				list.Add( 1061078, ArtifactRarity.ToString() ); // artifact rarity ~1_val~

			if ( this is IUsesRemaining && ((IUsesRemaining)this).ShowUsesRemaining )
				list.Add( 1060584, ((IUsesRemaining)this).UsesRemaining.ToString() ); // uses remaining: ~1_val~

			if ( m_Poison != null && m_PoisonCharges > 0 )
				list.Add( 1062412 + m_Poison.Level, m_PoisonCharges.ToString() );

			if ( m_Slayer != SlayerName.None )
				list.Add( 1017383 + (int)m_Slayer );

			base.AddResistanceProperties( list );

			int prop;

			if ( (prop = m_AosWeaponAttributes.UseBestSkill) != 0 )
				list.Add( 1060400 ); // use best weapon skill

			if ( (prop = (GetDamageBonus() + m_AosAttributes.WeaponDamage)) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = (GetHitChanceBonus() + m_AosAttributes.AttackChance)) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitColdArea) != 0 )
				list.Add( 1060416, prop.ToString() ); // hit cold area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitDispel) != 0 )
				list.Add( 1060417, prop.ToString() ); // hit dispel ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitEnergyArea) != 0 )
				list.Add( 1060418, prop.ToString() ); // hit energy area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitFireArea) != 0 )
				list.Add( 1060419, prop.ToString() ); // hit fire area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitFireball) != 0 )
				list.Add( 1060420, prop.ToString() ); // hit fireball ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitHarm) != 0 )
				list.Add( 1060421, prop.ToString() ); // hit harm ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechHits) != 0 )
				list.Add( 1060422, prop.ToString() ); // hit life leech ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLightning) != 0 )
				list.Add( 1060423, prop.ToString() ); // hit lightning ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLowerAttack) != 0 )
				list.Add( 1060424, prop.ToString() ); // hit lower attack ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLowerDefend) != 0 )
				list.Add( 1060425, prop.ToString() ); // hit lower defense ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitMagicArrow) != 0 )
				list.Add( 1060426, prop.ToString() ); // hit magic arrow ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechMana) != 0 )
				list.Add( 1060427, prop.ToString() ); // hit mana leech ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitPhysicalArea) != 0 )
				list.Add( 1060428, prop.ToString() ); // hit physical area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitPoisonArea) != 0 )
				list.Add( 1060429, prop.ToString() ); // hit poison area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechStam) != 0 )
				list.Add( 1060430, prop.ToString() ); // hit stamina leech ~1_val~%

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 )
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 )
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%

			if ( (prop = GetLowerStatReq()) != 0 )
				list.Add( 1060435, prop.ToString() ); // lower requirements ~1_val~%

			if ( (prop = m_AosAttributes.Luck) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosWeaponAttributes.MageWeapon) != 0 )
				list.Add( 1060438, prop.ToString() ); // mage weapon -~1_val~ skill

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_AosWeaponAttributes.SelfRepair) != 0 )
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~

			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

			int phys, fire, cold, pois, nrgy;

			GetDamageTypes( out phys, out fire, out cold, out pois, out nrgy );

			if ( phys != 0 )
				list.Add( 1060403, phys.ToString() ); // physical damage ~1_val~%

			if ( fire != 0 )
				list.Add( 1060405, fire.ToString() ); // fire damage ~1_val~%

			if ( cold != 0 )
				list.Add( 1060404, cold.ToString() ); // cold damage ~1_val~%

			if ( pois != 0 )
				list.Add( 1060406, pois.ToString() ); // poison damage ~1_val~%

			if ( nrgy != 0 )
				list.Add( 1060407, nrgy.ToString() ); // energy damage ~1_val~%

			list.Add( 1061168, "{0}\t{1}", MinDamage.ToString(), MaxDamage.ToString() ); // weapon damage ~1_val~ - ~2_val~
			list.Add( 1061167, Speed.ToString() ); // weapon speed ~1_val~

			if ( MaxRange > 1 )
				list.Add( 1061169, MaxRange.ToString() ); // range ~1_val~

			int strReq = AOS.Scale( StrRequirement, 100 - GetLowerStatReq() );

			if ( strReq > 0 )
				list.Add( 1061170, strReq.ToString() ); // strength requirement ~1_val~

			if ( Layer == Layer.TwoHanded )
				list.Add( 1061171 ); // two-handed weapon
			else
				list.Add( 1061824 ); // one-handed weapon

			if ( m_AosWeaponAttributes.UseBestSkill == 0 && m_AosWeaponAttributes.MageWeapon == 0 )
			{
				switch ( Skill )
				{
					case SkillName.Swords:  list.Add( 1061172 ); break; // skill required: swordsmanship
					case SkillName.Macing:  list.Add( 1061173 ); break; // skill required: mace fighting
					case SkillName.Fencing: list.Add( 1061174 ); break; // skill required: fencing
					case SkillName.Archery: list.Add( 1061175 ); break; // skill required: archery
				}
			}

			if ( m_Hits > 0 && m_MaxHits > 0 )
				list.Add( 1060639, "{0}\t{1}", m_Hits, m_MaxHits ); // durability ~1_val~ / ~2_val~
		}

		public bool IsMagic
		{
			get
			{
				return m_Slayer != SlayerName.None || m_DurabilityLevel != DurabilityLevel.Regular || m_DamageLevel != WeaponDamageLevel.Regular || m_AccuracyLevel != WeaponAccuracyLevel.Regular || ( m_Effect != SpellEffect.None && m_EffectCharges > 0 );
			}
		}

		public void OnIdentify( Mobile from )
		{
			if ( IsMagic && from.AccessLevel == AccessLevel.Player )
				m_Identified.Add( from );
		}

		private Packet m_MagicSingleClick = null;

		public override void SingleClickChanged()
		{
			base.SingleClickChanged ();
			Packet.Release( ref m_MagicSingleClick );
		}

		public override void SendSingleClickTo( Mobile from )
		{
			if ( !IsMagic || !( m_Identified.Contains( from ) || from.AccessLevel > AccessLevel.Counselor ) )
			{
				base.SendSingleClickTo( from );
			}
			else
			{
				if ( m_MagicSingleClick == null )
				{
					m_MagicSingleClick = new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", BuildMagicSingleClick() );
					m_MagicSingleClick.SetStatic();
				}

				from.NetState.Send( m_MagicSingleClick );
			}
		}

		public override string BuildSingleClick()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			if ( AppendLootType( sb ) )
				sb.Append( ", " );
			
			if ( m_Quality == CraftQuality.Exceptional )
				sb.Append( "exceptional, " );

			if ( IsMagic )
				sb.Append( "magic, " );

			if ( m_PoisonCharges > 0 && m_Poison != null )
				sb.Append( "poisoned, " );

			if ( sb.Length > 2 )
				sb.Remove( sb.Length - 2, 1 ); // remove the last comma

			AppendClickName( sb );
			InsertNamePrefix( sb );

			if ( m_Crafter != null && !m_Crafter.Deleted )
				sb.AppendFormat( " (crafted by {0})", m_Crafter.Name );

			return sb.ToString();
		}

		public virtual string BuildMagicSingleClick()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			if ( AppendLootType( sb ) )
				sb.Append( ", " );
			
			if ( m_Quality == CraftQuality.Exceptional )
				sb.Append( "exceptional, " );

			if ( m_DurabilityLevel != DurabilityLevel.Regular )
				sb.AppendFormat( "{0}, ", m_DurabilityLevel.ToString().ToLower() );

			if ( m_AccuracyLevel != WeaponAccuracyLevel.Regular )
			{
				if ( m_AccuracyLevel !=  WeaponAccuracyLevel.Accurate )
				{
					sb.Append( m_AccuracyLevel.ToString().ToLower() );
					sb.Append( ' ' );
				}
				sb.Append( "accurate, " );
			}

			if ( m_Slayer == SlayerName.Silver )
				sb.Append( "silver, " );

			if ( m_PoisonCharges > 0 && m_Poison != null )
				sb.Append( "poisoned, " );

			if ( sb.Length > 2 )
				sb.Remove( sb.Length - 2, 1 ); // remove the last comma

			AppendClickName( sb );
			InsertNamePrefix( sb );

			if ( m_DamageLevel > WeaponDamageLevel.Regular )
			{
				sb.Append( " of " );
				if ( m_DamageLevel == WeaponDamageLevel.Vanq )
					sb.Append( "vanquishing" );
				else
					sb.Append( m_DamageLevel.ToString().ToLower() );
			}

			if ( m_Effect != SpellEffect.None && m_EffectCharges > 0 )
			{
				if ( m_DamageLevel > WeaponDamageLevel.Regular )
					sb.Append( " and " );
				else
					sb.Append( " of " );
				sb.Append( SpellCastEffect.GetName( m_Effect ) );
				sb.AppendFormat( " with {0} charge{1}", m_EffectCharges, m_EffectCharges != 1 ? "s" : "" );
			}

			if ( m_Crafter != null && !m_Crafter.Deleted )
				sb.AppendFormat( " (crafted by {0})", m_Crafter.Name );

			return sb.ToString();
		}

		private static BaseWeapon m_Fists; // This value holds the default--fist--weapon

		public static BaseWeapon Fists
		{
			get{ return m_Fists; }
			set{ m_Fists = value; }
		}
	}

	public enum CheckSlayerResult
	{
		None,
		Slayer,
		Opposition
	}
}
