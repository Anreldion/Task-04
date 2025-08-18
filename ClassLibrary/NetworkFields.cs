using System.Net;
using System.Net.Sockets;

namespace Messenger
{
    /// <summary>
    /// Abstract class describing server and client fields
    /// </summary>
    public abstract class NetworkFields
    {
        /// <summary>
        /// Provides a Local Internet Protocol (IP) address.
        /// </summary>
        protected IPAddress LocalIpAddress { get; set; }

        /// <summary>
        /// Provides a Local Port address.
        /// </summary>
        protected int LocalPort { get; set; }

        /// <summary>
        /// Array size for received data.
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
