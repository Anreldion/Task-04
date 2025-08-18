using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Messenger.DTO;
using Messenger.Tools;

namespace Messenger.Server
{
    /// <summary>
    /// Server side class
    /// </summary>
    public class Server : NetworkFields, INewMessage
    {
        /// <summary>
        /// Private field for ClientId property.
        /// </summary>
        private int _clientId;
        /// <summary>
        /// An identifier for new connections.
        /// </summary>
        private int ClientId => ++_clientId;

        /// <summary>
        /// List for storing information about all connected clients.
        /// </summary>
        private readonly List<ClientHandler> _clientHandlerList = []; // все подключения

        /// <summary>
        /// Message lists from each client.
        /// </summary>
        public readonly ClientMessageDictionary MessageDictionary = new ClientMessageDictionary();

        
        public event Action<TcpClient, string> NewMessageEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> 
        /// </summary>
        /// <param name="local_ip">Local Internet Protocol (IP) address.</param>
        /// <param name="port">Local Port address.</param>
        public Server(IPAddress local_ip, int port)
        {
            LocalIpAddress = local_ip;
            LocalPort = port;
            Listener = new TcpListener(LocalIpAddress, LocalPort);
            Listener.Start();
            var listenThread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    while (true)
                    {
                        var tcpClient = Listener.AcceptTcpClient();
                        var clientHandler = new ClientHandler(ClientId, tcpClient, this);

                        _clientHandlerList.Add(clientHandler);
                        var clientThread = new Thread(new ThreadStart(clientHandler.Process));
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Disconnect();
                }
            }));
            listenThread.Start();
        }

        /// <summary>
        /// Remove client from collection
        /// </summary>
        /// <param name="id"></param>
        protected internal void RemoveConnection(string id)
        {
            var client = _clientHandlerList.FirstOrDefault(c => c.Id == id);
            if (client != null)
                _clientHandlerList.Remove(client);
        }

        /// <summary>
        /// Send message for all clients
        /// </summary>
        /// <param name="message">broadcast message</param>
        public void BroadcastMessage(string message)
        {
            var data = Encoding.Unicode.GetBytes(message);
            for (var i = 0; i < _clientHandlerList.Count; i++)
            {
                _clientHandlerList[i].NetworkStream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Save message from client
        /// </summary>
        /// <param name="tcpClient">TcpClient</param>
        /// <param name="message">Client message</param>
        public void SaveMessage(TcpClient tcpClient, string message)
        {
            MessageDictionary.AddMessage(tcpClient, message);
            NewMessageEvent?.Invoke(Client, message);
        }

        /// <summary>
        /// Stopping the server and disconnecting all clients
        /// </summary>
        public void Disconnect()
        {
            Listener.Stop();

            for (var i = 0; i < _clientHandlerList.Count; i++)
            {
                _clientHandlerList[i].Close();
            }
            Environment.Exit(0);
        }

        /// <inheritdoc  cref="object.ToString" />
        public override string ToString()
        {
            return $"Type: {GetType().Name}, IP: {LocalIpAddress}, port: {LocalPort}";
        }

        /// <inheritdoc  cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return obj is Server server && LocalIpAddress == server.LocalIpAddress && LocalPort == server.LocalPort;
        }

        /// <inheritdoc  cref="object.GetHashCode" />
        public override int GetHashCode() => HashCode.Combine(Listener, LocalIpAddress, LocalPort);

    }
}
