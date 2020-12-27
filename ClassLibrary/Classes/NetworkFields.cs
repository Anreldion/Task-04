using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Messenger.Classes
{
    /// <summary>
    /// Abstract class describing server-side and client-side fields
    /// </summary>
    public abstract class NetworkFields
    {
        /// <summary>
        /// Provides an Local Internet Protocol (IP) address.
        /// </summary>
        protected IPAddress LocalIPAddress { get; set; }

        /// <summary>
        /// Provides an Local Port address.
        /// </summary>
        protected int LocalPort { get; set; }

        /// <summary>
        /// Array size for receiving data.
        /// </summary>
        protected const int RxDBufferSize = 64;

        /// <summary>
        /// Client. Provides client connections for TCP network services.
        /// </summary>
        protected static TcpClient Client { get; set; }

        /// <summary>
        /// Server. Listens for connections from TCP network clients.
        /// </summary>
        protected static TcpListener Listener { get; set; } // сервер для прослушивания
    }
}
