using System;
using System.Collections.Generic;
using Carbine.Utility;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Collision
{
	internal class CollisionManagerOld
	{
		public CollisionManagerOld(int width, int height)
		{
			this.quad = new CollisionQuadtree(0, new Rectangle(0f, 0f, (float)width, (float)height));
		}

		public void Add(ICollidable collidable)
		{
			this.quad.Insert(collidable);
		}

		public void AddAll<T>(ICollection<T> collidables) where T : ICollidable
		{
			foreach (T t in collidables)
			{
				ICollidable collidable = t;
				this.Add(collidable);
			}
		}

		public void Remove(ICollidable collidable)
		{
			this.quad.Remove(collidable);
		}

		public void Update(ICollidable collidable)
		{
			this.quad.Remove(collidable);
			this.quad.Insert(collidable);
		}

		public PlaceFreeContext PlaceFree(ICollidable obj, Vector2f position)
		{
			Vector2f position2 = obj.Position;
			obj.Position = position;
			List<ICollidable> list = new List<ICollidable>();
			list = this.quad.Retrieve(obj);
			obj.Position = position2;
			foreach (ICollidable collidable in list)
			{
				if (this.PlaceFreeBroadPhase(obj, position, collidable))
				{
					bool flag = this.CheckPositionCollision(obj, position, collidable);
					if (flag)
					{
						return new PlaceFreeContext
						{
							PlaceFree = false,
							CollidingObject = collidable
						};
					}
				}
			}
			return new PlaceFreeContext
			{
				PlaceFree = true
			};
		}

		public List<ICollidable> ObjectsAtPosition(Vector2f position)
		{
			List<ICollidable> list = new List<ICollidable>();
			List<ICollidable> list2 = new List<ICollidable>();
			list2 = this.quad.Retrieve(position);
			foreach (ICollidable collidable in list2)
			{
				if (position.X >= collidable.Position.X + collidable.AABB.Position.X && position.X < collidable.Position.X + collidable.AABB.Position.X + collidable.AABB.Size.X && position.Y >= collidable.Position.Y + collidable.AABB.Position.Y && position.Y < collidable.Position.Y + collidable.AABB.Position.Y + collidable.AABB.Size.Y)
				{
					list.Add(collidable);
				}
			}
			return list;
		}

		private bool PlaceFreeBroadPhase(ICollidable objA, Vector2f position, ICollidable objB)
		{
			if (objA == objB)
			{
				return false;
			}
			if (objA.AABB.OnlyPlayer && !objB.AABB.IsPlayer)
			{
				return false;
			}
			if (!objA.Solid || !objB.Solid)
			{
				return false;
			}
			FloatRect floatRect = objA.AABB.GetFloatRect();
			floatRect.Left += position.X;
			floatRect.Top += position.Y;
			FloatRect floatRect2 = objB.AABB.GetFloatRect();
			floatRect2.Left += objB.Position.X;
			floatRect2.Top += objB.Position.Y;
			return floatRect.Intersects(floatRect2);
		}

		private bool CheckPositionCollision(ICollidable objA, Vector2f position, ICollidable objB)
		{
			int count = objA.Mesh.Edges.Count;
			int count2 = objB.Mesh.Edges.Count;
			for (int i = 0; i < count + count2; i++)
			{
				Vector2f vector2f;
				if (i < count)
				{
					vector2f = objA.Mesh.Normals[i];
				}
				else
				{
					vector2f = objB.Mesh.Normals[i - count];
				}
				vector2f = VectorMath.Normalize(vector2f);
				float minA = 0f;
				float minB = 0f;
				float maxA = 0f;
				float maxB = 0f;
				this.ProjectPolygon(vector2f, objA.Mesh, position, ref minA, ref maxA);
				this.ProjectPolygon(vector2f, objB.Mesh, objB.Position, ref minB, ref maxB);
				if (this.IntervalDistance(minA, maxA, minB, maxB) > 0f)
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckCollision(ICollidable objA, ICollidable objB)
		{
			if (objA == objB)
			{
				return false;
			}
			if (objA.AABB.OnlyPlayer && !objB.AABB.IsPlayer)
			{
				return false;
			}
			FloatRect floatRect = objA.AABB.GetFloatRect();
			floatRect.Left += objA.Position.X;
			floatRect.Top += objA.Position.Y;
			FloatRect floatRect2 = objB.AABB.GetFloatRect();
			floatRect2.Left += objB.Position.X;
			floatRect2.Top += objB.Position.Y;
			return floatRect.Intersects(floatRect2);
		}

		private float IntervalDistance(float minA, float maxA, float minB, float maxB)
		{
			if (minA < minB)
			{
				return minB - maxA;
			}
			return minA - maxB;
		}

		private void ProjectPolygon(Vector2f normal, Mesh mesh, Vector2f offset, ref float min, ref float max)
		{
			float num = VectorMath.DotProduct(normal, mesh.Vertices[0] + offset);
			min = num;
			max = num;
			for (int i = 0; i < mesh.Vertices.Count; i++)
			{
				num = VectorMath.DotProduct(mesh.Vertices[i] + offset, normal);
				if (num < min)
				{
					min = num;
				}
				else if (num > max)
				{
					max = num;
				}
			}
		}

		public void Draw(RenderTarget target)
		{
			this.quad.DebugDraw(target);
		}

		private CollisionQuadtree quad;
	}
}
