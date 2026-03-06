using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation;

public sealed class TreeGrowthSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        var cfg = world.Config;

        foreach (var tree in world.GetEntities<Tree>())
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
                    TrySpawnSaplingNear(world, (int)tree.Position.X, (int)tree.Position.Y, rng, tree);
            }
        }

    }

    private static void TrySpawnSaplingNear(World.World world, int x, int y, IRandom rng, Tree tree)
    {
        for (int attempt = 0; attempt < 6; attempt++)
        {
            int nx = x + rng.Next(-2, 3);
            int ny = y + rng.Next(-2, 3);

            if (world.TryPlaceEntityOnTile(tree.CreateSapling(), nx, ny))
                return;
        }
    }
}