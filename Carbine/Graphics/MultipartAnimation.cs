using System;
using Carbine.Utility;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Graphics
{
	public class MultipartAnimation : Renderable
	{
		public virtual Vector2f Scale { get; set; }

		public event MultipartAnimation.AnimationCompleteHandler OnAnimationComplete;

		public MultipartAnimation(string resource, Vector2f position, float speed, int depth)
		{
			this.textures = TextureManager.Instance.UseMultipart(resource);
			this.sprite = new Sprite(this.textures[0].Image);
			this.frames = this.textures.Length;
			this.speed = speed;
			this.Position = position;
			this.Origin = this.textures[0].GetSpriteDefinition("default").Origin;
			this.Depth = depth;
			this.Size = new Vector2f(this.textures[0].Image.Size.X, this.textures[0].Image.Size.Y);
			this.Scale = new Vector2f(1f, 1f);
			this.blend = Color.White;
			this.shader = new Shader(EmbeddedResources.GetStream("Carbine.Resources.pal.vert"), EmbeddedResources.GetStream("Carbine.Resources.pal.frag"));
			this.shader.SetParameter("image", this.textures[0].Image);
			this.shader.SetParameter("palette", this.textures[0].Palette);
			this.shader.SetParameter("palIndex", this.textures[0].CurrentPaletteFloat);
			this.shader.SetParameter("palSize", this.textures[0].PaletteSize);
			this.shader.SetParameter("blend", this.blend);
			this.shader.SetParameter("blendMode", 1f);
			this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.shader);
			this.Visible = true;
		}

		public void Reset()
		{
			this.frame = 0f;
			this.intFrame = (int)this.frame;
			this.lastFrame = this.intFrame;
			this.sprite.Texture = this.textures[this.intFrame].Image;
			this.renderStates.Texture = this.sprite.Texture;
			this.shader.SetParameter("image", this.sprite.Texture);
		}

		private void UpdateFrame(float newFrame)
		{
			this.frame = newFrame % (float)this.frames;
			this.lastFrame = this.intFrame;
			this.intFrame = (int)this.frame;
			if (this.OnAnimationComplete != null && newFrame >= (float)this.frames)
			{
				this.OnAnimationComplete(this);
			}
			if (this.visible && this.intFrame != this.lastFrame)
			{
				this.sprite.Texture = this.textures[this.intFrame].Image;
				this.renderStates.Texture = this.sprite.Texture;
				this.shader.SetParameter("image", this.sprite.Texture);
			}
		}

		public override void Draw(RenderTarget target)
		{
			if (this.visible)
			{
				float newFrame = this.frame + this.speed;
				this.UpdateFrame(newFrame);
				this.sprite.Position = this.Position;
				this.sprite.Origin = this.Origin;
				this.sprite.Scale = this.Scale;
				target.Draw(this.sprite, this.renderStates);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.sprite.Dispose();
				}
				for (int i = 0; i < this.textures.Length; i++)
				{
					TextureManager.Instance.Unuse(this.textures[i]);
				}
			}
			base.Dispose(disposing);
		}

		private const string SPRITE_NAME = "default";

		private Shader shader;

		private RenderStates renderStates;

		private IndexedTexture[] textures;

		private Sprite sprite;

		private Color blend;

		private float frame;

		private float speed;

		private int lastFrame;

		private int intFrame;

		private int frames;

		public delegate void AnimationCompleteHandler(MultipartAnimation anim);
	}
}
