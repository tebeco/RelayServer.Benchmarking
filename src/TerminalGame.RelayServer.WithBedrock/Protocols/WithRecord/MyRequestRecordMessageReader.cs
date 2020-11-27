using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Internal;
using System;
using System.Buffers;
using System.IO;
using System.Text.Json;

namespace TerminalGame.RelayServer.WithBedrock.WithRecord
{
    public class MyRequestRecordMessageReader : IMessageReader<MyRequestRecordMessage>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out MyRequestRecordMessage message)
        {
            var sequenceReader = new SequenceReader<byte>(input);
            if (!sequenceReader.TryReadBigEndian(out int length) || input.Length < length)
            {
                message = default!;
                return false;
            }

            bool completed = false;
            string payloadType = default!;
            string source = default!;
            string destination = default!;
            string payload = default!;

            var rawPayload = input.Slice(sequenceReader.Position, length);
            var reader = new Utf8JsonReader(rawPayload, isFinalBlock: true, state: default);
            reader.CheckRead();

            // We're always parsing a JSON object
            reader.EnsureObjectStart();

            do
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if (reader.ValueTextEquals(MyRequestRecordMessageWriter.PayloadTypePropertyNameBytes.EncodedUtf8Bytes))
                        {
                            payloadType = reader.ReadAsString(MyRequestRecordMessageWriter.PayloadTypePropertyName)
                                            ?? throw new InvalidDataException($"Expected '{MyRequestRecordMessageWriter.PayloadTypePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestRecordMessageWriter.SourcePropertyNameBytes.EncodedUtf8Bytes))
                        {
                            source = reader.ReadAsString(MyRequestRecordMessageWriter.SourcePropertyName)
                                        ?? throw new InvalidDataException($"Expected '{MyRequestRecordMessageWriter.SourcePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestRecordMessageWriter.DestinationPropertyNameBytes.EncodedUtf8Bytes))
                        {
                            destination = reader.ReadAsString(MyRequestRecordMessageWriter.DestinationPropertyName)
                                            ?? throw new InvalidDataException($"Expected '{MyRequestRecordMessageWriter.DestinationPropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestRecordMessageWriter.PayloadPropertyNameBytes.EncodedUtf8Bytes))
                        {
                            payload = reader.ReadAsString(MyRequestRecordMessageWriter.PayloadPropertyName)
                                        ?? throw new InvalidDataException($"Expected '{MyRequestRecordMessageWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else
                        {
                            reader.CheckRead();
                            reader.Skip();
                        }
                        break;
                    case JsonTokenType.EndObject:
                        completed = true;
                        break;
                }
            }
            while (!completed && reader.CheckRead());

            consumed = rawPayload.End;
            examined = consumed;

            message = payloadType switch
            {
                "INIT" => new InitRecordMessage(source),
                "MESSAGE" => new PayloadRecordMessage(source, destination, payload),
                _ => throw new InvalidDataException($"Expected '{MyRequestRecordMessageWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.")
            };
            return message != null;
        }
    }
}