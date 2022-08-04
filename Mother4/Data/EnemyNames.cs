using System;
using System.Collections.Generic;

namespace Mother4.Data
{
	internal class EnemyNames
	{
		public static string GetName(EnemyType enemy)
		{
			if (EnemyNames.names.ContainsKey(enemy))
			{
				return EnemyNames.names[enemy];
			}
			return "DUMMY";
		}

		public static string GetArticle(EnemyType enemy)
		{
			if (EnemyNames.articles.ContainsKey(enemy))
			{
				return EnemyNames.articles[enemy] + " ";
			}
			return "THE";
		}

		public static string GetSubjectivePronoun(EnemyType enemy)
		{
			if (EnemyNames.names.ContainsKey(enemy))
			{
				return EnemyNames.subjectivePronouns[enemy];
			}
			return "IT";
		}

		public static string GetPosessivePronoun(EnemyType enemy)
		{
			if (EnemyNames.names.ContainsKey(enemy))
			{
				return EnemyNames.posessivePronouns[enemy];
			}
			return "ITS";
		}

		private static Dictionary<EnemyType, string> names = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"Dopefish"
			},
			{
				EnemyType.MagicSnail,
				"Magic Snail"
			},
			{
				EnemyType.Stickat,
				"Stickat"
			},
			{
				EnemyType.Mouse,
				"Mouse"
			},
			{
				EnemyType.HermitCan,
				"Hermit Can"
			},
			{
				EnemyType.Flamingo,
				"Flamingo"
			},
			{
				EnemyType.AtomicPowerRobo,
				"Atomic Power Robo"
			},
			{
				EnemyType.CarbonPup,
				"Carbon Pup"
			},
			{
				EnemyType.MeltyRobot,
				"Melty Robot"
			},
			{
				EnemyType.ModernMind,
				"Modern Mind"
			},
            {
                EnemyType.NotSoDeer,
                "Not-So-Deer"
            },
            {
                EnemyType.MysteriousTank,
                "Mysterious Tank"
            },
            {
                EnemyType.RamblingMushroom,
                "Ramblin' Mushrooom"
            },
		};

		private static Dictionary<EnemyType, string> articles = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"the"
			},
			{
				EnemyType.MagicSnail,
				"the"
			},
			{
				EnemyType.Stickat,
				"the"
			},
			{
				EnemyType.Mouse,
				"the"
			},
			{
				EnemyType.HermitCan,
				"the"
			},
			{
				EnemyType.Flamingo,
				"Mr."
			},
			{
				EnemyType.AtomicPowerRobo,
				"the"
			},
			{
				EnemyType.CarbonPup,
				"the"
			},
			{
				EnemyType.MeltyRobot,
				"the"
			},
			{
				EnemyType.ModernMind,
				"the"
			},
            {
                EnemyType.NotSoDeer,
                "the"
            },
            {
                EnemyType.MysteriousTank,
                "the"
            },
            {
                EnemyType.RamblingMushroom,
                "the"
            },
		};

		private static Dictionary<EnemyType, string> subjectivePronouns = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"it"
			},
			{
				EnemyType.MagicSnail,
				"it"
			},
			{
				EnemyType.Stickat,
				"it"
			},
			{
				EnemyType.Mouse,
				"it"
			},
			{
				EnemyType.HermitCan,
				"it"
			},
			{
				EnemyType.Flamingo,
				"he"
			},
			{
				EnemyType.AtomicPowerRobo,
				"it"
			},
			{
				EnemyType.CarbonPup,
				"it"
			},
			{
				EnemyType.MeltyRobot,
				"it"
			},
			{
				EnemyType.ModernMind,
				"it"
			},
			{
				EnemyType.NotSoDeer,
				"it"
			},
            {
                EnemyType.MysteriousTank,
                "it"
            },
            {
                EnemyType.RamblingMushroom,
                "it"
            }
		};

		private static Dictionary<EnemyType, string> posessivePronouns = new Dictionary<EnemyType, string>
		{
			{
				EnemyType.Dummy,
				"its"
			},
			{
				EnemyType.MagicSnail,
				"its"
			},
			{
				EnemyType.Stickat,
				"its"
			},
			{
				EnemyType.Mouse,
				"its"
			},
			{
				EnemyType.HermitCan,
				"its"
			},
			{
				EnemyType.Flamingo,
				"his"
			},
			{
				EnemyType.AtomicPowerRobo,
				"its"
			},
			{
				EnemyType.CarbonPup,
				"its"
			},
			{
				EnemyType.MeltyRobot,
				"its"
			},
			{
				EnemyType.ModernMind,
				"its"
			},
            {
                EnemyType.NotSoDeer,
                "its"
            },
            {
                EnemyType.MysteriousTank,
                "its"
            },
            {
                EnemyType.RamblingMushroom,
                "its"
            },
		};
	}
}
