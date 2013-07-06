using System;
using System.Text;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class HairDye : BaseItem
	{
		public override int LabelNumber{ get{ return 1041060; } } // Hair Dye

		public static void Initialize()
		{
			Commands.CommandSystem.Register( "TileHairDye", AccessLevel.GameMaster, new Server.Commands.CommandEventHandler( TileHairDye ) );
		}

		public static void TileHairDye( Server.Commands.CommandEventArgs args )
		{
			for (int i=0;i<7;i++)
			{
				int min, max;
				switch ( i )
				{
					default:
					case 0:
						min = 1102; max = 1148;
						break;
					case 1:
						min = 1201; max = 1247 ;
						break;
					case 2:
						min = 1301; max = 1354 ;
						break;
					case 3:
						min = 1401; max = 1447 ;
						break;
					case 4:
						min = 1501; max = 1547 ;
						break;
					case 5:
						min = 1601; max = 1654 ;
						break;
					case 6:
						min = 2201; max = 2224 ;
						break;
					case 7:
						min = 2401; max = 2430 ;
						break;
				}

				for(int h=min;h<=max;h++)
				{
					Item item = new Static( 0xeff );
					item.Hue = h;
					item.Location = new Point3D( args.Mobile.Location.X + (h-min), args.Mobile.Location.Y + i, args.Mobile.Location.Z );
					item.Map = args.Mobile.Map;
				}
			}

			args.Mobile.SendMessage( "Done tiling hair dye." );
		}

		[Constructable]
		public HairDye() : base( 0xEFF )
		{
			/*if ( Utility.Random( 50 ) == 0 ) // 2% chance to be special dye
			{
				switch ( Utility.Random( 7 ) )
				{
					case 0:
						this.Hue = 12 + Utility.Random( 10 );
						break;
					case 1:
						this.Hue = 32 + Utility.Random( 13 );
						break;
					case 2:
						this.Hue = 54 + Utility.Random( 3 );
						break;
					case 3:
						this.Hue = 62 + Utility.Random( 10 );
						break;
					case 4:
						this.Hue = 81 + Utility.Random( 2 );
						break;
					case 5:
						this.Hue = 89 + Utility.Random( 2 );
						break;
					case 6:
						this.Hue = 1153 + Utility.Random( 2 );
						break;
				}
			}
			else
			{
				this.Hue = Utility.RandomHairHue(); // 1102 to 1149
			}*/

			switch ( Utility.Random( 8 ) )
			{
				default:
				case 0:
					this.Hue = Utility.RandomMinMax( 1102, 1148 );
					break;
				case 1:
					this.Hue = Utility.RandomMinMax( 1201, 1247 );
					break;
				case 2:
					this.Hue = Utility.RandomMinMax( 1301, 1354 );
					break;
				case 3:
					this.Hue = Utility.RandomMinMax( 1401, 1447 );
					break;
				case 4:
					this.Hue = Utility.RandomMinMax( 1501, 1547 );
					break;
				case 5:
					this.Hue = Utility.RandomMinMax( 1601, 1654 );
					break;
				case 6:
					this.Hue = Utility.RandomMinMax( 2201, 2224 );
					break;
				case 7:
					this.Hue = Utility.RandomMinMax( 2401, 2430 );
					break;
			}
			Weight = 1.0;
		}

		public HairDye( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			from.HairHue = this.Hue;
		    from.FacialHairHue = this.Hue;
			
            this.Delete();
		}
	}
}
