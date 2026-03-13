using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation;

public sealed class TreeGrowthSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        var cfg = world.Config;

        var trees = world.GetEntities<Tree>().ToList();
        var saplingsToSpawn = new List<Tree>();

        foreach (var tree in trees)
        {
            float g = tree.Stage switch
            {
                TreeStage.Sapling => tree.SaplingGrowthPerSec,
                TreeStage.Young => tree.YoungGrowthPerSec,
                _ => 0f
            };

            if (g > 0f)
            {
                tree.Growth += g * dt;
                if (tree.Growth >= 1f)
                {
                    tree.Growth = 0f;
                    if (tree.Stage != TreeStage.Mature)
                        tree.Stage++;
                }

                if (tree.HasFood && tree.MaxFoodValue > tree.FoodValue)
                {
                    tree.FoodValue += tree.FoodGrowthPerSec * dt;
                }
            }
            else if (tree.Stage == TreeStage.Mature)
            {
                if (rng.NextDouble() < tree.MatureSpreadChancePerSec * dt)
                    saplingsToSpawn.Add(tree);
            }
        }

        foreach (var parentTree in saplingsToSpawn)
        {
            TrySpawnSaplingNear(world, parentTree, rng);
        }

    }

    private static void TrySpawnSaplingNear(World.World world, Tree tree, IRandom rng)
    {
        var (x, y) = world.WorldToTile(tree.Position);

        for (int attempt = 0; attempt < 6; attempt++)
        {
            int nx = x + rng.Next(-2, 3);
            int ny = y + rng.Next(-2, 3);

            if (world.TryPlaceTileEntity(tree.CreateSapling(), nx, ny))
                return;
        }
    }
}