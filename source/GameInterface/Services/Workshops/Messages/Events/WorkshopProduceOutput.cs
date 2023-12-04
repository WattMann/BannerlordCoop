﻿using Common.Messaging;

namespace GameInterface.Services.Workshops.Messages.Events
{
    /// <summary>
    /// Client-side, called when a workshop should produce output
    /// </summary>
    public record WorkshopProduceOutput : IEvent
    {
        public byte[] Output { get; }
        public byte[] Workshop { get; }
        public int Count { get; }

        public WorkshopProduceOutput(byte[] output, byte[] workshop, int count)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
        }
    }
}
