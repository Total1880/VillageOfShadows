using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Utils;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Core.Simulation;

public sealed class TreeGrowthSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        var cfg = world.Config;

        for (int y = 0; y < world.Height; y++)
            for (int x = 0; x < world.Width; x++)
            {
                var t = world.Get(x, y);
                if (t.Entity is not Tree) continue;

                var tree = t.Entity as Tree;

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
                        TrySpawnSaplingNear(world, x, y, rng, tree);
                }
            }
    }

    private static void TrySpawnSaplingNear(World.World world, int x, int y, IRandom rng, Tree tree)
    {
        for (int attempt = 0; attempt < 6; attempt++)
        {
            int nx = x + rng.Next(-2, 3);
            int ny = y + rng.Next(-2, 3);
            if (!world.InBounds(nx, ny)) continue;

            var t = world.Get(nx, ny);
            if (t.Entity?.GetType() == typeof(Tree)) continue;

            t.Entity = tree.Create();

            return;
        }
    }
}