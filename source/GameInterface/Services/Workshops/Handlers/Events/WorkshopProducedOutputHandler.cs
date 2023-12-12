using Common.Messaging;
using Common.Serialization;
using GameInterface.Serialization;
using GameInterface.Serialization.External;
using GameInterface.Services.Workshops.Messages.Events;
using System.Reflection;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace GameInterface.Services.Workshops.Handlers.Events
{
    /// <summary>
    /// Handles WorkshopProducedOutput on client.
    /// Produces workshop output.
    /// </summary>
    internal class WorkshopProducedOutputHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IBinaryPackageFactory binaryPackageFactory;

        public WorkshopProducedOutputHandler(IMessageBroker messageBroker, IBinaryPackageFactory binaryPackageFactory)
        {
            if (ModInformation.IsServer) return;

            this.messageBroker = messageBroker;
            this.binaryPackageFactory = binaryPackageFactory;

            messageBroker.Subscribe<WorkshopProducedOutput>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<WorkshopProducedOutput>(Handle);
        }

        private void Handle(MessagePayload<WorkshopProducedOutput> payload)
        {
            var package_workshop = BinaryFormatterSerializer.Deserialize<WorkshopBinaryPackage>(payload.What.Workshop);
            var workshop = package_workshop.Unpack<Workshop>(binaryPackageFactory);

            var package_output = BinaryFormatterSerializer.Deserialize<EquipmentElementBinaryPackage>(payload.What.Output);
            var output = package_output.Unpack<EquipmentElement>(binaryPackageFactory);

            var parameters = new object[5];
            parameters[0] = output;
            parameters[1] = workshop.Settlement.Town;
            parameters[2] = workshop;
            parameters[3] = payload.What.Count;
            parameters[4] = !payload.What.AffectCapital;

            typeof(WorkshopsCampaignBehavior).GetMethod("ProduceOutput", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, parameters);
        }
    }
}
