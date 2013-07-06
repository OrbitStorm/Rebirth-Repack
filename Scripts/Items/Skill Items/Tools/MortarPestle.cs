using System;
using System.Reflection;
using Server;
using Server.Engines.Craft;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
	public class MortarPestle : BaseTool
	{
		private Type m_Contains;

		public override CraftSystem GetCraftInstance()
		{
			return new AlchemySystem();
		}

		public Type PotionToMake
		{
			get { return m_Contains; }
			set { m_Contains = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string PotionToMakeName
		{
			get { return m_Contains == null ? "-null-" : m_Contains.FullName; } 
			set { m_Contains = Type.GetType( value, false, true ); }
		}

		[Constructable]
		public MortarPestle() : base( 0xE9B )
		{
			Weight = 1.0;
		}

		[Constructable]
		public MortarPestle( int uses ) : base( uses, 0xE9B )
		{
			Weight = 1.0;
		}

		public MortarPestle( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Contains != null )
			{
				from.BeginTarget( 3, false, TargetFlags.None, new TargetCallback( OnTargetBottle ) );
				from.SendAsciiMessage( "Where is an empty bottle for your potion?" );
			}
			else
			{
				base.OnDoubleClick( from );
			}
		}

		private void OnTargetBottle( Mobile from, object target )
		{
			if ( target is Bottle )
			{
				Bottle bottle = (Bottle)target;
				object parent = bottle.RootParent;
				
				if ( parent == from || ( parent == null && from.InRange( bottle.GetWorldLocation(), 3 ) ) )
				{
					Item potion;
					try
					{
						ConstructorInfo ctor = m_Contains.GetConstructor( Type.EmptyTypes );
						potion = ctor.Invoke( null ) as Item;
					}
					catch
					{
						potion = null;
					}

					m_Contains = null;
					if ( potion != null )
					{
						from.PublicOverheadMessage( MessageType.Emote, 0x25, true, String.Format( "*{0} pours the completed potion into a bottle.*", from.Name ) );
						from.PlaySound( 0x240 );
						from.AddToBackpack( potion );

						bottle.Consume();
					}
					else
					{
						from.SendAsciiMessage( "Maybe there was nothing in the mortar after all...." );
					}
				}
				else
				{
					from.SendAsciiMessage( "The bottle must be in your backpack to use." );
				}
			}
			else
			{
				from.SendAsciiMessage( "That is not an empty bottle." );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			if ( m_Contains != null )
				writer.Write( m_Contains.FullName );
			else
				writer.Write( "" );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 1:
				{
					string name = reader.ReadString();
					if ( name != null && name.Length > 0 )
					{
						try
						{
							m_Contains = Type.GetType( name );
						}
						catch
						{
							m_Contains = null;
						}
					}
					goto case 0;
				}
				case 0:
				{
					break;
				}
			}
		}
	}
}
