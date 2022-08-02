using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Carbine.Audio;
using Carbine.Graphics;
using Carbine.GUI;
using Carbine.Input;
using Carbine.Scenes;
using Carbine.Scenes.Transitions;
using Carbine.Utility;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Carbine
{
	public static class Engine
	{
		public static RenderWindow Window
		{
			get
			{
				return Engine.window;
			}
		}

		public static RenderTexture FrameBuffer
		{
			get
			{
				return Engine.frameBuffer;
			}
		}

		public static Random Random
		{
			get
			{
				return Engine.rand;
			}
		}

		public static FontData DefaultFont
		{
			get
			{
				return Engine.defaultFont;
			}
		}

		public static bool Running { get; private set; }

		public static uint ScreenScale
		{
			get
			{
				return Engine.frameBufferScale;
			}
			set
			{
				Engine.frameBufferScale = Math.Max(0U, value);
				Engine.switchScreenMode = true;
			}
		}

		public static bool Fullscreen
		{
			get
			{
				return Engine.isFullscreen;
			}
			set
			{
				Engine.isFullscreen = value;
				Engine.switchScreenMode = true;
			}
		}

		public static float FPS
		{
			get
			{
				return Engine.fps;
			}
		}

		public static long Frame
		{
			get
			{
				return Engine.frameIndex;
			}
		}

		public static SFML.Graphics.Color ClearColor { get; set; }

		public static int SessionTime
		{
			get
			{
				return (int)TimeSpan.FromTicks(DateTime.Now.Ticks - Engine.startTicks).TotalSeconds;
			}
		}

		public static void Initialize(string[] args)
		{
			Engine.frameStopwatch = Stopwatch.StartNew();
			Engine.startTicks = DateTime.Now.Ticks;
			bool vsync = false;
			bool goFullscreen = false;
			for (int i = 0; i < args.Length; i++)
			{
				string a;
				if ((a = args[i]) != null)
				{
					if (!(a == "-fullscreen"))
					{
						if (!(a == "-vsync"))
						{
							if (a == "-scale")
							{
								uint screenScale = 1U;
								if (uint.TryParse(args[++i], out screenScale))
								{
									Engine.ScreenScale = screenScale;
								}
							}
						}
						else
						{
							vsync = true;
						}
					}
					else
					{
						goFullscreen = true;
					}
				}
			}
			Engine.frameBuffer = new RenderTexture(320U, 180U);
			Engine.frameBufferState = new RenderStates(BlendMode.Alpha, Transform.Identity, Engine.frameBuffer.Texture, null);
			Engine.frameBufferVertArray = new VertexArray(PrimitiveType.Quads, 4U);
			Engine.SetWindow(goFullscreen, vsync);
			InputManager.Instance.ButtonPressed += Engine.OnButtonPressed;
			int num = 160;
			int num2 = 90;
			Engine.frameBufferVertArray[0U] = new Vertex(new Vector2f((float)(-(float)num), (float)(-(float)num2)), new Vector2f(0f, 0f));
			Engine.frameBufferVertArray[1U] = new Vertex(new Vector2f((float)num, (float)(-(float)num2)), new Vector2f(320f, 0f));
			Engine.frameBufferVertArray[2U] = new Vertex(new Vector2f((float)num, (float)num2), new Vector2f(320f, 180f));
			Engine.frameBufferVertArray[3U] = new Vertex(new Vector2f((float)(-(float)num), (float)num2), new Vector2f(0f, 180f));
			Engine.rand = new Random();
			Engine.defaultFont = new FontData();
			Engine.debugText = new Text(string.Empty, Engine.defaultFont.Font, Engine.defaultFont.Size);
			Engine.ClearColor = SFML.Graphics.Color.Black;
			decimal num3 = decimal.Parse(string.Format("{0}.{1}", Engine.window.Settings.MajorVersion, Engine.window.Settings.MinorVersion));
			if (num3 < 2.1m)
			{
				string message = string.Format("OpenGL version {0} or higher is required. This system has version {1}.", 2.1m, num3);
				throw new InvalidOperationException(message);
			}
			Console.WriteLine("OpenGL v{0}.{1}", Engine.window.Settings.MajorVersion, Engine.window.Settings.MinorVersion);
			Engine.fpsString = new StringBuilder(32);
			Engine.SetCursorTimer(90);
			Engine.Running = true;
		}

		public static void StartSession()
		{
			Engine.startTicks = DateTime.Now.Ticks;
		}

		private static void SetCursorTimer(int duration)
		{
			Engine.cursorTimer = Engine.frameIndex + (long)duration;
		}

		private static void SetWindow(bool goFullscreen, bool vsync)
		{
			if (Engine.window != null)
			{
				Engine.window.Closed -= Engine.OnWindowClose;
				Engine.window.MouseMoved -= Engine.MouseMoved;
				InputManager.Instance.DetachFromWindow(Engine.window);
				Engine.window.Close();
				Engine.window.Dispose();
			}
			float num = (float)Math.Cos((double)Engine.screenAngle);
			float num2 = (float)Math.Sin((double)Engine.screenAngle);
			Styles style;
			VideoMode desktopMode;
			if (goFullscreen)
			{
				style = Styles.Fullscreen;
				desktopMode = VideoMode.DesktopMode;
				float num3 = Math.Min(desktopMode.Width / 320U, desktopMode.Height / 180U);
				float num4 = (desktopMode.Width - 320f * num3) / 2f;
				float num5 = (desktopMode.Height - 180f * num3) / 2f;
				int num6 = (int)(160f * num3);
				int num7 = (int)(90f * num3);
				Engine.frameBufferState.Transform = new Transform(num * num3, num2, num4 + (float)num6, -num2, num * num3, num5 + (float)num7, 0f, 0f, 1f);
			}
			else
			{
				int num8 = (int)(160U * Engine.ScreenScale);
				int num9 = (int)(90U * Engine.ScreenScale);
				style = Styles.Close;
				desktopMode = new VideoMode(320U * Engine.frameBufferScale, 180U * Engine.frameBufferScale);
				Engine.frameBufferState.Transform = new Transform(num * Engine.frameBufferScale, num2 * Engine.frameBufferScale, (float)num8, -num2 * Engine.frameBufferScale, num * Engine.frameBufferScale, (float)num9, 0f, 0f, 1f);
			}
			Engine.window = new RenderWindow(desktopMode, "Mother 4", style);
			Engine.window.Closed += Engine.OnWindowClose;
			Engine.window.MouseMoved += Engine.MouseMoved;
			Engine.window.MouseButtonPressed += Engine.MouseButtonPressed;
			InputManager.Instance.AttachToWindow(Engine.window);
			Engine.window.SetMouseCursorVisible(!goFullscreen);
			if (vsync || goFullscreen)
			{
				Engine.window.SetVerticalSyncEnabled(true);
			}
			else
			{
				Engine.window.SetFramerateLimit(60U);
			}
			if (Engine.iconFile != null)
			{
				Engine.window.SetIcon(32U, 32U, Engine.iconFile.GetBytesForSize(32));
			}
		}

		private static void MouseButtonPressed(object sender, MouseButtonEventArgs e)
		{
			if (e.Button == Mouse.Button.Left)
			{
				Engine.showCursor = true;
				Engine.window.SetMouseCursorVisible(Engine.showCursor);
				Engine.SetCursorTimer(90);
				if (Engine.frameIndex < Engine.clickFrame + 20L)
				{
					Engine.switchScreenMode = true;
					Engine.isFullscreen = !Engine.isFullscreen;
					Engine.clickFrame = long.MinValue;
					return;
				}
				Engine.clickFrame = Engine.frameIndex;
			}
		}

		private static void MouseMoved(object sender, MouseMoveEventArgs e)
		{
			if (!Engine.showCursor)
			{
				Engine.showCursor = true;
				Engine.window.SetMouseCursorVisible(Engine.showCursor);
			}
			Engine.SetCursorTimer(90);
		}

		public static void OnWindowClose(object sender, EventArgs e)
		{
			RenderWindow renderWindow = (RenderWindow)sender;
			renderWindow.Close();
			Engine.quit = true;
		}

		public unsafe static void OnButtonPressed(InputManager sender, Carbine.Input.Button b)
		{
			switch (b)
			{
			case Carbine.Input.Button.Escape:
				if (!Engine.isFullscreen)
				{
					Engine.quit = true;
					return;
				}
				Engine.switchScreenMode = true;
				Engine.isFullscreen = !Engine.isFullscreen;
				return;
			case Carbine.Input.Button.Tilde:
				Engine.debugDisplay = !Engine.debugDisplay;
				return;
			case Carbine.Input.Button.F1:
			case Carbine.Input.Button.F2:
			case Carbine.Input.Button.F3:
			case Carbine.Input.Button.F6:
			case Carbine.Input.Button.F7:
				break;
			case Carbine.Input.Button.F4:
				Engine.switchScreenMode = true;
				Engine.isFullscreen = !Engine.isFullscreen;
				return;
			case Carbine.Input.Button.F5:
				Engine.frameBufferScale = Engine.frameBufferScale % 5U + 1U;
				Engine.switchScreenMode = true;
				return;
			case Carbine.Input.Button.F8:
			{
				SFML.Graphics.Image image = Engine.frameBuffer.Texture.CopyToImage();
				byte[] array = new byte[image.Pixels.Length];
				fixed (byte* pixels = image.Pixels, ptr = array)
				{
					for (int i = 0; i < array.Length; i += 4)
					{
						ptr[i] = pixels[i + 2];
						ptr[i + 1] = pixels[i + 1];
						ptr[i + 2] = pixels[i];
						ptr[i + 3] = pixels[i + 3];
					}
					IntPtr scan = new IntPtr((void*)ptr);
					Bitmap image2 = new Bitmap((int)image.Size.X, (int)image.Size.Y, (int)(4U * image.Size.X), PixelFormat.Format32bppArgb, scan);
					Clipboard.SetImage(image2);
				}
				Console.WriteLine("Screenshot copied to clipboard");
				return;
			}
			case Carbine.Input.Button.F9:
			{
				SFML.Graphics.Image image3 = Engine.frameBuffer.Texture.CopyToImage();
				string text = string.Format("screenshot{0}.png", Directory.GetFiles("./", "screenshot*.png").Length);
				image3.SaveToFile(text);
				Console.WriteLine("Screenshot saved as \"{0}\"", text);
				return;
			}
			default:
				if (b != Carbine.Input.Button.F12)
				{
					return;
				}
				if (!SceneManager.Instance.IsTransitioning)
				{
					SceneManager.Instance.Transition = new ColorFadeTransition(0.5f, SFML.Graphics.Color.White);
					SceneManager.Instance.Pop();
				}
				break;
			}
		}

		public static void Update()
		{
			Engine.frameStopwatch.Restart();
			if (Engine.switchScreenMode)
			{
				Engine.SetWindow(Engine.isFullscreen, false);
				Engine.switchScreenMode = false;
			}
			if (Engine.frameIndex > Engine.cursorTimer)
			{
				Engine.showCursor = false;
				Engine.window.SetMouseCursorVisible(Engine.showCursor);
				Engine.cursorTimer = long.MaxValue;
			}
			AudioManager.Instance.Update();
			Engine.window.DispatchEvents();
			try
			{
				SceneManager.Instance.Update();
				TimerManager.Instance.Update();
				ViewManager.Instance.Update();
				ViewManager.Instance.UseView();
				Engine.frameBuffer.Clear(Engine.ClearColor);
				SceneManager.Instance.Draw();
			}
			catch (EmptySceneStackException)
			{
				Engine.quit = true;
			}
			catch (Exception ex)
			{
				SceneManager.Instance.AbortTransition();
				SceneManager.Instance.Clear();
				SceneManager.Instance.Transition = new InstantTransition();
				SceneManager.Instance.Push(new ErrorScene(ex));
			}
			ViewManager.Instance.UseDefault();
			if (Engine.debugDisplay)
			{
				if (Engine.frameIndex % 10L == 0L)
				{
					Engine.fpsString.Clear();
					Engine.fpsString.AppendFormat("GC: {0:D5} KB\n", GC.GetTotalMemory(false) / 1024L);
					Engine.fpsString.AppendFormat("FPS: {0:F1}", Engine.fpsAverage);
					Engine.debugText.DisplayedString = Engine.fpsString.ToString();
				}
				Engine.frameBuffer.Draw(Engine.debugText);
			}
			Engine.frameBuffer.Display();
			Engine.window.Clear(SFML.Graphics.Color.Black);
			Engine.window.Draw(Engine.frameBufferVertArray, Engine.frameBufferState);
			Engine.window.Display();
			Engine.Running = (!SceneManager.Instance.IsEmpty && !Engine.quit);
			Engine.frameStopwatch.Stop();
			Engine.fps = 1f / (float)Engine.frameStopwatch.ElapsedTicks * (float)Stopwatch.Frequency;
			Engine.fpsAverage = (Engine.fpsAverage + Engine.fps) / 2f;
			Engine.frameIndex += 1L;
		}

		public static void SetWindowIcon(string file)
		{
			if (File.Exists(file))
			{
				Engine.iconFile = new IconFile(file);
				Engine.window.SetIcon(32U, 32U, Engine.iconFile.GetBytesForSize(32));
			}
		}

		public const string CAPTION = "Mother 4";

		public const uint TARGET_FRAMERATE = 60U;

		private const decimal REQUIRED_OGL_VERSION = 2.1m;

		public const uint SCREEN_WIDTH = 320U;

		public const uint SCREEN_HEIGHT = 180U;

		public const uint HALF_SCREEN_WIDTH = 160U;

		public const uint HALF_SCREEN_HEIGHT = 90U;

		private const int CURSOR_TIMEOUT = 90;

		private const int ICON_SIZE = 32;

		private const int DOUBLE_CLICK_TIME = 20;

		public static readonly Vector2f SCREEN_SIZE = new Vector2f(320f, 180f);

		public static readonly Vector2f HALF_SCREEN_SIZE = Engine.SCREEN_SIZE / 2f;

		private static uint frameBufferScale = 2U;

		private static RenderWindow window;

		private static RenderTexture frameBuffer;

		private static RenderStates frameBufferState;

		private static VertexArray frameBufferVertArray;

		private static Random rand;

		private static FontData defaultFont;

		private static Text debugText;

		private static bool quit;

		private static bool isFullscreen;

		private static bool switchScreenMode;

		private static float fps;

		private static float fpsAverage;

		private static long frameIndex;

		private static long startTicks;

		private static long cursorTimer;

		private static bool showCursor;

		private static long clickFrame = long.MinValue;

		private static IconFile iconFile;

		private static StringBuilder fpsString;

		private static float screenAngle = 0f;

		public static bool debugDisplay;

		private static Stopwatch frameStopwatch;
	}
}
