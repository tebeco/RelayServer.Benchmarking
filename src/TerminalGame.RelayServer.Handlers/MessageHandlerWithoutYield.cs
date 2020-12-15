﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TerminalGame.RelayServer.Domain;

namespace TerminalGame.RelayServer.Handlers
{
    public class MessageHandlerWithoutYield
    {
        private long TotalRead { get; set; } = 0;
        private long CurrentRead { get; set; } = 0;
        private bool MessageIncomplete { get; set; }
        private int MessageSize { get; set; } = 0;

        private string AccumulateMessage { get; set; } = "";


        public IEnumerable<SocketMessage?> DecodeBuffer(byte[] buffer, long offset, long size)
        {
            AccumulateMessage += Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            while (AccumulateMessage.Length != 0 && !MessageIncomplete)
            {
                var sizeStartIndex = AccumulateMessage.IndexOf("{[", StringComparison.Ordinal);

                if (sizeStartIndex != -1)
                {
                    var sizeEndIndex = AccumulateMessage[sizeStartIndex..].IndexOf("][", StringComparison.Ordinal);
                    var messageEnd = sizeEndIndex + 2 +
                                     int.Parse(AccumulateMessage[(sizeStartIndex + 2)..sizeEndIndex]);

                    if (AccumulateMessage.Length >= messageEnd + 2)
                    {
                        var messageString = AccumulateMessage[(sizeEndIndex + 2)..messageEnd];

                        var message = DeserializeSocketMessage(messageString);

                        AccumulateMessage = AccumulateMessage[(messageEnd + 2)..];
                        MessageIncomplete = false;
                        yield return message;
                    }
                    else
                    {
                        MessageIncomplete = true;
                    }
                }
                else
                {
                    MessageIncomplete = true;
                }
            }
        }

        private static SocketMessage? DeserializeSocketMessage(string messageString)
        {
            SocketMessage? message;
            try
            {
                message = JsonSerializer.Deserialize<SocketMessage>(messageString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                message = new SocketMessage
                {
                    Destination = null,
                    Payload = "Error deserializing message",
                    Source = null,
                    PayloadType = SocketPayloadType.ERROR
                };
            }

            return message;
        }
    }
}