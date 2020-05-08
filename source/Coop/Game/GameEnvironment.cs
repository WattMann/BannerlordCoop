﻿using System.Collections.Generic;
using System.Linq;
using Coop.Game.Patch;
using Coop.Game.Persistence;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Coop.Game
{
    internal class GameEnvironment : IEnvironment
    {
        public GameEnvironment()
        {
            TimeControl.OnTimeControlChangeAttempt += value => RequestedTimeControlMode = value;
        }

        #region TimeControl
        public CampaignTimeControlMode TimeControlMode
        {
            get => Campaign.Current.TimeControlMode;
            set => TimeControl.SetForced_Campaign_TimeControlMode(value);
        }

        public CampaignTimeControlMode? RequestedTimeControlMode { get; set; }
        #endregion

        #region MobileParty
        public MobileParty GetMobilePartyByIndex(int iPartyIndex)
        {
            return MobileParty.All.SingleOrDefault(p => p.Party.Index == iPartyIndex);
        }

        public void AddRemoteMoveTo(MobileParty party, RemoteValue<Vec2> moveTo)
        {
            m_MoveToPoints[party] = moveTo;
            moveTo.OnValueChanged += pos =>
            {
                CampaignMapMovement.s_IsRemoteUpdate = true;
                try
                {
                    party.SetMoveGoToPoint(pos);
                }
                finally
                {
                    CampaignMapMovement.s_IsRemoteUpdate = false;
                }
            };
        }

        public void RemoveRemoteMoveTo(MobileParty party)
        {
            m_MoveToPoints.Remove(party);
        }

        public IReadOnlyDictionary<MobileParty, RemoteValue<Vec2>> RemoteMoveTo => m_MoveToPoints;

        private readonly Dictionary<MobileParty, RemoteValue<Vec2>> m_MoveToPoints =
            new Dictionary<MobileParty, RemoteValue<Vec2>>();
        #endregion
    }
}
