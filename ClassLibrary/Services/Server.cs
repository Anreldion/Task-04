using Messenger.Services.Interfaces;
using Messenger.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        
        private readonly ClientMessageDictionary _messageDictionary = new();
        public event Action<TcpClient, string> NewMessageEvent;

        public Server(IPAddress ip, int port)
        {
            _listener = new TcpListener(ip, port);
        }

        public void Start(int backlog = 1024)
        {
            _listener.Start(backlog);
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
                    var handler = new ClientHandler(tcp, this);
                    lock (_sync) _clients.Add(handler);
                    _ = handler.ProcessAsync(ct); 
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
            var data = Encoding.Unicode.GetBytes(message);
            List<ClientHandler> snapshot;
            lock (_sync) snapshot = _clients.ToList();

            foreach (var ch in snapshot)
            {
                try
                {
                    await ch.NetworkStream.WriteAsync(data, 0, data.Length, ct);
                }
                catch
                {
                    //ignore
                }
            }
        }

        public void SaveMessage(Guid guid, string message)
        {
            _messageDictionary.AddMessage(guid, message);
            NewMessageEvent?.Invoke(_client, message);
        }

        public void Dispose()
        {
            _cts.Cancel();
            List<ClientHandler> snapshot;
            lock (_sync) snapshot = _clients.ToList();
            foreach (var c in snapshot) c.Close();
            _listener.Stop();
        }

        public override string ToString() => $"Type: {GetType().Name}";
    }
}
