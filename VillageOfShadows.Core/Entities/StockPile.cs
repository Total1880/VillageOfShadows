namespace VillageOfShadows.Core.Entities
{
    public class StockPile : Building
    {
        public StockPile()
        {
            MaxInventory = 100;
        }
        public override Entity Create()
        {
            return new StockPile
            {
                Inventory = []
            };
        }
    }
}
