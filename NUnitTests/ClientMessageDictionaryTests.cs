using Messenger.Tools;
using NUnit.Framework;
using System;

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

    [TestCase("messageIn1", "messageIn2")]
    public void AddMessageTest(string messageIn1, string messageIn2)
    {
        var tcpClient1 = Guid.NewGuid();
        var tcpClient2 = Guid.NewGuid();

        // Act
        _dictionary.AddMessage(tcpClient1, messageIn1);
        _dictionary.AddMessage(tcpClient1, messageIn2);

        _dictionary.AddMessage(tcpClient2, messageIn1);
        _dictionary.AddMessage(tcpClient2, messageIn2);

        // Assert
        Assert.AreEqual(messageIn1, _dictionary.GetMessages(tcpClient1)[0]);
        Assert.AreEqual(messageIn2, _dictionary.GetMessages(tcpClient1)[1]);
    }

    [TestCase("messageIn1", "messageIn2")]
    public void GetMessagesTest(string messageIn1, string messageIn2)
    {
        var tcpClient1 = Guid.NewGuid();
        var tcpClient2 = Guid.NewGuid();
        _dictionary.AddMessage(tcpClient1, messageIn1);
        _dictionary.AddMessage(tcpClient1, messageIn2);
        _dictionary.AddMessage(tcpClient2, messageIn1);
        _dictionary.AddMessage(tcpClient2, messageIn2);

        // Act
        var list = _dictionary.GetMessages(tcpClient1);

        // Assert
        Assert.AreEqual(messageIn1, list[0]);
        Assert.AreEqual(messageIn2, list[1]);
    }
}

