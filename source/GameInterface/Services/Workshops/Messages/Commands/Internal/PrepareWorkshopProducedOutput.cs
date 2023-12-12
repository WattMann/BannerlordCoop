using Common.Messaging;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace GameInterface.Services.Workshops.Messages.Commands.Internal
{
    /// <summary>
    /// Should prepare and publish WorkshopProducedOutput.
    /// </summary>
    internal record PrepareWorkshopProducedOutput : IEvent
    {
        public EquipmentElement Output { get; }
        public Workshop Workshop { get; }
        public int Count { get; }
        public bool AffectCapital { get; }

        public PrepareWorkshopProducedOutput(EquipmentElement output, Workshop workshop, int count, bool affectCapital)
        {
            Output = output;
            Workshop = workshop;
            Count = count;
            AffectCapital = affectCapital;
        }
    }
}
