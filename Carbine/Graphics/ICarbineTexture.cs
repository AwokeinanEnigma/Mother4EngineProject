using System;
using SFML.Graphics;

namespace Carbine.Graphics
{
	public interface ICarbineTexture : IDisposable
	{
		Texture Image { get; }
	}
}
