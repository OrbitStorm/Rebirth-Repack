using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Spells;

namespace Server.Regions
{
	public class GuardedRegion : BaseRegion
	{
		private static object[] m_GuardParams = new object[1];
		private Type m_GuardType;
        private bool m_Disabled;
        private bool m_PCsOnly;

        public bool Disabled { get { return m_Disabled; } set { m_Disabled = value; } }
        public bool PCsOnly { get { return m_PCsOnly; } set { m_PCsOnly = value; } }
        public virtual RegionFragment Fragment { get { return RegionFragment.Wilderness; } }

		public virtual bool IsDisabled()
		{
			return m_Disabled;
		}

		public static void Initialize()
		{
			CommandSystem.Register( "CheckGuarded", AccessLevel.GameMaster, new CommandEventHandler( CheckGuarded_OnCommand ) );
			CommandSystem.Register( "SetGuarded", AccessLevel.Administrator, new CommandEventHandler( SetGuarded_OnCommand ) );
			CommandSystem.Register( "ToggleGuarded", AccessLevel.Administrator, new CommandEventHandler( ToggleGuarded_OnCommand ) );
            CommandSystem.Register("PartialGuarded", AccessLevel.GameMaster, new CommandEventHandler(PartialGuarded_OnCommand));
		}

        [Usage("CheckGuarded")]
        [Description("Returns a value indicating if the current region is guarded or not.")]
        private static void CheckGuarded_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            GuardedRegion reg = from.Region as GuardedRegion;

            if (reg == null)
                from.SendAsciiMessage("You are not in a guardable region.");
            else
                reg.TellGuardStatus(from);
        }

