using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.IO;
using Whirlpool.Core.Render;

namespace MidnightDrift.Game.Screens.Menus
{
    public class PSXIntro : Screen
    {
        private enum Stage
        {
            First,
            Second,
            Post
        }

        Stage stage = Stage.First;
        public Texture firstLogoTexture;
        public Texture firstTextTexture;

        Material mat;


        public override void Init()
        {
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
            if (stage == Stage.First)
            {
                var opacity = (Time.GetMilliseconds() <= 1000) ? (Time.GetMilliseconds() / 1000) : 1.0f;
                mat.SetVariable("Opacity", opacity);
                mat.SetVariable("Tint", Color4.White);
                Render2D.DrawQuad(new OpenTK.Vector2(0, 0), new OpenTK.Vector2(320, 240), FileBank.GetTexture("blank"), mat);
            }
            else if (stage == Stage.Second)
            {

            }
            else
            {
                // this should never actually happen
            }
        }
    }
}
