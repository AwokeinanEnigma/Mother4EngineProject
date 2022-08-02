using System;
using SFML.Graphics;

namespace Carbine.Graphics
{
	public class FullColorTexture : ICarbineTexture, IDisposable
	{
		public Texture Image
		{
			get
			{
				return this.imageTex;
			}
		}

		public FullColorTexture(Image image)
		{
			this.imageTex = new Texture(image);
		}

		public FullColorTexture(Texture tex)
		{
			this.imageTex = new Texture(tex);
		}

		~FullColorTexture()
		{
			this.Dispose(false);
		}

		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.imageTex.Dispose();
			}
			this.disposed = true;
		}

		private Texture imageTex;

		private bool disposed;
	}
}
