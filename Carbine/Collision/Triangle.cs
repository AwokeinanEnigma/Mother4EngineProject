using System;
using SFML.System;

namespace Carbine.Collision
{
	internal struct Triangle
	{
		public Triangle(Vector2f v1, Vector2f v2, Vector2f v3)
		{
			this.V1 = v1;
			this.V2 = v2;
			this.V3 = v3;
		}

		public Vector2f V1;

		public Vector2f V2;

		public Vector2f V3;
	}
}
