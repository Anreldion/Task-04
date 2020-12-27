using System;
using System.Net.Sockets;

namespace Messenger.Classes
{
    /// <summary>
    /// Interface allowing subscription to a new massage event.
    /// </summary>
    public interface INewMassage
    {
        /// <summary>
        /// Event allowing subscription to a new massage.
        /// </summary>
        event Action<TcpClient, string> NewMassageEvent;
    }
}
