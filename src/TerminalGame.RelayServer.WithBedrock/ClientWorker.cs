using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TerminalGame.RelayServer.Domain;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class ClientWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private ConnectionContext? _connection;

        public ClientWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            var client = new ClientBuilder(_serviceProvider)
                        .UseSockets()
                        .UseConnectionLogging("Client")
                        .Build();

            _connection = await client.ConnectAsync(new IPEndPoint(IPAddress.IPv6Loopback, 530), _hostApplicationLifetime.ApplicationStopping);

            if (_connection == null)
            {
                return;
            }

            //*/
            var protocol = new MyClientProtocol(_connection);
            await protocol.SendAsync(new InitMessage("1"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload0"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload1"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload2"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload3"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload4"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload5"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload6"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload7"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload8"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload9"));
            /*/
            await RawSendAsync(_connection);
            //*/
        }

        private async Task RawSendAsync(ConnectionContext connection)
        {
            var lines = new string[]
            {
                "{\"payloadType\":\"INIT\",\"source\":\"1\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}",
            };


            foreach (var line in lines)
            {
                WriteLineHeader(connection, line);
                WriteLine(connection, Encoding.UTF8, line);
            }

            await connection.Transport.Output.FlushAsync(_hostApplicationLifetime.ApplicationStopping);

            static void WriteLineHeader(ConnectionContext connection, string line)
            {
                var sizeSpan = connection.Transport.Output.GetSpan(4);
                BinaryPrimitives.WriteInt32BigEndian(sizeSpan, line.Length);

                connection.Transport.Output.Write(sizeSpan[..4]);
            }


            static void WriteLine(ConnectionContext connection, Encoding encoding, string line)
            {
                var length = encoding.GetByteCount(line);
                var payloadSpan = connection.Transport.Output.GetSpan(length);
                encoding.GetBytes(line, payloadSpan);
                connection.Transport.Output.Write(payloadSpan[..length]);
            }
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
