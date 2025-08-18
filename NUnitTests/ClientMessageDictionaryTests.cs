using Messenger.Tools;
using NUnit.Framework;
using System.IO;
using System.Net.Sockets;

namespace NUnitTests;
[TestFixture]
public class ClientMessageDictionaryTests
{
    private ClientMessageDictionary _dictionary;

    [OneTimeSetUp]
    public void SetUp()
    {
        _dictionary = new ClientMessageDictionary();
    }

    [TestCase("message_in_1", "message_in_2")]
    [Description("Testing AddMessage method")]
    public void AddMessageTest(string message_in_1, string message_in_2)
    {
        var tcpClient1 = new TcpClient();
        var tcpClient2 = new TcpClient();

        // Act
        _dictionary.AddMessage(tcpClient1, message_in_1);
        _dictionary.AddMessage(tcpClient1, message_in_2);

        _dictionary.AddMessage(tcpClient2, message_in_1);
        _dictionary.AddMessage(tcpClient2, message_in_2);

        // Assert
        Assert.AreEqual(message_in_1, _dictionary[tcpClient1][0]);
        Assert.AreEqual(message_in_2, _dictionary[tcpClient2][1]);
    }

    [TestCase("message_in_1", "message_in_2")]
    [Description("Testing AddMessage method")]
    public void GetMessagesTest(string message_in_1, string message_in_2)
    {
        var tcpClient1 = new TcpClient();
        var tcpClient2 = new TcpClient();
        _dictionary.AddMessage(tcpClient1, message_in_1);
        _dictionary.AddMessage(tcpClient1, message_in_2);
        _dictionary.AddMessage(tcpClient2, message_in_1);
        _dictionary.AddMessage(tcpClient2, message_in_2);

        // Act
        var list = _dictionary.GetMessages(tcpClient1);

        // Assert
        Assert.AreEqual(message_in_1, list[0]);
        Assert.AreEqual(message_in_2, list[1]);
    }

    [TestCase("Первое сообщение", "Второе сообщение")]
    [Description("Testing AddMessage method")]
    public void ToFileTest(string message_in_1, string message_in_2)
    {
        var tcpClient1 = new TcpClient();
        _dictionary.AddMessage(tcpClient1, message_in_1);
        _dictionary.AddMessage(tcpClient1, message_in_2);

        // Act
        _dictionary.ToFile("messages.txt");

        // Assert
        Assert.True(File.Exists("messages.txt"));
    }
}

