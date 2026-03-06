using System.Numerics;

namespace VillageOfShadows.Core.Entities;

public abstract class Entity
{
    public EntityId EntityId { get; } = EntityId.New();
    public Guid Id => EntityId.Value;
    public Vector2 Position;

    public virtual bool BlocksMovement => true;
}