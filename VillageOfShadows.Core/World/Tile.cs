using VillageOfShadows.Core.Entities;

namespace VillageOfShadows.Core.World
{
    public sealed class Tile
    {
        public TileType Type;
        public List<EntityId> EntityIds = new();
        public bool IsWalkable;
    }
}
