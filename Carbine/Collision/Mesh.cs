using System;
using System.Collections.Generic;
using Carbine.Utility;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Collision
{
	public class Mesh
	{
		public List<Vector2f> Vertices
		{
			get
			{
				return this.vertices;
			}
		}

		public List<Vector2f> Edges
		{
			get
			{
				return this.edges;
			}
		}

		public List<Vector2f> Normals
		{
			get
			{
				return this.normals;
			}
		}

		public AABB AABB
		{
			get
			{
				return this.aabb;
			}
		}

		public Vector2f Center
		{
			get
			{
				return new Vector2f(this.aabb.Size.X / 2f, this.aabb.Size.Y / 2f);
			}
		}

		public Mesh(List<Vector2f> points)
		{
			this.AddPoints(points);
		}

		public Mesh(FloatRect rectangle)
		{
			this.AddRectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		}

		public Mesh(IntRect rectangle)
		{
			this.AddRectangle((float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Width, (float)rectangle.Height);
		}

		private void AddPoints(List<Vector2f> points)
		{
			this.vertices = new List<Vector2f>();
			this.edges = new List<Vector2f>();
			this.normals = new List<Vector2f>();
			for (int i = 0; i < points.Count; i++)
			{
				this.vertices.Add(points[i]);
				int index = (i + 1) % points.Count;
				float x = points[index].X - points[i].X;
				float y = points[index].Y - points[i].Y;
				Vector2f vector2f = new Vector2f(x, y);
				this.edges.Add(vector2f);
				Vector2f item = VectorMath.RightNormal(vector2f);
				this.normals.Add(item);
			}
			this.aabb = this.GetAABB();
		}

		private void AddRectangle(float x, float y, float width, float height)
		{
			this.AddPoints(new List<Vector2f>
			{
				new Vector2f(x, y),
				new Vector2f(x + width, y),
				new Vector2f(x + width, y + height),
				new Vector2f(x, y + height)
			});
		}

		private AABB GetAABB()
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			float num4 = float.MaxValue;
			foreach (Vector2f vector2f in this.vertices)
			{
				num3 = ((vector2f.X < num3) ? vector2f.X : num3);
				num = ((vector2f.X > num) ? vector2f.X : num);
				num4 = ((vector2f.Y < num4) ? vector2f.Y : num4);
				num2 = ((vector2f.Y > num2) ? vector2f.Y : num2);
			}
			return new AABB(new Vector2f(num3, num4), new Vector2f(num - num3, num2 - num4));
		}

		private List<Vector2f> vertices;

		private List<Vector2f> edges;

		private List<Vector2f> normals;

		private AABB aabb;
	}
}
