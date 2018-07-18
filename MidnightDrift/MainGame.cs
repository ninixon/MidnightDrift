using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Whirlpool.Core.IO;
using Whirlpool.Core.IO.Assets;
using Whirlpool.Core;
using Whirlpool.Core.Render;
using MidnightDrift.Game.Vehicle;
using MidnightDrift.Game.Screens.Menus;
using OpenTK.Graphics;
using System.Collections.Generic;

namespace MidnightDrift.Game
{
    public class MainGame : BaseGame
    {
        public Splash splashScreen;
        public Texture floorTexture;
        public Texture buildingTexture;
        public Texture buildingTexture2;
        public Texture skyTexture;
        public Texture reflectionTexture;
        public Texture roadTexture;
        public Mesh floorMesh;
        public Mesh buildingMesh;
        public Mesh skyMesh;
        public Mesh roadMesh;
        public SceneProperties sceneProperties;

        public List<SceneObject> sceneObjects = new List<SceneObject>();

        public Vector3[] lights = new Vector3[8]{
            new Vector3(0, 4, -6),
            new Vector3(0, 4, -2),
            new Vector3(0, 4, 2),
            new Vector3(0, 4, 6),
            new Vector3(4, 4, -6),
            new Vector3(4, 4, -2),
            new Vector3(4, 4, 2),
            new Vector3(4, 4, 6)
        };

        public Color4[] lightTints = new Color4[8]{
            Color4.White,
            Color4.Red,
            Color4.Blue,
            Color4.Green,
            Color4.White,
            Color4.Red,
            Color4.Blue,
            Color4.Green
        };

        public Material standardMat;

        public CarController carController;

        #region "Game properties"
        public new string gameName = "Midnight Drift";
        public new string gameVersion = "2";
        public new string windowTitle = "%{gamename}";
        #endregion

        public override void OneSecondPassed()
        {
            
        }

        public override void Render()
        {
            //Renderer.RenderQuad(new Vector2(0, 0), new Vector2(320, 240), "blank", new Color4(33, 33, 33, 255));
            if (Time.GetSeconds() <= 0)
            {
                splashScreen.Render(Time.GetSeconds());
            }
            else
            {
                reflectionTexture.Bind();
                standardMat.SetVariable("ReflectionTexture", 2);
                standardMat.SetVariable("LightPositions", lights);
                standardMat.SetVariable("LightTints", lightTints);
                standardMat.SetVariable("LightCount", lights.Length);
                standardMat.SetVariable("MainLightPos", new Vector3(0, 2, 0));
                standardMat.SetVariable("MainLightTint", sceneProperties.MainLightTint);
                standardMat.SetVariable("MainLightConstant", sceneProperties.MainLightConstant);
                standardMat.SetVariable("MainLightLinear", sceneProperties.MainLightLinear);
                standardMat.SetVariable("MainLightQuadratic", sceneProperties.MainLightQuadratic);
                Render3D.defaultMaterial.SetVariable("AmbientLightStrength", sceneProperties.AmbientLightStrength);
                carController.Update();

                foreach (var obj in sceneObjects)
                {
                    Render3D.DrawMesh(obj.mesh, obj.position, obj.scale, obj.rotation, Quaternion.Identity, obj.texture, obj.material);
                }
                Render3D.DrawMesh(skyMesh, carController.position * -1.0f, new Vector3(50, 50, 50), Quaternion.Identity, Quaternion.Identity, skyTexture);
                carController.Render();
            }
        }

        public override void Update()
        {
            if (DebugHook.enabled) DebugHook.SendSceneData(sceneObjects.ToArray(), sceneProperties);
        }

        public override void Init()
        {
            carController = new CarController();
            carController.Init();
            sceneProperties = new SceneProperties();
            Title = "race test";
            floorTexture = TextureLoader.LoadAsset("Content\\Tracks\\grid.png");
            buildingTexture = TextureLoader.LoadAsset("Content\\Tracks\\building1Tex.jpg");
            buildingTexture2 = TextureLoader.LoadAsset("Content\\Tracks\\building2Tex.jpg");
            skyTexture = TextureLoader.LoadAsset("Content\\nightsky.png");
            floorMesh = MeshLoader.LoadAsset("Content\\Tracks\\circuit01.obj");
            skyMesh = MeshLoader.LoadAsset("Content\\sky.obj");
            buildingMesh = MeshLoader.LoadAsset("Content\\Tracks\\building1.obj");
            roadMesh = MeshLoader.LoadAsset("Content\\Tracks\\road01.obj");
            reflectionTexture = TextureLoader.LoadAsset("Content\\Cars\\reflection2.png");
            reflectionTexture.textureUnit = TextureUnit.Texture2;
            roadTexture = TextureLoader.LoadAsset("Content\\Tracks\\road.png");
            standardMat = new MaterialBuilder()
                .Build()
                .SetName("Standard Object Material")
                .Attach(new Shader("Shaders\\Standard\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\Standard\\frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();
            splashScreen = new Splash();
            splashScreen.Init(new Vector2(320, 240));
            RenderShared.renderResolution = new Vector2(320, 240);


            //for (int x = -10; x < 10; ++x)
            //    for (int y = -10; y < 10; ++y)
            //        if (x != 0 && y != 0)
            //            sceneObjects.Add(new SceneObject("Building", buildingMesh, new Vector3(40 * y, 0, 40 * x), new Vector3(10, 10, 10) * Math.Max((((float)Math.Sin(x * y) + 1.0f) / 2.0f), 0.5f), Quaternion.Identity, (x * y % 2 == 0) ? buildingTexture : buildingTexture2, standardMat));


            sceneObjects.Add(new SceneObject("Floor", floorMesh, new Vector3(0, -0.2f, 0), new Vector3(10, 1, 10), Quaternion.Identity, floorTexture, standardMat));

            for (int i = 0; i < 10; ++i)
                sceneObjects.Add(new SceneObject("Road", roadMesh, new Vector3(-i * 32, 0, 0), new Vector3(1, 1, 1), Quaternion.Identity, roadTexture, standardMat));
        }
    }
}
