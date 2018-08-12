using MidnightDrift.Game.Vehicle;
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
using Whirlpool.Core.Type;

namespace MidnightDrift.Game.Screens
{
    public class GameScreen : Screen
    {
        Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();
        SceneProperties sceneProperties;

        Vector3[] lights = new Vector3[8] {
            new Vector3(0, 4, -6),
            new Vector3(0, 4, -2),
            new Vector3(0, 4, 2),
            new Vector3(0, 4, 6),
            new Vector3(4, 4, -6),
            new Vector3(4, 4, -2),
            new Vector3(4, 4, 2),
            new Vector3(4, 4, 6)
        };

        Color4[] lightTints = new Color4[8] {
            Color4.White,
            Color4.Red,
            Color4.Blue,
            Color4.Green,
            Color4.White,
            Color4.Red,
            Color4.Blue,
            Color4.Green
        };

        Material standardMat, unlitMat;

        CarController carController;
        List<Material> materials = new List<Material>();
        List<SceneObject> sceneObjects = new List<SceneObject>();
        public override void Init()
        {
            carController = new CarController();
            carController.Init();
            sceneProperties = new SceneProperties();
            Dictionary<string, string> texturesToLoad = new Dictionary<string, string>()
            {
                { "floor", @"Content\Tracks\grid.png" },
                { "building", @"Content\Tracks\buildinguv.png" },
                { "building2", @"Content\Tracks\buildinguv.png" },
                { "sky", @"Content\vpsky.png" },
                { "reflection", @"Content\Cars\reflection2.png" },
                { "test", @"Content\advisorbg.png" },
                { "road", @"Content\Tracks\road.png" },
            };
            Dictionary<string, string> meshesToLoad = new Dictionary<string, string>()
            {
                { "floor", @"Content\Tracks\circuit01.obj" },
                { "sky", @"Content\vpsky.obj" },
                { "building", @"Content\Tracks\building2.obj" },
                { "road", @"Content\Tracks\road01.obj" },
                { "cube", @"Content\sky.obj" },
            };

            foreach (var texture in texturesToLoad)
                textures.Add(texture.Key, TextureLoader.LoadAsset(texture.Value));
            foreach (var mesh in meshesToLoad)
                meshes.Add(mesh.Key, MeshLoader.LoadAsset(mesh.Value));

            //reflectionTexture.textureUnit = TextureUnit.Texture2;
            standardMat = new MaterialBuilder()
                .Build()
                .SetName("Standard Object Material")
                .Attach(new Shader("Shaders\\Standard\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\Standard\\frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();
            unlitMat = new MaterialBuilder()
                .Build()
                .SetName("Standard Object Material")
                .Attach(new Shader("Shaders\\Unlit\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\Unlit\\frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();
            materials.Add(standardMat);
            materials.Add(unlitMat);
            materials.Add(Render3D.defaultMaterial);
            materials.Add(Render2D.defaultSpriteMaterial);
            materials.Add(carController.carMaterial);
            RenderShared.renderResolution = new Vector2(320, 240);


            for (int x = -10; x < 10; ++x)
                for (int y = -10; y < 10; ++y)
                    if (x != 0 && y != 0)
                        sceneObjects.Add(new SceneObject("Building", meshes["building"], new Vector3(40 * y, 0, 40 * x), new Vector3(-2, 2, 2) * Math.Max((((float)Math.Sin(x * y) + 2.0f) / 2.0f), 1.0f), Quaternion.Identity, Quaternion.FromEulerAngles(new Vector3(0, 90 * ((x * y) % 4), 0)), (x * y % 2 == 0) ? textures["building"] : textures["building2"], standardMat));


            sceneObjects.Add(new SceneObject("Floor", meshes["floor"], new Vector3(0, -0.2f, 0), new Vector3(10, 1, 10), Quaternion.Identity, Quaternion.Identity, textures["floor"], standardMat));

            for (int i = -10; i < 10; ++i)
                sceneObjects.Add(new SceneObject("Road " + (i + 11), meshes["road"], new Vector3(-i * 32, -0.18f, 0), new Vector3(1, 1, 1), Quaternion.Identity, Quaternion.Identity, textures["road"], standardMat));
        }

        public override void Render()
        {
            Render2D.DrawQuad(new Vector2(0, -120), new Vector2(320, 480), textures["sky"]);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            foreach (Material m in materials)
                m.SetVariable("AmbientLightStrength", sceneProperties.AmbientLightStrength);

            //reflectionTexture.Bind();
            standardMat.SetVariables(new Dictionary<string, Any>(){
                { "ReflectionTexture", 2 },
                { "LightPositions", new Any(lights) },
                { "LightTints", new Any(lightTints) },
                { "LightCount", lights.Length },
                { "AmbientLightTint", sceneProperties.AmbientLightTint },
                { "MainLightPos", sceneProperties.MainLightPos},
                { "MainLightTint", sceneProperties.MainLightTint},
                { "MainLightConstant", sceneProperties.MainLightConstant},
                { "MainLightLinear", sceneProperties.MainLightLinear},
                { "MainLightQuadratic", sceneProperties.MainLightQuadratic}

            });
            carController.Update(sceneProperties);

            foreach (var obj in sceneObjects)
            {
                Render3D.DrawMesh(obj.mesh, obj.position, obj.scale, obj.rotation, Quaternion.Identity, obj.texture, obj.material);
            }
            //Render3D.DrawMesh(skyMesh, carController.position * -1.0f, new Vector3(50, 50, 50), Quaternion.Identity, Quaternion.Identity, skyTexture, unlitMat, 
            //new Dictionary<string, Whirlpool.Core.Type.Any>()
            //{
            //    { "Model", Matrix4.Identity },
            //    { "MVP", Render3D.sceneCamera.view * Render3D.sceneCamera.projection }
            //});
            carController.Render(sceneProperties);
        }

        public override void Update()
        {
            if (DebugHook.enabled) DebugHook.SendSceneData(sceneObjects.ToArray(), materials.ToArray(), sceneProperties);
        }
    }
}
