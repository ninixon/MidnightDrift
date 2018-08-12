using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
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
    public class Splash : Screen
    {
        public Texture splashScreenTexture;
        Material mat;
        public override void Init()
        {
            splashScreenTexture = TextureLoader.LoadAsset("Content\\Screens\\Splash\\notice.png");
            mat = new MaterialBuilder()
                .Build()
                .SetName("Default Sprite Material")
                .Attach(new Shader("Shaders\\2DOpacity\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\2DOpacity\\frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();
        }
        public override void Update()
        {
        }
        public override void Render()
        {
            var opacity = (Time.GetMilliseconds() >= 5000) ? 1.0f - ((Time.GetMilliseconds() - 5000.0f) / 1000) : 1.0f;
            mat.SetVariable("Tint", Color4.White);
            mat.SetVariable("Opacity", opacity);
            Render2D.DrawQuad(new OpenTK.Vector2(0, 0), new OpenTK.Vector2(320, 240), splashScreenTexture, mat);
        }
    }
}
