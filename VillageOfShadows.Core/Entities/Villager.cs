using System.Numerics;
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities;

public sealed class Villager : Entity
{
    public Movement Movement;

    public Villager(Vector2 start)
    {
        Position = start;
        Movement.Target = start;
        Movement.Speed = 45f;
    }

    public Villager() : this(Vector2.Zero)
    {
    }
}