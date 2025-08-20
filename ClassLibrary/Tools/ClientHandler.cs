using Messenger.Services;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Tools
{
    public class ClientHandler
    {
        public Guid Id { get; } = Guid.NewGuid();

        private static TcpClient Client { get; set; }
        internal NetworkStream NetworkStream { get; private set; } = null!;
        private readonly Server _server;

        public ClientHandler(TcpClient tcpClient, Server server)
        {
            Client = tcpClient;
            _server = server;
        }

        public async Task ProcessAsync(CancellationToken ct)
        {
            try
            {
                NetworkStream = Client.GetStream();
                while (!ct.IsCancellationRequested)
                {
                    var msg = await ReadMessageAsync(NetworkStream, ct);
                    if (msg is null) break; 
                    _server.SaveMessage(Client, msg);
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

        private static async Task<string?> ReadMessageAsync(NetworkStream stream, CancellationToken ct)
        {
            var buf = new byte[NetConfig.RxDBufferSize];
            var sb = new StringBuilder();

            int count = await stream.ReadAsync(buf, 0, buf.Length, ct);
            if (count == 0) return null;
            sb.Append(Encoding.Unicode.GetString(buf, 0, count));

            while (stream.DataAvailable)
            {
                count = await stream.ReadAsync(buf, 0, buf.Length, ct);
                if (count == 0) break;
                sb.Append(Encoding.Unicode.GetString(buf, 0, count));
            }
            return sb.ToString();
        }

        internal void Close()
        {
            try { NetworkStream?.Close(); } catch { }
            try { Client?.Close(); } catch { }
        }
    }

}
