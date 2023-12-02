using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.Updater
{
    // <summary>
    // Contains all game update data.
    // </summary>
    public record GameUpdateData
    {
        public PartyBase[] Parties;
    }
}
