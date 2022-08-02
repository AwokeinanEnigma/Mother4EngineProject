using System;
using Carbine.Graphics;
using Mother4.Data;
using SFML.System;

namespace Mother4.Battle.UI
{
	internal class BattleSmash
	{
		public bool Done
		{
			get
			{
				return this.done;
			}
		}

		public BattleSmash(RenderPipeline pipeline, Vector2f position)
		{
			this.pipeline = pipeline;
			this.smashGraphic = new IndexedColorGraphic(Paths.GRAPHICS + "smash.dat", "smash", position, 32767);
			this.smashGraphic.OnAnimationComplete += this.SmashgraphicAnimationComplete;
			this.pipeline.Add(this.smashGraphic);
		}

		private void SmashgraphicAnimationComplete(Graphic graphic)
		{
			this.pipeline.Remove(this.smashGraphic);
			this.done = true;
		}

		private RenderPipeline pipeline;

		private Graphic smashGraphic;

		private bool done;
	}
}
