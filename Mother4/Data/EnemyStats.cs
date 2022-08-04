using System;
using System.Collections.Generic;
using Mother4.Battle;

namespace Mother4.Data
{
	internal class EnemyStats
	{
		public static StatSet GetStats(EnemyType enemy)
		{
			StatSet result;
			if (EnemyStats.statDict.ContainsKey(enemy))
			{
				result = EnemyStats.statDict[enemy];
			}
			else
			{
				result = EnemyStats.statDict[EnemyType.Dummy];
			}
			return result;
		}

		private static Dictionary<EnemyType, StatSet> statDict = new Dictionary<EnemyType, StatSet>
		{
			{
				EnemyType.Dummy,
				new StatSet
				{
					Level = 1,
					HP = 100,
					PP = 0,
					Offense = 3,
					Defense = 2,
					Guts = 1,
					IQ = 1,
					Luck = 2,
					Speed = 2
				}
			},
			{
				EnemyType.MagicSnail,
				new StatSet
				{
					Level = 1,
					HP = 100,
					PP = 0,
					Offense = 5,
					Defense = 2,
					Guts = 1,
					IQ = 1,
					Luck = 2,
					Speed = 500
				}
			},
			{
                EnemyType.Stickat,
                new StatSet
                {
                    Level = 1,
                    HP = 40,
                    PP = 0,
                    Offense = 5,
                    Defense = 2,
                    Guts = 1,
                    IQ = 1,
                    Luck = 2,
                    Speed = 2
                }
			},
            {
                EnemyType.ModernMind,
                new StatSet
                {
                    Level = 20,
                    HP = 500,
                    PP = 0,
                    Offense = 5,
                    Defense = 2,
                    Guts = 1,
                    IQ = 1,
                    Luck = 2,
                    Speed = 20
                }
            },
            {
                EnemyType.MysteriousTank,
                new StatSet
                {
                    Level = 25,
                    HP = 750,
                    PP = 0,
                    Offense = 7,
                    Defense = 4,
                    Guts = 1,
                    IQ = 1,
                    Luck = 2,
                    Speed = 10
                }
            },
			{
				EnemyType.MeltyRobot,
				new StatSet
				{
					Level = 1,
					HP = 1,
					PP = 0,
					Offense = 3,
					Defense = 2,
					Guts = 1,
					IQ = 1,
					Luck = 2,
					Speed = 2
				}
			},
			{
				EnemyType.NotSoDeer,
				new StatSet
				{
					Level = 1,
					HP = 100,
					PP = 10,
					Offense = 4,
					Defense = 2,
					Guts = 1,
					IQ = 1,
					Luck = 2,
					Speed = 2
				}
			},
            {
                EnemyType.RamblingMushroom,
                new StatSet
                {
                    Level = 1,
                    HP = 75,
                    PP = 0,
                    Offense = 5,
                    Defense = 2,
                    Guts = 1,
                    IQ = 1,
                    Luck = 2,
                    Speed = 2
                }
            }
		};
	}
}
