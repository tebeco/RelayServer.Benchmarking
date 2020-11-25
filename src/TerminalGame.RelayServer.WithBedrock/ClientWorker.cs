using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class ClientWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private Client? _client;
        private ConnectionContext? _connection;

        public ClientWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            _client = new ClientBuilder(_serviceProvider)
                        .UseSockets()
                        .UseConnectionLogging("Client")
                        .Build();

            _connection = await _client.ConnectAsync(new IPEndPoint(IPAddress.IPv6Loopback, 530), _hostApplicationLifetime.ApplicationStopping);

            if (_connection == null)
            {
                return;
            }

            var payload = 
      "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
    ;
            var encoding = Encoding.UTF8;
            var length = encoding.GetByteCount(payload);
            var buffer = ArrayPool<byte>.Shared.Rent(length);

            await _connection.Transport.Output.WriteAsync(buffer, _hostApplicationLifetime.ApplicationStopping);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
