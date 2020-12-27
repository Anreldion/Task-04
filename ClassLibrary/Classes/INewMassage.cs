using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Messenger.Classes
{
    /// <summary>
    /// Interface that allows subscribe to an new massage event.
    /// </summary>
    public interface INewMassage
    {
        /// <summary>
        /// Event that allows subscribe to an new massage.
        /// </summary>
        event Action<TcpClient, string> NewMassageEvent;
    }
}
