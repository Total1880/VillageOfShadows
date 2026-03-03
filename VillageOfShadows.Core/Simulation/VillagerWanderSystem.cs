using Microsoft.Xna.Framework;
using VillageOfShadows.Core.Utils;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Core.Simulation;

public sealed class VillagerWanderSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        var cfg = world.Config;

        foreach (var v in world.Villagers)
        {
            var toTarget = v.Target - v.Position;
            float dist = toTarget.Length();

            if (dist < cfg.VillagerRetargetDistancePx)
            {
                v.Target = PickRandomWalkableTarget(world, rng);
                continue;
            }

            var dir = toTarget / dist;
            var nextPos = v.Position + dir * cfg.VillagerSpeedPixelsPerSec * dt;

            // simpele collision: als volgende tile niet walkable is, kies nieuw target
            var nextTile = world.WorldToTile(nextPos);
            if (!world.IsWalkableTile(nextTile.X, nextTile.Y))
            {
                v.Target = PickRandomWalkableTarget(world, rng);
                continue;
            }

            v.Position = nextPos;
        }
    }

    private static Vector2 PickRandomWalkableTarget(World.World world, IRandom rng)
    {
        for (int i = 0; i < 30; i++)
        {
            int tx = rng.Next(0, world.Width);
            int ty = rng.Next(0, world.Height);
            if (world.IsWalkableTile(tx, ty))
                return world.TileCenter(tx, ty);
        }

        // fallback: gewoon ergens
        return world.TileCenter(rng.Next(0, world.Width), rng.Next(0, world.Height));
    }
}