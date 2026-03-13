using System.Numerics;
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities;

public abstract class Actor : Entity
{
    public Movement Movement;
    public InventoryStack? Carrying { get; set; }
    public int CarryCapacity { get; set; } = 2;
    public bool IsCarrying => Carrying is not null && Carrying.Amount > 0;
    public override void SetPosition(Vector2 position)
    {
        Position = position;
    }
}
