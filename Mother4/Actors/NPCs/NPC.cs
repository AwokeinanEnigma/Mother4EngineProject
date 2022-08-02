using System;
using System.Collections.Generic;
using Carbine;
using Carbine.Actors;
using Carbine.Collision;
using Carbine.Graphics;
using Carbine.Maps;
using Carbine.Utility;
using Mother4.Actors.Animation;
using Mother4.Actors.NPCs.Movement;
using Mother4.Data;
using Mother4.Overworld;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Actors.NPCs
{
	internal class NPC : SolidActor
	{
		public List<Map.NPCtext> Text
		{
			get
			{
				return this.text;
			}
		}

		public List<Map.NPCtext> TeleText
		{
			get
			{
				return this.teleText;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
				this.forceSpriteUpdate = true;
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
				return this.depth;
			}
		}

		public Vector2f EmoticonPoint
		{
			get
			{
				return new Vector2f(this.position.X, this.position.Y - this.npcGraphic.Origin.Y);
			}
		}

		public NPC(RenderPipeline pipeline, CollisionManager colman, Map.NPC npcData, object moverData) : base(null)
		{
			this.pipeline = pipeline;
			this.name = npcData.Name;
			this.direction = (int)npcData.Direction;
			this.text = npcData.Text;
			this.teleText = npcData.TeleText;
			this.shadow = npcData.Shadow;
			this.sticky = npcData.Sticky;
			NPC.MoveMode mode = (NPC.MoveMode)npcData.Mode;
			this.speed = npcData.Speed;
			this.delay = (int)npcData.Delay;
			this.distance = (int)npcData.Distance;
			this.startPosition.X = (float)npcData.X;
			this.startPosition.Y = (float)npcData.Y;
			this.SetMoveMode(mode, moverData);
			this.position = this.startPosition;
			this.depthOverride = (npcData.DepthOverride > int.MinValue);
			this.depth = (this.depthOverride ? npcData.DepthOverride : ((int)this.position.Y));
			this.pipeline = pipeline;
			if (npcData.Sprite != null && npcData.Sprite.Length > 0)
			{
				this.hasSprite = true;
				this.ChangeSprite(Paths.GRAPHICS + npcData.Sprite + ".dat", "stand south");
				if (this.shadow)
				{
					this.shadowGraphic = new Graphic(Paths.GRAPHICS + "shadow.png", this.position, new IntRect(0, 0, 15, 4), new Vector2f(8f, 1f), this.depth - 1);
					this.pipeline.Add(this.shadowGraphic);
				}
				int width = this.npcGraphic.TextureRect.Width;
				int height = this.npcGraphic.TextureRect.Height;
				this.mesh = new Mesh(new FloatRect((float)(-(float)(width / 2)), -3f, (float)width, 6f));
			}
			else
			{
				this.mesh = new Mesh(new FloatRect(0f, 0f, (float)npcData.Width, (float)npcData.Height));
			}
			this.aabb = this.mesh.AABB;
			this.isSolid = npcData.Solid;
			this.collisionManager = colman;
			this.collisionManager.Add(this);
			this.lastVelocity = VectorMath.ZERO_VECTOR;
			this.state = NPC.State.Idle;
			this.ChangeState(this.state);
		}

		public void SetMoveMode(NPC.MoveMode moveMode, object moverData)
		{
			NPC.MoveMode moveMode2 = moveMode;
			if (moverData is Map.Path)
			{
				moveMode2 = NPC.MoveMode.Path;
			}
			if (moverData is Map.Area)
			{
				moveMode2 = NPC.MoveMode.Area;
			}
			switch (moveMode2)
			{
			case NPC.MoveMode.RandomTurn:
				this.mover = new RandomTurnMover(60);
				return;
			case NPC.MoveMode.FacePlayer:
				this.mover = new FacePlayerMover();
				return;
			case NPC.MoveMode.Random:
				this.mover = new RandomMover(this.speed, (float)this.distance, this.delay);
				return;
			case NPC.MoveMode.Path:
			{
				Map.Path path = (Map.Path)moverData;
				bool loop = moveMode > NPC.MoveMode.None;
				this.mover = new PathMover(this.speed, this.delay, loop, path.Points);
				this.startPosition.X = (float)((int)path.Points[0].X);
				this.startPosition.Y = (float)((int)path.Points[0].Y);
				return;
			}
			case NPC.MoveMode.Area:
			{
				Map.Area area = (Map.Area)moverData;
				this.mover = new AreaMover(this.speed, this.delay, (float)this.distance, (float)area.Rectangle.Left, (float)area.Rectangle.Top, (float)area.Rectangle.Width, (float)area.Rectangle.Height);
				return;
			}
			default:
				this.mover = new NoneMover();
				return;
			}
		}

		public void SetMover(Mover mover)
		{
			this.mover = mover;
		}

		public void Telepathize()
		{
			this.effectGraphic = new IndexedColorGraphic(Paths.GRAPHICS + "telepathy.dat", "pulse", VectorMath.Truncate(this.position - new Vector2f(0f, this.npcGraphic.Origin.Y - this.npcGraphic.Size.Y / 4f)), 2147450881);
			this.pipeline.Add(this.effectGraphic);
		}

		public void Untelepathize()
		{
			this.pipeline.Remove(this.effectGraphic);
			this.effectGraphic.Dispose();
			this.effectGraphic = null;
		}

		public void ChangeSprite(string resource, string subsprite)
		{
			if (this.npcGraphic != null)
			{
				this.pipeline.Remove(this.npcGraphic);
			}
			this.npcGraphic = new IndexedColorGraphic(resource, subsprite, this.position, this.depth);
			this.pipeline.Add(this.npcGraphic);
			if (this.animator == null)
			{
				this.animator = new AnimationControl(this.npcGraphic, this.direction);
			}
			this.animator.ChangeGraphic(this.npcGraphic);
			this.animator.UpdateSubsprite(this.velocity, TerrainType.None, false, false);
			this.hasSprite = true;
		}

		public void OverrideSubsprite(string subsprite)
		{
			if (this.hasSprite)
			{
				this.animator.OverrideSubsprite(subsprite);
			}
		}

		public void ClearOverrideSubsprite()
		{
			if (this.hasSprite)
			{
				this.animator.ClearOverride();
			}
		}

		private void ChangeState(NPC.State newState)
		{
			if (newState != this.state)
			{
				this.lastState = this.state;
				this.state = newState;
			}
		}

		public void StartTalking()
		{
			if (!this.talkPause)
			{
				this.velocity.X = 0f;
				this.velocity.Y = 0f;
				this.stateMemory = this.state;
			}
			this.ChangeState(NPC.State.Talking);
			this.talkPause = false;
		}

		public void PauseTalking()
		{
			if (this.state == NPC.State.Talking)
			{
				this.ChangeState(NPC.State.Idle);
				this.talkPause = true;
			}
		}

		public void StopTalking()
		{
			this.ChangeState(this.stateMemory);
			this.lockedMovement = false;
			this.talkPause = false;
			this.animator.ClearOverride();
		}

		public void StartMoving()
		{
			this.ChangeState(NPC.State.Moving);
		}

		public void ForceDepth(int newDepth)
		{
			this.depthOverride = true;
			this.depth = newDepth;
			this.forceSpriteUpdate = true;
		}

		public void ResetDepth()
		{
			this.depthOverride = false;
		}

		public override void Update()
		{
			this.lastVelocity = this.velocity;
			if (this.state != NPC.State.Talking)
			{
				if (!this.MovementLocked)
				{
					this.changed = this.mover.GetNextMove(ref this.position, ref this.velocity, ref this.direction);
				}
				base.Update();
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
				if ((int)this.lastPosition.X != (int)this.position.X || (int)this.lastPosition.Y != (int)this.position.Y || (int)this.lastZOffset != (int)this.zOffset || this.forceSpriteUpdate)
				{
					if (this.state != NPC.State.Moving)
					{
						this.ChangeState(NPC.State.Moving);
						if (this.hasSprite)
						{
							this.npcGraphic.Frame = 1f;
						}
					}
					if (!this.depthOverride)
					{
						this.depth = (int)this.position.Y;
					}
					if (this.hasSprite)
					{
						this.graphicOffset.Y = -this.zOffset;
						this.npcGraphic.Position = VectorMath.Truncate(this.position + this.graphicOffset);
						this.npcGraphic.Depth = this.depth;
						this.pipeline.Update(this.npcGraphic);
						if (this.shadow)
						{
							this.shadowGraphic.Position = VectorMath.Truncate(this.position);
							this.shadowGraphic.Depth = this.depth - 1;
							this.pipeline.Update(this.shadowGraphic);
						}
					}
					this.forceSpriteUpdate = false;
				}
				else if (this.state != NPC.State.Idle)
				{
					this.ChangeState(NPC.State.Idle);
				}
			}
			if (this.hasSprite)
			{
				this.animator.UpdateSubsprite(this.velocity, this.direction, TerrainType.None, false, false, this.state == NPC.State.Talking);
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
						if (this.shadow)
						{
							this.shadowGraphic.Dispose();
						}
					}
					this.disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		private RenderPipeline pipeline;

		private IndexedColorGraphic npcGraphic;

		private Graphic shadowGraphic;

		private IndexedColorGraphic effectGraphic;

		private string name;

		private int direction;

		private bool talkPause;

		private NPC.State state;

		private NPC.State lastState;

		private NPC.State stateMemory;

		private List<Map.NPCtext> text;

		private List<Map.NPCtext> teleText;

		private Vector2f lastVelocity;

		private Vector2f startPosition;

		private Vector2f graphicOffset;

		private float lastZOffset;

		private float speed;

		private int delay;

		private int distance;

		private float hopFactor;

		private long hopFrame;

		private Mover mover;

		private bool changed;

		private bool shadow;

		private bool sticky;

		private int depth;

		private bool depthOverride;

		private bool hasSprite;

		private bool forceSpriteUpdate;

		private AnimationControl animator;

		public enum State
		{
			Idle,
			Talking,
			Moving
		}

		public enum MoveMode
		{
			None,
			RandomTurn,
			FacePlayer,
			Random,
			Path,
			Area
		}
	}
}
