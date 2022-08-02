using System;
using System.Collections.Generic;

namespace Mother4.Data
{
	internal class EnemyMusic
	{
		public static string GetMusic(EnemyType enemy)
		{
			if (EnemyMusic.musics.ContainsKey(enemy))
			{
				return Paths.AUDIO + EnemyMusic.musics[enemy] + ".wav";
			}
			return "";
		}

		private const string RESOURCE_EXT = ".wav";

		private static Dictionary<EnemyType, string> musics = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"Battle Against a Clueless Foe"
			},
			{
				EnemyType.MagicSnail,
				"Battle Against a Clueless Foe"
			},
			{
				EnemyType.Stickat,
				"Battle Against a Clueless Foe"
			},
			{
				EnemyType.Mouse,
				"Battle Against a Clueless Foe"
			},
			{
				EnemyType.HermitCan,
				"Battle Against an Intense Opponent"
			},
			{
				EnemyType.Flamingo,
				"Battle Against an Intense Opponent"
			},
			{
				EnemyType.AtomicPowerRobo,
				"Battle Against a Familiar Foe"
			},
			{
				EnemyType.CarbonPup,
				"Battle Against a Hot Opponent"
			},
			{
				EnemyType.MeltyRobot,
				"Battle Against a Hot Opponent"
			},
			{
				EnemyType.ModernMind,
				"Battle Against an Ultra-Dimensional Foe"
			},
			{
				EnemyType.NotSoDeer,
				"Battle Against an Intense Opponent"
			}
		};
	}
}
