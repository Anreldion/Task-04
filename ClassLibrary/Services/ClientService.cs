using Messenger.Services.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Messenger.Services
{

    public class ClientService : INewMessage
    {
        
        private const int RxDBufferSize = 64;
        private static TcpClient Client { get; set; }
        protected static TcpListener Listener { get; set; }
        private NetworkStream NetworkStream { get; set; }
        public event Action<TcpClient, string> NewMessageEvent;

        public ClientService(IPAddress ipAddress, int port)
        {
            Client = new TcpClient();
            Client.Connect(ipAddress, port);
            NetworkStream = Client.GetStream();

            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
        }

        public void SendMessage(string message)
        {
            var data = Encoding.Unicode.GetBytes(message);
            NetworkStream.Write(data, 0, data.Length);
        }

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
                        if (count == 0) { Disconnect(); return; }
                        builder.Append(Encoding.Unicode.GetString(data, 0, count));
                    }
                    while (NetworkStream.DataAvailable);

                    NewMessageEvent?.Invoke(Client, builder.ToString());
                }
                catch
                {
                    Disconnect();
                    return;
                }
            }
        }
        
        private void Disconnect()
        {
            try { NetworkStream?.Close(); } catch { }
            try { Client?.Close(); } catch { }
        }

        public override string ToString() => $"Type: {GetType().Name}";
    }
}
