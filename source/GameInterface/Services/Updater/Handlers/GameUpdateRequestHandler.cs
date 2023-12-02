using Autofac;
using Common.Messaging;
using Common.Serialization;
using GameInterface.Serialization.External;
using GameInterface.Serialization;
using GameInterface.Services.Updater.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;
using GameInterface.Serialization.Internal;

namespace GameInterface.Services.Updater.Handlers
{
    /// <summary>
    /// Handles GameUpdateRequest
    /// </summary>
    public class GameUpdateRequestHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IBinaryPackageFactory binaryPackageFactory;

        public GameUpdateRequestHandler(IMessageBroker messageBroker, IBinaryPackageFactory binaryPackageFactory) {
            this.messageBroker = messageBroker;
            this.binaryPackageFactory = binaryPackageFactory;

            messageBroker.Subscribe<GameUpdateRequest>(Handle);
        }

        public void Handle(MessagePayload<GameUpdateRequest> msg)
        {
            GameUpdateData data = new();
            //TODO: fill with data
            var package = binaryPackageFactory.GetBinaryPackage<GameUpdateBinaryPackage>(data);
            messageBroker.Publish(this, new GameUpdateResponse(BinaryFormatterSerializer.Serialize(package)));
        }
    }
}
