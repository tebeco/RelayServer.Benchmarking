using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Internal;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Text.Json;

namespace TerminalGame.RelayServer.WithBedrock.WithRecord
{
    public class MyRequestRecordMessageWriter : IMessageWriter<MyRequestRecordMessage>
    {
        public void WriteMessage(MyRequestRecordMessage message, IBufferWriter<byte> stream)
        {
            WriteHeaders(message, stream);
            WriteContent(message, stream);
        }

        private static void WriteHeaders(MyRequestRecordMessage message, IBufferWriter<byte> stream)
        {
            var size = MyRequestMessageExtensions.GetMessageLength(message);

            var sizeSpan = stream.GetSpan(4);
            BinaryPrimitives.WriteInt32BigEndian(sizeSpan, size);
            stream.Advance(4);
        }

        private static void WriteContent(MyRequestRecordMessage message, IBufferWriter<byte> stream)
        {
            var reusableWriter = ReusableUtf8JsonWriter.Get(stream);

            try
            {
                var writer = reusableWriter.GetJsonWriter();

                writer.WriteStartObject();

                switch (message)
                {
                    case InitRecordMessage m:
                        WriteInitMessage(m, writer);
                        break;
                    case PayloadRecordMessage m:
                        WritePayLoadMessage(m, writer);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}");
                }

                writer.WriteEndObject();
                writer.Flush();
                Debug.Assert(writer.CurrentDepth == 0);
            }
            finally
            {
                ReusableUtf8JsonWriter.Return(reusableWriter);
            }
        }

        private static void WriteInitMessage(InitRecordMessage message, Utf8JsonWriter writer)
        {
            WritePayloadType(message, writer);
            WriteSource(message, writer);
        }

        private static void WritePayLoadMessage(PayloadRecordMessage message, Utf8JsonWriter writer)
        {
            WritePayloadType(message, writer);
            WriteSource(message, writer);
            WriteDestination(message, writer);
            WritePayload(message, writer);
        }

        public const string PayloadTypePropertyName = "payloadType";
        public static readonly JsonEncodedText PayloadTypePropertyNameBytes = JsonEncodedText.Encode(PayloadTypePropertyName);
        public static readonly JsonEncodedText PayloadTypeInitPropertyValue = JsonEncodedText.Encode(MyPayloadTypeStrings.Init);
        public static readonly JsonEncodedText PayloadTypePayloadPropertyValue = JsonEncodedText.Encode(MyPayloadTypeStrings.Payload);
        private static void WritePayloadType(MyRequestRecordMessage message, Utf8JsonWriter writer)
        {
            var payloadType = message.PayloadType switch
            {
                MyPayloadType.Init => PayloadTypeInitPropertyValue,
                MyPayloadType.Payload => PayloadTypePayloadPropertyValue,
                _ => throw new InvalidOperationException($"Unsupported message type: {message.PayloadType}")
            };

            writer.WriteString(PayloadTypePropertyNameBytes, payloadType);
        }

        public const string SourcePropertyName = "source";
        public static readonly JsonEncodedText SourcePropertyNameBytes = JsonEncodedText.Encode(SourcePropertyName);
        private static void WriteSource(MyRequestRecordMessage message, Utf8JsonWriter writer)
        {
            writer.WriteString(SourcePropertyNameBytes, message.Source);
        }

        public const string DestinationPropertyName = "destination";
        public static readonly JsonEncodedText DestinationPropertyNameBytes = JsonEncodedText.Encode(DestinationPropertyName);
        private static void WriteDestination(PayloadRecordMessage message, Utf8JsonWriter writer)
        {
            writer.WriteString(DestinationPropertyNameBytes, message.Destination);
        }

        public const string PayloadPropertyName = "payload";
        public static readonly JsonEncodedText PayloadPropertyNameBytes = JsonEncodedText.Encode(PayloadPropertyName);
        private static void WritePayload(PayloadRecordMessage message, Utf8JsonWriter writer)
        {
            writer.WriteString(PayloadPropertyNameBytes, message.Payload);
        }
    }
}
