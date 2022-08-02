using System;
using Carbine.Utility;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Collision
{
	public class SolidStatic : ICollidable
	{
		public Vector2f Position { get; set; }

		public Vector2f Velocity
		{
			get
			{
				return VectorMath.ZERO_VECTOR;
			}
		}

		public AABB AABB { get; private set; }

		public Mesh Mesh { get; private set; }

		public bool Solid { get; set; }

		public VertexArray DebugVerts { get; private set; }

		public SolidStatic(Mesh mesh)
		{
			this.Mesh = mesh;
			this.AABB = mesh.AABB;
			this.Position = new Vector2f(0f, 0f);
			this.Solid = true;
			VertexArray vertexArray = new VertexArray(PrimitiveType.LinesStrip, (uint)(mesh.Vertices.Count + 1));
			for (int i = 0; i < mesh.Vertices.Count; i++)
			{
				vertexArray[(uint)i] = new Vertex(mesh.Vertices[i], Color.Red);
			}
			vertexArray[(uint)mesh.Vertices.Count] = new Vertex(mesh.Vertices[0], Color.Red);
			this.DebugVerts = vertexArray;
		}

		public void Collision(CollisionContext context)
		{
		}
	}
}
