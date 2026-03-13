using System.Numerics;

namespace VillageOfShadows.Core.Entities;

public abstract class Entity
{
    public EntityId EntityId { get; init; } = EntityId.New();
    public Vector2 Position { get; set; }

    protected Entity()
    {
    }

    public virtual void SetPosition(Vector2 position)
    {
        Position = position;
    }
}
