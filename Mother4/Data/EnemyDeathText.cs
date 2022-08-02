using System;
using System.Collections.Generic;

namespace Mother4.Data
{
	internal class EnemyDeathText
	{
		public static string Get(EnemyType enemy)
		{
			if (EnemyDeathText.texts.ContainsKey(enemy))
			{
				return EnemyDeathText.texts[enemy];
			}
			return "was defeated!";
		}

		private static Dictionary<EnemyType, string> texts = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"no longer lives!"
			},
			{
				EnemyType.MagicSnail,
				"went back to normal."
			},
			{
				EnemyType.Stickat,
				"disappeared into thin air."
			},
			{
				EnemyType.Mouse,
				"went back to normal."
			},
			{
				EnemyType.HermitCan,
				"hid itself away forever."
			},
			{
				EnemyType.Flamingo,
				"flew off to bother someone else."
			},
			{
				EnemyType.AtomicPowerRobo,
				"blew up into a million pieces."
			},
			{
				EnemyType.CarbonPup,
				"bit the dust."
			},
			{
				EnemyType.MeltyRobot,
				"melted into nothingness."
			},
			{
				EnemyType.ModernMind,
				"burst into light!"
			},
			{
				EnemyType.NotSoDeer,
				"went back to normal."
			}
		};
	}
}
