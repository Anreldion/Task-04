using System.Net;
using System.Net.Sockets;
using Messenger.Services;
using NUnit.Framework;

namespace NUnitTests;

[TestFixture]
internal class ClientServiceTests
{
    private ServerService _server;
    private const string ServerIp = "127.0.0.2";
    private const int ServerPort = 8080;
    private string _expected;

    [OneTimeSetUp]
    public void SetUp()
    {
        _server = new ServerService(IPAddress.Parse(ServerIp), ServerPort);
    }

    [TestCase("127.0.0.2", 8080, "Test 1")]
    [TestCase("127.0.0.2", 8080, "Тест 2")]
    [Description("Testing SendMessage method")]
    public void SendMessageTest(string ip, int port, string expected_message)
    {
        var client = new ClientService(IPAddress.Parse(ip), port);

        _server.NewMessageEvent += (client, get_new_message) =>
        {
            Assert.AreEqual(expected_message, get_new_message);
        };

        _server.NewMessageEvent += AssertMethod;
        _expected = expected_message;

        client.SendMessage(expected_message);
    }

    private void AssertMethod(TcpClient client, string get_new_message)
    {
        Assert.AreEqual(_expected, get_new_message);
    }
}