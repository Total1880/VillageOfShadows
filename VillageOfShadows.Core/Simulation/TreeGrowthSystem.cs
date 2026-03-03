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
                if (!t.HasTree) continue;

                float g = t.TreeStage switch
                {
                    TreeStage.Sapling => cfg.SaplingGrowthPerSec,
                    TreeStage.Young => cfg.YoungGrowthPerSec,
                    _ => 0f
                };

                if (g > 0f)
                {
                    t.TreeGrowth += g * dt;
                    if (t.TreeGrowth >= 1f)
                    {
                        t.TreeGrowth = 0f;
                        if (t.TreeStage != TreeStage.Mature)
                            t.TreeStage++;
                    }
                }
                else if (t.TreeStage == TreeStage.Mature)
                {
                    if (rng.NextDouble() < cfg.MatureSpreadChancePerSec * dt)
                        TrySpawnSaplingNear(world, x, y, rng);
                }
            }
    }

    private static void TrySpawnSaplingNear(World.World world, int x, int y, IRandom rng)
    {
        for (int attempt = 0; attempt < 6; attempt++)
        {
            int nx = x + rng.Next(-2, 3);
            int ny = y + rng.Next(-2, 3);
            if (!world.InBounds(nx, ny)) continue;

            var t = world.Get(nx, ny);
            if (t.HasTree) continue;

            t.HasTree = true;
            t.TreeStage = TreeStage.Sapling;
            t.TreeGrowth = 0f;
            return;
        }
    }
}