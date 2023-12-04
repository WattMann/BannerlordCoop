using Common.Messaging;

namespace GameInterface.Services.Workshops.Messages.Events
{
    /// <summary>
    /// Called when a workshop produces output
    /// </summary>
    public record WorkshopProduceOutput : IEvent
    {
        public byte[] Output { get; }
        public byte[] Workshop { get; }
        public int Count { get; }

        public WorkshopProduceOutput(byte[] output, byte[] workshop, int count)
        {
            this.Output = output;
            this.Workshop = workshop;
            this.Count = count;
        }
    }
}
