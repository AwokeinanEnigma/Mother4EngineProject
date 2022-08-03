using System;
using Carbine;
using Carbine.Flags;
using Carbine.Graphics;
using Carbine.GUI;
using Carbine.Input;
using Carbine.Utility;
using Mother4.Data;
using Mother4.GUI.Text;
using SFML.Graphics;
using SFML.System;

namespace Mother4.GUI
{
	// Token: 0x020000F4 RID: 244
	internal class OverworldTextBox : TextBox
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x00021A4C File Offset: 0x0001FC4C
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x00021A59 File Offset: 0x0001FC59
		public string Nametag
		{
			get
			{
				return this.nametag.Name;
			}
			set
			{
				this.SetNametag(value);
			}
		}


        public OverworldTextBox()
            : base(OverworldTextBox.BOX_POSITION, OverworldTextBox.BOX_SIZE, true)
        {
            this.size = OverworldTextBox.BOX_SIZE;
            this.canTransitionIn = true;
            this.canTransitionOut = true;
            this.state = OverworldTextBox.AnimationState.Hidden;
            Vector2f finalCenter = ViewManager.Instance.FinalCenter;
            Vector2f vector2f = finalCenter - ViewManager.Instance.View.Size / 2f;
            this.dimmer = (Shape)new RectangleShape(Engine.SCREEN_SIZE);
            this.dimmer.Origin = Engine.HALF_SCREEN_SIZE;
            this.dimmer.Position = finalCenter;
            this.nametag = new Mother4.GUI.Nametag(string.Empty, VectorMath.ZERO_VECTOR, 0);
            this.nametag.Visible = false;
            this.nametagVisible = false;
            InputManager.Instance.ButtonPressed += new InputManager.ButtonPressedHandler(this.ButtonPressed);
        }


