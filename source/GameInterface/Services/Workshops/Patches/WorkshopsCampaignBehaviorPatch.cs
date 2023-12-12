using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using Common.Messaging;
using GameInterface.Services.Workshops.Messages.Commands;

namespace GameInterface.Services.Characters.Patches;

[HarmonyPatch(typeof(WorkshopsCampaignBehavior))]
internal class WorkshopsCampaignBehaviorPatch
{
    private static readonly WorkshopsCampaignBehavior instance = new();

    [HarmonyPatch(nameof(WorkshopsCampaignBehavior.RegisterEvents))]
    [HarmonyPrefix]
    static bool RegisterEventsPrefix() => ModInformation.IsServer; // Run only on server

    [HarmonyPatch("ProduceOutput")]
    [HarmonyPostfix]
    static void ProduceOutputPostfix(EquipmentElement outputItem, Town town, Workshop workshop, int count, bool doNotEffectCapital)
    {
        if (ModInformation.IsClient) 
            return;
        MessageBroker.Instance.Publish(instance, new PrepareWorkshopProducedOutput(outputItem, workshop, count, doNotEffectCapital));
    } 
}
