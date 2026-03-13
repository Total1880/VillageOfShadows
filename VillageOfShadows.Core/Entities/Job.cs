using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public abstract class Job
    {
        public EntityId Id { get; init; } = EntityId.New();
        public JobType Type { get; init; }
        public bool IsClaimed { get; set; }
        public bool IsCompleted { get; set; }
        public EntityId? ClaimedByEntityId { get; set; }
    }
}
