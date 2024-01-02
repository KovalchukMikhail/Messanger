using ChatNetwork;
using Messanger.Services;

namespace Messanger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(new NetMqMessageSource());
            server.Start();
        }
    }
}
