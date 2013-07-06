using System;
using System.Collections; using System.Collections.Generic;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Regions;
using Server.Menus.Questions;

namespace Server.Items
{
	[DispellableFieldAttribute]
	public class Moongate : BaseItem
	{
		private Point3D m_Target;
		private Map m_TargetMap;
		private bool m_bDispellable;

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Target
		{
			get
			{
				return m_Target;
			}
			set
			{
				m_Target = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map TargetMap
		{
			get
			{
				return m_TargetMap;
			}
			set
			{
				m_TargetMap = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Dispellable
		{
			get
			{
				return m_bDispellable;
			}
			set
			{
				m_bDispellable = value;
			}
		}

		public virtual bool ShowFeluccaWarning{ get{ return false; } }

		[Constructable]
		public Moongate() : this( Point3D.Zero, null )
		{
			m_bDispellable = true;
		}

		[Constructable]
		public Moongate(bool bDispellable) : this( Point3D.Zero, null )
		{
			m_bDispellable = bDispellable;
		}

		public Moongate( Point3D target, Map targetMap ) : base( 0xF6C )
		{
			Movable = false;
			Light = LightType.Circle300;

			m_Target = target;
			m_TargetMap = targetMap;
		}

		public Moongate( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.Player )
				return;

			CheckGate( from, 1 );
		}

		public override bool OnMoveOver( Mobile m )
		{
			return CheckGate( m, 1 );
		}

		public virtual bool CheckGate( Mobile m, int range )
		{
			if ( m is BaseVendor || m is Banker || m is WanderingHealer || m is EvilWanderingHealer || m is BaseGuard || m is BaseShieldGuard )
			{
				return false;
			}
			else if ( m is BaseCreature && !((BaseCreature)m).CanBeGated )
			{
				m.Say( true, "* The moongate seems to have no effect. *" );
				return false;
			}
			else if ( !m.InRange( this, range ) )
			{
				m.SendLocalizedMessage( 500446 ); // That is too far away.
				return false;
			}
			else if ( m_Target == Point3D.Zero || m_TargetMap == null || m_TargetMap == Map.Internal )
			{
				m.SendAsciiMessage( "This moongate does not seem to go anywhere." );
				return true;
			}
			else if ( m.Player && m.NetState != null && IsInTown( m.Region ) && !IsInTown( this.Target, this.TargetMap ) )
			{
				new MoongateConfirm( this, 0 ).SendTo( m.NetState );
				return true;
			}
			else
			{
				return UseGate( m, range );
			}
		}

		public virtual bool UseGate( Mobile m, int range )
		{
			if ( Deleted )
			{
				return true;
			}
			else if ( !m.InRange( this, range ) )
			{
				m.SendLocalizedMessage( 500446 ); // That is too far away.
				return false;
			}
			else if ( m_Target == Point3D.Zero || m_TargetMap == null || m_TargetMap == Map.Internal )
			{
				m.SendAsciiMessage( "This moongate does not seem to go anywhere." );
				return true;
			}
			else
			{
				BaseCreature.TeleportPets( m, m_Target, m_TargetMap );

				m.Map = m_TargetMap;
				m.Location = m_Target;

				m.PlaySound( 0x1FE );
				return false;
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_Target );
			writer.Write( m_TargetMap );
			
			// Version 1
			writer.Write( m_bDispellable );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Target = reader.ReadPoint3D();
			m_TargetMap = reader.ReadMap();

			if (version >= 1)
			{
				m_bDispellable = reader.ReadBool();
			}
		}

		public static bool IsInTown( Point3D p, Map map )
		{
			if ( map == null )
				return false;

			GuardedRegion reg = Region.Find( p, map ) as GuardedRegion;

			return ( reg != null && !reg.IsDisabled() );
		}

		public static bool IsInTown( Region r )
		{
			return r is GuardedRegion && !((GuardedRegion)r).IsDisabled();
		}
	}

	public class MoongateConfirm : QuestionMenu
	{
		private static string[] m_Options = new string[] { "Yes", "No" };
		private static string m_Prompt = "Dost thou wish to step into the moongate?";

		private int m_Range;
		private Moongate m_Gate;

		public MoongateConfirm( Moongate gate, int range ) : base( m_Prompt, m_Options )
		{
			m_Range = range;
			m_Gate = gate;
		}

		public override void OnResponse(NetState state, int index)
		{
			if ( index == 0 )
				m_Gate.UseGate( state.Mobile, m_Range );
		}
	}
}
