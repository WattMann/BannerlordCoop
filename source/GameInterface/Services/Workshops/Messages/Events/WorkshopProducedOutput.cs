using Common.Messaging;

namespace GameInterface.Services.Workshops.Messages.Events
{
    /// <summary>
    /// Server-side, called when a workshop produced output
    /// </summary>
    public record WorkshopProducedOutput : IEvent
    {
        public byte[] Output { get; }
        public byte[] Workshop { get; }
        public int Count { get; }

        public WorkshopProducedOutput(byte[] output, byte[] workshop, int count)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
        }
    }
}
