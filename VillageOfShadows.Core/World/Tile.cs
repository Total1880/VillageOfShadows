using VillageOfShadows.Core.Entities;

namespace VillageOfShadows.Core.World
{
    public sealed class Tile
    {
        public TileType Type { get; set; } = TileType.Grass;

        // Altijd geïnitialiseerd (nooit null)
        public List<EntityId> EntityIds { get; } = new();

        public bool IsWalkable { get; set; } = true;
    }
}
