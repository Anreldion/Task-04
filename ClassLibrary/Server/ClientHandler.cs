using System.Net.Sockets;
using System.Text;

namespace Messenger.Server
{
    /// <summary>
    /// Handling client messages and closing connection if lost class.
    /// </summary>
    public class ClientHandler : NetworkFields
    {
        /// <summary>
        /// Client identifier
        /// </summary>
        protected internal string Id { get; }

        /// <summary>
        /// Provides the underlying stream of data for network access.
        /// </summary>
        protected internal NetworkStream NetworkStream { get; private set; }

        /// <summary>
        /// Server object 
        /// </summary>
        private readonly Server _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientHandler"/>
        /// </summary>
        /// <param name="clientId">Unique id</param>
        /// <param name="tcpClient">TcpClient link</param>
        /// <param name="serverObject">Server link</param>
        public ClientHandler(int clientId, TcpClient tcpClient, Server serverObject)
        {
            Id = clientId.ToString();
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
                        _server.SaveMessage(Client, message);
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
            var data = new byte[RxDBufferSize]; // буфер для получаемых данных
            var builder = new StringBuilder();
            do
            {
                var count = NetworkStream.Read(data, 0, data.Length);
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
            if (NetworkStream != null)
            {
                NetworkStream.Close();
            }
            if (Client != null)
            {
                Client.Close();
            }
        }
    }

}
