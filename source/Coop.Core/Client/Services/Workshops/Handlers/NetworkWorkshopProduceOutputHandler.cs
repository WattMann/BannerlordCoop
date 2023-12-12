using Common.Messaging;
using Common.Network;
using Coop.Core.Server.Services.Workshops.Messages;
using GameInterface.Services.Workshops.Messages.Commands;

namespace Coop.Core.Client.Services.Workshops.Handlers
{
    internal class NetworkWorkshopProduceOutputHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;

        public NetworkWorkshopProduceOutputHandler(IMessageBroker messageBroker, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.network = network;

            this.messageBroker.Subscribe<NetworkWorkshopProduceOutput>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<NetworkWorkshopProduceOutput>(Handle);
        }

        private void Handle(MessagePayload<NetworkWorkshopProduceOutput> payload)
        {
            var msg = new WorkshopProduceOutput(payload.What.Output, payload.What.Workshop, payload.What.Count);
            messageBroker.Publish(this, msg);
        }
    }
}
