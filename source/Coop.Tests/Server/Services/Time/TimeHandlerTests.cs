﻿using Autofac;
using Common.Messaging;
using Common.Network;
using Coop.Core.Server.Connections;
using Coop.Core.Server.Services.Time.Handlers;
using Coop.Core.Server.Services.Time.Messages;
using Coop.Tests.Mocks;
using GameInterface.Services.Heroes.Enum;
using GameInterface.Services.Heroes.Messages;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using Xunit;
using Xunit.Abstractions;

namespace Coop.Tests.Server.Services.Time
{
    public class TimeHandlerTests
    {
        private ServerTestComponent serverTestComponent;

        public TimeHandlerTests(ITestOutputHelper output)
        {
            serverTestComponent = new ServerTestComponent(output);
        }

        [Fact]
        public void NetworkRequestTimeSpeedChange_Publishes_SetTimeControlMode()
        {
            // Arrange
            var handler = serverTestComponent.Container.Resolve<TimeHandler>();
            var broker = serverTestComponent.Container.Resolve<MockMessageBroker>();
            var payload = new NetworkRequestTimeSpeedChange(TimeControlEnum.Pause);
            var message = new MessagePayload<NetworkRequestTimeSpeedChange>(null, payload);

            // Act
            handler.Handle_NetworkRequestTimeSpeedChange(message);

            // Assert
            Assert.Single(broker.PublishedMessages);
            Assert.IsType<SetTimeControlMode>(broker.PublishedMessages[0]);
            var setTimeControlMode = (SetTimeControlMode)broker.PublishedMessages[0];
            Assert.Equal(payload.NewControlMode, setTimeControlMode.NewTimeMode);
        }

        [Fact]
        public void TimeSpeedChanged_Publishes_NetworkTimeSpeedChanged()
        {
            // Arrange
            var handler = serverTestComponent.Container.Resolve<TimeHandler>();
            var network = serverTestComponent.Container.Resolve<MockNetwork>();
            var message = new MessagePayload<AttemptedTimeSpeedChanged>(null, new AttemptedTimeSpeedChanged(CampaignTimeControlMode.StoppablePlay));

            network.CreatePeer();

            // Act
            handler.Handle_TimeSpeedChanged(message);

            // Assert
            Assert.NotEmpty(network.Peers);
            foreach(var peer in network.Peers)
            {
                var speedChangedMessage = Assert.Single(network.GetPeerMessages(peer));
                Assert.IsType<NetworkTimeSpeedChanged>(speedChangedMessage);
            }
        }
    }
}
