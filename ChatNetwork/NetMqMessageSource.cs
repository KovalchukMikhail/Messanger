using ChatCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetMQ.Sockets;
using NetMQ;
using ChatCommon.Interfaces;

namespace ChatNetwork
{
    public class NetMqMessageSource : IMessageSource
    {
        public NetMessage Receive(ref IPEndPoint ep, UdpClient udpClient)
        {
            string address = "tcp://*:" + ((IPEndPoint)udpClient.Client.LocalEndPoint).Port.ToString();
            using (var client = new ResponseSocket())
            {
                client.Bind(address);
                string msg = client.ReceiveFrameString();
                client.SendFrame("Сообщение доставлено");
                NetMessage message =  NetMessage.DeserializeMessgeFromJSON(msg) ?? new NetMessage();
                if(!string.IsNullOrEmpty(message.IpUser) && !string.IsNullOrEmpty(message.PortUser))
                {
                    ep.Address = IPAddress.Parse(message.IpUser);
                    ep.Port = Convert.ToInt32(message.PortUser);
                }
                return message;
            }
        }

        public void Send(NetMessage message, ref IPEndPoint ep, UdpClient udpClient)
        {
            string address = "tcp://" + ep.Address + ":" + ep.Port;
            string jsonMessage = message.SerialazeMessageToJSON();
            using (var client = new RequestSocket())
            {
                client.Connect(address);
                client.SendFrame(jsonMessage);
                var msg = client.ReceiveFrameString();
            }
        }
    }
}
