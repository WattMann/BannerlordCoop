using Common.Messaging;

namespace GameInterface.Services.Workshops.Messages.Events
{
    /// <summary>
    /// Should be called when a workshop has produces an output.
    /// </summary>
    public record WorkshopProducedOutput : IEvent
    {
        public byte[] Output { get; }
        public byte[] Workshop { get; }
        public int Count { get; }
        public bool AffectCapital { get; }

        public WorkshopProducedOutput(byte[] output, byte[] workshop, int count, bool affectCapital)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
            AffectCapital = affectCapital;
        }
    }
}
