namespace VillageOfShadows.Core.Entities;

public readonly struct EntityId
{
    public Guid Value { get; }

    public EntityId(Guid value)
    {
        Value = value;
    }

    public static EntityId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}