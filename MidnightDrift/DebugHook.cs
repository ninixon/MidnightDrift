#if DEBUG
using MidnightDrift.Game.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Whirlpool.Core.IO;
using Whirlpool.Core.Render;
using Whirlpool.Core.Type;

namespace MidnightDrift.Game
{
    public static class DebugHook
    {
        public static bool enabled;
        public static float lastUpdate;

        public static SceneObject[] sceneObjects;
        public static Material[] materialObjects;
        public static string debugData = "";
        public static string consoleOutput = "";

        public static void Start()
        {
            Logging.Write("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", LogStatus.Warning);
            Logging.Write("!!!            W A R N I N G            !!!", LogStatus.Warning);
            Logging.Write("!!!        Debug mode is ENABLED.       !!!", LogStatus.Warning);
            Logging.Write("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", LogStatus.Warning);
            enabled = true;

            Logging.OnWrite += OnLog;
        }

        public static void OnLog(object sender, LogEventArgs e)
        {
            consoleOutput += e.loggedStr + "\n";
        }

        public static void SendSceneData(SceneObject[] objects, Material[] materials, SceneProperties properties)
        {
            if (!enabled) return;
            if (Time.currentTime - lastUpdate <= 0.5f) return;

            sceneObjects = objects;
            materialObjects = materials;
            
        }

        public static void ClearDebugData()
        {
            debugData = "";
        }

        public static void PushDebugData(string name, Any obj)
        {
            debugData += name + " = " + obj.GetValue().ToString() + "\n";
        }
    }
}
#endif