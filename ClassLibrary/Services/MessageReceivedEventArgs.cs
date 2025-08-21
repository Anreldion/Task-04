using System;
using System.Net;
using System.Net.Sockets;

namespace Messenger.Services
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        public Guid? ClientId { get; }
        public IPEndPoint? RemoteEndPoint { get; }
        public string Message { get; }
        public string? Transliterated { get; }
        public DateTimeOffset ReceivedAt { get; }

        public MessageReceivedEventArgs(
            string message,
            Guid? clientId = null,
            IPEndPoint? remote = null,
            string? transliterated = null,
            DateTimeOffset? receivedAt = null)
        {
            Message = message;
            ClientId = clientId;
            RemoteEndPoint = remote;
            Transliterated = transliterated;
            ReceivedAt = receivedAt ?? DateTimeOffset.UtcNow;
        }
    }
}
