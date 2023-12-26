using Messanger.Abstracts;
using Messanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger.Services
{
    public class UdpMessageSource : IMessageSource
    {
        public NetMessage Receive(ref IPEndPoint ep, UdpClient udpClient)
        {
            byte[] data = udpClient.Receive(ref ep);
            string str = Encoding.UTF8.GetString(data);
            return NetMessage.DeserializeMessgeFromJSON(str) ?? new NetMessage();
        }

        public void Send(NetMessage message, ref IPEndPoint ep, UdpClient udpClient)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message.SerialazeMessageToJSON());
            udpClient.SendAsync(buffer, buffer.Length, ep);
        }
    }
}
