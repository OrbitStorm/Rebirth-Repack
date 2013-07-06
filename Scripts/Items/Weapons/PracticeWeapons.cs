using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class PracticeSword : Longsword
	{
		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 2; } }
		public override int OldSpeed{ get{ return base.OldSpeed + 5; } }
		public override int NumDice{ get{ return 0; } }

		[Constructable]
		public PracticeSword()
		{
			Name = "longsword (practice weapon)";
		}

		public PracticeSword( Serial serial ) : base( serial )
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
	}

	public class PracticeKryss : Kryss
	{
		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 2; } }
		public override int OldSpeed{ get{ return base.OldSpeed + 5; } }
		public override int NumDice{ get{ return 0; } }

		[Constructable]
		public PracticeKryss()
		{
			Name = "kryss (practice weapon)";
		}

		public PracticeKryss( Serial serial ) : base( serial )
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
	}

	public class PracticeBow : Bow
	{
		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 2; } }
		public override int OldSpeed{ get{ return base.OldSpeed + 5; } }
		public override int NumDice{ get{ return 0; } }

		[Constructable]
		public PracticeBow()
		{
			Name = "bow (practice weapon)";
		}

		public PracticeBow( Serial serial ) : base( serial )
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
	}

	public class PracticeHatchet : Hatchet
	{
		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 2; } }
		public override int OldSpeed{ get{ return base.OldSpeed + 5; } }
		public override int NumDice{ get{ return 0; } }

		[Constructable]
		public PracticeHatchet()
		{
			Name = "hatchet (practice weapon)";
		}

		public PracticeHatchet( Serial serial ) : base( serial )
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
	}

	public class PracticeClub : Club
	{
		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 2; } }
		public override int OldSpeed{ get{ return base.OldSpeed + 5; } }
		public override int NumDice{ get{ return 0; } }

		[Constructable]
		public PracticeClub()
		{
			Name = "club (practice weapon)";
		}

		public PracticeClub( Serial serial ) : base( serial )
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
	}
}

