using Common.Messaging;
using Common.Serialization;
using GameInterface.Serialization;
using GameInterface.Serialization.External;
using GameInterface.Services.Workshops.Messages.Commands;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace GameInterface.Services.Workshops.Handlers.Commands
{
    /// <summary>
    /// Handles WorkshopProduceOutput.
    /// Updates the town's item roster.
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
            var package_workshop = BinaryFormatterSerializer.Deserialize<WorkshopBinaryPackage>(payload.What.Workshop);
            var workshop = package_workshop.Unpack<Workshop>(binaryPackageFactory);

            var package_output = BinaryFormatterSerializer.Deserialize<EquipmentElementBinaryPackage>(payload.What.Output);
            var output = package_output.Unpack<EquipmentElement>(binaryPackageFactory);

            workshop.Settlement.ItemRoster.AddToCounts(output, payload.What.Count);
        }
    }
}
