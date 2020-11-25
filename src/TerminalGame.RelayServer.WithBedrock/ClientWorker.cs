using Bedrock.Framework;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class ClientWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var client = new ClientBuilder(_serviceProvider)
                        .UseSockets()
                        .UseConnectionLogging()
                        .Build();

            await using var connection = await client.ConnectAsync(new IPEndPoint(IPAddress.IPv6Loopback, 530), stoppingToken);

            if (connection == null)
            {
                return;
            }

            //var fooProtocol = new MyCustomProtocol(connection);
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    var response = await fooProtocol.SendAsync(request);

            //}
        }
    }
}