using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Messenger.Tools
{
    public class ClientMessageDictionary
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentBag<string>> _messages = new();
        
        public void AddMessage(Guid guid, string message)
        {
            var bag = _messages.GetOrAdd(guid, _ => []);
            bag.Add(message);
        }

        public List<string> GetMessages(Guid guid)
        {
            return _messages.TryGetValue(key: guid, out var bag) ? bag.ToList() : [];
        }
    }
}
