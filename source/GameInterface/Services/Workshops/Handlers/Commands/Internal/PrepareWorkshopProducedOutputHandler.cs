﻿using Common.Messaging;
using Common.Serialization;
using GameInterface.Serialization;
using GameInterface.Serialization.External;
using GameInterface.Services.Workshops.Messages.Commands.Internal;
using GameInterface.Services.Workshops.Messages.Events;

namespace GameInterface.Services.Workshops.Handlers.Commands.Internal
{
    /// <summary>
    /// Handles PrepareWorkshopProducedOutput and publishes WorkshopProducedOutput on server.
    /// </summary>
    internal class PrepareWorkshopProducedOutputHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IBinaryPackageFactory binaryPackageFactory;

        public PrepareWorkshopProducedOutputHandler(IMessageBroker messageBroker, IBinaryPackageFactory binaryPackageFactory)
        {
            if (ModInformation.IsClient) return;
            this.messageBroker = messageBroker;
            this.binaryPackageFactory = binaryPackageFactory;

            messageBroker.Subscribe<PrepareWorkshopProducedOutput>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<PrepareWorkshopProducedOutput>(Handle);
        }

        private void Handle(MessagePayload<PrepareWorkshopProducedOutput> payload)
        {
            var package_output = binaryPackageFactory.GetBinaryPackage<EquipmentElementBinaryPackage>(payload.What.Output);
            var package_workshop = binaryPackageFactory.GetBinaryPackage<WorkshopBinaryPackage>(payload.What.Workshop);

            messageBroker.Publish(this,
                new WorkshopProducedOutput(
                    BinaryFormatterSerializer.Serialize(package_output),
                    BinaryFormatterSerializer.Serialize(package_workshop),
                    payload.What.Count,
                    payload.What.AffectCapital)
                );
        }
    }
}
