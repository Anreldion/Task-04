using System;
using System.Net.Sockets;

namespace Messenger.Services.Interfaces
{
    /// <summary>
    /// Interface allowing subscription to a new message event.
    /// </summary>
    public interface INewMessage
    {
        /// <summary>
        /// Event allowing subscription to a new message.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
