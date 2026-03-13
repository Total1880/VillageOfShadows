using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation;

public sealed class VillagerJobSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        foreach (var villager in world.GetEntities<Villager>())
        {
            if (villager.State == VillagerState.Idle)
            {
                TryAssignJob(world, villager);
            }
        }
    }

    private void TryAssignJob(World.World world, Villager villager)
    {
        var job = world.GetJobs<ChopTreeJob>()
            .FirstOrDefault(j => !j.IsClaimed && !j.IsCompleted);

        if (job == null)
            return;

        job.IsClaimed = true;
        job.ClaimedByEntityId = villager.EntityId;
        villager.CurrentJobId = job.Id;
        villager.State = VillagerState.MovingToJob;

        world.TryGetEntity(job.TreeId, out var tree);
        if (tree != null)
        {
            villager.Movement.Target = tree.Position;
        }
    }
}
