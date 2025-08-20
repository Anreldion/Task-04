using Messenger.Services.Interfaces;
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
            _stream = _client.GetStream();
            _ = ReceiveLoopAsync(_cts.Token);
        }

        public async Task SendMessageAsync(string message, CancellationToken ct = default)
        {
            var data = Encoding.Unicode.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length, ct);
        }

        private async Task ReceiveLoopAsync(CancellationToken ct)
        {
            try
            {
                var buf = new byte[NetConfig.RxDBufferSize];
                var sb = new StringBuilder();

                while (!ct.IsCancellationRequested)
                {
                    var count = await _stream.ReadAsync(buf, 0, buf.Length, ct);
                    if (count == 0) break;

                    sb.Clear();
                    sb.Append(Encoding.Unicode.GetString(buf, 0, count));
                    while (_stream.DataAvailable)
                    {
                        count = await _stream.ReadAsync(buf, 0, buf.Length, ct);
                        if (count == 0) break;
                        sb.Append(Encoding.Unicode.GetString(buf, 0, count));
                    }

                    NewMessageEvent?.Invoke(_client, sb.ToString());
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
        }
    }
}
