using Common.Logging;
using Common.Messaging;
using Common.Network;
using Coop.Core.Client.Services.Updater.Messages;
using Coop.Core.Server.Services.Clans.Handler;
using Coop.Core.Server.Services.Updater.Messages;
using GameInterface.Services.Clans.Messages;
using GameInterface.Services.Updater.Messages;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coop.Core.Server.Services.Updater.Handlers
{
    /// <summary>
    /// Handles NetworkRequestGameUpdate
    /// </summary>
    public class NetworkRequestGameUpdateHandler : IHandler
    {

        private readonly ILogger logger;
        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;

        public NetworkRequestGameUpdateHandler(IMessageBroker messageBroker, INetwork network)
        {
            logger = LogManager.GetLogger<NetworkRequestGameUpdateHandler>();
            this.messageBroker = messageBroker;
            this.network = network;

            messageBroker.Subscribe<GameUpdateResponse>(Handle);
            messageBroker.Subscribe<NetworkRequestGameUpdate>(Handle);
        }

        public void Handle(MessagePayload<NetworkRequestGameUpdate> obj)
        {
            var msg = new GameUpdateRequest();
            messageBroker.Publish(this, msg);
        }

        public void Handle(MessagePayload<GameUpdateResponse> obj)
        {
           //TODO: send data back to client
        }

        public void Dispose()
        {
            
        }
    }
}
