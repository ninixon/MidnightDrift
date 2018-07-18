using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.IO;

namespace MidnightDrift.Game
{
    public static class DebugHook
    {
        public static bool enabled;
        public static UdpClient server;
        public static IPEndPoint clientEndPoint;
        public static float lastUpdate;
        public static void Start()
        {
            Logging.Write("Debug hook enabled.", LogStatus.Warning);
            server = new UdpClient(31019);
            enabled = true;
        }

        public static void SendSceneData(SceneObject[] objects, SceneProperties properties)
        {
            if (!enabled) return;
            if (Time.currentTime - lastUpdate <= 0.5f) return;
            lastUpdate = Time.currentTime;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 31019);
            if (server.Available > 0) if (server.Receive(ref endPoint)[0] == 0xFF) { clientEndPoint = endPoint; Logging.Write("Debug hook client connected."); }
            if (clientEndPoint == null) return;
            server.Send(new byte[] { 0x00 }, 1, clientEndPoint);

            {
                List<byte> bytes = new List<byte>();
                foreach (var variable in properties.GetType().GetFields())
                {
                    if (variable.GetValue(properties) == null) continue;
                    foreach (byte b in Encoding.ASCII.GetBytes(variable.Name + "^" + variable.GetValue(properties).ToString()))
                    {
                        bytes.Add(b);
                    }
                    bytes.Add(Encoding.ASCII.GetBytes("\n")[0]);
                }
                server.Send(bytes.ToArray(), bytes.Count, clientEndPoint);
            }

            foreach (var obj in objects)
            {
                List<byte> bytes = new List<byte>();
                foreach (var variable in obj.GetType().GetFields())
                {
                    foreach (byte b in Encoding.ASCII.GetBytes(variable.Name + "^" + variable.GetValue(obj).ToString()))
                    {
                        bytes.Add(b);
                    }
                    bytes.Add(Encoding.ASCII.GetBytes("\n")[0]);
                }
                server.Send(bytes.ToArray(), bytes.Count, clientEndPoint);
            }


            server.Send(new byte[] { 0xFF }, 1, clientEndPoint);
        }
    }
}
