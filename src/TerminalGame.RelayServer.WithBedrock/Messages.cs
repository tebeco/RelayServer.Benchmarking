using System.IO;

namespace TerminalGame.RelayServer.WithBedrock
{

    public static class MyPayloadTypeStrings
    {
        public const string Init = "INIT";
        public const string Payload = "MESSAGE";

        public static string ToString(this MyPayloadType myPayloadType) =>
            myPayloadType switch
            {
                MyPayloadType.Init => Init,
                MyPayloadType.Payload => Payload,
                _ => throw new InvalidDataException($"Expected '{nameof(MyPayloadType)}' to be of type {myPayloadType.GetType().Name}.")
            };
    }

    public enum MyPayloadType
    {
        Init,
        Payload
    }

     public abstract record MyRequestRecordMessage(string Source, MyPayloadType PayloadType);

    public record InitRecordMessage(string Source) : MyRequestRecordMessage(Source, MyPayloadType.Init);
    public record PayloadRecordMessage(string Source, string Destination, string Payload) : MyRequestRecordMessage(Source, MyPayloadType.Payload);

    public interface IStructMessage
    {
        MyPayloadType PayloadType { get; }
        string Source { get; }
    }

    public struct InitStructMessage : IStructMessage
    {
        public InitStructMessage(string source)
        {
            PayloadType = MyPayloadType.Init;
            Source = source;
        }
        public MyPayloadType PayloadType { get; }

        public string Source { get; }
    }

    public struct PayloadStructMessage : IStructMessage
    {
        public PayloadStructMessage(string source, string destination, string payload)
        {
            PayloadType = MyPayloadType.Payload;
            Source = source;
            Destination = destination;
            Payload = payload;
        }

        public MyPayloadType PayloadType { get; }
        public string Source { get; }
        public string Destination { get; }
        public string Payload { get; }
    }
}
