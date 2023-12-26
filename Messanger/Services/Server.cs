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
    public class Server
    {
        Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        IPEndPoint ep;
        private readonly IMessageSource _messageSouce;
        UdpClient udpClient = new UdpClient(12345);
        public Server()
        {
            _messageSouce = new UdpMessageSource();
            ep = new IPEndPoint(IPAddress.Any, 0);

        }
        bool work = true;
        public void Stop()
        {
            work = false;
        }
        private void Register(NetMessage message, ref IPEndPoint ep)
        {
            Console.WriteLine($"Message Register name = {message.NickNameFrom}");
            if(clients.TryAdd(message.NickNameFrom, ep))
            {
                using(ChatContext context = new ChatContext())
                {
                    context.Users.Add(new User() { FullName = message.NickNameFrom });
                    context.SaveChangesAsync();
                }
            }
        }
        private void RelyMessage(NetMessage message, ref IPEndPoint ep)
        {
            if(clients.TryGetValue(message.NickNameTo, out IPEndPoint epTo))
            {
                int? id = 0;
                using (var ctx = new ChatContext())
                {
                    var fromUser = ctx.Users.First(x => x.FullName == message.NickNameFrom);
                    var toUser = ctx.Users.First(x => x.FullName == message.NickNameTo);
                    var msg = new Message() { UserFrom = fromUser, UserTo = toUser, IsSent = false, Text = message.Text };
                    ctx.Messages.Add(msg);
                    ctx.SaveChanges();
                    id = msg.MessageId;
                }
                message.Id = id;
                _messageSouce.Send(message, ref epTo, udpClient);

                Console.WriteLine($"Message Relied, from = {message.NickNameFrom} to = {message.NickNameTo}");
            }
            else
            {
                NetMessage answer = new NetMessage() { Text = "Пользователь не найден." };
                Console.WriteLine("Пользователь не найден.");
                _messageSouce.Send(answer, ref ep, udpClient);
            }

        }
        void ConfirmMessageReceived(int? id, ref IPEndPoint ep)
        {
            Console.WriteLine("Message confirmation id=" + id);
            using(var ctx = new ChatContext())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.MessageId == id);
                if(msg != null)
                {
                    msg.IsSent = true;
                    ctx.SaveChangesAsync();
                }
            }
        }
        void ProcessMessage(NetMessage message, ref IPEndPoint ep)
        {
            switch (message.Command)
            {
                case Command.Register: Register(message, ref ep);break;
                case Command.Message: RelyMessage(message, ref ep);break;
                case Command.Confirmation: ConfirmMessageReceived(message.Id, ref ep);break;
                default: break; 
            }
        }
        public void Start()
        {
            Console.WriteLine("Сервер ожидает сообщение");

            while (work)
            {
                try
                {
                    var message = _messageSouce.Receive(ref ep, udpClient);
                    Console.WriteLine(message);
                    ProcessMessage(message, ref ep);
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
