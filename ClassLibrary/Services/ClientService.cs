using Messenger.Services.Interfaces;
using Messenger.Tools;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Services
{

    public class ClientService : INewMessage, IDisposable
    {
        private TcpClient _client = default!;
        private NetworkStream _stream = default!;
        private readonly CancellationTokenSource _cts = new();
        public event Action<TcpClient, string> NewMessageEvent;

        public async Task ConnectAsync(IPAddress ip, int port, CancellationToken ct = default)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port, ct);
            _client.NoDelay = true;
            _client.ReceiveBufferSize = 64 * 1024;
            _client.SendBufferSize = 64 * 1024;

            _stream = _client.GetStream();
            _ = ReceiveLoopAsync(_cts.Token);
        }

        public async Task SendMessageAsync(string message, CancellationToken ct = default)
        {
            using var wcts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);
            wcts.CancelAfter(TimeSpan.FromSeconds(10)); 
            await Framing.WriteWithLengthAsync(_stream, message, wcts.Token);
        }

        private async Task ReceiveLoopAsync(CancellationToken ct)
        {
            try
            {
                var idleTimeout = TimeSpan.FromMinutes(5);
                while (!ct.IsCancellationRequested)
                {
                    using var rcts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    rcts.CancelAfter(idleTimeout); 
                    string? msg = await Framing.ReadWithLengthAsync(_stream, rcts.Token);
                    if (msg is null) break;
                    NewMessageEvent?.Invoke(_client, msg);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
            finally { Dispose(); }
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
        }
    }
}
