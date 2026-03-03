using Microsoft.Xna.Framework;

namespace VillageOfShadows.Core.Entities;

public sealed class Villager
{
    public Guid Id { get; } = Guid.NewGuid();

    // World-space in pixels
    public Vector2 Position;
    public Vector2 Target;

    public Villager(Vector2 start)
    {
        Position = start;
        Target = start;
    }
}