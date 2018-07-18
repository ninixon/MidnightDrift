using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.Render;

namespace MidnightDrift.Game
{
    public class SceneObject
    {
        public string name;
        public Mesh mesh;
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
        public Texture texture;
        public Material material;

        public SceneObject(string name, Mesh mesh, Vector3 position, Vector3 scale, Quaternion rotation, Texture texture = null, Material material = null)
        {
            this.name = name;
            this.mesh = mesh;
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
            this.texture = texture;
            this.material = material;
        }
    }

    public class SceneProperties : SceneObject
    {
        public float MainLightConstant = 0.5f;
        public float MainLightLinear = 0.001f;
        public float MainLightQuadratic = 0.1f;
        public Color4 MainLightTint = Color4.White;
        public float AmbientLightStrength = 0.8f;

        public SceneProperties() : base("Scene", new Mesh(), new Vector3(0, 0, 0), new Vector3(0, 0, 0), Quaternion.Identity){ }


    }


}
