using System;
using System.Text;

namespace TerminalGame.RelayServer.WithBedrock
{
    public static class MyRequestMessageExtensions
    {
        public static int GetMessageLength(this MyRequestRecordMessage message) =>
            message switch
            {
                InitRecordMessage msg => msg.GetMessageLength(),
                PayloadRecordMessage msg => msg.GetMessageLength(),
                _ => throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}")
            };

        public const string EmptyInitJsonMessage = "{\"payloadType\":\"INIT\",\"source\":\"\"}";
        public const string EmptyPayloadJsonMessage = "{\"payloadType\":\"MESSAGE\",\"destination\":\"\",\"source\":\"\",\"payload\":\"\"}";

        public static int GetMessageLength(this InitRecordMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Source);

            return EmptyInitJsonMessage.Length + sourceLength;
        }

        public static int GetMessageLength(this PayloadRecordMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Source);
            var destinationLength = Encoding.UTF8.GetByteCount(message.Destination);
            var payloadLength = Encoding.UTF8.GetByteCount(message.Payload);

            return EmptyPayloadJsonMessage.Length + sourceLength + destinationLength + payloadLength;
        }

        public static int GetMessageLength(this IStructMessage message) =>
            message switch
            {
                InitStructMessage msg => msg.GetMessageLength(),
                PayloadStructMessage msg => msg.GetMessageLength(),
                _ => throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}")
            };

        public static int GetMessageLength(this InitStructMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Source);

            return EmptyInitJsonMessage.Length + sourceLength;
        }

        public static int GetMessageLength(this PayloadStructMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Source);
            var destinationLength = Encoding.UTF8.GetByteCount(message.Destination);
            var payloadLength = Encoding.UTF8.GetByteCount(message.Payload);

            return EmptyPayloadJsonMessage.Length + sourceLength + destinationLength + payloadLength;
        }
    }
}
