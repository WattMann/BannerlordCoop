using Common.Messaging;

namespace GameInterface.Services.Workshops.Messages.Commands
{
    /// <summary>
    /// Should produce workshop output.
    /// </summary>
    public record WorkshopProduceOutput : ICommand
    {
        public byte[] Output { get; }
        public byte[] Workshop { get; }
        public int Count { get; }
        public bool AffectCapital { get; }

        public WorkshopProduceOutput(byte[] output, byte[] workshop, int count, bool affectCapital)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
            AffectCapital = affectCapital; 
        }
    }
}
