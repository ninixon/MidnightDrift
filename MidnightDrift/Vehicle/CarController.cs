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

namespace MidnightDrift.Game.Vehicle
{
    public class CarController
    {
        public Mesh testMesh;
        public Material carMaterial;
        public Mesh wheelMesh;

        public Texture testTexture;
        public Vector3 position;
        public float currentAccel;
        public float accel = 10.0f;
        public float maxAccel = 1.0f;
        public float decel = 10.0f;
        public float rotationY;
        public float turningLimit = 90.0f;
        public float turningSensitivity = 720.0f;
        public float turningDecreaseMultiplier = 4.0f;
        public float turningWeight = 0;
        public float delta = 0;

        public Quaternion rotation;
        public Vector3 currentEuler;
        public Vector3 velocity;
        public Texture decalTexture;

        public Vector3 forward { get
            {
                return rotation * new Vector3(-1, 0, 0);
            } }
        public void Update()
        {
            delta = (float)Time.lastFrameTime / 20;
            //delta = 0.0016f;
            Vector3 normalizedVel = velocity.Normalized();
            if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.W])
            {
                if (currentAccel < maxAccel)
                    Accelerate(accel);
            }
            else
            {
                if (currentAccel > 0)
                    Accelerate(-decel);
            }

            position -= forward * currentAccel;

            if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.A] && Math.Round(currentAccel, 2) != 0)
            {
                if (turningWeight < 0)
                    turningWeight += turningSensitivity * delta * currentAccel * turningDecreaseMultiplier;
                else
                    turningWeight += turningSensitivity * delta * currentAccel;
            }
            else if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.D] && Math.Round(currentAccel, 2) != 0)
            {
                if (turningWeight > 0)
                    turningWeight -= turningSensitivity * delta * currentAccel * turningDecreaseMultiplier;
                else
                    turningWeight -= turningSensitivity * delta * currentAccel;
            }
            else
            {
                if (turningWeight > 0)
                    turningWeight -= turningSensitivity * delta * turningDecreaseMultiplier;
                if (turningWeight < 0)
                    turningWeight += turningSensitivity * delta * turningDecreaseMultiplier;
            }
            if (turningWeight > turningLimit) turningWeight = turningLimit;
            if (turningWeight < -turningLimit) turningWeight = -turningLimit;
            Turn(turningWeight);
            Rotate(new Vector3(0, 1, 0) * turningWeight);
            //Render3D.sceneCamera.position = position + new Vector3(10, 4, 0);
            //Render3D.sceneCamera.lookAtPos = position + new Vector3(0, 1, 0);
            Render3D.sceneCamera.worldPosition = position;
            Render3D.sceneCamera.fieldOfView = 50 + (currentAccel * 10);
            //Render3D.sceneCamera.vAngle = -currentEuler.Y + (-turningWeight / 280.0f);
            //Render3D.sceneCamera.hAngle = InputHandler.GetStatus().mousePosition.Y / 100.0f;
            Render3D.sceneCamera.vAngle = -currentEuler.Y + (InputHandler.GetStatus().mousePosition.X / 100.0f);

            Render3D.defaultMaterial.SetVariable("MainLightPos", new Vector3((float)Math.Sin(Time.currentTime) * 4, 4, (float)Math.Cos(Time.currentTime) * 4));
        }

        public void Turn(float amount)
        {
            turningWeight += amount * delta;
        }

        public void Accelerate(float acceleration)
        {
            currentAccel += acceleration * delta;
        }

        public void Rotate(Vector3 addition)
        {
            currentEuler += addition * delta;
            rotation = Quaternion.FromEulerAngles(currentEuler);
        }

        public Vector3 QuatToEuler(Quaternion quaternion)
        {
            Vector3 eulerAngles = new Vector3();

            double sinX = 2.0 * (quaternion.W * quaternion.X + quaternion.Y * quaternion.Z);
            double cosX = 1.0 - 2.0 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y);

            eulerAngles.X = (float)Math.Atan2(sinX, cosX);

            double sinY = 2.0 * (quaternion.W * quaternion.Y - quaternion.Z * quaternion.X);
            if (Math.Abs(sinY) >= 1)
                eulerAngles.Y = (float)Math.PI / 2 * Math.Sign(sinY);
            else
                eulerAngles.Y = (float)Math.Asin(sinY);

            double sinZ = 2.0 * (quaternion.W * quaternion.Z + quaternion.X * quaternion.Y);
            double cosZ = 1.0 - 2.0 * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);

            eulerAngles.Z = (float)Math.Atan2(sinZ, cosZ);

            return eulerAngles;
        }

        public void Render()
        {
            Vector3[] wheelPositions = new Vector3[4]
            {
                new Vector3(-1.65f, 0.2f, 1.05f),
                new Vector3(-1.65f, 0.2f, -1.05f),
                new Vector3(1.65f, 0.2f, 1.05f),
                new Vector3(1.65f, 0.2f, -1.05f)
            };
            carMaterial.SetVariable("DecalTexture", 0);
            carMaterial.SetVariable("ReflectionTexture", 2);
            carMaterial.SetVariable("MainLightPos", new Vector3(0, 4, 0));
            carMaterial.SetVariable("MainLightTint", Color4.White);
            carMaterial.SetVariable("MainLightConstant", 0.5f);
            carMaterial.SetVariable("MainLightLinear", 0.001f);
            carMaterial.SetVariable("MainLightQuadratic", 0.1f);
            carMaterial.SetVariable("Rotation", currentEuler);
            var renderRotation = Quaternion.FromEulerAngles(new Vector3(currentEuler) { Y = -currentEuler.Y });
            var wheelRenderRotation = Quaternion.FromEulerAngles(new Vector3(currentEuler) { Y = -currentEuler.Y });
            decalTexture?.Bind();
            for (int i = 0; i < wheelPositions.Length; ++i)
                Render3D.DrawMesh(wheelMesh, 
                    wheelPositions[i] + position * -1.0f, 
                    new Vector3(0.2f, 0.2f, 0.2f) * ((i % 2 == 0) ? -1 : 1), 
                    wheelRenderRotation, 
                    Quaternion.FromEulerAngles(new Vector3(-currentAccel * Time.currentTime * 10.0f, 0, 0)) * Quaternion.FromEulerAngles(new Vector3(0, ((i < 2) ? -turningWeight / 90.0f : 0), 0)), 
                    testTexture, carMaterial);

            Render3D.DrawMesh(testMesh, position * -1.0f, new Vector3(0.2f, 0.2f, 0.2f), renderRotation, Quaternion.Identity, testTexture, carMaterial);
        }

        public void Init()
        {
            carMaterial = new MaterialBuilder()
                .Build()
                .SetName("Car Material")
                .Attach(new Shader("Shaders\\Cars\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\Cars\\Frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();
            testMesh = MeshLoader.LoadAsset("Content\\Cars\\car01.obj");
            wheelMesh = MeshLoader.LoadAsset("Content\\Cars\\wheel01.obj");
            testTexture = TextureLoader.LoadAsset("Content\\Cars\\main.png");
            decalTexture = TextureLoader.LoadAsset("Content\\Cars\\decal.png");
            decalTexture.textureUnit = TextureUnit.Texture1;
        }
    }
}
