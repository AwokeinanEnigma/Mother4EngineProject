using System;
using Carbine.Actors;
using Carbine.Collision;
using Carbine.Graphics;
using Carbine.Utility;
using Mother4.Actors.Animation;
using Mother4.Actors.NPCs.Movement;
using Mother4.Data;
using Mother4.Overworld;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Actors.NPCs
{
	internal class Enemy : SolidActor
	{
		public EnemyType Type
		{
			get
			{
				return this.enemyType;
			}
		}

		public Graphic Graphic
		{
			get
			{
				return this.npcGraphic;
			}
		}

		public Enemy(RenderPipeline pipeline, CollisionManager colman, EnemyType enemyType, Vector2f position, FloatRect spawnArea) : base(colman)
		{
			this.pipeline = pipeline;
			this.position = position;
			this.enemyType = enemyType;
			this.mover = new MushroomMover(this, 100f, 2f);
			this.npcGraphic = new IndexedColorGraphic(EnemyGraphics.GetFilename(enemyType), "walk south", this.Position, (int)this.Position.Y);
			this.pipeline.Add(this.npcGraphic);
			this.hasDirection = new bool[8];
			this.hasDirection[0] = (this.npcGraphic.GetSpriteDefinition("walk east") != null);
			this.hasDirection[1] = (this.npcGraphic.GetSpriteDefinition("walk northeast") != null);
			this.hasDirection[2] = (this.npcGraphic.GetSpriteDefinition("walk north") != null);
			this.hasDirection[3] = (this.npcGraphic.GetSpriteDefinition("walk northwest") != null);
			this.hasDirection[4] = (this.npcGraphic.GetSpriteDefinition("walk west") != null);
			this.hasDirection[5] = (this.npcGraphic.GetSpriteDefinition("walk southwest") != null);
			this.hasDirection[6] = (this.npcGraphic.GetSpriteDefinition("walk south") != null);
			this.hasDirection[7] = (this.npcGraphic.GetSpriteDefinition("walk southeast") != null);
			this.shadowGraphic = new Graphic(Paths.GRAPHICS + "shadow.png", this.Position, new IntRect(0, 0, 15, 4), new Vector2f(8f, 1f), (int)(this.Position.Y - 1f));
			this.pipeline.Add(this.shadowGraphic);
			int width = this.npcGraphic.TextureRect.Width;
			int height = this.npcGraphic.TextureRect.Height;
			this.mesh = new Mesh(new FloatRect((float)(-(float)(width / 2)), -3f, (float)width, 6f));
			this.aabb = this.mesh.AABB;
			this.animator = new AnimationControl(this.npcGraphic, this.direction);
			this.animator.UpdateSubsprite(this.velocity, this.direction, TerrainType.None, false, false);
		}

		public void OverrideSubsprite(string subsprite)
		{
			this.animator.OverrideSubsprite(subsprite);
		}

		public void ClearOverrideSubsprite()
		{
			this.animator.ClearOverride();
		}

		public void FreezeSpriteForever()
		{
			this.npcGraphic.SpeedModifier = 0f;
		}

		public override void Update()
		{
			this.lastVelocity = this.velocity;
			if (!this.lockedMovement)
			{
				this.changed = this.mover.GetNextMove(ref this.position, ref this.velocity, ref this.direction);
			}
			if (this.changed)
			{
				this.animator.UpdateSubsprite(this.velocity, this.direction, TerrainType.None, false, false);
				this.npcGraphic.Position = VectorMath.Truncate(this.position);
				this.npcGraphic.Depth = (int)this.position.Y;
				this.pipeline.Update(this.npcGraphic);
				this.shadowGraphic.Position = VectorMath.Truncate(this.position);
				this.shadowGraphic.Depth = (int)this.position.Y - 1;
				this.pipeline.Update(this.shadowGraphic);
				Vector2f v = new Vector2f(this.velocity.X, 0f);
				Vector2f v2 = new Vector2f(0f, this.velocity.Y);
				this.lastPosition = this.position;
				if (this.collisionManager.PlaceFree(this, this.position + v).PlaceFree)
				{
					this.position += v;
				}
				if (this.collisionManager.PlaceFree(this, this.position + v2).PlaceFree)
				{
					this.position += v2;
				}
				this.collisionManager.Update(this, this.lastPosition, this.position);
				this.changed = false;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				try
				{
					if (disposing)
					{
						this.pipeline.Remove(this.npcGraphic);
						this.pipeline.Remove(this.shadowGraphic);
						this.npcGraphic.Dispose();
						this.shadowGraphic.Dispose();
					}
					this.disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		private static readonly Vector2f HALO_OFFSET = new Vector2f(0f, -32f);

		private RenderPipeline pipeline;

		private IndexedColorGraphic npcGraphic;

		private IndexedColorGraphic haloGraphic;

		private Graphic shadowGraphic;

		private Mover mover;

		private bool[] hasDirection;

		private Vector2f lastVelocity;

		private int direction;

		private bool changed;

		private EnemyType enemyType;

		private AnimationControl animator;
	}
}
