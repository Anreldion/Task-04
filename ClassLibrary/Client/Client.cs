using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Messenger.Client
{
    /// <summary>
    /// Client-side class
    /// </summary>
    public class Client : NetworkFields, INewMessage
    {
        /// <summary>
        /// Provides the underlying stream of data for network access.
        /// </summary>
        private static NetworkStream NetworkStream { get; set; }

        public event Action<TcpClient, string> NewMessageEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> 
        /// </summary>
        /// <param name="local_ip">Local Internet Protocol (IP) address.</param>
        /// <param name="port">Local Port address.</param>
        public Client(IPAddress local_ip, int port)
        {
            LocalIpAddress = local_ip;
            LocalPort = port;

            Client = new TcpClient();
            Client.Connect(LocalIpAddress, port);
            NetworkStream = Client.GetStream();

            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
        }
        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            var data = Encoding.Unicode.GetBytes(message);
            NetworkStream.Write(data, 0, data.Length);
        }
        /// <summary>
        /// Receive message from server
        /// </summary>
        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    var data = new byte[RxDBufferSize];
                    var builder = new StringBuilder();
                    do
                    {
                        var count = NetworkStream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, count));
                    }
                    while (NetworkStream.DataAvailable);
                    NewMessageEvent?.Invoke(Client, builder.ToString());
                }
                catch
                {
                    Disconnect();
                }
            }
        }
        /// <summary>
        /// Close connection
        /// </summary>
        static void Disconnect()
        {
            if (NetworkStream != null)
            {
                NetworkStream.Close();
            }
            if (Client != null)
            {
                Client.Close();
            }

            Environment.Exit(0);
        }

        /// <inheritdoc 
        /// cref="object.ToString"
        /// />
        public override string ToString()
        {
            return $"Type: {GetType().Name}, IP: {LocalIpAddress}, port: {LocalPort}";
        }

        /// <inheritdoc 
        /// cref="object.Equals(object)"
        /// />
        public override bool Equals(object obj)
        {
            return obj is Client client && LocalIpAddress == client.LocalIpAddress && LocalPort == client.LocalPort;
        }

        /// <inheritdoc 
        /// cref="object.GetHashCode"
        /// />
        public override int GetHashCode() => HashCode.Combine(Client, NetworkStream, LocalIpAddress, LocalPort);

    }
}
