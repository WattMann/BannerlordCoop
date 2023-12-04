using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using Common.Messaging;
using GameInterface.Services.Workshops.Messages.Events;

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
        MessageBroker.Instance.Publish(instance, new PrepareWorkshopProduceOutput(outputItem, workshop, count));
    } 
}
