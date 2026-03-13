using System.Drawing;
using System.Numerics;

namespace VillageOfShadows.Core.Entities;

public abstract class TileEntity : Entity
{
    public Point Tile { get; set; }

    public void SetTilePosition(Point tile, Vector2 worldPosition)
    {
        Tile = tile;
        Position = worldPosition;
    }
}
