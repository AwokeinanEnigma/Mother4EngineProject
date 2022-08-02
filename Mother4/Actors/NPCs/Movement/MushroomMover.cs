using System;
using Carbine.Graphics;
using Carbine.Utility;
using SFML.System;

namespace Mother4.Actors.NPCs.Movement
{
	internal class MushroomMover : Mover
	{
		public MushroomMover(Enemy enemy, float chaseThreshold, float speed)
		{
			this.enemy = enemy;
			this.chaseThreshold = chaseThreshold;
			this.speed = speed;
		}

		public override bool GetNextMove(ref Vector2f position, ref Vector2f velocity, ref int direction)
		{
			this.changed = false;
			if (this.mode == MushroomMover.Mode.Wait)
			{
				if (ViewManager.Instance.FollowActor != null)
				{
					float num = VectorMath.Magnitude(ViewManager.Instance.FollowActor.Position - position);
					this.mode = ((num < this.chaseThreshold) ? MushroomMover.Mode.Pop : MushroomMover.Mode.Wait);
				}
				velocity = VectorMath.ZERO_VECTOR;
			}
			else if (this.mode == MushroomMover.Mode.Pop)
			{
				this.enemy.OverrideSubsprite("pop");
				this.enemy.Graphic.OnAnimationComplete += this.OnAnimationComplete;
				this.mode = MushroomMover.Mode.PopWait;
				this.changed = true;
			}
			else if (this.mode == MushroomMover.Mode.Chase && ViewManager.Instance.FollowActor != null)
			{
				direction = VectorMath.VectorToDirection(ViewManager.Instance.FollowActor.Position - position);
				velocity = VectorMath.DirectionToVector(direction) * this.speed;
				this.changed = true;
			}
			return this.changed;
		}

		private void OnAnimationComplete(Graphic graphic)
		{
			this.mode = MushroomMover.Mode.Chase;
			this.enemy.ClearOverrideSubsprite();
			this.enemy.Graphic.OnAnimationComplete -= this.OnAnimationComplete;
		}

		private MushroomMover.Mode mode;

		private bool changed;

		private Enemy enemy;

		private float chaseThreshold;

		private float speed;

		private enum Mode
		{
			Wait,
			Pop,
			PopWait,
			Chase
		}
	}
}
