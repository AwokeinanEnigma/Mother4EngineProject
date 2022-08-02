using System;
using SFML.System;

namespace Carbine.Graphics
{
	public class SpriteDefinition
	{
		public string Name { get; private set; }

		public Vector2i Coords { get; private set; }

		public Vector2i Bounds { get; private set; }

		public Vector2f Origin { get; private set; }

		public int Frames { get; private set; }

		public float[] Speeds { get; private set; }

		public bool FlipX { get; private set; }

		public bool FlipY { get; private set; }

		public int Mode { get; private set; }

		public int[] Data { get; private set; }

		public SpriteDefinition(string name, Vector2i coords, Vector2i bounds, Vector2f origin, int frames, float[] speeds, bool flipX, bool flipY, int mode, int[] data)
		{
			this.Name = name;
			this.Coords = coords;
			this.Bounds = bounds;
			this.Origin = origin;
			this.Frames = frames;
			this.Speeds = speeds;
			this.FlipX = flipX;
			this.FlipY = flipY;
			this.Mode = mode;
			this.Data = data;
		}
	}
}
