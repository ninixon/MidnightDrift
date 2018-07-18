using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.IO;

namespace MidnightDrift.Game
{
    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            try
            {
#endif
            using (var g = new MainGame())
            {
                foreach (string arg in args)
                {
                    switch (arg)
                    {
                        case "-nc":
                            UnsafeNativeMethods.HideConsole();
                            break;
                        case "-dbg":
                            DebugHook.Start();
                            // Start a debug session
                            break;
                        default:
                            Logging.Write("Unrecognized argument '" + arg + "'.", LogStatus.Error);
                            break;
                    }
                }
                g.Run();
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
#endif
        }
    }
}
