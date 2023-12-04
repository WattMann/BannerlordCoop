using Common.Messaging;
using Common.Network;
using GameInterface.Services.Workshops.Messages.Events;
using System.Runtime.InteropServices;

namespace Coop.Core.Client.Services.Workshops.Handlers
{
    internal class NetworkWorkshopProduceOutput : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;

        public NetworkWorkshopProduceOutput(IMessageBroker messageBroker, INetwork network)
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

        }
    }
}
