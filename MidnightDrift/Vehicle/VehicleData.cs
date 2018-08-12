using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightDrift.Game.Vehicle
{
    public class VehicleData
    {
        public string name;
        public float drag;
        public float rollingResistance;
        public float maxSpeed;
        public float mass;
        public float engineIncr;
        public float engineDecr;
        public float brake;
        public float[] gearRatios;
        public Dictionary<string, float> torquePoints;
        public float differentialRatio;
        public float transmissionEfficiency;
        public float wheelRadius;
        public Vector3[] wheelPositions;
        public float gearChangeUpperThreshold;
        public float gearChangeLowerThreshold;
    }
}
