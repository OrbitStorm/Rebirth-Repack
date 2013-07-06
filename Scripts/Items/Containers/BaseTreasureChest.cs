using Server;
using Server.Items;
using Server.Network;
using System;

namespace Server.Items
{
	public abstract class BaseTreasureChest : LockableContainer
	{
		private TreasureLevel m_TreasureLevel;
		private short m_MaxSpawnTime = 60;
		private short m_MinSpawnTime = 10;
		private TreasureResetTimer m_ResetTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public TreasureLevel Level
		{
			get
			{
				return m_TreasureLevel;
			}
			set
			{
				m_TreasureLevel = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public short MaxSpawnTime
		{
			get
			{
				return m_MaxSpawnTime;
			}
			set
			{
				m_MaxSpawnTime = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public short MinSpawnTime
		{
			get
			{
				return m_MinSpawnTime;
			}
			set
			{
				m_MinSpawnTime = value;
			}
		}
		
		public BaseTreasureChest( int itemID ) : base ( itemID )
		{
			m_TreasureLevel = TreasureLevel.Level2;
			Locked = true;
			Movable = false;
			
			Key key = (Key)FindItemByType( typeof(Key) );

			if( key != null )
				key.Delete();

			SetLockLevel();
			GenerateTreasure();
		}

		public BaseTreasureChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (byte) m_TreasureLevel );
			writer.Write( m_MinSpawnTime );
			writer.Write( m_MaxSpawnTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_TreasureLevel = (TreasureLevel)reader.ReadByte();
			m_MinSpawnTime = reader.ReadShort();
			m_MaxSpawnTime = reader.ReadShort();

			if( !Locked )
				StartResetTimer();
		}

		protected virtual void SetLockLevel()
		{
			switch( m_TreasureLevel )
			{
				case TreasureLevel.Level1:
					RequiredSkill = 62;
					LockLevel = 60;
					MaxLockLevel = 75;
					break;

				case TreasureLevel.Level2:
					RequiredSkill = 72;
					LockLevel = 70;
					MaxLockLevel = 85;
					break;

				case TreasureLevel.Level3:
					RequiredSkill = 82;
					LockLevel = 80;
					MaxLockLevel = 95;
					break;

				case TreasureLevel.Level4:
					RequiredSkill = 92;
					LockLevel = 90;
					MaxLockLevel = 100;
					break;

				case TreasureLevel.Level5:
					RequiredSkill = 95;
					LockLevel = 95;
					MaxLockLevel = 110;
					break;
			}
		}

		private void StartResetTimer()
		{
			if( m_ResetTimer == null )
				m_ResetTimer = new TreasureResetTimer( this );
			else
				m_ResetTimer.Delay = TimeSpan.FromMinutes( Utility.Random( m_MinSpawnTime, m_MaxSpawnTime ));
				
			m_ResetTimer.Start();
		}

		protected virtual void GenerateTreasure()
		{
			switch( m_TreasureLevel )
			{
				case TreasureLevel.Level1:
					LootPack.ChestLvl1.Generate( this );
					break;

				case TreasureLevel.Level2:
					LootPack.ChestLvl2.Generate( this );
					break;

				case TreasureLevel.Level3:
					LootPack.ChestLvl3.Generate( this );
					break;

				case TreasureLevel.Level4:
					LootPack.ChestLvl4.Generate( this );
					break;

				case TreasureLevel.Level5:
					LootPack.ChestLvl5.Generate( this );
					break;
			}
		}

		public override void LockPick( Mobile from )
		{
			base.LockPick( from );

			StartResetTimer();
		}

		public void ClearContents()
		{
			for ( int i = 0; i < Items.Count ; i++ )
			{
				if ( ((Item)Items[i]).Movable )
				{
					((Item)Items[i]).Delete();
					i--;
				}
			}
		}

		public void Reset()
		{
			if( m_ResetTimer != null )
			{
				if( m_ResetTimer.Running )
					m_ResetTimer.Stop();
			}

			Locked = true;
			
			ClearContents();
			GenerateTreasure();
		}

		public enum TreasureLevel
		{
			Level1, 
			Level2, 
			Level3, 
			Level4, 
			Level5 
		}; 

		private class TreasureResetTimer : Timer
		{
			private BaseTreasureChest m_Chest;
			
			public TreasureResetTimer( BaseTreasureChest chest ) : base ( TimeSpan.FromMinutes( Utility.Random( chest.MinSpawnTime, chest.MaxSpawnTime ) ) )
			{
				m_Chest = chest;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Chest.Reset();
			}
		}
	}
}

