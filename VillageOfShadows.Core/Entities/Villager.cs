using System.Numerics;
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities;

public sealed class Villager : Actor
{
    public VillagerState State { get; set; } = VillagerState.Idle;
    public EntityId? CurrentJobId { get; set; }
    public float WorkProgress { get; set; }
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