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
        if(ChopTreeJobs(world, villager)) return;
        if(GatherFoodJob(world, villager)) return;
    }

    private static bool ChopTreeJobs(World.World world, Villager villager)
    {
        var chopTreeJob = world.GetJobs<ChopTreeJob>()
            .FirstOrDefault(j => !j.IsClaimed && !j.IsCompleted);

        if (chopTreeJob == null)
            return false;

        chopTreeJob.IsClaimed = true;
        chopTreeJob.ClaimedByEntityId = villager.EntityId;
        villager.CurrentJobId = chopTreeJob.Id;
        villager.State = VillagerState.MovingToJob;

        world.TryGetEntity(chopTreeJob.TreeId, out var tree);
        if (tree != null)
        {
            villager.Movement.Target = tree.Position;
        }

        return true;
    }

    private static bool GatherFoodJob(World.World world, Villager villager)
    {
        var gatherFoodFromTreeJob = world.GetJobs<GatherFoodFromTreeJob>()
            .FirstOrDefault(j => !j.IsClaimed && !j.IsCompleted);

        if (gatherFoodFromTreeJob == null)
            return false;

        gatherFoodFromTreeJob.IsClaimed = true;
        gatherFoodFromTreeJob.ClaimedByEntityId = villager.EntityId;
        villager.CurrentJobId = gatherFoodFromTreeJob.Id;
        villager.State = VillagerState.MovingToJob;

        world.TryGetEntity(gatherFoodFromTreeJob.TreeId, out var tree);
        if (tree != null)
        {
            villager.Movement.Target = tree.Position;
        }

        return true;
    }
}
