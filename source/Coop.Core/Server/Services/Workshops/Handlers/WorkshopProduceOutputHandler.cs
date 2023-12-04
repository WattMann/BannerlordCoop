using Common.Messaging;
using Common.Network;
using Coop.Core.Server.Services.Workshops.Messages;
using GameInterface.Services.Workshops.Messages.Events;

namespace Coop.Core.Server.Services.Workshops.Handlers
{
    internal class WorkshopProduceOutputHandler : IHandler
    {
        private IMessageBroker messageBroker;
        private INetwork network;

        public WorkshopProduceOutputHandler(IMessageBroker broker, INetwork network)
        {
            this.messageBroker = broker;
            this.network = network;

            this.messageBroker.Subscribe<WorkshopProduceOutput>(Handle);
        }
        public void Dispose()
        {
            this.messageBroker.Unsubscribe<WorkshopProduceOutput>(Handle);
        }

        private void Handle(MessagePayload<WorkshopProduceOutput> payload)
        {
            network.SendAll(new NetworkWorkshopProduceOutput(payload.What.Output, payload.What.Workshop, payload.What.Count));
        }

    }
}
