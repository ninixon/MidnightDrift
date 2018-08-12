using MidnightDrift.Game.HUD;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whirlpool.Core.IO.Assets;

namespace MidnightDrift.Game.Screens.Menus
{
    public class MainMenu : Screen
    {
        public BitmapLabel label;
        public override void Init()
        {

            label = new BitmapLabel(new BitmapFont(TextureLoader.LoadAsset("Content\\Fonts\\5x10mono.png"), new Vector2(5, 10), new Vector2(0, 0), new char[] { '!', '\"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '¦' }), new Vector2(16, 16), "");
            label.text = "Test";
        }

        public override void Render()
        {
            label.Draw();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
