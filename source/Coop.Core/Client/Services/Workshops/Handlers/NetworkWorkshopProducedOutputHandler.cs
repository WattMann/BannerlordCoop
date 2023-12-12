using Common.Messaging;
using Common.Network;
using Coop.Core.Server.Services.Workshops.Messages;
using GameInterface.Services.Workshops.Messages.Events;

namespace Coop.Core.Client.Services.Workshops.Handlers
{
    internal class NetworkWorkshopProducedOutputHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;

        public NetworkWorkshopProducedOutputHandler(IMessageBroker messageBroker, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.network = network;

            this.messageBroker.Subscribe<NetworkWorkshopProducedOutput>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<NetworkWorkshopProducedOutput>(Handle);
        }

        private void Handle(MessagePayload<NetworkWorkshopProducedOutput> payload)
        {
            var msg = new WorkshopProducedOutput(payload.What.Output, payload.What.Workshop, payload.What.Count, payload.What.AffectCapital);
            messageBroker.Publish(this, msg);
        }
    }
}
