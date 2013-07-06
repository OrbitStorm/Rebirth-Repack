using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Multis;

namespace Server.SkillHandlers
{
	public class Hiding
	{
		public static void Initialize()
		{
			SkillInfo.Table[21].Callback = new SkillUseCallback( OnUse );
		}

		public static bool CheckCombat( Mobile m, int range )
		{
			if ( m.Combatant != null && !m.Combatant.Deleted && m.Combatant.Alive && m.CanSee( m.Combatant ) && m.InRange( m.Combatant, (int)(range*1.5) ) && m.Combatant.InLOS( m ) )
				return true;

			IPooledEnumerable eable = m.GetMobilesInRange( range );
			foreach ( Mobile check in eable )
			{
				if ( check.Combatant == m && check.InLOS( m ) )
				{
					eable.Free();
					return true;
				}
			}

			eable.Free();
			return false;
		}

		public static TimeSpan OnUse( Mobile m )
		{
			bool ok;
			if ( CheckCombat( m, (int)(13 - m.Skills[SkillName.Hiding].Value / 10 )) )
				ok = m.CheckSkill( SkillName.Hiding, 90, 140 );
			else
				ok =  m.CheckSkill( SkillName.Hiding, 0, 100 );

			if ( !ok )
			{
				m.RevealingAction();
				m.LocalOverheadMessage( MessageType.Regular, 0x22, true, "You can't seem to hide here." );
			}
			else
			{
				m.Hidden = true;
				m.LocalOverheadMessage( MessageType.Regular, 0x1F4, true, "You have hidden yourself well." );
			}
			return TimeSpan.FromSeconds( 10.0 );
		}
	}
}

