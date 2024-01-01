using ChatCommon;
using ChatCommon.Interfaces;
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
        public Client(string name, ref IPEndPoint remoteEndPoint, IMessageSource messageSouce, UdpClient udpClient)
        {
            _name = name;
            _messageSouce = messageSouce;
            _remoteEndPoint = remoteEndPoint;
            _udpClient = udpClient;
        }
        
        public void ClientListener()
        {
                try
                {
                    var messageReceived = _messageSouce.Receive(ref _remoteEndPoint, _udpClient);
                    Console.WriteLine($"Received a message from {messageReceived.NickNameFrom}:");
                    Console.WriteLine(messageReceived.Text);

                    Confirm(messageReceived, _remoteEndPoint);
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
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12346);
            var message = new NetMessage() { NickNameFrom = _name, NickNameTo = null, Command = Command.Register };
            _messageSouce.Send(message, ref remoteEndPoint, _udpClient);
        }
        public void ClientSender()
        {
            Register(ref _remoteEndPoint);
            while(true)
            {
                try
                {
                    Console.WriteLine("Введите имя получателя");
                    string nameTo = Console.ReadLine();
                    Console.WriteLine("Введите сообщение и нажмите Enter: ");
                    var text = Console.ReadLine();
                    var message = new NetMessage() { Command = Command.Message, NickNameFrom = _name, NickNameTo = nameTo, Text = text };
                    _messageSouce.Send(message, ref _remoteEndPoint, _udpClient);
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
    }
}
