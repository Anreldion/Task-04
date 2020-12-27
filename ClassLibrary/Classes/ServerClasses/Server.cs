using Messenger.ServerClasses.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Messenger.Classes.ServerClasses
{
    /// <summary>
    /// Server-side class
    /// </summary>
    public class Server : NetworkFields, INewMassage
    {
        /// <summary>
        /// Private field for ClientId property.
        /// </summary>
        private int clientId = 0;
        /// <summary>
        /// An identifier for new connections.
        /// </summary>
        private int ClientId => ++clientId;

        /// <summary>
        /// List for storing information about all connected clients.
        /// </summary>
        List<ClientHandler> clientHandlerList = new List<ClientHandler>(); // все подключения

        /// <summary>
        /// Message lists from each client.
        /// </summary>
        public ClientMessageDictionary messageDictionary = new ClientMessageDictionary();

        ///<inheritdoc cref="INewMassage.MessageRecived"/>
        public event Action<TcpClient, string> NewMassageEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/ class> 
        /// </summary>
        /// <param name="local_ip">Local Internet Protocol (IP) address.</param>
        /// <param name="port">Local Port address.</param>
        public Server(IPAddress local_ip, int port)
        {
            LocalIPAddress = local_ip;
            LocalPort = port;
            Listener = new TcpListener(LocalIPAddress, LocalPort);
            Listener.Start();
            Thread listenThread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    while (true)
                    {
                        TcpClient tcpClient = Listener.AcceptTcpClient();
                        ClientHandler clientHandler = new ClientHandler(ClientId, tcpClient, this);

                        clientHandlerList.Add(clientHandler);
                        Thread clientThread = new Thread(new ThreadStart(clientHandler.Process));
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
            ClientHandler client = clientHandlerList.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clientHandlerList.Remove(client);
        }

        /// <summary>
        /// Send broadcast message
        /// </summary>
        /// <param name="message">broadcast message</param>
        public void BroadcastMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clientHandlerList.Count; i++)
            {
                clientHandlerList[i].Network_stream.Write(data, 0, data.Length);       
            }
        }

        /// <summary>
        /// Save message from client
        /// </summary>
        /// <param name="tcpClient">TcpClient</param>
        /// <param name="message">Client message</param>
        public void SaveMessage(TcpClient tcpClient, string message)
        {
            messageDictionary.AddMessage(tcpClient, message);
            NewMassageEvent?.Invoke(Client, message);
        }

        /// <summary>
        /// Stopping the server and disconnecting all clients
        /// </summary>
        public void Disconnect()
        {
            Listener.Stop();

            for (int i = 0; i < clientHandlerList.Count; i++)
            {
                clientHandlerList[i].Close();
            }
            Environment.Exit(0);
        }

        /// <inheritdoc 
        /// cref="object.ToString"
        /// />
        public override string ToString()
        {
            return String.Format("Type: {0}, IP: {1}, port: {2}", GetType().Name, LocalIPAddress, LocalPort);
        }

        /// <inheritdoc 
        /// cref="object.Equals(object)"
        /// />
        public override bool Equals(object obj)
        {
            return obj is Server server && LocalIPAddress == server.LocalIPAddress && LocalPort == server.LocalPort;
        }

        /// <inheritdoc 
        /// cref="object.GetHashCode"
        /// />
        public override int GetHashCode() => HashCode.Combine(Listener, LocalIPAddress, LocalPort);

    }
}
