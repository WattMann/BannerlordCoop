using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameInterface.Services.Updater.Messages
{
    // <summary>
    // Sent to the game interface by server
    // </summary>
    public record GameUpdateRequest : ICommand
    {
        public GameUpdateRequest()
        {
        }
    }
}
