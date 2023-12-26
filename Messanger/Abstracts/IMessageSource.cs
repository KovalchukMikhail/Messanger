using Messanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger.Abstracts
{
    public interface IMessageSource
    {
        void Send(NetMessage message, ref IPEndPoint ep, UdpClient udpClient);
        NetMessage Receive(ref IPEndPoint ep, UdpClient udpClient);
    }
}
