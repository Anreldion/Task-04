using Messenger.Classes.ClientClasses;
using Messenger.Classes.ClientClasses.Tools;
using Messenger.Classes.ServerClasses;
using Messenger.ServerClasses.Tools;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NUnitTests
{

    /// <summary>
    /// Testing method of <see cref="Transliteration"/> class
    /// </summary>
    [TestFixture]
    public class TransliterationTests
    {
        /// <summary>
        /// Testing <see cref="Transliteration.Run(string)"/ method>
        /// </summary>
        /// <param name="expected_result">Expected result</param>
        /// <param name="message_in">Message in</param>
        [TestCase("Bulba", "Бульба")]
        [TestCase("Schaste", "Счастье")]
        [TestCase("Dozhdlivaia pogoda", "Дождливая погода")]
        [TestCase("I have read a great book. — IA prochital zamechatelnuiu knigu", "I have read a great book. — Я прочитал замечательную книгу")]
        [TestCase("Lednik Eiiafiadlaiekiudl", "Ледник Эйяфьядлайёкюдль")]
        [Description("Testing Transliteration.Run method")]
        public void RunTest(string expected_result, string message_in)
        {
            // Assert
            Assert.AreEqual(expected_result, Transliteration.Run(message_in));
        }
    }

    /// <summary>
    /// Testing method of <see cref="Client"/> class
    /// </summary>
    [TestFixture]
    class ClientTests
    {
        private Server server;
        private string serverIP = "127.0.0.2";
        private const int serverPort = 8080;
        string expected;
        //ClientMessageDictionary messageDictionary;

        [OneTimeSetUp]
        public void SetUp()
        {
            server = new Server(IPAddress.Parse(serverIP), serverPort);
            //messageDictionary = new ClientMessageDictionary();
        }

        /// <summary>
        /// Testing <see cref="Client.SendMessage(string)"/ method>
        /// </summary>
        /// <param name="ip">Local IP address</param>
        /// <param name="port">Local port address</param>
        /// <param name="expected_message">Expected receive message</param>
        [TestCase("127.0.0.2", 8080, "Test 1")]
        [TestCase("127.0.0.2", 8080, "Тест 2")]
        [Description("Testing SendMessage method")]
        public void SendMessageTest(string ip, int port, string expected_message)
        {
            Client client = new Client(IPAddress.Parse(ip), port);

            server.NewMassageEvent += (client, get_new_message) =>
            {
                Assert.AreEqual(expected_message, get_new_message);
            };

            server.NewMassageEvent += AssertMethod;
            expected = expected_message;

            client.SendMessage(expected_message);
        }

        void AssertMethod(TcpClient client, string get_new_message)
        {
            Assert.AreEqual(expected, get_new_message);
        }
    }

    /// <summary>
    /// Testing <see cref="ClientMessageDictionary"/> class
    /// </summary>
    [TestFixture]
    public class ClientMessageDictionaryTests
    {
        ClientMessageDictionary dictionary;

        [OneTimeSetUp]
        public void SetUp()
        {
            dictionary = new ClientMessageDictionary();
        }

        /// <summary>
        /// Testing <see cref="ClientMessageDictionary.AddMessage(TcpClient, string)"/ method>
        /// </summary>
        /// <param name="expected_result">Expected result</param>
        /// <param name="message_in">Message in</param>
        [TestCase("message_in_1", "message_in_2")]
        [Description("Testing AddMessage method")]
        public void AddMessageTest(string message_in_1, string message_in_2)
        {
            TcpClient tcpClient1 = new TcpClient();
            TcpClient tcpClient2 = new TcpClient();

            // Act
            dictionary.AddMessage(tcpClient1, message_in_1);
            dictionary.AddMessage(tcpClient1, message_in_2);

            dictionary.AddMessage(tcpClient2, message_in_1);
            dictionary.AddMessage(tcpClient2, message_in_2);

            // Assert
            Assert.AreEqual(message_in_1, dictionary[tcpClient1][0]);
            Assert.AreEqual(message_in_2, dictionary[tcpClient2][1]);
        }
    }

    /// <summary>
    /// Testing method of <see cref="Server"/> class
    /// </summary>
    [TestFixture]
    class ServerTests
    {
        private Server server;
        private const string serverIP = "127.0.0.1";
        private const int serverPort = 8080;

        [OneTimeSetUp]
        public void SetUp()
        {
            server = new Server(IPAddress.Parse(serverIP), serverPort);
        }
        /// <summary>
        /// Testing <see cref="Server"/ method>
        /// </summary>
        /// <param name="ip">Local IP address</param>
        /// <param name="port">Local port address</param>
        /// <param name="send_message">Send message</param>
        /// <param name="expected_message">Expected receive message</param>
        [TestCase("127.0.0.1", 8080, "Тест", "Test")]
        //[TestCase("127.0.0.1", 8080, "Тест кейс", "Test keis")]
        [Description("Testing BroadcastMessage method")]
        public void BroadcastMessageTest(string ip, int port, string send_message, string expected_message)
        {
            Client client = new Client(IPAddress.Parse(ip), port);

            client.NewMassageEvent += delegate (TcpClient client, string message)
            {
                Assert.AreEqual(expected_message, Transliteration.Run(send_message));
            };
            client.SendMessage("");
            server.BroadcastMessage(send_message);
        }
    }
    
}
