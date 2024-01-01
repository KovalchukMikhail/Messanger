using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatCommon.Interfaces
{
    public interface IMessageSource
    {
        void Send(NetMessage message, ref IPEndPoint ep, UdpClient udpClient);
        NetMessage Receive(ref IPEndPoint ep, UdpClient udpClient);
    }
}
