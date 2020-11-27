using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyCustomProtocol<TMessageReader, TMessage> : ConnectionHandler
        where TMessageReader : IMessageReader<TMessage>, new()
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public MyCustomProtocol(ILogger<MyCustomProtocol<TMessageReader, TMessage>> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            // Use a length prefixed protocol
            var protocol = new TMessageReader();
            var reader = connection.CreateReader();

            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                try
                {
                    var result = await reader.ReadAsync(protocol);
                    var message = result.Message;

                    int? payloadLength = message switch
                    {
                        PayloadRecordMessage msg => msg.Payload.Length,
                        PayloadStructMessage msg => msg.Payload.Length,
                        _ => null
                    };

                    if (payloadLength is not null)
                    {
                        _logger.LogInformation("Received a {MessageType} with an inner payload of {Length} bytes", message.GetType().Name, payloadLength);
                    }

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    reader.Advance();
                }
            }
        }
    }
}
