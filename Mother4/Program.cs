using System;
using System.IO;
using Carbine;
using Carbine.Audio;
using Carbine.Scenes;
using Mother4.Data;
using Mother4.Scenes;

namespace Mother4
{
	internal class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				Engine.Initialize(args);
				AudioManager.Instance.MusicVolume = Settings.MusicVolume;
				AudioManager.Instance.EffectsVolume = Settings.EffectsVolume;
				Scene newScene = new TitleScene();
				SceneManager.Instance.Push(newScene);
				while (Engine.Running)
				{
					Engine.Update();
				}
			}
            catch (Exception value)
            {
                StreamWriter streamWriter = new StreamWriter("error.log", true);
                streamWriter.WriteLine("At {0}:", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                streamWriter.WriteLine(value);
                streamWriter.WriteLine();
                streamWriter.Close();
            }
		}
	}
}
