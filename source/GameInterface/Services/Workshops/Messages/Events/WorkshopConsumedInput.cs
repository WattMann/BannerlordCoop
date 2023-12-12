using Common.Messaging;

namespace GameInterface.Services.Workshops.Messages.Events
{
    /// <summary>
    /// Should be called when a workshop consumes an input.
    /// </summary>
    public class WorkshopConsumedInput : IEvent
    {
        public byte[] Input { get; }
        public byte[] Workshop { get; }
        public bool AffectCapital { get; }

        public WorkshopConsumedInput(byte[] input, byte[] workshop, bool affectCapital)
        {
            Input = input;
            Workshop = workshop;
            AffectCapital = affectCapital;
        }
    }
}
