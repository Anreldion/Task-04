using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Messenger.ServerClasses.Tools
{
    /// <summary>
    /// Client message dictionary
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

        /// <summary>
        /// Get a list of messages.
        /// </summary>
        /// <param name="tcpClient">Provides client connections for TCP network services.</param>
        /// <returns>List of messages</returns>
        public List<string> GetMessages(TcpClient tcpClient)
        {
            List<string> list = new List<string>();
            if(TryGetValue(tcpClient, out var value))
            {
                foreach (var item in value)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Save messages to file.
        /// </summary>
        /// <param name="name">File name</param>
        public void ToFile(string name)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(name, false, Encoding.Default))
                {
                    foreach (List<string> list in this.Values)
                    {
                        foreach (string item in list)
                        {
                            sw.WriteLine(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
