using System;
using SFML.System;

namespace Carbine.Collision
{
	public class CollisionContext
	{
		public ICollidable Other { get; private set; }

		public bool Colliding { get; private set; }

		public bool WillCollide { get; private set; }

		public Vector2f MinimumTranslation { get; private set; }

		public CollisionContext(ICollidable other, bool colliding, bool willCollide, Vector2f minTranslation)
		{
			this.Other = other;
			this.Colliding = colliding;
			this.WillCollide = willCollide;
			this.MinimumTranslation = minTranslation;
		}
	}
}
