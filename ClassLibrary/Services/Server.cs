using Messenger.Services.Interfaces;
using Messenger.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Services
{
    /// <summary>
    /// Server side class
    /// </summary>
    public class Server : INewMessage, IDisposable
    {
        private readonly TcpListener _listener;
        private readonly object _sync = new();
        private readonly List<ClientHandler> _clients = [];
        private readonly CancellationTokenSource _cts = new();
        private readonly SemaphoreSlim _concurrency;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public int Backlog { get; }
        private readonly ClientMessageDictionary _messageDictionary = new();

        public event Action<TcpClient, string> NewMessageEvent;

        public Server(IPAddress ip, int port, int maxConcurrentClients = 2000, int backlog = 1024)
        {
            _listener = new TcpListener(ip, port);
            _concurrency = new SemaphoreSlim(maxConcurrentClients);
            Backlog = backlog;
        }

        public void Start()
        {
            _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _listener.Start(Backlog);
            _ = AcceptLoopAsync(_cts.Token);
        }

        public void Stop() => _cts.Cancel();
        private async Task AcceptLoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var tcp = await _listener.AcceptTcpClientAsync(ct);
                    tcp.NoDelay = true;
                    tcp.ReceiveBufferSize = 64 * 1024;
                    tcp.SendBufferSize = 64 * 1024;

                    await _concurrency.WaitAsync(ct);
                    var handler = new ClientHandler(tcp, this);
                    lock (_sync) _clients.Add(handler);
                    _ = handler.ProcessAsync(ct).ContinueWith(_ => _concurrency.Release(), TaskScheduler.Default);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            finally
            {
                _listener.Stop();
            }
        }

        internal void RemoveConnection(Guid id)
        {
            lock (_sync)
            {
                var idx = _clients.FindIndex(c => c.Id == id);
                if (idx >= 0) _clients.RemoveAt(idx);
            }
        }

        public async Task BroadcastAsync(string message, CancellationToken ct = default)
        {
            List<ClientHandler> snapshot;
            lock (_sync) snapshot = _clients.ToList();

            foreach (var ch in snapshot)
            {
                try
                {
                    using var wcts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    wcts.CancelAfter(TimeSpan.FromSeconds(5));
                    await Framing.WriteWithLengthAsync(ch.NetworkStream, message, wcts.Token);
                }
                catch { }
            }
        }

        public void SaveMessage(Guid clientId, TcpClient client, string message)
        {
            _messageDictionary.AddMessage(clientId, message);
            var remote = client.Client.RemoteEndPoint as IPEndPoint;
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(
                message: message,
                clientId: clientId,
                remote: remote));
        }

        public void Dispose()
        {
            _cts.Cancel();
            List<ClientHandler> snapshot;
            lock (_sync) snapshot = _clients.ToList();
            foreach (var c in snapshot) c.Close();
            _listener.Stop();
        }
    }
}
