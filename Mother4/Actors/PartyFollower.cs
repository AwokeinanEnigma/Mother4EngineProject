using System;
using Carbine.Graphics;
using Carbine.Utility;
using Mother4.Actors.Animation;
using Mother4.Data;
using Mother4.Overworld;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Actors
{
	internal class PartyFollower : IDisposable
	{
		public int Place
		{
			get
			{
				return this.place;
			}
			set
			{
				this.place = value;
			}
		}

		public float Width
		{
			get
			{
				return this.followerGraphic.Size.X;
			}
		}

		public CharacterType Character
		{
			get
			{
				return this.character;
			}
		}

		public int Direction
		{
			get
			{
				return this.direction;
			}
		}

		public Vector2f Position
		{
			get
			{
				return this.position;
			}
		}

		public PartyFollower(RenderPipeline pipeline, PartyTrain recorder, CharacterType character, Vector2f position, int direction, bool useShadow)
		{
			this.pipeline = pipeline;
			this.recorder = recorder;
			this.character = character;
			this.place = 0;
			this.isDead = (CharacterStats.GetStats(this.character).HP <= 0);
			this.useShadow = (useShadow && !this.isDead);
			this.position = position;
			this.velocity = VectorMath.ZERO_VECTOR;
			this.direction = direction;
			string file = CharacterGraphics.GetFile(character);
			this.followerGraphic = new IndexedColorGraphic(file, "walk south", this.position, (int)this.position.Y - 1);
			this.followerGraphic.SpeedModifier = 0f;
			this.followerGraphic.Frame = 0f;
			this.pipeline.Add(this.followerGraphic);
			if (this.useShadow)
			{
				this.shadowGraphic = new Graphic(Paths.GRAPHICS + "shadow.png", this.Position, new IntRect(0, 0, 15, 4), new Vector2f(8f, 1f), (int)this.position.Y - 2);
				this.pipeline.Add(this.shadowGraphic);
			}
			Console.WriteLine("Is {0} dead? {1}", Enum.GetName(typeof(CharacterType), this.character), this.isDead);
			this.animator = new AnimationControl(this.followerGraphic, direction);
			this.animator.UpdateSubsprite(VectorMath.ZERO_VECTOR, TerrainType.None, this.isDead, this.isCrouch);
		}

		~PartyFollower()
		{
			this.Dispose(false);
		}

		private void RecorderReset(Vector2f position, int direction)
		{
			this.position = position;
			this.direction = direction;
		}

		public void Update(Vector2f newPosition, Vector2f newVelocity, TerrainType newTerrain)
		{
			this.lastPosition = this.position;
			this.position = newPosition;
			this.velocity = newVelocity;
			this.terrain = newTerrain;
			this.lastRunning = this.isRunning;
			this.isRunning = this.recorder.Running;
			this.isCrouch = this.recorder.Crouching;
			this.direction = VectorMath.VectorToDirection(this.velocity);
			this.lastMoving = this.moving;
			if ((int)this.lastPosition.X != (int)this.position.X || (int)this.lastPosition.Y != (int)this.position.Y)
			{
				this.followerGraphic.Position = new Vector2f((float)((int)this.position.X), (float)((int)this.position.Y));
				this.followerGraphic.Depth = (int)this.Position.Y;
				this.pipeline.Update(this.followerGraphic);
				if (this.useShadow)
				{
					this.shadowGraphic.Position = this.followerGraphic.Position;
					this.shadowGraphic.Depth = (int)this.Position.Y - 1;
					this.pipeline.Update(this.shadowGraphic);
				}
				this.moving = true;
			}
			else
			{
				this.moving = false;
			}
			this.animator.UpdateSubsprite(this.velocity * (this.isRunning ? 2f : 1f) * (this.moving ? 1f : 0f), this.terrain, this.isDead, this.isCrouch);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.pipeline.Remove(this.followerGraphic);
					this.pipeline.Remove(this.shadowGraphic);
					this.followerGraphic.Dispose();
					this.shadowGraphic.Dispose();
				}
				this.disposed = true;
			}
		}

		private bool disposed;

		private RenderPipeline pipeline;

		private PartyTrain recorder;

		private CharacterType character;

		private int place;

		private int direction;

		private Vector2f velocity;

		private Vector2f position;

		private Vector2f lastPosition;

		private TerrainType terrain;

		private IndexedColorGraphic followerGraphic;

		private Graphic shadowGraphic;

		private bool isRunning;

		private bool lastRunning;

		private bool moving;

		private bool lastMoving;

		private bool useShadow;

		private bool isDead;

		private bool isCrouch;

		private AnimationControl animator;
	}
}
