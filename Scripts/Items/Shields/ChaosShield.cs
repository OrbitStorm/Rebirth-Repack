using System;
using Server;
using Server.Guilds;

namespace Server.Items
{
	public abstract class VirtueShield : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 1; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }
		public override int InitMinHits{ get{ return 100; } }
		public override int InitMaxHits{ get{ return 125; } }
		public override int AosStrReq{ get{ return 95; } }

		public VirtueShield( int itemid ) : base( itemid )
		{
		}

		public VirtueShield( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}

		public override bool OnEquip( Mobile from )
		{
			if ( !Validate( from ) )
			{
				Destroy();
				return false;
			}
			else
			{
				return base.OnEquip( from );
			}
		}

		public override void OnAdded( object parent )
		{
			if ( !Validate( RootParent as Mobile ) )
				Destroy();
		}

		public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
		{
			return false;
		}

		public override bool OnDragLift(Mobile from)
		{
			if ( RootParent == null && Map != Map.Internal )
			{
				Destroy();
				return false;
			}

			return base.OnDragLift (from);
		}

		public override bool OnDroppedToWorld(Mobile from, Point3D p)
		{
			from.PlaySound( 0x307 );
			Delete();
			return false;
		}

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			parent.PlaySound( 0x307 );
			Delete();

			return DeathMoveResult.MoveToCorpse;
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			return true;
		}

		public virtual void Destroy()
		{
			if ( !Deleted )
			{
				object root = RootParent;
				if ( root is Mobile )
				{
					Mobile from = (Mobile)root;
					
					// exploison type thing
					from.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
					from.PlaySound( 0x307 );

					from.SendAsciiMessage( "Thou hast strayed from the path of virtue!" );
					from.Kill();
				}

				Delete();
			}
		}

		public bool Validate( Mobile m )
		{
			if ( m == null )
				return false;
			else if ( !m.Player || m.AccessLevel > AccessLevel.Player )
				return true;
			else
				return m.Karma >= (int)Noto.NobleLordLady;
		}
	}

	public class ChaosShield : VirtueShield
	{
		public override int ArmorBase{ get{ return 32; } }

		[Constructable]
		public ChaosShield() : base( 0x1BC3 )
		{
			Weight = 5.0;
		}

		public ChaosShield( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}
	}
}
