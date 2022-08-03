using System;
using Carbine.Graphics;
using Carbine.GUI;
using Mother4.Data;
using Mother4.GUI;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Battle.UI
{
	internal class BattleCard : IDisposable
	{
		public Vector2f Position
		{
			get
			{
				return this.position;
			}
		}

		public Graphic CardGraphic
		{
			get
			{
				return this.card;
			}
		}

		public BattleCard(RenderPipeline pipeline, Vector2f position, int depth, string name, int hp, int maxHp, int pp, int maxPp, float meterFill)
		{
			this.position = position;
			this.card = new IndexedColorGraphic(BattleCard.BATTLEUI_DAT, "card", position, depth);
			this.card.CurrentPalette = Settings.WindowFlavor;
			this.hpLabel = new IndexedColorGraphic(BattleCard.BATTLEUI_DAT, "hp", position + BattleCard.HPLABEL_POSITION, depth + 2);
			this.hpLabel.CurrentPalette = Settings.WindowFlavor;
			this.ppLabel = new IndexedColorGraphic(BattleCard.BATTLEUI_DAT, "pp", position + BattleCard.PPLABEL_POSITION, depth + 2);
			this.ppLabel.CurrentPalette = Settings.WindowFlavor;
			this.nameTag = new TextRegion(position, depth + 2, Fonts.Main, name);
			this.nameTag.Color = Color.Black;
			this.nametagX = (int)((float)(this.card.TextureRect.Width / 2) - this.nameTag.Size.X / 2f);
			this.nameTag.Position = position + new Vector2f((float)this.nametagX, 6f) + BattleCard.NAME_POSITION;
			pipeline.Add(this.card);
			pipeline.Add(this.hpLabel);
			pipeline.Add(this.ppLabel);
			pipeline.Add(this.nameTag);
			this.meter = new BattleMeter(pipeline, position + BattleCard.METER_OFFSET, meterFill, depth + 1);
			this.odoHP = new Odometer(pipeline, position + BattleCard.HPODO_POSITION, depth + 2, 3, hp, maxHp);
			this.odoPP = new Odometer(pipeline, position + BattleCard.PPODO_POSITION, depth + 2, 3, pp, maxPp);
			this.springMode = BattleCard.SpringMode.Normal;
		}

		~BattleCard()
		{
			this.Dispose(false);
		}

		public void SetHP(int newHP)
		{
						this.odoHP.SetValue(newHP);
		}

		public void SetPP(int newPP)
		{
			this.odoPP.SetValue(newPP);
		}

		public void SetMeter(float newFill)
		{
			this.meter.SetFill(newFill);
		}

		public void SetGroovy(bool groovy)
		{
			this.meter.SetGroovy(groovy);
		}

		public void SetTargetY(float newTargetY)
		{
			this.SetTargetY(newTargetY, false);
		}

		public void SetTargetY(float newTargetY, bool instant)
		{
			this.targetY = newTargetY;
			if (instant)
			{
				this.position.Y = this.targetY;
			}
		}

		public void SetSpring(BattleCard.SpringMode mode, Vector2f amplitude, Vector2f speed, Vector2f decay)
		{
			this.springMode = mode;
			this.xSpring = 0f;
			this.xDampTarget = amplitude.X;
			this.xSpeedTarget = speed.X;
			this.xDecayTarget = decay.X;
			this.ySpring = 0f;
			this.yDampTarget = amplitude.Y;
			this.ySpeedTarget = speed.Y;
			this.yDecayTarget = decay.Y;
			this.ramping = true;
		}

		public void AddSpring(Vector2f amplitude, Vector2f speed, Vector2f decay)
		{
			this.xDampTarget += amplitude.X;
			this.xSpeedTarget += speed.X;
			this.xDecayTarget += decay.X;
			this.yDampTarget += amplitude.Y;
			this.ySpeedTarget += speed.Y;
			this.yDecayTarget += decay.Y;
			this.ramping = true;
		}

		private void UpdateSpring()
		{
			if (this.ramping)
			{
				this.xDamp += (this.xDampTarget - this.xDamp) / 2f;
				this.xSpeed += (this.xSpeedTarget - this.xSpeed) / 2f;
				this.xDecay += (this.xDecayTarget - this.xDecay) / 2f;
				this.yDamp += (this.yDampTarget - this.yDamp) / 2f;
				this.ySpeed += (this.ySpeedTarget - this.ySpeed) / 2f;
				this.yDecay += (this.yDecayTarget - this.yDecay) / 2f;
				if ((int)this.xDamp == (int)this.xDampTarget && (int)this.xSpeed == (int)this.xSpeedTarget && (int)this.xDecay == (int)this.xDecayTarget && (int)this.yDamp == (int)this.yDampTarget && (int)this.ySpeed == (int)this.ySpeedTarget && (int)this.yDecay == (int)this.yDecayTarget)
				{
					this.ramping = false;
				}
			}
			else
			{
				this.xDamp = ((this.xDamp > 0.5f) ? (this.xDamp * this.xDecay) : 0f);
				this.yDamp = ((this.yDamp > 0.5f) ? (this.yDamp * this.yDecay) : 0f);
			}
			this.xSpring += this.xSpeed;
			this.ySpring += this.ySpeed;
			this.offset.X = (float)Math.Sin((double)this.xSpring) * this.xDamp;
			this.offset.Y = (float)Math.Sin((double)this.ySpring) * this.yDamp;
			if (this.springMode == BattleCard.SpringMode.BounceUp)
			{
				this.offset.Y = -Math.Abs(this.offset.Y);
				return;
			}
			if (this.springMode == BattleCard.SpringMode.BounceDown)
			{
				this.offset.Y = Math.Abs(this.offset.Y);
			}
		}

		private void UpdatePosition()
		{
			if (this.position.Y < this.targetY - 0.5f)
			{
				this.position.Y = this.position.Y + (this.targetY - this.position.Y) / 2f;
				return;
			}
			if (this.position.Y > this.targetY + 0.5f)
			{
				this.position.Y = this.position.Y + (this.targetY - this.position.Y) / 2f;
				return;
			}
			if ((int)this.position.Y != (int)this.targetY)
			{
				this.position.Y = this.targetY;
			}
		}

		private void MoveGraphics(Vector2f gPosition)
		{
			gPosition.X = (float)((int)gPosition.X);
			gPosition.Y = (float)((int)gPosition.Y);
			this.card.Position = gPosition;
			this.hpLabel.Position = gPosition + BattleCard.HPLABEL_POSITION;
			this.ppLabel.Position = gPosition + BattleCard.PPLABEL_POSITION;
			this.nameTag.Position = gPosition + new Vector2f((float)this.nametagX, 6f) + BattleCard.NAME_POSITION;
			this.meter.Position = gPosition + BattleCard.METER_OFFSET;
			this.odoHP.Position = gPosition + BattleCard.HPODO_POSITION;
			this.odoPP.Position = gPosition + BattleCard.PPODO_POSITION;
		}

		public void Update()
		{
			this.UpdateSpring();
			this.UpdatePosition();
			this.MoveGraphics(this.position + this.offset);
			this.odoHP.Update();
			this.odoPP.Update();
			this.meter.Update();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.card.Dispose();
					this.hpLabel.Dispose();
					this.ppLabel.Dispose();
					this.odoHP.Dispose();
					this.odoPP.Dispose();
					this.meter.Dispose();
					this.nameTag.Dispose();
				}
				this.disposed = true;
			}
		}

		private const float DAMP_HIGHPASS = 0.5f;

		private static readonly string BATTLEUI_DAT = Paths.GRAPHICS + "battleui.dat";

		private static readonly Vector2f HPLABEL_POSITION = new Vector2f(10f, 23f);

		private static readonly Vector2f PPLABEL_POSITION = new Vector2f(10f, 34f);

		private static readonly Vector2f HPODO_POSITION = new Vector2f(28f, 22f);

		private static readonly Vector2f PPODO_POSITION = new Vector2f(28f, 33f);

		private static readonly Vector2f METER_OFFSET = new Vector2f(1f, 1f);

		private static readonly Vector2f NAME_POSITION = new Vector2f(0f, 2f);

		private bool disposed;

		private IndexedColorGraphic card;

		private IndexedColorGraphic hpLabel;

		private IndexedColorGraphic ppLabel;

		private TextRegion nameTag;

		private int nametagX;

		private BattleMeter meter;

		private Odometer odoHP;

		private Odometer odoPP;

		private BattleCard.SpringMode springMode;

		private Vector2f position;

		private Vector2f offset;

		private float xSpring;

		private float ySpring;

		private float xSpeed;

		private float xSpeedTarget;

		private float ySpeed;

		private float ySpeedTarget;

		private float xDamp;

		private float xDampTarget;

		private float yDamp;

		private float yDampTarget;

		private float xDecay;

		private float xDecayTarget;

		private float yDecay;

		private float yDecayTarget;

		private bool ramping;

		private float targetY;

		public enum SpringMode
		{
			Normal,
			BounceUp,
			BounceDown
		}
	}
}
