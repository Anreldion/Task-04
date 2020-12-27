using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Messenger.ServerClasses.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientMessageDictionary : Dictionary<TcpClient, List<string>>
    {
        /// <summary>
        /// Add message to dictionary
        /// </summary>
        /// <param name="tcpClient">Provides client connections for TCP network services.</param>
        /// <param name="message">Message line</param>
        public void AddMessage(TcpClient tcpClient, string message)
        {
            if (TryGetValue(tcpClient, out var value))
            {
                value.Add(message);
                return;
            }
            Add(tcpClient, new List<string> { message });
        }
    }
}
