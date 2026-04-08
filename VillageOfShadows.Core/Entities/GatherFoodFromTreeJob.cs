using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public sealed class GatherFoodFromTreeJob : Job
    {
        public EntityId TreeId { get; init; }

        public GatherFoodFromTreeJob(EntityId treeId)
        {
            Type = JobType.GatherFood;
            TreeId = treeId;
        }
    }
}
