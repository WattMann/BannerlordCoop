﻿using Common.Messaging;
using ProtoBuf;

namespace Coop.Core.Server.Services.Workshops.Messages
{
    internal class NetworkWorkshopProduceOutput : IMessage
    {
        [ProtoMember(1)]
        public byte[] Output { get; }

        [ProtoMember(2)]
        public byte[] Workshop { get; }

        [ProtoMember(3)]
        public int Count { get; }

        public NetworkWorkshopProduceOutput(byte[] output, byte[] workshop, int count)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
        }
    }
}