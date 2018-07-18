using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.IO;
using Whirlpool.Core.IO.Assets;
using Whirlpool.Core.Render;

namespace MidnightDrift.Game.Screens.Menus
{
    public class Splash
    {
        public Vector2 windowSize;
        public Texture splashScreenTexture;
        public void Init(Vector2 windowSize_)
        {
            windowSize = windowSize_;
            splashScreenTexture = TextureLoader.LoadAsset("Content\\Screens\\Splash\\notice.png");
        }
        public void Render(float seconds)
        {
            var opacity = (Time.GetMilliseconds() >= 5000) ? 1.0f - ((Time.GetMilliseconds() - 5000.0f) / 1000) : 1.0f;
            Render2D.DrawQuad(new OpenTK.Vector2(0, 0), new OpenTK.Vector2(windowSize.X, windowSize.Y), splashScreenTexture);
        }
    }
}
