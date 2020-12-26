using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Messenger.Classes
{
    public abstract class NetworkFields
    {
        protected IPAddress LocalIPAddress { get; set; }
        protected int LocalPort { get; set; }

        protected const int RxDBufferSize = 64;
        //protected NetworkStream network_stream { get; set; }
        protected static TcpClient Client { get; set; }
        protected static TcpListener Listener { get; set; } // сервер для прослушивания

        


    }
}
