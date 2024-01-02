using ChatCommon;
using ChatCommon.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessangerClient
{
    public class Client
    {
        private readonly string _name;
        private readonly IMessageSource _messageSouce;
        private IPEndPoint _remoteEndPoint;
        private UdpClient _udpClient;
        private string UserIp;
        private string UserPort;
        public Client(string name, ref IPEndPoint remoteEndPoint, IMessageSource messageSouce, UdpClient udpClient)
        {
            _name = name;
            _messageSouce = messageSouce;
            _remoteEndPoint = remoteEndPoint;
            _udpClient = udpClient;
            UserIp = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            UserPort = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port.ToString();
        }
        
        public void ClientListener()
        {
            try
            {
                IPEndPoint endPoint = GetEndPoint();
                var messageReceived = _messageSouce.Receive(ref endPoint, _udpClient);
                Console.WriteLine($"Received a message from {messageReceived.NickNameFrom}:");
                Console.WriteLine(messageReceived.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving the message" + ex.Message);
            }
        }
        public void Confirm(NetMessage message, IPEndPoint remoteEndPoint)
        {
            message.Command = Command.Confirmation;
            _messageSouce.Send(message, ref remoteEndPoint, _udpClient);

        }
        public void Register(ref IPEndPoint remoteEndPoint)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5556);
            var message = new NetMessage() { NickNameFrom = _name, NickNameTo = null, Command = Command.Register, IpUser = UserIp, PortUser = UserPort};
            _messageSouce.Send(message, ref remoteEndPoint, _udpClient);
        }
        public void ClientSender()
        {
            IPEndPoint endPoint = GetEndPoint();
            Register(ref endPoint);
            while(true)
            {
                try
                {
                    Console.WriteLine("Введите имя получателя");
                    string nameTo = Console.ReadLine();
                    Console.WriteLine("Введите сообщение и нажмите Enter: ");
                    var text = Console.ReadLine();
                    var message = new NetMessage() { Command = Command.Message, NickNameFrom = _name, NickNameTo = nameTo, Text = text, IpUser = UserIp, PortUser = UserPort };
                    endPoint = GetEndPoint();
                    _messageSouce.Send(message, ref endPoint, _udpClient);
                    Console.WriteLine("Сообщение отправлено");
                    ClientListener();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения" + ex.Message);
                }
            }
        }
        public void Start()
        {
            ClientSender();
        }

        public IPEndPoint GetEndPoint()
        {
            return new IPEndPoint(_remoteEndPoint.Address, _remoteEndPoint.Port);
        }
    }
}
