using Common.Messaging;
using Common.Serialization;
using GameInterface.Serialization;
using GameInterface.Serialization.External;
using GameInterface.Services.Workshops.Messages.Events;

namespace GameInterface.Services.Workshops.Handlers.Commands
{
    /// <summary>
    /// Handles PrepareWorkshopProduceOutput and publishes WorkshopProduceOutput
    /// </summary>
    internal class PrepareWorkshopProduceOutputHandler : IHandler
    {
        private IMessageBroker messageBroker;
        private IBinaryPackageFactory binaryPackageFactory;

        public PrepareWorkshopProduceOutputHandler(IMessageBroker messageBroker, IBinaryPackageFactory binaryPackageFactory)
        {
            this.messageBroker = messageBroker;
            this.binaryPackageFactory = binaryPackageFactory;

            this.messageBroker.Subscribe<PrepareWorkshopProduceOutput>(Handle);
        }

        public void Dispose()
        {
            this.messageBroker.Unsubscribe<PrepareWorkshopProduceOutput>(Handle);
        }

        private void Handle(MessagePayload<PrepareWorkshopProduceOutput> payload)
        {
            var package_output = binaryPackageFactory.GetBinaryPackage<EquipmentElementBinaryPackage>(payload.What.Output);
            var package_workshop = binaryPackageFactory.GetBinaryPackage<EquipmentElementBinaryPackage>(payload.What.Output);

            messageBroker.Publish(this, 
                new WorkshopProduceOutput(
                    BinaryFormatterSerializer.Serialize(package_output),
                    BinaryFormatterSerializer.Serialize(package_workshop),
                    payload.What.Count)
                );
        }
    }
}