		// Token: 0x060005A2 RID: 1442 RVA: 0x00021B38 File Offset: 0x0001FD38
		private void ButtonPressed(InputManager sender, Button b)
		{
			if (this.isWaitingOnPlayer && b == Button.A)
			{
				base.ContinueFromWait();
			}
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x00021B4C File Offset: 0x0001FD4C
		protected override void Recenter()
		{
			Vector2f finalCenter = ViewManager.Instance.FinalCenter;
			Vector2f vector2f = finalCenter - ViewManager.Instance.View.Size / 2f;
			this.position = new Vector2f(vector2f.X + OverworldTextBox.BOX_POSITION.X, vector2f.Y + OverworldTextBox.BOX_POSITION.Y);
			this.window.Position = new Vector2f(this.position.X, this.position.Y + this.textboxY);
			this.advanceArrow.Position = new Vector2f(vector2f.X + OverworldTextBox.BUTTON_POSITION.X, vector2f.Y + OverworldTextBox.BUTTON_POSITION.Y);
			this.nametag.Position = new Vector2f(vector2f.X + OverworldTextBox.NAMETAG_POSITION.X, vector2f.Y + OverworldTextBox.NAMETAG_POSITION.Y + this.textboxY);
			this.typewriter.Position = new Vector2f(vector2f.X + OverworldTextBox.TEXT_POSITION.X, vector2f.Y + OverworldTextBox.TEXT_POSITION.Y + this.textboxY);
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x00021C8A File Offset: 0x0001FE8A
		private void SetNametag(string namestring)
		{
			if (namestring != null && namestring.Length > 0)
			{
				this.nametag.Name = TextProcessor.ProcessReplacements(namestring);
				this.nametagVisible = true;
			}
			else
			{
				this.nametagVisible = false;
			}
			this.nametag.Visible = this.nametagVisible;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x00021CCA File Offset: 0x0001FECA
		public override void Reset()
		{
			base.Reset();
			this.SetNametag(null);
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00021CDC File Offset: 0x0001FEDC
		private void UpdateTextboxAnimation(float amount)
		{
			this.textboxY = 4f * (1f - Math.Max(0f, Math.Min(1f, amount)));
			this.typewriter.Position = new Vector2f((float)((int)(ViewManager.Instance.Viewrect.Left + OverworldTextBox.TEXT_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + OverworldTextBox.TEXT_POSITION.Y + this.textboxY)));
			this.window.Position = new Vector2f((float)((int)(ViewManager.Instance.Viewrect.Left + OverworldTextBox.BOX_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + OverworldTextBox.BOX_POSITION.Y + this.textboxY)));
			this.nametag.Position = new Vector2f((float)((int)(ViewManager.Instance.Viewrect.Left + OverworldTextBox.NAMETAG_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + OverworldTextBox.NAMETAG_POSITION.Y + this.textboxY)));
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00021E00 File Offset: 0x00020000
		public override void Show()
		{
			if (!this.visible)
			{
				this.window.FrameStyle = (FlagManager.Instance[4] ? WindowBox.Style.Telepathy : Settings.WindowStyle);
				this.visible = true;
				this.Recenter();
				this.window.Visible = true;
				this.typewriter.Visible = true;
				this.nametag.Visible = this.nametagVisible;
				this.state = OverworldTextBox.AnimationState.SlideIn;
				this.slideProgress = (this.canTransitionIn ? 0f : 1f);
				this.UpdateTextboxAnimation(0f);
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x00021E9C File Offset: 0x0002009C
		public override void Hide()
		{
			if (this.visible)
			{
				this.Recenter();
				this.advanceArrow.Visible = false;
				this.state = OverworldTextBox.AnimationState.SlideOut;
				this.slideProgress = (this.canTransitionOut ? 1f : 0f);
				this.UpdateTextboxAnimation(this.slideProgress * 2f);
			}
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x00021EF6 File Offset: 0x000200F6
		public void SetDimmer(float dim)
		{
			this.dimmer.FillColor = new Color(0, 0, 0, (byte)(255f * dim));
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x00021F14 File Offset: 0x00020114
		public override void Update()
		{
			switch (this.state)
			{
			case OverworldTextBox.AnimationState.SlideIn:
				if (this.slideProgress < 1f)
				{
					this.UpdateTextboxAnimation(this.slideProgress);
					this.slideProgress += 0.2f;
					return;
				}
				this.state = OverworldTextBox.AnimationState.Textbox;
				this.UpdateTextboxAnimation(1f);
				base.Dequeue();
				return;
			case OverworldTextBox.AnimationState.Textbox:
				base.Update();
				return;
			case OverworldTextBox.AnimationState.SlideOut:
				if (this.slideProgress > 0f)
				{
					this.UpdateTextboxAnimation(this.slideProgress);
					this.slideProgress -= 0.2f;
					return;
				}
				this.state = OverworldTextBox.AnimationState.Hidden;
				this.UpdateTextboxAnimation(0f);
				this.typewriter.Visible = false;
				this.nametag.Visible = false;
				this.window.Visible = false;
				this.visible = false;
				return;
			default:
				return;
			}
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x00021FF0 File Offset: 0x000201F0
		public override void Draw(RenderTarget target)
		{
			if (this.nametag.Visible)
			{
				this.nametag.Draw(target);
			}
			if (this.window.Visible)
			{
				this.window.Draw(target);
			}
			if (this.typewriter.Visible)
			{
				this.typewriter.Draw(target);
			}
			if (this.advanceArrow.Visible)
			{
				this.advanceArrow.Draw(target);
			}
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x00022061 File Offset: 0x00020261
		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.nametag.Dispose();
				}
				InputManager.Instance.ButtonPressed -= this.ButtonPressed;
			}
			base.Dispose(disposing);
		}

		// Token: 0x04000765 RID: 1893
		private const Button ADVANCE_BUTTON = Button.A;

		// Token: 0x04000766 RID: 1894
		private const float TEXTBOX_ANIM_SPEED = 0.2f;

		// Token: 0x04000767 RID: 1895
		private const float TEXTBOX_Y_OFFSET = 4f;

		// Token: 0x04000768 RID: 1896
		private static readonly Vector2f BOX_POSITION = new Vector2f(16f, 120f);

		// Token: 0x04000769 RID: 1897
		private static readonly Vector2f BOX_SIZE = new Vector2f(231f, 56f);

		// Token: 0x0400076A RID: 1898
		private static readonly Vector2f TEXT_POSITION = new Vector2f(OverworldTextBox.BOX_POSITION.X + 10f, OverworldTextBox.BOX_POSITION.Y + 8f);

		// Token: 0x0400076B RID: 1899
		private static readonly Vector2f TEXT_SIZE = new Vector2f(OverworldTextBox.BOX_SIZE.X - 31f, OverworldTextBox.BOX_SIZE.Y - 14f);

		// Token: 0x0400076C RID: 1900
		private static readonly Vector2f NAMETAG_POSITION = new Vector2f(OverworldTextBox.BOX_POSITION.X + 3f, OverworldTextBox.BOX_POSITION.Y - 14f);

		// Token: 0x0400076D RID: 1901
		private static readonly Vector2f NAMETEXT_POSITION = new Vector2f(OverworldTextBox.NAMETAG_POSITION.X + 5f, OverworldTextBox.NAMETAG_POSITION.Y + 1f);

		// Token: 0x0400076E RID: 1902
		private static readonly Vector2f BUTTON_POSITION = new Vector2f(OverworldTextBox.BOX_POSITION.X + OverworldTextBox.BOX_SIZE.X - 14f, OverworldTextBox.BOX_POSITION.Y + OverworldTextBox.BOX_SIZE.Y - 6f);

		// Token: 0x0400076F RID: 1903
		protected Nametag nametag;

		// Token: 0x04000770 RID: 1904
		protected bool nametagVisible;

		// Token: 0x04000771 RID: 1905
		private OverworldTextBox.AnimationState state;

		// Token: 0x04000772 RID: 1906
		private float textboxY;

		// Token: 0x04000773 RID: 1907
		private bool canTransitionIn;

		// Token: 0x04000774 RID: 1908
		private bool canTransitionOut;

		// Token: 0x04000775 RID: 1909
		private float slideProgress;

		// Token: 0x04000776 RID: 1910
		private Shape dimmer;

		// Token: 0x020000F5 RID: 245
		private enum AnimationState
		{
			// Token: 0x04000778 RID: 1912
			SlideIn,
			// Token: 0x04000779 RID: 1913
			Textbox,
			// Token: 0x0400077A RID: 1914
			SlideOut,
			// Token: 0x0400077B RID: 1915
			Hidden
		}
	}
}
