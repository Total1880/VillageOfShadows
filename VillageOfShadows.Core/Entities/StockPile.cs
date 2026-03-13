using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public sealed class Stockpile : Building
    {
        public StockpileKind Kind { get; set; }

        public Stockpile()
        {
            Kind = StockpileKind.Temporary;
            MaxInventory = 5;
            Inventory = new List<InventoryStack>();
        }
    }
}
