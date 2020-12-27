using System;
using System.Net.Sockets;
using System.Text;

namespace Messenger.Classes.ServerClasses
{
    /// <summary>
    /// Handling client messages and closing connection if lost class.
    /// </summary>
    public class ClientHandler : NetworkFields
    {
        /// <summary>
        /// Client identifier
        /// </summary>
        protected internal string Id { get; private set; }

        /// <summary>
        /// Provides the underlying stream of data for network access.
        /// </summary>
        protected internal NetworkStream Network_stream { get; private set; }

        /// <summary>
        /// Server object 
        /// </summary>
        Server server;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientHandler"/ class> 
        /// </summary>
        /// <param name="clientId">Unique identificator</param>
        /// <param name="tcpClient">TcpClient link</param>
        /// <param name="serverObject">Server link</param>
        public ClientHandler(int clientId, TcpClient tcpClient, Server serverObject)
        {
            Id = clientId.ToString();
            NetworkFields.Client = tcpClient;
            server = serverObject;
        }

        /// <summary>
        /// The process of receiving new messages and closing the connection if lost.
        /// </summary>
        public void Process()
        {
            try
            {
                Network_stream = Client.GetStream();
                while (true)
                {
                    try
                    {
                        string message = GetMessage();
                        server.SaveMessage(Client, message);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        /// <summary>
        /// Reading an incoming message and converting to a string
        /// </summary>
        /// <returns>Message</returns>
        private string GetMessage()
        {
            byte[] data = new byte[RxDBufferSize]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            do
            {
                int count = Network_stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, count));
            }
            while (Network_stream.DataAvailable);
            return builder.ToString();
        }

        /// <summary>
        /// Close connection
        /// </summary>
        protected internal void Close()
        {
            if (Network_stream != null)
            {
                Network_stream.Close();
            }
            if (Client != null)
            {
                Client.Close();
            }
        }
    }

}
