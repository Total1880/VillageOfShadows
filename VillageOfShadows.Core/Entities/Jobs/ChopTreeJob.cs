using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities.Jobs
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
