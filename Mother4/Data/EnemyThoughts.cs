using System;
using System.Collections.Generic;

namespace Mother4.Data
{
	internal class EnemyThoughts
	{
		public static string GetLike(EnemyType enemy)
		{
			if (EnemyThoughts.likes.ContainsKey(enemy))
			{
				return EnemyThoughts.likes[enemy];
			}
			return "NOTHING";
		}

		private static Dictionary<EnemyType, string> likes = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"Commander Keen"
			},
			{
				EnemyType.MagicSnail,
				"shell ettiquite"
			},
			{
				EnemyType.Stickat,
				"furballs"
			},
			{
				EnemyType.Mouse,
				"Red Leicester"
			},
			{
				EnemyType.HermitCan,
				"soda"
			},
			{
				EnemyType.Flamingo,
				"krill"
			},
			{
				EnemyType.AtomicPowerRobo,
				"isotopes and gamma particles"
			},
			{
				EnemyType.CarbonPup,
				"crystalline structures"
			},
			{
				EnemyType.MeltyRobot,
				"Back to the Future II"
			},
            {
                EnemyType.ModernMind,
				"the interconnectedness of all things"
			},
            {
                EnemyType.RamblingMushroom,
                "the state of the mushroom colony"
            }
		};
	}
}
