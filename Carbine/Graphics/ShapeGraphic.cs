using System;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Graphics
{
	public class ShapeGraphic : Renderable
	{
		public Shape Shape
		{
			get
			{
				return this.shape;
			}
		}

		public override Vector2f Position
		{
			get
			{
				return this.shape.Position;
			}
			set
			{
				this.shape.Position = value;
			}
		}

		public override Vector2f Origin
		{
			get
			{
				return this.shape.Origin;
			}
			set
			{
				this.shape.Origin = value;
			}
		}

		public Color FillColor
		{
			get
			{
				return this.shape.FillColor;
			}
			set
			{
				this.shape.FillColor = value;
			}
		}

		public Color OutlineColor
		{
			get
			{
				return this.shape.OutlineColor;
			}
			set
			{
				this.shape.OutlineColor = value;
			}
		}

		public ShapeGraphic(Shape shape, Vector2f position, Vector2f origin, Vector2f size, int depth)
		{
			this.size = size;
			this.depth = depth;
			this.shape = shape;
			this.shape.Position = position;
			this.shape.Origin = origin;
		}

		public override void Draw(RenderTarget target)
		{
			target.Draw(this.shape);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private Shape shape;
	}
}
