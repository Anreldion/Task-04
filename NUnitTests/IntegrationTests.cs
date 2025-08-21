using Messenger.Services;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NUnitTests
{
    public class IntegrationTests
    {
        [Test]
        public async Task ServerReceivesAndBroadcasts_Message_RoundtripOk()
        {
            var port = GetFreePort();
            var server = new Server(IPAddress.Loopback, port);
            server.Start();

            string? serverSeen = null;
            var msgSeen = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            server.MessageReceived += (s, e) =>
            {
                serverSeen = e.Message;
                msgSeen.TrySetResult(e.Message);
            };

            var client = new ClientService();
            string? clientSeen = null;
            var clientTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            client.MessageReceived += (s, e) =>
            {
                clientSeen = e.Message;
                clientTcs.TrySetResult(e.Message);
            };

            await client.ConnectAsync(IPAddress.Loopback, port);
            await client.SendMessageAsync("Привет сервер");

            var seen = await Task.WhenAny(msgSeen.Task, Task.Delay(TimeSpan.FromSeconds(2)));
            Assert.AreSame(msgSeen.Task, seen, "Server did not receive message");
            Assert.AreEqual("Привет сервер", serverSeen);

            await server.BroadcastAsync("Эхо: Привет сервер");

            var seenCli = await Task.WhenAny(clientTcs.Task, Task.Delay(TimeSpan.FromSeconds(2)));
            Assert.AreSame(clientTcs.Task, seenCli, "Client did not receive broadcast");
            Assert.AreEqual("Эхо: Привет сервер", clientSeen);

            client.Dispose();
            server.Dispose();
        }

        private static int GetFreePort()
        {
            var l = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int p = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return p;
        }
    }
}
