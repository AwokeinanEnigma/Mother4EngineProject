using System;
using Carbine;
using Carbine.Actors;
using Carbine.Collision;
using Carbine.Graphics;
using Carbine.Input;
using Carbine.Utility;
using Mother4.Actors.Animation;
using Mother4.Actors.NPCs.Movement;
using Mother4.Data;
using Mother4.Overworld;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Actors
{
	internal class Player : SolidActor
	{
		public Vector2f CheckVector
		{
			get
			{
				return this.checkVector;
			}
		}

		public int Direction
		{
			get
			{
				return this.direction;
			}
		}

		public bool Running
		{
			get
			{
				return this.isRunning;
			}
		}

		public float HopFactor
		{
			get
			{
				return this.hopFactor;
			}
			set
			{
				this.hopFactor = value;
				this.hopFrame = Engine.Frame;
			}
		}

		public int Depth
		{
			get
			{
				return this.playerGraphic.Depth;
			}
		}

		public Vector2f EmoticonPoint
		{
			get
			{
				return new Vector2f(this.position.X, this.position.Y - this.playerGraphic.Origin.Y);
			}
		}

		public override bool MovementLocked
		{
			get
			{
				return this.lockedMovement;
			}
			set
			{
				this.lockedMovement = value;
				this.playerGraphic.SpeedModifier = (float)(this.lockedMovement ? 0 : 1);
				this.recorder.MovementLocked = this.lockedMovement;
			}
		}

		public event Player.OnCollisionHanlder OnCollision;

		public event Player.OnTelepathyAnimationCompleteHanlder OnTelepathyAnimationComplete;

		public Player(RenderPipeline pipeline, CollisionManager colman, PartyTrain recorder, Vector2f position, int direction, CharacterType character, bool useShadow, bool isOcean, bool isRunning) : base(colman)
		{
			this.position = position;
			this.moveVector = new Vector2f(0f, 0f);
			this.pipeline = pipeline;
			this.mesh = new Mesh(new FloatRect(-8f, -3f, 15f, 6f));
			this.aabb = base.Mesh.AABB;
			this.checkVector = new Vector2f(0f, 10f);
			this.direction = direction;
			this.character = character;
			if (isOcean)
			{
				this.terrainType = TerrainType.Ocean;
			}
			else
			{
				this.terrainType = TerrainType.None;
			}
			this.recorder = recorder;
			this.recorder.Reset(this.position, this.direction, this.terrainType);
			this.speed = 0f;
			this.isDead = (CharacterStats.GetStats(this.character).HP <= 0);
			this.isRunning = isRunning;
			this.runVector = VectorMath.DirectionToVector(this.direction);
			string file = CharacterGraphics.GetFile(character);
			this.ChangeSprite(file, "walk south");
			this.isShadowEnabled = (useShadow && !isOcean);
			this.shadowGraphic = new Graphic(Paths.GRAPHICS + "shadow.png", position, new IntRect(0, 0, 15, 4), new Vector2f(7f, 1f), (int)position.Y - 1);
			this.shadowGraphic.Visible = this.isShadowEnabled;
			pipeline.Add(this.shadowGraphic);
			InputManager.Instance.ButtonPressed += this.ButtonPressed;
			InputManager.Instance.ButtonReleased += this.ButtonReleased;
			TimerManager.Instance.OnTimerEnd += this.CrouchTimerEnd;
		}

		public void Telepathize()
		{
			Console.WriteLine("honk");
			this.effectGraphic = new IndexedColorGraphic(Paths.GRAPHICS + "telepathy.dat", "telepathy", VectorMath.Truncate(this.position - new Vector2f(0f, this.playerGraphic.Origin.Y - this.playerGraphic.Size.Y / 4f)), 2147450881);
			this.effectGraphic.OnAnimationComplete += this.effectGraphic_OnAnimationComplete;
			this.pipeline.Add(this.effectGraphic);
		}

		private void effectGraphic_OnAnimationComplete(Graphic graphic)
		{
			this.effectGraphic.OnAnimationComplete -= this.effectGraphic_OnAnimationComplete;
			this.pipeline.Remove(this.effectGraphic);
			this.effectGraphic.Dispose();
			this.effectGraphic = null;
			if (this.OnTelepathyAnimationComplete != null)
			{
				this.OnTelepathyAnimationComplete();
			}
		}

		private void ButtonPressed(InputManager sender, Button b)
		{
			if (!this.lockedMovement && b == Button.B)
			{
				this.isRunButtonPressed = true;
			}
		}

		private void ButtonReleased(InputManager sender, Button b)
		{
			if (!this.lockedMovement && b == Button.B)
			{
				this.isRunButtonReleased = true;
			}
		}

		private void CrouchTimerEnd(int timerIndex)
		{
			if (this.crouchTimerIndex == timerIndex)
			{
				this.isRunTimerComplete = true;
			}
		}

		public void SetPosition(Vector2f position)
		{
			this.SetPosition(position, false);
		}

		public void SetPosition(Vector2f position, bool extend)
		{
			this.position = position;
			this.lastPosition = position;
			this.recorder.Reset(this.position, this.direction, this.terrainType, extend);
			this.UpdateGraphics();
		}

		public void SetDirection(int dir)
		{
			while (dir < 0)
			{
				dir += 8;
			}
			this.direction = dir % 8;
			this.animator.UpdateSubsprite(VectorMath.DirectionToVector(this.direction), this.terrainType, this.isDead, this.isCrouch);
		}

		public void SetShadow(bool isVisible)
		{
			this.isShadowEnabled = isVisible;
			this.shadowGraphic.Visible = this.isShadowEnabled;
		}

		public void SetMover(Mover mover)
		{
			this.mover = mover;
		}

		public void ClearMover()
		{
			this.mover = null;
		}

		public void OverrideSubsprite(string subsprite)
		{
			this.animator.OverrideSubsprite(subsprite);
			this.animator.UpdateSubsprite(this.velocity, this.terrainType, this.isDead, this.isCrouch);
		}

		public void ClearOverrideSubsprite()
		{
			this.animator.ClearOverride();
		}

		public void ChangeSprite(string resource, string subsprite)
		{
			if (this.playerGraphic != null)
			{
				this.pipeline.Remove(this.playerGraphic);
			}
			this.playerGraphic = new IndexedColorGraphic(resource, subsprite, this.position, (int)this.position.Y);
			this.pipeline.Add(this.playerGraphic);
			if (this.animator == null)
			{
				this.animator = new AnimationControl(this.playerGraphic, this.direction);
			}
			this.animator.ChangeGraphic(this.playerGraphic);
			this.animator.UpdateSubsprite(this.velocity, TerrainType.None, false, false);
		}

		private void HandleDelegateFlags()
		{
			if (this.isRunButtonPressed)
			{
				if (!this.isRunning)
				{
					this.isCrouch = true;
					this.crouchTimerIndex = TimerManager.Instance.StartTimer(10);
					this.runVector = VectorMath.DirectionToVector(this.direction);
					this.moveVector = VectorMath.ZERO_VECTOR;
				}
				else
				{
					this.isRunning = false;
				}
				this.isRunButtonPressed = false;
			}
			if (this.isRunButtonReleased)
			{
				if (this.isRunReady)
				{
					this.isRunReady = false;
					this.isRunning = true;
				}
				else
				{
					TimerManager.Instance.Cancel(this.crouchTimerIndex);
				}
				this.animator.ClearOverride();
				this.isCrouch = false;
				this.isRunButtonReleased = false;
			}
			if (this.isRunTimerComplete)
			{
				this.isRunReady = true;
				this.isRunTimerComplete = false;
			}
		}

		public override void Input()
		{
			this.HandleDelegateFlags();
			if (this.mover == null)
			{
				if (!this.lockedMovement)
				{
					if (this.moveVector.X != 0f || this.moveVector.Y != 0f)
					{
						this.lastMoveVector = this.moveVector;
						this.lastSpeed = this.speed;
					}
					Vector2f v = VectorMath.Truncate(InputManager.Instance.Axis);
					this.speed = (this.isRunning ? 3f : 1f);
					this.recorder.Running = this.isRunning;
					this.recorder.Crouching = this.isCrouch;
					if (!this.isCrouch && !this.isRunning)
					{
						this.moveVector = v;
						if (v.X != 0f || v.Y != 0f)
						{
							this.direction = VectorMath.VectorToDirection(this.moveVector);
						}
					}
					else if (this.isCrouch)
					{
						if (v.X != 0f || v.Y != 0f)
						{
							this.runVector = v;
							this.direction = VectorMath.VectorToDirection(v);
							this.animator.UpdateSubsprite(this.velocity, this.direction, this.terrainType, this.isDead, this.isCrouch);
						}
					}
					else if (this.isRunning)
					{
						if (v.X != 0f || v.Y != 0f)
						{
							this.runVector = v;
						}
						this.moveVector = this.runVector;
						this.direction = VectorMath.VectorToDirection(this.moveVector);
					}
				}
				else
				{
					this.moveVector.X = 0f;
					this.moveVector.Y = 0f;
				}
				if (((this.lastMoveVector.X != this.moveVector.X || this.lastMoveVector.Y != this.moveVector.Y) && (this.moveVector.X != 0f || this.moveVector.Y != 0f)) || this.speed != this.lastSpeed)
				{
					this.checkVector = VectorMath.Truncate(VectorMath.Normalize(this.moveVector) * 11f);
				}
				this.velocity = this.moveVector;
				this.animator.UpdateSubsprite(this.velocity * this.speed, this.terrainType, this.isDead, this.isCrouch);
				this.isSolid = !InputManager.Instance.State[Button.L];
			}
		}

		protected override void HandleCollision(PlaceFreeContext pfc)
		{
			if (this.OnCollision != null)
			{
				this.OnCollision(this, pfc);
			}
			if (!(pfc.CollidingObject is Portal) && !(pfc.CollidingObject is TriggerArea))
			{
				this.isRunning = false;
			}
		}

		private void UpdateGraphics()
		{
			this.graphicOffset.Y = -this.zOffset;
			this.playerGraphic.Position = VectorMath.Truncate(this.position + this.graphicOffset);
			this.playerGraphic.Depth = (int)this.Position.Y;
			this.pipeline.Update(this.playerGraphic);
			if (this.isShadowEnabled)
			{
				this.shadowGraphic.Position = VectorMath.Truncate(this.position);
				this.shadowGraphic.Depth = this.playerGraphic.Depth - 1;
				this.pipeline.Update(this.shadowGraphic);
			}
		}

		public override void Update()
		{
			if (this.mover != null)
			{
				this.mover.GetNextMove(ref this.position, ref this.velocity, ref this.direction);
				this.speed = (float)((int)VectorMath.Magnitude(this.velocity));
				this.animator.UpdateSubsprite(this.velocity, this.terrainType, this.isDead, this.isCrouch);
				this.velocity.X = Math.Max(-1f, Math.Min(1f, this.velocity.X));
				this.velocity.Y = Math.Max(-1f, Math.Min(1f, this.velocity.Y));
				this.moveVector = this.velocity;
			}
			int num = 0;
			while ((float)num < this.speed)
			{
				base.Update();
				if ((int)this.lastPosition.X != (int)this.position.X || (int)this.lastPosition.Y != (int)this.position.Y)
				{
					this.recorder.Record(this.position, this.moveVector, this.terrainType);
				}
				num++;
			}
			if (this.hopFactor >= 1f)
			{
				this.lastZOffset = this.zOffset;
				this.zOffset = (float)Math.Sin((double)((float)(Engine.Frame - this.hopFrame) / (this.hopFactor * 0.3f))) * this.hopFactor;
				if (this.zOffset < 0f)
				{
					this.zOffset = 0f;
					this.hopFactor = 0f;
				}
			}
			if ((int)this.lastPosition.X != (int)this.position.X || (int)this.lastPosition.Y != (int)this.position.Y || (int)this.lastZOffset != (int)this.zOffset)
			{
				this.UpdateGraphics();
			}
		}

		public override void Collision(CollisionContext context)
		{
			if (!(context.Other is TriggerArea))
			{
				base.Collision(context);
				this.playerGraphic.Position = this.Position;
				this.playerGraphic.Depth = (int)this.Position.Y;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!this.disposed)
			{
				if (disposing)
				{
					this.pipeline.Remove(this.playerGraphic);
					this.pipeline.Remove(this.shadowGraphic);
					this.playerGraphic.Dispose();
					this.shadowGraphic.Dispose();
				}
				InputManager.Instance.ButtonPressed -= this.ButtonPressed;
				InputManager.Instance.ButtonReleased -= this.ButtonReleased;
				TimerManager.Instance.OnTimerEnd -= this.CrouchTimerEnd;
				this.disposed = true;
			}
		}

		private const float HOT_POINT_LENGTH = 11f;

		private const float SPEED_WALK = 1f;

		private const float SPEED_RUN = 3f;

		private const float SPEED_CYCLE = 4f;

		private const int RUN_TIMER_DURATION = 10;

		private Vector2f moveVector;

		private Vector2f runVector;

		private Vector2f lastMoveVector;

		private RenderPipeline pipeline;

		private PartyTrain recorder;

		private Graphic shadowGraphic;

		private IndexedColorGraphic playerGraphic;

		private int direction;

		private float speed;

		private float lastSpeed;

		private CharacterType character;

		private Mover mover;

		private bool isShadowEnabled;

		private bool isDead;

		private bool isCrouch;

		private bool isRunning;

		private bool isRunReady;

		private int crouchTimerIndex;

		private float hopFactor;

		private long hopFrame;

		private float lastZOffset;

		private Vector2f graphicOffset;

		private bool isRunButtonPressed;

		private bool isRunButtonReleased;

		private bool isRunTimerComplete;

		private IndexedColorGraphic effectGraphic;

		private TerrainType terrainType;

		private AnimationControl animator;

		private Vector2f checkVector;

		public delegate void OnCollisionHanlder(Player sender, PlaceFreeContext data);

		public delegate void OnTelepathyAnimationCompleteHanlder();
	}
}
