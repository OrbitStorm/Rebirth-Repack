using System;
using Server;
using Server.Targeting;

namespace Server.Items
{
	public interface IIdentifiable
	{
		void OnIdentify( Mobile from );
	}

	public class ItemIdentification
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile from )
		{
			from.SendLocalizedMessage( 500343 ); // What do you wish to appraise and identify?
			from.Target = new InternalTarget();

			return TimeSpan.FromSeconds( 10.0 );
		}

		public class InternalTarget : Target
		{
			private bool m_CheckSkill;
			private TargetCallback m_OK;

			public InternalTarget() : this( true, null )
			{
			}

			public InternalTarget( bool checkSkill, TargetCallback ok  ) :  base ( 8, false, TargetFlags.None )
			{
				AllowNonlocal = true;
				m_CheckSkill = checkSkill;
				m_OK = ok;
			}

			protected override void OnNonlocalTarget(Mobile from, object targeted)
			{
				OnTarget (from, targeted);
			}

			protected override void OnTargetInSecureTrade(Mobile from, object targeted)
			{
				OnTarget (from, targeted);
			}

			protected override void OnTargetNotAccessible(Mobile from, object targeted)
			{
				OnTarget (from, targeted);
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Item )
				{
					if ( !m_CheckSkill || from.CheckTargetSkill( SkillName.ItemID, o, 0, 100 ) )
					{
						if ( o is IIdentifiable )
							((IIdentifiable)o).OnIdentify( from );
						else if ( o is Spellbook )
							from.SendAsciiMessage( "You guess it contains about {0} spells....", Math.Max( 0, ((Spellbook)o).SpellCount + Utility.RandomMinMax( -8, 8 ) ) );

						((Item)o).OnSingleClick( from );

						if ( m_OK != null )
							m_OK( from, o );
					}
					else
					{
						from.SendLocalizedMessage( 500353 ); // You are not certain...
					}
				}
				else if ( o is Mobile )
				{
					((Mobile)o).OnSingleClick( from );
				}
				else
				{
					from.SendLocalizedMessage( 500353 ); // You are not certain...
				}
			}
		}
	}
}
