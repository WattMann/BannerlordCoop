using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coop.Core.Client.Services.Updater.Messages
{
    // <summary>
    // Sent by the client to the server to request game update
    // </summary>
    public record NetworkRequestGameUpdate : ICommand
    {
    }
}
