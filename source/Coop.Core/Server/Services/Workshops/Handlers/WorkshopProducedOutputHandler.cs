using Common.Messaging;
using Common.Network;
using Coop.Core.Server.Services.Workshops.Messages;
using GameInterface.Services.Workshops.Messages.Events;

namespace Coop.Core.Server.Services.Workshops.Handlers
{
    internal class WorkshopProducedOutputHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;

        public WorkshopProducedOutputHandler(IMessageBroker messageBroker, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.network = network;

            messageBroker.Subscribe<WorkshopProducedOutput>(Handle);
        }
        public void Dispose()
        {
            messageBroker.Unsubscribe<WorkshopProducedOutput>(Handle);
        }

        private void Handle(MessagePayload<WorkshopProducedOutput> payload)
        {
            network.SendAll(new NetworkWorkshopProduceOutput(payload.What.Output, payload.What.Workshop, payload.What.Count, payload.What.AffectCapital));
        }

    }
}
