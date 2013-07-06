using System;
using Server.Multis;
using Server.Targeting;
using Server.Items;
using Server.Regions;

namespace Server.SkillHandlers
{
	public class DetectHidden
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.DetectHidden].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile src )
		{
			src.SendLocalizedMessage( 500819 );//Where will you search?
			src.Target = new InternalTarget();
			src.RevealingAction();

			return TimeSpan.FromSeconds( 7.5 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 12, true, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile src, object targ )
			{
				src.RevealingAction();

				bool foundAnyone = true;

				double srcSkill = src.Skills[SkillName.DetectHidden].Value;
				Point3D p;
				if ( targ is Mobile )
					p = ((Mobile)targ).Location;
				else if ( targ is Item )
					p = ((Item)targ).Location;
				else if ( targ is IPoint3D )
					p = new Point3D( (IPoint3D)targ );
				else 
					p = src.Location;

				if ( src.CheckSkill( SkillName.DetectHidden, 0.0, 100.0 ) )
				{
					IPooledEnumerable inRange = src.Map.GetMobilesInRange( p, 2 );
					foreach ( Mobile trg in inRange )
					{
						if ( trg.Hidden && src != trg )
						{
							double ss = srcSkill + Utility.RandomMinMax( -20, 20 );
							double ts = trg.Skills[SkillName.Hiding].Value + Utility.RandomMinMax( -20, 20 );
							if ( src.AccessLevel >= trg.AccessLevel && ( ss >= ts ) )
							{
								trg.RevealingAction();
								trg.SendLocalizedMessage( 500814 ); // You have been revealed!
								foundAnyone = true;
							}
						}
					}
					inRange.Free();
				}

				if ( !foundAnyone )
					src.SendLocalizedMessage( 500817 ); // You can see nothing hidden there.
			}
		}
	}
}
