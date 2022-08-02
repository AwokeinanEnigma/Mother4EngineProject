using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Graphics
{
	internal class GraphicGroup : Graphic
	{
		public Graphic this[int index]
		{
			get
			{
				return this.graphics[index];
			}
		}

		public GraphicGroup(Vector2f position, Vector2f origin, int depth)
		{
			this.graphics = new List<Graphic>();
			this.Position = position;
			this.Origin = origin;
			this.Depth = depth;
			this.Size = new Vector2f(0f, 0f);
		}

		public void Add(Graphic graphic)
		{
			this.graphics.Add(graphic);
			this.FindSize();
		}

		public void Remove(Graphic graphic)
		{
			this.graphics.Remove(graphic);
		}

		private void FindSize()
		{
		}

		public override void Draw(RenderTarget target)
		{
			foreach (Graphic graphic in this.graphics)
			{
				graphic.Draw(target);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				foreach (Graphic graphic in this.graphics)
				{
					graphic.Dispose();
				}
			}
			this.disposed = true;
		}

		private List<Graphic> graphics;
	}
}
