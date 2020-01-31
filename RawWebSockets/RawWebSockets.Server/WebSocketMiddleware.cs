using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RawWebSockets.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RawWebSockets.Server
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<WebSocketMiddleware> _logger;

        public WebSocketMiddleware(RequestDelegate next, ILogger<WebSocketMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                //todo
                var clientId = context.Request.Query["clientid"];

                var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                var cancellationToken = context.RequestAborted;

                string message;
                do
                {
                    message = await webSocket.ReceiveAsync(cancellationToken);

                    await HandleMessageAsync(message, cancellationToken);
                } while (!string.IsNullOrWhiteSpace(message));
            }
            else
            {
                _logger.LogWarning("Not a WebSocket request");
            }
        }

        private async Task HandleMessageAsync(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling message: {message}");

            //todo
        }
    }
}
