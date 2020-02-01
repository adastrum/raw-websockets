using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace RawWebSockets.Common
{
    public class WebSocketConnectionManager : IWebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections;

        public WebSocketConnectionManager()
        {
            _connections = new ConcurrentDictionary<string, WebSocket>();
        }

        public void AddConnection(string clientId, WebSocket webSocket)
        {
            _connections.TryAdd(clientId, webSocket);
        }

        public async Task CloseConnectionAsync(string clientId, CancellationToken cancellationToken = default)
        {
            if (_connections.TryRemove(clientId, out var webSocket))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            }
        }

        public async Task SendAsync(string clientId, string message, CancellationToken cancellationToken = default)
        {
            if (_connections.TryGetValue(clientId, out var webSocket))
            {
                await webSocket.SendAsync(message, cancellationToken);
            }
            else
            {
                //todo
            }
        }
    }
}
