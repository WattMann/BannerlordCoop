using Common.Messaging;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace GameInterface.Services.Workshops.Messages.Events
{
    /// <summary>
    /// Prepares WorkshopProducedOutput
    /// </summary>
    internal record PrepareWorkshopProducedOutput : IEvent
    {
        public EquipmentElement Output { get; }
        public Workshop Workshop { get; }
        public int Count { get; }

        public PrepareWorkshopProducedOutput(EquipmentElement output, Workshop workshop, int count)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
        }
    }
}
