using System;
using System.Collections.Generic;

namespace Mother4.Data
{
	internal class EnemyGraphics
	{
		public static string GetFilename(EnemyType enemy)
		{
			if (EnemyGraphics.graphics.ContainsKey(enemy))
			{
				return Paths.GRAPHICS + EnemyGraphics.graphics[enemy];
			}
			return Paths.GRAPHICS + "dopefish.dat";
		}

		private static Dictionary<EnemyType, string> graphics = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"dopefish.dat"
			},
			{
				EnemyType.MagicSnail,
				"magicsnail.dat"
			},
			{
				EnemyType.Stickat,
				"stickat.dat"
			},
			{
				EnemyType.Mouse,
				"mouse.dat"
			},
			{
				EnemyType.HermitCan,
				"hermitcan.dat"
			},
			{
				EnemyType.Flamingo,
				"flamingo.dat"
			},
			{
				EnemyType.AtomicPowerRobo,
				"atomicpowerrobo.dat"
			},
			{
				EnemyType.CarbonPup,
				"carbonpup.dat"
			},
			{
				EnemyType.ModernMind,
				"modernmind.dat"
			},
			{
				EnemyType.MeltyRobot,
				"meltyrobot.dat"
			},
            {
                EnemyType.NotSoDeer,
                "notsodeer.dat"
            },
            {
                EnemyType.MysteriousTank,
                "FATFUCKINGTANK.dat"
			},
            {
            EnemyType.RamblingMushroom,
            "mushroomfight.dat"

            }
		};
	}
}
