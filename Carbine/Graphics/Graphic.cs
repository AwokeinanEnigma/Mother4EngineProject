using System;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Graphics
{
	public class Graphic : Renderable
	{
		public event Graphic.AnimationCompleteHandler OnAnimationComplete;

		public virtual float Rotation { get; set; }

		public virtual Color Color
		{
			get
			{
				return this.sprite.Color;
			}
			set
			{
				this.sprite.Color = value;
			}
		}

		public virtual Vector2f Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		public IntRect TextureRect
		{
			get
			{
				return this.sprite.TextureRect;
			}
			set
			{
				this.sprite.TextureRect = value;
				this.Size = new Vector2f((float)value.Width, (float)value.Height);
				this.startTextureRect = value;
				this.frame = 0f;
			}
		}

		public ICarbineTexture Texture
		{
			get
			{
				return this.texture;
			}
		}

		public int Frames { get; protected set; }

		public float Frame
		{
			get
			{
				return this.frame;
			}
			set
			{
				this.frame = Math.Max(0f, Math.Min((float)this.Frames, value));
			}
		}

		public float[] SpeedSet
		{
			get
			{
				return this.speeds;
			}
			set
			{
				this.speeds = value;
			}
		}

		public float SpeedModifier
		{
			get
			{
				return this.speedModifier;
			}
			set
			{
				this.speedModifier = value;
			}
		}

		public Graphic(string resource, Vector2f position, IntRect textureRect, Vector2f origin, int depth)
		{
			this.texture = TextureManager.Instance.UseUnprocessed(resource);
			this.sprite = new Sprite(this.texture.Image);
			this.sprite.TextureRect = textureRect;
			this.startTextureRect = textureRect;
			this.Position = position;
			this.Origin = origin;
			this.Size = new Vector2f((float)textureRect.Width, (float)textureRect.Height);
			this.Depth = depth;
			this.Rotation = 0f;
			this.scale = new Vector2f(1f, 1f);
			this.finalScale = this.scale;
			this.speedModifier = 1f;
			this.sprite.Position = this.Position;
			this.sprite.Origin = this.Origin;
			this.speeds = new float[]
			{
				1f
			};
			this.speedIndex = 0f;
			this.Visible = true;
		}

		protected Graphic()
		{
		}

		protected void UpdateAnimation()
		{
			int num = this.startTextureRect.Left + (int)this.frame * (int)this.size.X;
			int left = num % (int)this.sprite.Texture.Size.X;
			int top = this.startTextureRect.Top + num / (int)this.sprite.Texture.Size.X * (int)this.size.Y;
			this.sprite.TextureRect = new IntRect(left, top, (int)this.Size.X, (int)this.Size.Y);
			if (this.OnAnimationComplete != null && this.frame + this.GetFrameSpeed() >= (float)this.Frames)
			{
				this.OnAnimationComplete(this);
			}
			this.speedIndex = (this.speedIndex + this.GetFrameSpeed()) % (float)this.speeds.Length;
			this.IncrementFrame();
		}

		protected virtual void IncrementFrame()
		{
			this.frame = (this.frame + this.GetFrameSpeed()) % (float)this.Frames;
		}

		protected float GetFrameSpeed()
		{
			return this.speeds[(int)this.speedIndex % this.speeds.Length] * this.speedModifier;
		}

		public void Translate(Vector2f v)
		{
			this.Translate(v.X, v.Y);
		}

		public virtual void Translate(float x, float y)
		{
			this.position.X = this.position.X + x;
			this.position.Y = this.position.Y + y;
		}

		public override void Draw(RenderTarget target)
		{
			if (this.visible)
			{
				if (this.Frames > 0)
				{
					this.UpdateAnimation();
				}
				this.sprite.Position = this.Position;
				this.sprite.Origin = this.Origin;
				this.sprite.Rotation = this.Rotation;
				this.finalScale = this.scale;
				this.sprite.Scale = this.finalScale;
				target.Draw(this.sprite);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.sprite != null)
				{
					this.sprite.Dispose();
				}
				TextureManager.Instance.Unuse(this.texture);
			}
			this.disposed = true;
		}

		protected Sprite sprite;

		protected ICarbineTexture texture;

		protected IntRect startTextureRect;

		protected float frame;

		protected float speedIndex;

		protected float[] speeds;

		protected Vector2f scale;

		protected Vector2f finalScale;

		protected float speedModifier;

		public delegate void AnimationCompleteHandler(Graphic graphic);
	}
}
