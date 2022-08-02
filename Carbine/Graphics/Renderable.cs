using System;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Graphics
{
	public abstract class Renderable : IDisposable
	{
		~Renderable()
		{
			this.Dispose(false);
		}

		public virtual Vector2f Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public virtual Vector2f Origin
		{
			get
			{
				return this.origin;
			}
			set
			{
				this.origin = value;
			}
		}

		public virtual Vector2f Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public virtual int Depth
		{
			get
			{
				return this.depth;
			}
			set
			{
				this.depth = value;
			}
		}

		public virtual bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
			}
		}

		public abstract void Draw(RenderTarget target);

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected Vector2f position;

		protected Vector2f origin;

		protected Vector2f size;

		protected int depth;

		protected bool visible = true;

		protected bool disposed;
	}
}
