namespace VillageOfShadows.Core.Entities;

public readonly struct EntityId : IEquatable<EntityId>
{
    public Guid Value { get; }

    public EntityId(Guid value)
    {
        Value = value;
    }

    public static EntityId New() => new(Guid.NewGuid());

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(EntityId other) => Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is EntityId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);
    public static bool operator !=(EntityId left, EntityId right) => !left.Equals(right);
}