using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RawWebSockets.Common
{
    public static class WebSocketExtensions
    {
        private const int _bufferSize = 1024 * 4;

        public static async Task<string> ReceiveAsync(this WebSocket webSocket, CancellationToken cancellationToken)
        {
            var messageBytes = new List<byte>();
            var bufferBytes = new byte[_bufferSize];
            WebSocketReceiveResult webSocketReceiveResult;

            do
            {
                var buffer = new ArraySegment<byte>(bufferBytes);

                try
                {
                    webSocketReceiveResult = await webSocket.ReceiveAsync(buffer, cancellationToken);
                }
                catch (WebSocketException ex)
                {
                    //todo

                    return null;
                }

                var receivedBytes = new ArraySegment<byte>(bufferBytes, 0, webSocketReceiveResult.Count);
                messageBytes.AddRange(receivedBytes);
            } while (!webSocketReceiveResult.EndOfMessage && webSocketReceiveResult.MessageType != WebSocketMessageType.Close);

            var message = Encoding.UTF8.GetString(messageBytes.ToArray());

            return message;
        }

        public static async Task SendAsync(this WebSocket webSocket, string message, CancellationToken cancellationToken)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            for (var bytesSent = 0; bytesSent < messageBytes.Length; bytesSent += _bufferSize)
            {
                var bytesLeft = messageBytes.Length - bytesSent;
                var count = bytesLeft < _bufferSize ? bytesLeft : _bufferSize;
                var buffer = new ArraySegment<byte>(messageBytes, bytesSent, count);

                try
                {
                    await webSocket.SendAsync(buffer, WebSocketMessageType.Text, false, cancellationToken);
                }
                catch (WebSocketException ex)
                {
                    //todo

                    return;
                }
            }

            await webSocket.SendAsync(ArraySegment<byte>.Empty, WebSocketMessageType.Text, true, cancellationToken);
        }
    }
}
