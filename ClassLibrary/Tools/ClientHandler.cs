using Messenger.Services;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Tools
{
    public class ClientHandler
    {
        public Guid Id { get; } = Guid.NewGuid();

        private readonly TcpClient _client;
        internal NetworkStream NetworkStream { get; private set; } = null!;
        private readonly Server _server;

        public ClientHandler(TcpClient tcpClient, Server server)
        {
            _client = tcpClient;
            _server = server;
        }

        public async Task ProcessAsync(CancellationToken serverCt)
        {
            using var linkCts = CancellationTokenSource.CreateLinkedTokenSource(serverCt);
            var ct = linkCts.Token;

            try
            {
                NetworkStream = _client.GetStream();
                var idleTimeout = TimeSpan.FromMinutes(2);

                while (!ct.IsCancellationRequested)
                {
                    using var readCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    readCts.CancelAfter(idleTimeout);

                    var msg = await Framing.ReadWithLengthAsync(NetworkStream, readCts.Token);
                    if (msg is null) break;

                    _server.SaveMessage(Id, _client, msg);

                    using var writeCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    writeCts.CancelAfter(TimeSpan.FromSeconds(10));
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        internal void Close()
        {
            try { NetworkStream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
        }
    }

}
