using System;
using Carbine.Collision;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Actors
{
	public abstract class SolidActor : Actor, ICollidable
	{
		public Vector2f LastPosition
		{
			get
			{
				return this.lastPosition;
			}
		}

		public AABB AABB
		{
			get
			{
				return this.aabb;
			}
		}

		public Mesh Mesh
		{
			get
			{
				return this.mesh;
			}
		}

		public bool Solid
		{
			get
			{
				return this.isSolid;
			}
			set
			{
				this.isSolid = value;
			}
		}

		public VertexArray DebugVerts
		{
			get
			{
				return this.GetDebugVerts();
			}
		}

		public virtual bool MovementLocked
		{
			get
			{
				return this.lockedMovement;
			}
			set
			{
				this.lockedMovement = value;
			}
		}

		public SolidActor(CollisionManager colman)
		{
			this.collisionManager = colman;
			this.isSolid = true;
			if (this.collisionManager != null)
			{
				this.collisionManager.Add(this);
			}
		}

		private VertexArray GetDebugVerts()
		{
			if (this.debugVerts == null)
			{
				VertexArray vertexArray = new VertexArray(PrimitiveType.LinesStrip, (uint)(this.mesh.Vertices.Count + 1));
				for (int i = 0; i < this.mesh.Vertices.Count; i++)
				{
					vertexArray[(uint)i] = new Vertex(this.mesh.Vertices[i], Color.Green);
				}
				vertexArray[(uint)this.mesh.Vertices.Count] = new Vertex(this.mesh.Vertices[0], Color.Green);
				this.debugVerts = vertexArray;
			}
			return this.debugVerts;
		}

		protected virtual void HandleCollision(PlaceFreeContext pfc)
		{
		}

		public override void Update()
		{
			this.lastPosition = this.position;
			PlaceFreeContext pfc = (this.collisionManager != null) ? this.collisionManager.PlaceFree(this, this.position) : new PlaceFreeContext
			{
				PlaceFree = true
			};
			if (!pfc.PlaceFree)
			{
				this.HandleCollision(pfc);
			}
			else
			{
				if (this.Velocity.X != 0f && !this.lockedMovement)
				{
					this.moveTemp = new Vector2f(this.Position.X + this.Velocity.X, this.Position.Y);
					pfc = ((this.collisionManager != null) ? this.collisionManager.PlaceFree(this, this.moveTemp) : new PlaceFreeContext
					{
						PlaceFree = true
					});
					if (pfc.PlaceFree)
					{
						this.position = this.moveTemp;
					}
					else
					{
						this.velocity.X = 0f;
						this.HandleCollision(pfc);
					}
				}
				if (this.Velocity.Y != 0f && !this.lockedMovement)
				{
					this.moveTemp = new Vector2f(this.Position.X, this.Position.Y + this.Velocity.Y);
					pfc = ((this.collisionManager != null) ? this.collisionManager.PlaceFree(this, this.moveTemp) : new PlaceFreeContext
					{
						PlaceFree = true
					});
					if (pfc.PlaceFree)
					{
						this.position = this.moveTemp;
					}
					else
					{
						this.velocity.Y = 0f;
						this.HandleCollision(pfc);
					}
				}
			}
			if (!this.lastPosition.Equals(this.position) && this.collisionManager != null)
			{
				this.collisionManager.Update(this, this.lastPosition, this.position);
			}
		}

		public virtual void Collision(CollisionContext context)
		{
		}

		private VertexArray debugVerts;

		protected bool lockedMovement;

		protected CollisionManager collisionManager;

		protected bool isSolid;

		protected AABB aabb;

		protected Mesh mesh;

		private Vector2f moveTemp;

		protected Vector2f lastPosition;
	}
}
