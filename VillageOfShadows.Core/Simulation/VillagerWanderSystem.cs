using System.Numerics;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation;

public sealed class VillagerWanderSystem : IWorldSystem
{
    public void Update(World.World world, float dt, IRandom rng)
    {
        var cfg = world.Config;

        foreach (var v in world.GetEntities<Villager>())
        {
            var toTarget = v.Movement.Target - v.Position;
            float dist = toTarget.Length();

            if (dist < 1)
            {
                v.Movement.Target = PickRandomWalkableTarget(world, rng);
                continue;
            }

            var dir = toTarget / dist;
            var nextPos = v.Position + dir * v.Movement.Speed * dt;

            // simpele collision: als volgende tile niet walkable is, kies nieuw target
            var nextTile = world.WorldToTile(nextPos);
            if (!world.IsWalkableTile(nextTile.tx, nextTile.ty))
            {
                v.Movement.Target = PickRandomWalkableTarget(world, rng);
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
                return world.TileToWorldCenter(tx, ty);
        }

        // fallback: gewoon ergens
        return world.TileToWorldCenter(rng.Next(0, world.Width), rng.Next(0, world.Height));
    }
}