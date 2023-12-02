using Common.Messaging;
using GameInterface.Services.Updater;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coop.Core.Server.Services.Updater.Messages
{
    // <summary>
    // Sent by the server to the client, contains game update information.
    // What information is sent is decided by the server.
    // </summary>
    public class NetworkGameUpdate : ICommand
    {
        [ProtoMember(1)]
        public byte[] Data;

        public NetworkGameUpdate(byte[] data)
        {
            this.Data = data;
        }
    }
}
