using System;
using System.Collections.Generic;

namespace Mother4.Data
{
	internal class EnemyBattleBackgrounds
	{
		public static string GetFile(EnemyType enemy)
		{
			if (EnemyBattleBackgrounds.backgrounds.ContainsKey(enemy))
			{
				return Paths.GRAPHICS + "BBG/xml/" + EnemyBattleBackgrounds.backgrounds[enemy];
			}
			return "";
		}

		private static Dictionary<EnemyType, string> backgrounds = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"Fiestabands1.xml"
			},
			{
				EnemyType.MagicSnail,
				"stripedcandy.xml"
			},
			{
				EnemyType.Stickat,
				"stripedcandy.xml"
			},
			{
				EnemyType.Mouse,
				"Flowerpop1.xml"
			},
			{
				EnemyType.HermitCan,
				"Blopple.xml"
			},
			{
				EnemyType.Flamingo,
				"Blopple.xml"
			},
			{
				EnemyType.AtomicPowerRobo,
				"Technodrome.xml"
			},
			{
				EnemyType.CarbonPup,
				"Diamondine.xml"
			},
			{
				EnemyType.MeltyRobot,
				"Diamondine.xml"
			},
			{
				EnemyType.ModernMind,
				"blue.xml"
			},
            {
                EnemyType.NotSoDeer,
                "Deer.xml"
			},
            {
				EnemyType.MysteriousTank,
				"FATFUCKINGTANK.xml"
			}
		};
	}
}
