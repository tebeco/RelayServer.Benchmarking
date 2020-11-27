using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using System.Threading;
using System.Threading.Tasks;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyClientProtocol<TMessageWritter, TMessage>
        where TMessageWritter : IMessageWriter<TMessage>, new()
    {
        private readonly ConnectionContext _connection;
        private readonly ProtocolReader _reader;
        private readonly TMessageWritter _messageWriter;

        public MyClientProtocol(ConnectionContext connection)
        {
            _connection = connection;
            _reader = connection.CreateReader();

            _messageWriter = new TMessageWritter();
        }

        public async ValueTask SendAsync(TMessage requestMessage, CancellationToken cancellationToken)
        {
            // Write request message length
            _messageWriter.WriteMessage(requestMessage, _connection.Transport.Output);

            await _connection.Transport.Output.FlushAsync(cancellationToken);
        }
    }
}
