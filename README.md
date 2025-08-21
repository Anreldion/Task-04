# Task-04
**Delegates. Events.**

## Structure

### `ClassLibrary` folder
Contains core classes and tools for server–client messaging.

- **Interfaces**
  - `INewMessage.cs` — interface that allows subscription to a new message event (`event EventHandler<MessageReceivedEventArgs>`).

- **Services**
  - `ClientService.cs` — client-side service. Connects to the server, sends and receives messages. Raises `MessageReceived` event with both original and transliterated text.
  - `Server.cs` — server-side service. Accepts clients, stores messages, broadcasts data, raises `MessageReceived` for subscribers.
  - `ClientHandler.cs` — represents one active client connection, processes its input and forwards messages to the server.
  - `MessageReceivedEventArgs.cs` — strongly typed event args for delivering messages (`ClientId`, `RemoteEndPoint`, `Message`, `Transliterated`).

- **Tools**
  - `Framing.cs` — length-prefixed framing for reliable message transmission.
  - `ClientMessageDictionary.cs` — thread-safe dictionary for storing messages per client (`ConcurrentDictionary<Guid, …>`).
  - `Transliterator.cs` — converts Russian text to Latin transcription according to a fixed map.

### `NUnitTests` folder
Unit and integration tests using **NUnit**:
- `TransliteratorTests.cs` — verifies transliteration rules.
- `ClientMessageDictionaryTests.cs` — checks safe add/get per client.
- `IntegrationTests.cs` — starts a server and client, sends/receives messages, validates event flow.

## Key Features
- Uses .NET event pattern (`EventHandler<T>`) instead of raw delegates.
- Events do **not** expose `TcpClient` directly — only `ClientId` and `IPEndPoint`.
- Client automatically provides both **original** and **transliterated** message in event args.
- Supports multiple concurrent clients with connection limit.
- Safe message framing with prefix length (up to 4 MB).
- Thread-safe storage of per-client messages.
- NUnit test coverage (unit + integration).

## Example: subscribing to events
```csharp
var server = new Server(IPAddress.Loopback, 9001);

server.MessageReceived += (s, e) =>
{
    Console.WriteLine($"[Server] {e.ClientId} {e.RemoteEndPoint}: {e.Message}");
};

var client = new ClientService();
client.MessageReceived += (s, e) =>
{
    Console.WriteLine($"[Client] {e.Message} | translit: {e.Transliterated}");
};