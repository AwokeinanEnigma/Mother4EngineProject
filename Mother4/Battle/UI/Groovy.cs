using System;
using Carbine.Graphics;
using Mother4.Data;
using SFML.System;

namespace Mother4.Battle.UI
{
	internal class Groovy : IDisposable
	{
		public Groovy(RenderPipeline pipeline, Vector2f position)
		{
			this.pipeline = pipeline;
			this.groovy = new IndexedColorGraphic(Paths.GRAPHICS + "groovy.dat", "groovy", position, 32767);
			this.groovy.OnAnimationComplete += this.AnimationComplete;
			this.pipeline.Add(this.groovy);
		}

		~Groovy()
		{
			this.Dispose(false);
		}

		private void AnimationComplete(Graphic graphic)
		{
			this.groovy.SpeedModifier = 0f;
			this.groovy.Frame = (float)(this.groovy.Frames - 1);
		}

		public void Update()
		{
			if (!this.done)
			{
				this.timer++;
				if (this.timer > 60)
				{
					this.pipeline.Remove(this.groovy);
					this.done = true;
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				if (!this.done)
				{
					this.pipeline.Remove(this.groovy);
				}
				this.groovy.Dispose();
			}
			this.disposed = true;
		}

		private const int TIME_TO_LIVE = 60;

		private bool disposed;

		private Graphic groovy;

		private RenderPipeline pipeline;

		private int timer;

		private bool done;
	}
}
