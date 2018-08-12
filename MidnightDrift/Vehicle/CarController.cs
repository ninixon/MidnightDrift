using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string car = "180sx-tuned";
        const float MILES_PER_HOUR = 100.0f;
        float lastUpdate = 0.0f;
        Mesh testMesh;
        public Material carMaterial;
        Mesh wheelMesh;

        Texture testTexture;
        public Vector3 position;
        float currentAccel;
        float accel = 5.0f;
        float maxAccel = 1.25f;
        float decel = 10.0f;
        float turningLimit = 90.0f;
        float turningSensitivity = 720.0f;
        float turningDecreaseMultiplier = 4.0f;
        float turningWeight = 0;
        float delta = 0;

        Vector3 cameraDebugPos = Vector3.Zero;

        public Quaternion rotation;
        public Vector3 currentEuler;
        Vector3 velocity;
        Texture decalTexture;
        VehicleData data;
        Vector3 acceleration;

        NetClient netClient;

        bool debugKeyPressedLastFrame = false;

        public Vector3 forward { get
            {
                return rotation * new Vector3(-1, 0, 0);
            } }
        public void Update(SceneProperties sceneProperties)
        {
            Vector3 force = Vector3.Zero;
            delta = (float)Time.lastFrameTime / 20;
            if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.F1] && !debugKeyPressedLastFrame)
            {
                currentEuler = Vector3.Zero;
                sceneProperties.cameraMode = (CameraMode)(((int)sceneProperties.cameraMode + 1) % 4);
                debugKeyPressedLastFrame = true;
            }
            else if (!InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.F1] && debugKeyPressedLastFrame)
            {
                debugKeyPressedLastFrame = false;
            }
            if (Time.currentTime - lastUpdate >= 1)
            {
                Logging.Write((currentAccel * MILES_PER_HOUR) + "MPH");
                lastUpdate = Time.currentTime;
            }
            if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.W])
            {
                if (currentAccel < maxAccel)
                {
                    force += new Vector3(accel, 0, 0);
                    Accelerate(accel);
                }
            }
            else
            {
                if (currentAccel > 0)
                {
                    force -= new Vector3(decel, 0, 0);
                    Accelerate(-decel);
                }
            }

            force *= 100000;

            acceleration = force / data.mass;
            velocity = velocity + (delta * acceleration);
            //position = position + (delta * velocity);

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

            Render3D.sceneCamera.worldPosition = position;
            Render3D.sceneCamera.fieldOfView = 50 + (currentAccel * 10);
            switch (sceneProperties.cameraMode)
            {
                case CameraMode.Bonnet:
                    Render3D.sceneCamera.position = new Vector3(-1.5f, 1, 0);
                    Render3D.sceneCamera.vAngle = -currentEuler.Y;
                    break;
                case CameraMode.Follow:
                    if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.E])
                    {
                        Render3D.sceneCamera.position = new Vector3(9, 2, 0) - new Vector3(currentAccel, 0, 0);
                        Render3D.sceneCamera.lookAtPos = new Vector3(-10, 3, 0);
                    }
                    else
                    {
                        Render3D.sceneCamera.position = new Vector3(-9, 2, 0) + new Vector3(currentAccel, 0, 0);
                        Render3D.sceneCamera.lookAtPos = new Vector3(10, 3, 0);
                    }
                    Render3D.sceneCamera.vAngle = -currentEuler.Y + (-turningWeight / 280.0f);
                    break;
                case CameraMode.FollowDebug:
                    Render3D.sceneCamera.hAngle = -currentEuler.Y + InputHandler.GetStatus().mousePosition.Y / 100.0f;
                    Render3D.sceneCamera.position = new Vector3(10, 2, 0) + new Vector3(currentAccel, 0, 0);
                    if (InputHandler.GetStatus().mouseButtonLeft)
                        Render3D.sceneCamera.vAngle += (InputHandler.GetStatus().mouseDelta.X);
                    break;
                case CameraMode.Freecam:
                    if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.Up])
                        cameraDebugPos += new Vector3(1, 0, 0);
                    if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.Down])
                        cameraDebugPos -= new Vector3(1, 0, 0);
                    if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.Left])
                        cameraDebugPos += new Vector3(0, 0, 1);
                    if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.Right])
                        cameraDebugPos -= new Vector3(0, 0, 1);
                    if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.Q])
                        cameraDebugPos += new Vector3(0, 1, 0);
                    if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.E])
                        cameraDebugPos -= new Vector3(0, 1, 0);
                    //Render3D.sceneCamera.worldPosition = Vector3.Zero;
                    Render3D.sceneCamera.position = cameraDebugPos;
                    Render3D.sceneCamera.lookAtPos = cameraDebugPos + new Vector3(-10, 0, 0);
                    Render3D.sceneCamera.hAngle = InputHandler.GetStatus().mousePosition.Y / 100.0f;
                    Render3D.sceneCamera.vAngle = InputHandler.GetStatus().mousePosition.X / 100.0f;
                    break;
                default:
                    break;
            }
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
            currentEuler += addition * currentAccel * delta;
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

        public void Render(SceneProperties sceneProperties)
        {
            Vector3[] wheelPositions = new Vector3[4]
            {
                new Vector3(-1.65f, 0.2f, 1.0f),
                new Vector3(-1.65f, 0.2f, -1.0f),
                new Vector3(1.65f, 0.2f, 1.0f),
                new Vector3(1.65f, 0.2f, -1.0f)
            };
            carMaterial.SetVariable("DecalTexture", 1);
            carMaterial.SetVariable("ReflectionTexture", 2);
            carMaterial.SetVariable("MainLightPos", new Vector3(0, 4, 0));
            carMaterial.SetVariable("MainLightTint", Color4.White);
            carMaterial.SetVariable("MainLightConstant", 0.5f);
            carMaterial.SetVariable("MainLightLinear", 0.001f);
            carMaterial.SetVariable("MainLightQuadratic", 0.1f);
            carMaterial.SetVariable("Rotation", currentEuler);
            carMaterial.SetVariable("AmbientLightStrength", sceneProperties.AmbientLightStrength);
            var renderRotation = Quaternion.FromEulerAngles(new Vector3(currentEuler) { Y = -currentEuler.Y });
            var wheelRenderRotation = Quaternion.FromEulerAngles(new Vector3(currentEuler) { Y = -currentEuler.Y });
            decalTexture?.Bind();
            for (int w = 0; w < wheelPositions.Length; ++w)
                Render3D.DrawMesh(wheelMesh,
                    (wheelPositions[w] + position + new Vector3(0, -0.35f, 0)) * -1.0f,
                    new Vector3(0.19f, 0.19f, 0.19f) * ((w % 2 == 1) ? -1 : 1),
                    wheelRenderRotation,
                    Quaternion.FromEulerAngles(new Vector3(-currentAccel * Time.currentTime * 10.0f, 0, 0)) * Quaternion.FromEulerAngles(new Vector3(0, ((w >= 2) ? -turningWeight / 180.0f : 0), 0)),
                    testTexture);
            Render3D.DrawMesh(testMesh, position * -1.0f, new Vector3(0.2f, 0.2f, 0.2f), renderRotation, Quaternion.FromEulerAngles(new Vector3(-currentAccel / 100.0f, 0, turningWeight / 1080.0f)), testTexture, carMaterial);
        }

        //public void RenderMultipleCars(ref Vector3 wheelPositions)
        //{
        //    // draw cars in V formation
        //    for (int i = 0; i < 5; ++i)
        //    {
        //        Vector3 offset = Vector3.Zero;
        //        if (i < 2) offset = new Vector3(i - 3, 0, i - 2);
        //        else if (i > 2) offset = new Vector3(-i, 0, i - 2);
        //        else if (i == 2) offset = new Vector3(0, 0, i - 2);

        //        offset += new Vector3(0, 0, 1) * (float)Math.Sin(offset.X * offset.Z * Time.currentTime);

        //        offset *= 4.0f;
        //        for (int w = 0; w < wheelPositions.Length; ++w)
        //            Render3D.DrawMesh(wheelMesh,
        //                (wheelPositions[w] + position + offset + new Vector3(0, -0.35f, 0)) * -1.0f,
        //                new Vector3(0.19f, 0.19f, 0.19f) * ((w % 2 == 1) ? -1 : 1),
        //                wheelRenderRotation,
        //                Quaternion.FromEulerAngles(new Vector3(-currentAccel * Time.currentTime * 10.0f, 0, 0)) * Quaternion.FromEulerAngles(new Vector3(0, ((w < 2) ? -turningWeight / 180.0f : 0) * (float)Math.Sin(offset.X * offset.Z * Time.currentTime), 0)),
        //                testTexture);
        //        Render3D.DrawMesh(testMesh, (position + offset) * -1.0f, new Vector3(0.2f, 0.2f, 0.2f), renderRotation, Quaternion.FromEulerAngles(new Vector3(-currentAccel / 100.0f, 0, turningWeight / 1080.0f)), testTexture, carMaterial);
        //    }
        //}

        public void Init()
        {
            netClient = new NetClient();
            carMaterial = new MaterialBuilder()
                .Build()
                .SetName("Car Material")
                .Attach(new Shader("Shaders\\Cars\\vert.glsl", ShaderType.VertexShader))
                .Attach(new Shader("Shaders\\Cars\\Frag.glsl", ShaderType.FragmentShader))
                .Link()
                .GetMaterial();
            testMesh = MeshLoader.LoadAsset("Content\\Cars\\" + car + "\\model.obj");
            wheelMesh = MeshLoader.LoadAsset("Content\\Cars\\wheel01.obj");
            testTexture = TextureLoader.LoadAsset("Content\\Cars\\" + car + "\\texture.png");
            decalTexture = TextureLoader.LoadAsset("Content\\Cars\\" + car + "\\decal.png");
            decalTexture.textureUnit = TextureUnit.Texture1;
            using (var streamReader = new StreamReader("Content\\Cars\\" + car + "\\data.json"))
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<VehicleData>(streamReader.ReadToEnd());
        }
    }
}
