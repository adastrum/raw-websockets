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

            await Task.WhenAll
            (
                SendAsync(clientWebSocket, cancellationToken),
                ReceiveAsync(clientWebSocket, cancellationToken)
            );
        }

        private static async Task SendAsync(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
        {
            while (true)
            {
                var message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                {
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);

                    return;
                }

                await clientWebSocket.SendAsync(message, cancellationToken);
            }
        }

        private static async Task ReceiveAsync(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
        {
            while (true)
            {
                var message = await clientWebSocket.ReceiveAsync(cancellationToken);

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
