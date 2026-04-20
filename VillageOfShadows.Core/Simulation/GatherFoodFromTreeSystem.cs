using System.Numerics;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Entities.Jobs;
using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation;

public sealed class GatherFoodFromTreeSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        foreach (var villager in world.GetEntities<Villager>())
        {
            if (villager.CurrentJobId == null)
                continue;

            var job = world.Jobs
                .OfType<GatherFoodFromTreeJob>()
                .FirstOrDefault(j => j.Id == villager.CurrentJobId.Value);
            if (job == null) continue;
            if (job.IsCompleted)
            {
                ResetVillager(villager);
                continue;
            }

            world.TryGetEntity(job.TreeId, out Entity tree);
            if (tree == null)
            {
                job.IsCompleted = true;
                ResetVillager(villager);
                continue;
            }

            float dist = Vector2.Distance(villager.Position, tree.Position);

            if (dist > 4f)
            {
                villager.State = VillagerState.MovingToJob;
                villager.Movement.Target = tree.Position;
                continue;
            }

            villager.State = VillagerState.Working;
            villager.WorkProgress += dt;

            if (villager.WorkProgress >= ((Tree)tree).GatherFoodWorkRequired)
            {
                CompleteGather(world, villager, tree as Tree, job);
            }
        }
    }

    private void CompleteGather(World.World world, Villager villager, Tree tree, GatherFoodFromTreeJob job)
    {
        job.IsCompleted = true;

        StockpileHelper.DropResourceNear(world, tree.Tile.X, tree.Tile.Y, ResourceType.Apples, (int)tree.FoodValue, 5);

        world.RemoveFoodFromTree(tree.EntityId);

        ResetVillager(villager);
    }

    private void ResetVillager(Villager villager)
    {
        villager.CurrentJobId = null;
        villager.WorkProgress = 0f;
        villager.State = VillagerState.Idle;
        villager.Movement.Target = villager.Position;
    }
}
