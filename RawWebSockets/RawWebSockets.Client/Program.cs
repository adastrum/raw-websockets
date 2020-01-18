using RawWebSockets.Common;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace RawWebSockets.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = Guid.NewGuid().ToString();
            var uri = new Uri($"ws://localhost:5000?clientid={clientId}");
            var clientWebSocket = new ClientWebSocket();
            var cancellationToken = default(CancellationToken);

            await clientWebSocket.ConnectAsync(uri, cancellationToken);

            var webSocketService = new WebSocketService();

            await Task.WhenAll
            (
                SendAsync(clientWebSocket, webSocketService, cancellationToken),
                ReceiveAsync(clientWebSocket, webSocketService, cancellationToken)
            );
        }

        private static async Task SendAsync(ClientWebSocket clientWebSocket, WebSocketService webSocketService, CancellationToken cancellationToken)
        {
            while (true)
            {
                var message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                {
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);

                    return;
                }

                await webSocketService.SendAsync(clientWebSocket, message, cancellationToken);
            }
        }

        private static async Task ReceiveAsync(ClientWebSocket clientWebSocket, WebSocketService webSocketService, CancellationToken cancellationToken)
        {
            while (true)
            {
                var message = await webSocketService.ReceiveAsync(clientWebSocket, cancellationToken);

                if (string.IsNullOrWhiteSpace(message))
                {
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);

                    return;
                }

                Console.WriteLine(message);
            }
        }
    }
}
