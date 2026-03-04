using VillageOfShadows.Core.Entities;

namespace VillageOfShadows.Core.World
{
    public sealed class Tile
    {
        public TileType Type;
        public Entity? Entity;
        public bool isWalkable;
    }
}
