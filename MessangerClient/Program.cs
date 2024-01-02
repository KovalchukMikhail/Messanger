using ChatNetwork;
using System.Net;
using System.Net.Sockets;

namespace MessangerClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5556);
            Client client = new Client("Mikl", ref remoteEndPoint, new NetMqMessageSource(), new UdpClient(5555));
            client.Start();
        }
    }
}
