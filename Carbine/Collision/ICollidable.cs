using System;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Collision
{
	public interface ICollidable
	{
		Vector2f Position { get; set; }

		Vector2f Velocity { get; }

		AABB AABB { get; }

		Mesh Mesh { get; }

		bool Solid { get; set; }

		VertexArray DebugVerts { get; }

		void Collision(CollisionContext context);
	}
}
