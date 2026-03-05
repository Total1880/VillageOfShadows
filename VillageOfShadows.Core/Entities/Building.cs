namespace VillageOfShadows.Core.Entities
{
    public abstract class Building : Entity
    {
        public float InventorySize { get; set; } = 0;
        public float MaxInventory { get; init; } = 0;
        public required IList<Entity> Inventory { get; set; }
    }
}
