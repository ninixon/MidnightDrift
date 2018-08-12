using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Whirlpool.Core.IO;
using Whirlpool.Core.IO.Assets;
using Whirlpool.Core;
using Whirlpool.Core.Render;
using MidnightDrift.Game.Vehicle;
using MidnightDrift.Game.Screens.Menus;
using MidnightDrift.Game.Screens;
using OpenTK.Graphics;
using System.Collections.Generic;
using MidnightDrift.Game.HUD;

namespace MidnightDrift.Game
{
    public enum CameraMode
    {
        Bonnet,
        Follow,
        FollowDebug,
        Freecam
    };

    public class MainGame : BaseGame
    {
        Screen currentScreen;
        GameScreen mainGame;
        
        public BitmapLabel label;

        #region "Game properties"
        public new string gameName = "Midnight Drift";
        public new string gameVersion = "3";
        public new string windowTitle = "%{gamename}";
        #endregion

        public override void OneSecondPassed()
        {
            
        }

        public override void Render()
        {
            mainGame.Render();
            label.text = "Test";
            label.Draw();
        }

        public override void Update()
        {
            mainGame.Update();
        }

        public override void Init()
        {
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);
            
            Title = "race test";
           
            PostProcessing.GetInstance().frameBufferMaterial = new MaterialBuilder()
                .Build()
                .SetName("Standard Object Material")
                .Attach(new Shader("Shaders\\2D\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\Framebuffer\\frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();

           
            currentScreen = new Splash();
            currentScreen.Init();
            
            label = new BitmapLabel(new BitmapFont(TextureLoader.LoadAsset("Content\\Fonts\\5x10mono.png"), new Vector2(5, 10), new Vector2(0, 0), new char[] { '!', '\"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '¦' }), new Vector2(16, 16), "");

            mainGame = new GameScreen();
            mainGame.Init();
        }
    }

}
