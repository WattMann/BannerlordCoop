using Common.Messaging;
using ProtoBuf;

namespace Coop.Core.Server.Services.Workshops.Messages
{
    /// <summary>
    /// Sent to the client when a workshop on the server produces output
    /// </summary>
    public class NetworkWorkshopProduceOutput : IMessage
    {
        [ProtoMember(1)]
        public byte[] Output { get; }

        [ProtoMember(2)]
        public byte[] Workshop { get; }

        [ProtoMember(3)]
        public int Count { get; }

        [ProtoMember(4)]
        public bool AffectCapital { get; }

        public NetworkWorkshopProduceOutput(byte[] output, byte[] workshop, int count, bool affectCapital)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
            AffectCapital = affectCapital;
        }
    }
}
