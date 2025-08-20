using System;
using System.Net.Sockets;
using System.Text;
using Messenger.Services;

namespace Messenger.Tools
{
    /// <summary>
    /// Handling client messages and closing connection if lost class.
    /// </summary>
    public class ClientHandler
    {
        /// <summary>
        /// Array size for received data.
        /// </summary>
        protected const int RxDBufferSize = 64;

        /// <summary>
        /// ClientService. Provides client connections for TCP network services.
        /// </summary>
        protected static TcpClient Client { get; set; }

        /// <summary>
        /// Server. Listens for connections from TCP network clients.
        /// </summary>
        protected static TcpListener Listener { get; set; }
        /// <summary>
        /// ClientService identifier
        /// </summary>
        protected internal Guid Id { get; }

        /// <summary>
        /// Provides the underlying stream of data for network access.
        /// </summary>
        protected internal NetworkStream NetworkStream { get; private set; }

        /// <summary>
        /// Server object 
        /// </summary>
        private readonly ServerService _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientHandler"/>
        /// </summary>
        /// <param name="tcpClient">TcpClient link</param>
        /// <param name="serverObject">Server link</param>
        public ClientHandler(TcpClient tcpClient, ServerService serverObject)
        {
            Id = Guid.NewGuid();
            Client = tcpClient;
            _server = serverObject;
        }

        /// <summary>
        /// The process of receiving new messages and closing the connection if lost.
        /// </summary>
        public void Process()
        {
            try
            {
                NetworkStream = Client.GetStream();
                while (true)
                {
                    try
                    {
                        var message = GetMessage();
                        _server.SaveMessage(Id, message);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        /// <summary>
        /// Reading an incoming message and converting to a string
        /// </summary>
        /// <returns>Message</returns>
        private string GetMessage()
        {
            var data = new byte[RxDBufferSize]; 
            var builder = new StringBuilder();
            do
            {
                var count = NetworkStream.Read(data, 0, data.Length);
                if (count == 0) break;
                builder.Append(Encoding.Unicode.GetString(data, 0, count));
            }
            while (NetworkStream.DataAvailable);
            return builder.ToString();
        }

        /// <summary>
        /// Close connection
        /// </summary>
        protected internal void Close()
        {
            try { NetworkStream?.Close(); } catch { }
            try { Client?.Close(); } catch { }
        }
    }

}
