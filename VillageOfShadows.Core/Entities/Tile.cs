namespace VillageOfShadows.Core.Entities
{
    public sealed class Tile
    {
        public bool IsWalkable { get; set; } = true;
        public List<EntityId> EntityIds { get; } = new();
    }
}
