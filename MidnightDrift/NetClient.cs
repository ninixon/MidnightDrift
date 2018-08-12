using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MidnightDrift.Game
{
    public class NetClient
    {
        UdpClient client;

        public Vector3 playerPositions;

        public NetClient()
        {
            client = new UdpClient();
            client.Connect("localhost", 19542);
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x05);
            foreach (byte b in Encoding.ASCII.GetBytes("xezno"))
                bytes.Add(b);
            client.Send(bytes.ToArray(), bytes.Count);
            client.Close();
        }
    }
}
