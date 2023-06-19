﻿using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace GameInterface.Services.Sieges.Patches;

[HarmonyPatch(typeof(SiegeEventCampaignBehavior))]
internal class DisableSiegeEventCampaignBehavior
{
    [HarmonyPatch(nameof(SiegeEventCampaignBehavior.RegisterEvents))]
    static bool Prefix() => false;
}
