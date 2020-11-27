using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Internal;
using System;
using System.Buffers;
using System.IO;
using System.Text.Json;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyRequestStructMessageReader : IMessageReader<IStructMessage>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out IStructMessage message)
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
                        if (reader.ValueTextEquals(MyRequestStructMessageWriter.PayloadTypePropertyNameBytes.EncodedUtf8Bytes))
                        {
                            payloadType = reader.ReadAsString(MyRequestStructMessageWriter.PayloadTypePropertyName)
                                            ?? throw new InvalidDataException($"Expected '{MyRequestStructMessageWriter.PayloadTypePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestStructMessageWriter.SourcePropertyNameBytes.EncodedUtf8Bytes))
                        {
                            source = reader.ReadAsString(MyRequestStructMessageWriter.SourcePropertyName)
                                        ?? throw new InvalidDataException($"Expected '{MyRequestStructMessageWriter.SourcePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestStructMessageWriter.DestinationPropertyNameBytes.EncodedUtf8Bytes))
                        {
                            destination = reader.ReadAsString(MyRequestStructMessageWriter.DestinationPropertyName)
                                            ?? throw new InvalidDataException($"Expected '{MyRequestStructMessageWriter.DestinationPropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestStructMessageWriter.PayloadPropertyNameBytes.EncodedUtf8Bytes))
                        {
                            payload = reader.ReadAsString(MyRequestStructMessageWriter.PayloadPropertyName)
                                        ?? throw new InvalidDataException($"Expected '{MyRequestStructMessageWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.");
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
                "INIT" => new InitStructMessage(source),
                "MESSAGE" => new PayloadStructMessage(source, destination, payload),
                _ => throw new InvalidDataException($"Expected '{MyRequestStructMessageWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.")
            };
            return message != null;
        }
    }
}