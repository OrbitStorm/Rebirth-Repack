using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a slimy corpse" )]
	public class Slime : BaseCreature
	{
		[Constructable]
		public Slime() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.45, 0.8 )
		{
			Body = 51;
			Name = Utility.Random( 10 ) != 0 ? "a slime" : "a gwanus" ;
			Hue = Utility.RandomSlimeHue();
			SetStr( 22, 34 );
			SetHits( 11, 17 );
			SetDex( 16, 21 );
			SetStam( 16, 21 );
			SetInt( 16, 20 );
			SetMana( 16, 20 );
			Karma = -125;

			Tamable = true;
			MinTameSkill = 40;
			BaseSoundID = 456;
			SetSkill( SkillName.Tactics, 19.3, 34 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );
			SetSkill( SkillName.Parry, 15.1, 21 );
			SetSkill( SkillName.Wrestling, 19.3, 34 );

			SetDamage( 1, 5 );

			if ( Utility.Random( 3 ) == 0 )
				PackItem( Loot.RandomGem() );
			if ( Utility.Random( 3 ) == 0 )
				PackItem( new LesserPoisonPotion() );
			if ( Utility.Random( 3 ) == 0 )
				PackGold( 1, 10 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }		
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override Poison HitPoison{ get{ return Poison.Lesser; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish | FoodType.FruitsAndVegies | FoodType.GrainsAndHay | FoodType.Eggs; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( !willKill && amount >= this.Hits/4 && this.Hits >= this.HitsMax / 2 && Utility.RandomBool() )
			{
				Slime s = new Slime();
				s.Combatant = from;
				s.SetHits( this.HitsMax );
				s.Hits = (this.Hits-amount) / 2;
				if ( s.Hits <= 0 )
					s.Hits = 1;
				this.Hits = s.Hits;
				s.Hue = this.Hue;
				
				s.MoveToWorld( Location, Map );

				while ( s.Backpack != null && s.Backpack.Items.Count > 0 )
					((Item)s.Backpack.Items[0]).Delete();

				s.Emote( "*The slime splits when struck!*" );

				this.PlaySound( Utility.Random( 5 ) + 456 );
			}
			base.OnDamage( amount, from, willKill );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );
			if ( Utility.Random( 3 ) == 0 )
			{
				BaseWeapon w = attacker.Weapon as BaseWeapon;
				if ( w == null || w is Fists || w is BaseRanged || w.MaxHits <= 0 || !this.InRange( attacker.Location, 1 ) )
					return;

				if ( w.Hits == 0 )
					w.Delete();
				else
					w.Hits--;
				if ( attacker.NetState != null )
					attacker.PrivateOverheadMessage( Server.Network.MessageType.Regular, attacker.EmoteHue, true, "*Acid blood scars your weapon!*", attacker.NetState );
			}
		}

		public Slime( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}

