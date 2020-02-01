using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace RawWebSockets.Common
{
    public interface IWebSocketConnectionManager
    {
        void AddConnection(string clientId, WebSocket webSocket);

        Task CloseConnectionAsync(string clientId, CancellationToken cancellationToken = default);

        Task SendAsync(string clientId, string message, CancellationToken cancellationToken = default);
    }
}