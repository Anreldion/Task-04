using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Messenger.Classes.ClientClasses
{
    /// <summary>
    /// Client-side class
    /// </summary>
    public class Client : NetworkFields, INewMassage
    {
        /// <summary>
        /// Provides the underlying stream of data for network access.
        /// </summary>
        protected static NetworkStream Network_stream { get; private set; }

        ///<inheritdoc cref="INewMassage.MessageRecived"/>
        public event Action<TcpClient, string> NewMassageEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/ class> 
        /// </summary>
        /// <param name="local_ip">Local Internet Protocol (IP) address.</param>
        /// <param name="port">Local Port address.</param>
        public Client(IPAddress local_ip, int port)
        {
            LocalIPAddress = local_ip;
            LocalPort = port;

            Client = new TcpClient();
            Client.Connect(LocalIPAddress, port);
            Network_stream = Client.GetStream();

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }
        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Network_stream.Write(data, 0, data.Length);
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
                    byte[] data = new byte[RxDBufferSize];
                    StringBuilder builder = new StringBuilder();
                    do
                    {
                        int count = Network_stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, count));
                    }
                    while (Network_stream.DataAvailable);
                    NewMassageEvent?.Invoke(Client, builder.ToString());
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
            if (Network_stream != null)
            {
                Network_stream.Close();
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
            return String.Format("Type: {0}, IP: {1}, port: {2}", GetType().Name, LocalIPAddress, LocalPort);
        }

        /// <inheritdoc 
        /// cref="object.Equals(object)"
        /// />
        public override bool Equals(object obj)
        {
            return obj is Client client && LocalIPAddress == client.LocalIPAddress && LocalPort == client.LocalPort;
        }

        /// <inheritdoc 
        /// cref="object.GetHashCode"
        /// />
        public override int GetHashCode() => HashCode.Combine(Client, Network_stream, LocalIPAddress, LocalPort);

    }
}
