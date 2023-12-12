using Common.Messaging;

namespace GameInterface.Services.Workshops.Messages.Commands
{
    /// <summary>
    /// Produces workshop output.
    /// </summary>
    public record WorkshopProduceOutput : ICommand
    {
        public byte[] Output { get; }
        public byte[] Workshop { get; }
        public int Count { get; }

        public WorkshopProduceOutput(byte[] output, byte[] workshop, int count)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
        }
    }
}
