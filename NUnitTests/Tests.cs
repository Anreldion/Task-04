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
    [TestFixture]
    class ClientTests
    {
        private Server server;
        private string serverIP = "127.0.0.2";
        private const int serverPort = 8080;
        //ClientMessageDictionary messageDictionary;

        [OneTimeSetUp]
        public void SetUp()
        {
            server = new Server(IPAddress.Parse(serverIP), serverPort);
            //messageDictionary = new ClientMessageDictionary();
        }

        [TestCase("127.0.0.2", 8080, "Test")]
        [TestCase("127.0.0.2", 8080, "Тест")]
        [TestCase("127.0.0.2", 8080, "OK")]
        [Description("")]
        public void SendMessageTest(string ip, int port, string send_message)
        {
            Client client = new Client(IPAddress.Parse(ip), port);

            server.NewMassageEvent += (client, new_message) =>
            {
                Assert.AreEqual(new_message, send_message);
            };

            client.SendMessage(send_message);
        }
    }
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

        [TestCase("127.0.0.1", 8080, "Тест", "Test")]
        [TestCase("127.0.0.1", 8080, "Тест кейс", "Test keis")]
        public void BroadcastMessage_Test(string ip, int port, string send_message, string expected_message)
        {
            Client client = new Client(IPAddress.Parse(ip), port);

            client.RxDMessageEvent += delegate (TcpClient client, string message)
            {
                Assert.AreEqual(expected_message, Transliteration.Run(send_message));
            };

            server.BroadcastMessage(send_message);
        }
    }
    /// <summary>
    /// Testing methods of <see cref="Transliteration"/> class
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
}
