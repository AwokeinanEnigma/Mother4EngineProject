using System;
using System.IO;
using System.Reflection;
using Carbine;
using Carbine.Audio;
using Carbine.Graphics;
using Carbine.GUI;
using Carbine.Input;
using Carbine.Scenes;
using Carbine.Scenes.Transitions;
using Mother4.Data;
using Mother4.GUI;
using Mother4.GUI.Modifiers;
using Mother4.Scenes.Transitions;
using Rufini.Strings;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Scenes
{
	internal class TitleScene : StandardScene
	{
		public TitleScene()
		{
			Fonts.LoadFonts(Settings.Locale);
			string[] items;
			if (File.Exists("sav.dat"))
			{
				this.canContinue = true;
				items = new string[]
				{
					"Map Test",
					"New Game",
					"Continue",
					"Options",
					"Quit"
				};
			}
			else
			{
				this.canContinue = false;
				items = new string[]
				{
					"Map Test",
					"New Game",
					"Options",
					"Quit"
				};
			}
			this.optionList = new ScrollingList(new Vector2f(32f, 80f), 0, items, 5, 16f, 80f, Paths.GRAPHICS + "realcursor.dat");
			this.optionList.UseHighlightTextColor = true;
			//optionList.fak= true;
			this.pipeline.Add(this.optionList);
			this.titleImage = new IndexedColorGraphic(Paths.GRAPHICS + "title.dat", "title", new Vector2f(160f, 44f), 100);
			Version version = Assembly.GetEntryAssembly().GetName().Version;
			this.versionText = new TextRegion(new Vector2f(2f, 164f), 0, Fonts.Main, string.Format("{0}.{1} {2} {3} {4}", new object[]
			{
				version.Major,
				version.Minor,
				version.Build,
				version.Revision,
				StringFile.Instance.Get("psi.alpha")
			}));
			this.versionText.Color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 128);
			this.pipeline.Add(this.titleImage);
			this.pipeline.Add(this.versionText);
			this.mod = new GraphicTranslator(this.titleImage, new Vector2f(160f, 36f), 30);
			this.sfxCursorY = AudioManager.Instance.Use(Paths.AUDIO + "cursory.wav", AudioType.Sound);
			this.sfxConfirm = AudioManager.Instance.Use(Paths.AUDIO + "confirm.wav", AudioType.Sound);
			this.sfxCancel = AudioManager.Instance.Use(Paths.AUDIO + "cancel.wav", AudioType.Sound);
		}

		private void AxisPressed(InputManager sender, Vector2f axis)
		{
			if (axis.Y < -0.1f)
			{
				if (this.optionList.SelectPrevious())
				{
					this.sfxCursorY.Play();
					return;
				}
			}
			else if (axis.Y > 0.1f && this.optionList.SelectNext())
			{
				this.sfxCursorY.Play();
			}
		}

		private void ButtonPressed(InputManager sender, Button b)
		{
			if (b == Button.A || b == Button.Start)
			{
				this.DoSelection();
				return;
			}
			switch (b)
			{
			case Button.F1:
			{
				SceneManager.Instance.Transition = new IrisTransition(1f);
				PartyManager.Instance.Clear();
						PartyManager.Instance.AddAll(new CharacterType[]
						{
					CharacterType.Travis,
					CharacterType.Floyd,
					CharacterType.Meryl,
					CharacterType.Leo
						});
						OverworldScene newScene = new OverworldScene("debug_room.dat", new Vector2f(256f, 128f), 6, false, false, false);
				SceneManager.Instance.Push(newScene);
				return;
			}
			case Button.F2:
				SceneManager.Instance.Transition = new ColorFadeTransition(0.5f, Color.Black);
				SceneManager.Instance.Push(new SaveScene(SaveScene.Location.Belring, SaveFileManager.Instance.CurrentProfile));
				return;
				case Button.Eight:
					Engine.ScreenScale = 5;
					PartyManager.Instance.AddAll(new CharacterType[]
	{
					CharacterType.Travis,
					CharacterType.Floyd,
					CharacterType.Meryl,
					CharacterType.Leo
	}); SceneManager.Instance.Transition = new BattleSwirlTransition(Overworld.BattleSwirlOverlay.Style.Blue);;
					SceneManager.Instance.Push(new BattleScene(new EnemyType[1] { EnemyType.MysteriousTank }, true));
					return;
				default:
				return;
			}
		}

		private void DoSelection()
		{
			int num = this.optionList.SelectedIndex;
			if (!this.canContinue && num > 1)
			{
				num++;
			}
			switch (num)
			{
			case 0:
				this.sfxConfirm.Play();
				SceneManager.Instance.Transition = new ColorFadeTransition(1f, Color.Black);
				SceneManager.Instance.Push(new MapTestSetupScene());
				return;
			case 1:
				this.sfxConfirm.Play();
				SceneManager.Instance.Transition = new ColorFadeTransition(1f, Color.Black);
				SceneManager.Instance.Push(new NamingScene());
				return;
			case 2:
				this.sfxConfirm.Play();
				SceneManager.Instance.Transition = new ColorFadeTransition(1f, Color.Black);
				SceneManager.Instance.Push(new ProfilesScene());
				return;
			case 3:
				this.sfxConfirm.Play();
				SceneManager.Instance.Transition = new ColorFadeTransition(1f, Color.Black);
				SceneManager.Instance.Push(new OptionsScene());
				return;
			case 4:
				this.sfxConfirm.Play();
				SceneManager.Instance.Pop();
				return;
			default:
				return;
			}
		}

		public override void Focus()
		{
			base.Focus();
			ViewManager.Instance.Center = new Vector2f(160f, 90f);
			Engine.ClearColor = Color.Black;
			AudioManager.Instance.SetBGM(Paths.AUDIO + "test.mp3");
			AudioManager.Instance.BGM.Play();
			InputManager.Instance.AxisPressed += this.AxisPressed;
			InputManager.Instance.ButtonPressed += this.ButtonPressed;
		}

		public override void Unfocus()
		{
			base.Unfocus();
			InputManager.Instance.AxisPressed -= this.AxisPressed;
			InputManager.Instance.ButtonPressed -= this.ButtonPressed;
		}

		public override void Update()
		{
			base.Update();
			this.mod.Update();
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.titleImage.Dispose();
					this.optionList.Dispose();
					this.versionText.Dispose();
				}
				AudioManager.Instance.Unuse(this.sfxCursorY);
				AudioManager.Instance.Unuse(this.sfxConfirm);
				AudioManager.Instance.Unuse(this.sfxCancel);
			}
			base.Dispose(disposing);
		}

		private TextRegion versionText;

		private ScrollingList optionList;

		private IndexedColorGraphic titleImage;

		private CarbineSound sfxCursorY;

		private CarbineSound sfxConfirm;

		private CarbineSound sfxCancel;

		private IGraphicModifier mod;

		private bool canContinue;
	}
}