        [Usage("SetGuarded <true|false>")]
        [Description("Enables or disables guards for the current region.")]
        private static void SetGuarded_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length == 1)
            {
                GuardedRegion reg = from.Region as GuardedRegion;

                if (reg == null)
                {
                    from.SendAsciiMessage("You are not in a guardable region.");
                }
                else
                {
                    reg.Disabled = !e.GetBoolean(0);
                    from.SendAsciiMessage("After your changes:");
                    reg.TellGuardStatus(from);
                }
            }
            else
            {
                from.SendAsciiMessage("Format: SetGuarded <true|false>");
            }
        }

        [Usage("ToggleGuarded")]
        [Description("Toggles the state of guards for the current region.")]
        private static void ToggleGuarded_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            GuardedRegion reg = from.Region as GuardedRegion;

            if (reg == null)
            {
                from.SendAsciiMessage("You are not in a guardable region.");
            }
            else
            {
                reg.Disabled = !reg.Disabled;
                from.SendAsciiMessage("After your changes:");
                reg.TellGuardStatus(from);
            }
        }

        [Usage("PartialGuarded")]
        [Description("Toggles the state of guards (against NPCs only) for the current region.")]
        private static void PartialGuarded_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            GuardedRegion reg = from.Region as GuardedRegion;

            if (reg == null)
            {
                from.SendAsciiMessage("You are not in a guardable region.");
            }
            else
            {
                reg.PCsOnly = !reg.PCsOnly;
                from.SendAsciiMessage("After your changes:");
                reg.TellGuardStatus(from);
            }
        }

        private void TellGuardStatus(Mobile from)
        {
            if (Disabled)
                from.SendAsciiMessage("Guards in this region are totally disabled.");
            else if (PCsOnly)
                from.SendAsciiMessage("Guards in this region will NOT attack NPCs.");
            else
                from.SendAsciiMessage("Guards in this region are fully activated.");
        }

		public static GuardedRegion Disable( GuardedRegion reg )
		{
			reg.Disabled = true;
			return reg;
		}

		public virtual bool AllowReds{ get{ return true; } }

        public virtual Type DefaultGuardType
		{
			get
			{
				return typeof( WarriorGuard );
			}
		}
        
		public GuardedRegion( string name, Map map, int priority, params Rectangle3D[] area ) : base( name, map, priority, area )
		{
			m_GuardType = DefaultGuardType;
		}

		public GuardedRegion( string name, Map map, int priority, params Rectangle2D[] area )
			: base( name, map, priority, area )
		{
			m_GuardType = DefaultGuardType;
		}
		
		public GuardedRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
			XmlElement el = xml["guards"];

			if ( ReadType( el, "type", ref m_GuardType, false ) )
			{
				if ( !typeof( Mobile ).IsAssignableFrom( m_GuardType ) )
				{
					Console.WriteLine( "Invalid guard type for region '{0}'", this );
					m_GuardType = DefaultGuardType;
				}
			}
			else
			{
				m_GuardType = DefaultGuardType;
			}

			bool disabled = false;
			if ( ReadBoolean( el, "disabled", ref disabled, false ) )
				this.Disabled = disabled;
		}

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override void MakeGuard(Mobile focus)
        {
            if (IsDisabled())
                return;

            if (PCsOnly && focus is BaseCreature && !(((BaseCreature)focus).Controled || ((BaseCreature)focus).Summoned))
            {
                BaseGuard useGuard = null;

                foreach (Mobile m in focus.GetMobilesInRange(12))
                {
                    if (m is WeakWarriorGuard)
                    {
                        WeakWarriorGuard g = (WeakWarriorGuard)m;

                        if ((g.Focus == null || !g.Focus.Alive || g.Focus.Deleted) &&
                            (useGuard == null || g.GetDistanceToSqrt(focus) < useGuard.GetDistanceToSqrt(focus))
                            )
                        {
                            useGuard = g;
                        }
                    }
                }

                if (useGuard != null)
                    useGuard.Focus = focus;
            }
            else
            {
                BaseGuard useGuard = null, curGuard = null;
                foreach (Mobile m in focus.GetMobilesInRange(10))
                {
                    if (m is BaseGuard && !(m is WeakWarriorGuard))
                    {
                        BaseGuard g = (BaseGuard)m;

                        if (g.Focus == focus)
                        {
                            curGuard = g;
                        }
                        else if ((g.Focus == null || !g.Focus.Alive || g.Focus.Deleted) &&
                            (useGuard == null || g.GetDistanceToSqrt(focus) < useGuard.GetDistanceToSqrt(focus))
                            )
                        {
                            useGuard = g;
                        }
                    }
                }

                if (useGuard != null)
                {
                    useGuard.Focus = focus;
                }
                else if (curGuard == null)
                {
                    m_GuardParams[0] = focus;

                    Activator.CreateInstance(m_GuardType, m_GuardParams);
                }
            }
        }

        private bool IsEvil(Mobile m)
        {
            // allow dreads in town with partial guards
            return (!PCsOnly && m.Player && m.Karma <= (int)Noto.Dark && m.Alive) || (m is BaseCreature && (m.Body.IsMonster || ((BaseCreature)m).AlwaysMurderer));
        }

        public override void OnEnter(Mobile m)
        {
            if (IsDisabled())
                return;

            //m.SendLocalizedMessage( 500112 ); // You are now under the protection of the town guards.

            if (IsEvil(m))
                CheckGuardCandidate(m);
        }

        public override void OnExit(Mobile m)
        {
            if (IsDisabled())
                return;

            //m.SendLocalizedMessage( 500113 ); // You have left the protection of the town guards.
        }

        public override void OnSpeech(SpeechEventArgs args)
        {
            if (IsDisabled())
                return;

            if (args.Mobile.Alive && args.HasKeyword(0x0007)) // *guards*
                CallGuards(args.Mobile.Location);
        }

        public override void OnAggressed(Mobile aggressor, Mobile aggressed, bool criminal)
        {
            base.OnAggressed(aggressor, aggressed, criminal);

            if (!IsDisabled() && aggressor != aggressed && criminal && aggressor.InRange(aggressed, 20))
                CheckGuardCandidate(aggressor);
        }

        public override void OnCriminalAction(Mobile m, bool message)
        {
            base.OnCriminalAction(m, message);

            if (!IsDisabled())
                CheckGuardCandidate(m);
        }

        public override void SpellDamageScalar(Mobile caster, Mobile target, ref double scalar)
        {
            if (!IsDisabled())
            {
                if (target == caster)
                    return;

                if (PCsOnly && (!caster.Player || !target.Player))
                    return;

                scalar = 0;
            }
        }

        private Hashtable m_GuardCandidates = new Hashtable();

        public void CheckGuardCandidate(Mobile m)
        {
            if (IsDisabled())
                return;

            if (IsGuardCandidate(m))
            {
                if (AddGuardCandidate(m))
                {
                    Map map = m.Map;

                    if (map != null)
                    {
                        Mobile fakeCall = null;
                        double prio = 0.0;

                        foreach (Mobile v in m.GetMobilesInRange(8))
                        {
                            if (!v.Player && v.Body.IsHuman && v != m && !(v is PlayerVendor) && !IsGuardCandidate(v))
                            {
                                double dist = m.GetDistanceToSqrt(v);

                                if (fakeCall == null || dist < prio)
                                {
                                    fakeCall = v;
                                    prio = dist;
                                }
                            }
                        }

                        if (fakeCall != null)
                        {
                            if (!(fakeCall is BaseGuard))
                                fakeCall.Say(Utility.RandomList(1007037, 501603, 1013037, 1013038, 1013039, 1013041, 1013042, 1013043, 1013052));

                            MakeGuard(m);
                            RemoveGuardCandidate(m);
                        }
                    }
                }
            }
        }

        public bool AddGuardCandidate(Mobile m)
        {
            GuardTimer timer = (GuardTimer)m_GuardCandidates[m];

            if (timer == null)
            {
                timer = new GuardTimer(m, m_GuardCandidates);
                timer.Start();

                m_GuardCandidates[m] = timer;
                m.SendLocalizedMessage(502275); // Guards can now be called on you!

                return true;
            }
            else
            {
                timer.Stop();
                timer.Start();

                return false;
            }
        }

        public void RemoveGuardCandidate(Mobile m)
        {
            GuardTimer timer = (GuardTimer)m_GuardCandidates[m];

            if (timer != null)
            {
                timer.Stop();
                m_GuardCandidates.Remove(m);
                m.SendLocalizedMessage(502276); // Guards can no longer be called on you.	
            }
        }

        public void CallGuards(Point3D p)
        {
            if (IsDisabled())
                return;

            IPooledEnumerable eable = Map.GetMobilesInRange(p, 14);

            foreach (Mobile m in eable)
            {
                if (IsGuardCandidate(m) && ((IsEvil(m) && GetMobiles().Contains(m)) || m_GuardCandidates.Contains(m)))
                {
                    MakeGuard(m);
                    m_GuardCandidates.Remove(m);
                    m.SendLocalizedMessage(502276); // Guards can no longer be called on you.
                }
            }

            eable.Free();
        }

        public bool IsGuardCandidate(Mobile m)
        {
            if (m is BaseGuard || !m.Alive || m.AccessLevel > AccessLevel.Player || m.Blessed || IsDisabled())
                return false;

            if ( PCsOnly && !m.Player )
            	return false;

            return IsEvil(m) || m.Criminal;
        }

        private class GuardTimer : Timer
        {
            private Mobile m_Mobile;
            private Hashtable m_Table;

            public GuardTimer(Mobile m, Hashtable table)
                : base(TimeSpan.FromSeconds(15.0))
            {
                Priority = TimerPriority.TwoFiftyMS;

                m_Mobile = m;
                m_Table = table;
            }

            protected override void OnTick()
            {
                if (m_Table.Contains(m_Mobile))
                {
                    m_Table.Remove(m_Mobile);
                    m_Mobile.SendLocalizedMessage(502276); // Guards can no longer be called on you.
                }
            }
        }
	}
}
