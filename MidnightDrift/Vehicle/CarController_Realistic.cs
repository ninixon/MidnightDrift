using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.Audio;
using Whirlpool.Core.IO;
using Whirlpool.Core.IO.Assets;
using Whirlpool.Core.Render;
using Whirlpool.Core.Type;

namespace MidnightDrift.Game.Vehicle
{
    public class CarController_Realistic
    {
        public const float MPH_CONST = 2.24f; // m/s to mph
        public const float MULT_CONST = 100000.0f; // cm to m
        string car = "180sx-tuned";
        float lastUpdate = 0.0f;
        Mesh testMesh;
        public Material carMaterial;
        Mesh wheelMesh;
        Texture testTexture;

        string rpmAtGearChanges = "";
        
        // TODO: transforms
        public Vector3 position;
        Quaternion rotation;
        Vector3 currentEuler;
        Vector3 velocity;
        Texture decalTexture;
        float delta;
        float userRpm = 0; // clamped between 0 and 6400*usable gears. not used in calculations, used for input
        float rpm = 0; // clamped between 0 and 6400*usable gears
        float displayRpm = 0; // clamped between 0 and whatever the last torque point is
        int gear = 0; // 0 is gear 1

        public VehicleData data;

        Track engineSound;

        public Vector3 forward { get
            {
                return rotation * new Vector3(1, 0, 0);
            } }
        public void Update(SceneProperties sceneProperties)
        {
            DebugHook.ClearDebugData();
            delta = (float)Time.lastFrameTime / 20;


            // Acceleration
            if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.W])
                userRpm += data.engineIncr;
            else if (rpm > 0)
                userRpm -= data.engineDecr;
            else if (rpm != 0)
                userRpm = 0;

            rpm = userRpm * data.gearRatios[gear] * data.differentialRatio * data.transmissionEfficiency;
            rpm = Math.Min(rpm, 6400);

            
            Vector3 drag = -data.drag * velocity * velocity.Length;
            Vector3 rollingResistance = -data.rollingResistance * velocity;
            
            // Braking
            if (InputHandler.GetStatus().keyboardKeys[OpenTK.Input.Key.Space] && velocity.LengthWithSign() > 0)
                velocity = -forward * data.brake;

            if (velocity.Length < 0)
                velocity = Vector3.Zero; // TODO: reverse

            Vector3 traction = forward * GetTorqueCurve() * data.gearRatios[gear] * data.differentialRatio * data.transmissionEfficiency * MULT_CONST;
            Vector3 longitudinalForce = traction + drag + rollingResistance;
            Vector3 acceleration = longitudinalForce / data.mass;
            velocity = velocity + (delta * acceleration);

            if (rpm > data.gearChangeUpperThreshold && gear < 4)
            {
                rpmAtGearChanges += gear + " -> " + (gear + 1) + " @ " + rpm + "\n";
                gear++;
            }
            else if (rpm < data.gearChangeLowerThreshold && gear > 0)
            {
                rpmAtGearChanges += gear + " -> " + (gear - 1) + " @ " + rpm + "\n";
                gear--;
            }
            //var enginePitch = Math.Min(Math.Max(0.2f + (Math.Min(rpm / 6400, 0.8f)), 0.5f), 1.0f);
            //engineSound.SetPitch(enginePitch);

            Dictionary<string, Any> debugData = new Dictionary<string, Any>()
            {
                { "= = = = Transform properties", "= = =" },
                { "Velocity", velocity },
                { "Position", position },

                { "= = = = Engine Logic", "= = =" },
                { "Traction", traction },
                { "Drag", drag },
                { "Rolling resistance", rollingResistance },
                { "Longitudinal force", longitudinalForce },
                { "Acceleration", acceleration },


                { "= = = = Car Logic", "= = =" },
                { "Gear", gear },
                { "Current gear ratio", data.gearRatios[gear] },
                { "Current torque", GetTorqueCurve() },
                { "RPM", rpm },
                { "Input RPM", userRpm },
                { "Gear change @ RPM", rpmAtGearChanges },
                { "Speed (MPH)", (velocity.Length / 100) * MPH_CONST },


                { "= = = = Constants", "= = =" },
                { "Car name", data.name },
                { "Constant drag", data.drag },
                { "Constant rolling resistance", data.rollingResistance },
                { "Constant mass", data.mass },
                { "Transmission efficiency", data.transmissionEfficiency },
                { "Differential ratio", data.differentialRatio }
            };

            foreach (KeyValuePair<string, Any> a in debugData)
            {
                DebugHook.PushDebugData(a.Key, a.Value);
            }
            
            position = position + (delta * velocity);

            
            
            Render3D.sceneCamera.position = new Vector3(6, 2, 0);
            Render3D.sceneCamera.vAngle = -currentEuler.Y;
            Render3D.sceneCamera.worldPosition = position;
            Render3D.sceneCamera.fieldOfView = 55;
        }

        public float GetTorqueCurve()
        {
            int nearestPoint = ((int)rpm / 500);
            if (nearestPoint + 1 > data.torquePoints.Count)
                nearestPoint = data.torquePoints.Count - 1;

            nearestPoint *= 500;
            nearestPoint = Math.Max(Math.Min(nearestPoint, 6000), 0);
            
            var nearestPointA = data.torquePoints[nearestPoint.ToString()];
            var nearestPointB = data.torquePoints[(nearestPoint + 500).ToString()];

            return Lerp(nearestPointA, nearestPointB, (rpm % 500) / 500);
        }

        public float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
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
            for (int i = 0; i < data.wheelPositions.Length; ++i)
                Render3D.DrawMesh(wheelMesh,
                    data.wheelPositions[i] + position * -1.0f, 
                    new Vector3(0.19f, 0.19f, 0.19f) * ((i % 2 == 0) ? -1 : 1), 
                    wheelRenderRotation, 
                    Quaternion.Identity, 
                    testTexture);

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
            testMesh = MeshLoader.LoadAsset("Content\\Cars\\" + car + "\\model.obj");
            wheelMesh = MeshLoader.LoadAsset("Content\\Cars\\wheel01.obj");
            testTexture = TextureLoader.LoadAsset("Content\\Cars\\" + car + "\\texture.png");
            decalTexture = TextureLoader.LoadAsset("Content\\Cars\\" + car + "\\decal.png");
            decalTexture.textureUnit = TextureUnit.Texture1;
            using (var streamReader = new StreamReader("Content\\Cars\\" + car + "\\data.json"))
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<VehicleData>(streamReader.ReadToEnd());

            engineSound = TrackLoader.LoadAsset("Content\\Cars\\" + car + "\\engine.wav");
            engineSound.OnPlaybackStop += (s, e) =>
            {
                engineSound.PlayBeginning();
            };
            engineSound.Play();
            // Rolling resistance is always (max speed) * drag, with max speed in metres per second
            data.rollingResistance = (data.maxSpeed / MPH_CONST) * data.drag;
        }
    }
}
