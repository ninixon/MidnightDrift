using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightDrift.Game.Screens
{
    public abstract class Screen
    {
        public abstract void Init();
        public abstract void Render();
        public abstract void Update();
    }
}
