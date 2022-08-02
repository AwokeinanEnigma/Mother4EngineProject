using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Carbine.Graphics
{
	public class RenderPipeline
	{
		public RenderTarget Target
		{
			get
			{
				return this.target;
			}
		}

		public RenderPipeline(RenderTarget target)
		{
			this.target = target;
			this.renderables = new List<Renderable>();
			this.renderablesToAdd = new Stack<Renderable>();
			this.renderablesToRemove = new Stack<Renderable>();
			this.uids = new Dictionary<Renderable, int>();
			this.depthCompare = new RenderPipeline.RenderableComparer(this);
			this.viewRect = default(FloatRect);
			this.rendRect = default(FloatRect);
		}

		public void Add(Renderable renderable)
		{
			if (!this.renderables.Contains(renderable))
			{
				this.renderablesToAdd.Push(renderable);
				return;
			}
			Console.WriteLine("Tried to add renderable that already exists in the RenderPipeline.");
		}

		public void AddAll<T>(IList<T> addRenderables) where T : Renderable
		{
			int count = addRenderables.Count;
			for (int i = 0; i < count; i++)
			{
				this.Add(addRenderables[i]);
			}
		}

		public void Remove(Renderable renderable)
		{
			if (renderable != null)
			{
				this.renderablesToRemove.Push(renderable);
			}
		}

		public void Update(Renderable renderable)
		{
			this.needToSort = true;
		}

		private void DoAdditions()
		{
			while (this.renderablesToAdd.Count > 0)
			{
				Renderable renderable = this.renderablesToAdd.Pop();
				this.renderables.Add(renderable);
				this.uids.Add(renderable, this.rendCount);
				this.needToSort = true;
				this.rendCount++;
			}
		}

		private void DoRemovals()
		{
			while (this.renderablesToRemove.Count > 0)
			{
				Renderable renderable = this.renderablesToRemove.Pop();
				this.renderables.Remove(renderable);
				this.uids.Remove(renderable);
			}
		}

		public void Each(Action<Renderable> forEachFunc)
		{
			int count = this.renderables.Count;
			for (int i = 0; i < count; i++)
			{
				forEachFunc(this.renderables[i]);
			}
		}

		public void Draw()
		{
			this.DoAdditions();
			this.DoRemovals();
			if (this.needToSort)
			{
				this.renderables.Sort(this.depthCompare);
				this.needToSort = false;
			}
			View view = this.target.GetView();
			this.viewRect.Left = view.Center.X - view.Size.X / 2f;
			this.viewRect.Top = view.Center.Y - view.Size.Y / 2f;
			this.viewRect.Width = view.Size.X;
			this.viewRect.Height = view.Size.Y;
			int count = this.renderables.Count;
			for (int i = 0; i < count; i++)
			{
				Renderable renderable = this.renderables[i];
				if (renderable.Visible)
				{
					this.rendRect.Left = renderable.Position.X - renderable.Origin.X;
					this.rendRect.Top = renderable.Position.Y - renderable.Origin.Y;
					this.rendRect.Width = renderable.Size.X;
					this.rendRect.Height = renderable.Size.Y;
					if (this.rendRect.Intersects(this.viewRect))
					{
						renderable.Draw(this.target);
					}
				}
			}
		}

		private RenderTarget target;

		private List<Renderable> renderables;

		private Stack<Renderable> renderablesToAdd;

		private Stack<Renderable> renderablesToRemove;

		private bool needToSort;

		private RenderPipeline.RenderableComparer depthCompare;

		private Dictionary<Renderable, int> uids;

		private int rendCount;

		private FloatRect viewRect;

		private FloatRect rendRect;

		private class RenderableComparer : IComparer<Renderable>
		{
			public RenderableComparer(RenderPipeline pipeline)
			{
				this.pipeline = pipeline;
			}

			public int Compare(Renderable x, Renderable y)
			{
				if (x.Depth != y.Depth)
				{
					return x.Depth - y.Depth;
				}
				return this.pipeline.uids[y] - this.pipeline.uids[x];
			}

			private RenderPipeline pipeline;
		}
	}
}
