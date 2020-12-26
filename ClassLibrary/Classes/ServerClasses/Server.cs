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
    public class Server : NetworkFields
    {
        private int clientId = 0;
        private int ClientId => ++clientId;

        List<ClientHandler> clientHandlerList = new List<ClientHandler>(); // все подключения
        
        /// <summary>
        /// 
        /// </summary>
        public ClientMessageDictionary messageDictionary = new ClientMessageDictionary();


        public event Action<TcpClient, string> NewMassageEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_addr"></param>
        /// <param name="port"></param>
        public Server(IPAddress local_addr, int port)
        {
            LocalIPAddress = local_addr;
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

                        clientHandler.NewMassageEvent+= (client, message) =>
                        {
                            NewMassageEvent?.Invoke(client, message);
                        };

                        //clientHandler.RxDMessageEvent += (client, message) => 
                        //{
                        //    RxDMessageEvent?.Invoke(client, message);
                        //};
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
            listenThread.Start(); //старт потока
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
    }
}
