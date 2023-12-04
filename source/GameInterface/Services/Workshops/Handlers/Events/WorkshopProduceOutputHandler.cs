using Common.Messaging;
using GameInterface.Serialization;
using GameInterface.Services.Workshops.Messages.Events;

namespace GameInterface.Services.Workshops.Handlers.Events
{
    /// <summary>
    /// Handles WorkshopProduceOutput on client
    /// </summary>
    internal class WorkshopProduceOutputHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IBinaryPackageFactory binaryPackageFactory;

        public WorkshopProduceOutputHandler(IMessageBroker messageBroker, IBinaryPackageFactory binaryPackageFactory)
        {
            this.messageBroker = messageBroker;
            this.binaryPackageFactory = binaryPackageFactory;

            messageBroker.Subscribe<WorkshopProduceOutput>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<WorkshopProduceOutput>(Handle);
        }

        private void Handle(MessagePayload<WorkshopProduceOutput> payload)
        {

        }
    }
}
