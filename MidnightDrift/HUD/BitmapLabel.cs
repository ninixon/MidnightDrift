using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.Render;

namespace MidnightDrift.Game.HUD
{
    public class BitmapFont
    {
        public Vector2 totalSize;
        public Vector2 charSize;
        public Vector2 charOffset;
        public Texture texture;

        public Dictionary<char, Vector2> texturePositions;

        public BitmapFont(Texture fontTexture, Vector2 charSize, Vector2 charOffset, char[] charMap)
        {
            this.charSize = charSize;
            this.charOffset = charOffset;
            this.texture = fontTexture;
            this.totalSize = new Vector2(fontTexture.width, fontTexture.height);
            this.texturePositions = new Dictionary<char, Vector2>();

            byte[] data = fontTexture.GetData();
            for (int i = 0; i < charMap.Length; ++i)
            {
                texturePositions.Add(charMap[i], new Vector2(charSize.X * i, 0));
            }
        }
    }

    public class BitmapLabel
    {
        public BitmapFont font;
        public Vector2 position;
        public string text;
        Material fontMaterial;

        public BitmapLabel(BitmapFont font, Vector2 position, string text)
        {
            this.font = font;
            this.position = position;
            this.text = text;
            fontMaterial = new MaterialBuilder()
                .Build()
                .SetName("Bitmap Font Material")
                .Attach(new Shader("Shaders\\Text\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\Text\\Frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();
        }

        public void Draw()
        {
            Vector2 currentPos = position;
            foreach (var c in text)
            {
                if (c == '\n') { currentPos += new Vector2(0, font.charSize.Y); currentPos.X = position.X; continue; }
                if (c == ' ') continue;
                if (!font.texturePositions.ContainsKey(c)) break;
                fontMaterial.SetVariables(new Dictionary<string, Whirlpool.Core.Type.Any>()
                {
                    //{ "fontCharPosition", Render2D.PixelsToNDCImgSize(font.texturePositions[c], font.totalSize) },
                    //{ "fontCharSize", Render2D.PixelsToNDCImg(font.charSize, font.totalSize) }
                    { "fontCharPosition", font.texturePositions[c] },
                    { "fontCharSize", font.charSize },
                    { "fontTexSize", font.totalSize }
                });
                Render2D.DrawQuad(currentPos, font.charSize, font.texture, fontMaterial);
                currentPos += new Vector2(font.charSize.X, 0);
            }
        }
    }
}
