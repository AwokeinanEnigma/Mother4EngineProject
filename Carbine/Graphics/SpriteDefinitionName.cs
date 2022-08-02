using System;

namespace Carbine.Graphics
{
	internal struct SpriteDefinitionName
	{
		public SpriteDefinitionName(string name)
		{
			this.Name = name;
		}

		public override bool Equals(object obj)
		{
			return obj is SpriteDefinitionName && string.Equals(((SpriteDefinitionName)obj).Name, this.Name, StringComparison.OrdinalIgnoreCase);
		}

		public readonly string Name;
	}
}
