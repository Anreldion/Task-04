using System.Net;
using System.Net.Sockets;
using Messenger.Services;
using Messenger.Tools;
using NUnit.Framework;

namespace NUnitTests;

[TestFixture]
internal class ServerTests
{
    private Server _server;
    private const string ServerIp = "127.0.0.1";
    private const int ServerPort = 8080;

    [OneTimeSetUp]
    public void SetUp()
    {
        _server = new Server(IPAddress.Parse(ServerIp), ServerPort);
    }

    [TestCase("127.0.0.1", 8080, "Тест", "Test")]
    [TestCase("127.0.0.1", 8080, "Тест кейс", "Test keis")]
    [Description("Testing BroadcastMessage method")]
    public void BroadcastMessageTest(string ip, int port, string send_message, string expected_message)
    {
        var client = new ClientService(IPAddress.Parse(ip), port);

        client.NewMessageEvent += delegate (TcpClient client, string message)
        {
            Assert.AreEqual(expected_message, Transliterator.ToLatin(send_message));
        };
        _server.BroadcastMessage(send_message);
    }
}