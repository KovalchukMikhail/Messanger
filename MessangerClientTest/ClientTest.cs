using Messanger.Abstracts;
using Messanger.Models;
using MessangerClient;
using Moq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace MessangerClientTest
{
    public class ClientTest
    {
        [Fact]
        public void ClientListenerConsoleAnswerTest()
        {
            // Arrange
            NetMessage messagAnswer = new NetMessage() { Text = "Тестовый ответ", NickNameFrom = "Тестовый акаунт"};
            string adress = "127.0.0.1";
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(adress), 12345);
            UdpClient udpClient = new UdpClient(12346);
            var messageSourceMock = new Mock<IMessageSource>();
            messageSourceMock.Setup(m => m.Receive(ref remoteEndPoint, udpClient)).Returns(messagAnswer);
            var clientMock = new Mock<Client>();
            var output = new StringWriter();
            TextWriter old = Console.Out;
            Console.SetOut(output);
            Client client = new Client("Mikl", ref remoteEndPoint, messageSourceMock.Object, udpClient);

            // Act
            client.ClientListener();

            // Assert
            string actual = output.ToString();
            Assert.Equal($"Получено сообщение от Тестовый акаунт:\r\nТестовый ответ\r\n", actual);
            Console.SetOut(old);
        }
    }
}