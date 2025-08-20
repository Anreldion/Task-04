using Messenger.Services.Interfaces;
using Messenger.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Messenger.Services
{
    /// <summary>
    /// Server side class
    /// </summary>
    public class ServerService : INewMessage
    {
        private readonly TcpClient _client;
        private readonly TcpListener _listener;
        private readonly List<ClientHandler> _clientHandlerList = [];
        private readonly ClientMessageDictionary _messageDictionary = new();
        private readonly object _sync = new();


        public event Action<TcpClient, string> NewMessageEvent;

        public ServerService(IPAddress ipAddress, int port, int maxConcurrent = 1000)
        {
            _listener = new TcpListener(ipAddress, port);
            _listener.Start();

            var listenThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        var tcpClient = _listener.AcceptTcpClient();
                        var clientHandler = new ClientHandler(tcpClient, this);

                        lock (_sync)
                            _clientHandlerList.Add(clientHandler);

                        var clientThread = new Thread(clientHandler.Process)
                        {
                            IsBackground = true
                        };
                        clientThread.Start();
                    }
                }
                catch (SocketException)
                {
                    
                }
                finally
                {
                    Disconnect();
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        /// <summary>
        /// Remove client from collection
        /// </summary>
        /// <param name="id"></param>
        protected internal void RemoveConnection(Guid id)
        {
            lock (_sync)
            {
                var client = _clientHandlerList.FirstOrDefault(c => c.Id == id);
                if (client != null)
                    _clientHandlerList.Remove(client);
            }
        }

        /// <summary>
        /// Send message for all clients
        /// </summary>
        /// <param name="message">broadcast message</param>
        public void BroadcastMessage(string message)
        {
            var data = Encoding.Unicode.GetBytes(message);

            List<ClientHandler> snapshot;
            lock (_sync)
                snapshot = _clientHandlerList.ToList();

            foreach (var ch in snapshot)
            {
                try
                {
                    ch.NetworkStream.Write(data, 0, data.Length);
                }
                catch
                {
                    //ignore 
                }
            }
        }

        /// <summary>
        /// Save message from client
        /// </summary>
        /// <param name="guid">TcpClient</param>
        /// <param name="message">ClientService message</param>
        public void SaveMessage(Guid guid, string message)
        {
            _messageDictionary.AddMessage(guid, message);
            NewMessageEvent?.Invoke(_client, message);
        }

        /// <summary>
        /// Stopping the server and disconnecting all clients
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _listener.Stop();
            }
            catch
            {
                //ignore
            }

            List<ClientHandler> snapshot;
            lock (_sync)
                snapshot = _clientHandlerList.ToList();

            foreach (var ch in snapshot)
                ch.Close();
        }

        public override string ToString() => $"Type: {GetType().Name}";
    }
}
