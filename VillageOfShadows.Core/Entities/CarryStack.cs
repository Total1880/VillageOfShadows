using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities;

public sealed class CarryStack
{
    public required ResourceType ResourceType { get; init; }
    public int Amount { get; set; }
}