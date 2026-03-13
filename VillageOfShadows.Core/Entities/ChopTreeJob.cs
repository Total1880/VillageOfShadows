using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public sealed class ChopTreeJob : Job
    {
        public EntityId TreeId { get; init; }

        public ChopTreeJob(EntityId treeId)
        {
            Type = JobType.ChopTree;
            TreeId = treeId;
        }
    }
}
