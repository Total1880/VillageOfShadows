using System.Numerics;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation;

public sealed class ChopTreeSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        foreach (var villager in world.GetEntities<Villager>())
        {
            if (villager.CurrentJobId == null)
                continue;

            var job = world.Jobs
                .OfType<ChopTreeJob>()
                .FirstOrDefault(j => j.Id == villager.CurrentJobId.Value);

            if (job == null || job.IsCompleted)
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

            if (villager.WorkProgress >= ((Tree)tree).ChopWorkRequired)
            {
                CompleteChop(world, villager, tree as Tree, job);
            }
        }
    }

    private void CompleteChop(World.World world, Villager villager, Tree tree, ChopTreeJob job)
    {
        job.IsCompleted = true;

        // boom verwijderen
        world.RemoveEntity(tree.EntityId);

        StockpileHelper.DropResourceNear(world, tree.Tile.X, tree.Tile.Y, ResourceType.Wood, tree.WoodYield, 5);

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
