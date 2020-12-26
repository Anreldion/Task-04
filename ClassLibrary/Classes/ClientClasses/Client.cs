using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Messenger.Classes.ClientClasses
{
    public class Client : NetworkFields
    {
        protected static NetworkStream Network_stream { get; private set; }
        public event Action<TcpClient, string> RxDMessageEvent;
        //public delegate void newMassageHandler(TcpClient tcpClient, string message);
        //public event newMassageHandler newMassageEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_addr"></param>
        /// <param name="port"></param>
        public Client(IPAddress local_addr, int port)
        {
            LocalIPAddress = local_addr;
            LocalPort = port;

            Client = new TcpClient();
            Client.Connect(LocalIPAddress, port);
            Network_stream = Client.GetStream();

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Network_stream.Write(data, 0, data.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[RxDBufferSize]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    do
                    {
                        int count = Network_stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, count));
                    }
                    while (Network_stream.DataAvailable);
                    RxDMessageEvent?.Invoke(Client, builder.ToString());
                }
                catch
                {
                    Disconnect();
                }
            }
        }
        /// <summary>
        /// 
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

            Environment.Exit(0); //завершение процесса
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
