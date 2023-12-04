using Common.Messaging;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace GameInterface.Services.Workshops.Messages.Events
{
    /// <summary>
    /// Prepares and publishes WorkshopProduceOutput
    /// </summary>
    internal record PrepareWorkshopProduceOutput : IEvent
    {
        public EquipmentElement Output { get; }
        public Workshop Workshop { get; }
        public int Count { get; }

        public PrepareWorkshopProduceOutput(EquipmentElement output, Workshop workshop, int count)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
        }
    }
}
