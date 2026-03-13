using System.Numerics;
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities;

public abstract class Actor : Entity
{
    public Movement Movement;

    public override void SetPosition(Vector2 position)
    {
        Position = position;
    }
}