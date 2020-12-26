using System;
using System.Net.Sockets;
using System.Text;

namespace Messenger.Classes.ServerClasses
{
    public class ClientHandler : NetworkFields
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Network_stream { get; private set; }
        //TcpClient tcpClient;
        Server server; // объект сервера
        //protected const int RxDBufferSize = 64;

        public event Action<TcpClient, string> RxDMessageEvent;

        //public delegate void newMassageHandler(TcpClient tcpClient, string message);
        //public event newMassageHandler newMassageEvent;

        public event Action<TcpClient, string> NewMassageEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="tcpClient"></param>
        /// <param name="serverObject"></param>
        public ClientHandler(int clientId, TcpClient tcpClient, Server serverObject)
        {
            Id = clientId.ToString();
            NetworkFields.Client = tcpClient;
            server = serverObject;
        }

        /// <summary>
        /// 
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
                        NewMassageEvent?.Invoke(Client, message);
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
