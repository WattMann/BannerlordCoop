using Common.Messaging;
using GameInterface.Serialization.Internal;
using GameInterface.Serialization;
using System.Globalization;

namespace GameInterface.Services.Updater.Messages
{
    public record GameUpdateResponse : ICommand
    {
        public byte[] Data;

        public GameUpdateResponse(byte[] data)
        {
            this.Data = data;
        }
    }
}
