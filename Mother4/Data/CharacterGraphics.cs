using System;
using System.Collections.Generic;

namespace Mother4.Data
{
	internal static class CharacterGraphics
	{
		public static string GetFile(CharacterType character)
		{
			return CharacterGraphics.GetFile(character, true);
		}

		public static string GetFile(CharacterType character, bool fullPath)
		{
			if (!CharacterGraphics.graphics.ContainsKey(character))
			{
				return "";
			}
			if (!fullPath)
			{
				return CharacterGraphics.graphics[character];
			}
			return Paths.GRAPHICS + CharacterGraphics.graphics[character] + CharacterGraphics.EXTENSION;
		}

		private static string EXTENSION = ".dat";

		private static Dictionary<CharacterType, string> graphics = new Dictionary<CharacterType, string>
		{
			{
				CharacterType.Travis,
				"travis"
			},
			{
				CharacterType.Meryl,
				"meryl"
			},
			{
				CharacterType.Floyd,
				"floyd"
			},
			{
				CharacterType.Leo,
				"leo"
			},
			{
				CharacterType.Zack,
				"zack"
			},
			{
				CharacterType.Renee,
				"sensitivityinred"
			},
			{
				CharacterType.Dog,
				"mutt"
			}
		};
	}
}
