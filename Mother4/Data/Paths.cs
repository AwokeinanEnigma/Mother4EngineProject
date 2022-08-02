using System;
using System.IO;

namespace Mother4.Data
{
	internal static class Paths
	{
		public static readonly string RESOURCES = "Resources" + Path.DirectorySeparatorChar;

		public static readonly string AUDIO = Path.Combine(Paths.RESOURCES, "Audio", "") + Path.DirectorySeparatorChar;

		public static readonly string GRAPHICS = Path.Combine(Paths.RESOURCES, "Graphics", "") + Path.DirectorySeparatorChar;

		public static readonly string PSI_GRAPHICS = Path.Combine(Paths.GRAPHICS, "PSI", "") + Path.DirectorySeparatorChar;

		public static readonly string MAPS = Path.Combine(Paths.RESOURCES, "Maps", "") + Path.DirectorySeparatorChar;

		public static readonly string PSI = Path.Combine(Paths.RESOURCES, "Psi", "") + Path.DirectorySeparatorChar;

		public static readonly string TEXT = Path.Combine(Paths.RESOURCES, "Text", "") + Path.DirectorySeparatorChar;

		public static readonly string BATTLE_SWIRL = Path.Combine(Paths.GRAPHICS, "swirl", "") + Path.DirectorySeparatorChar;
	}
}
