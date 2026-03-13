using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Simulation;

public static class StockpileHelper
{
    public static void DropResourceNear(
        World.World world,
        int originX,
        int originY,
        ResourceType type,
        int amount,
        int tempPileCapacity = 5)
    {
        int remaining = amount;

        // eerst bestaande tijdelijke stockpiles in de buurt vullen
        foreach (var pile in world.GetEntities<Stockpile>())
        {
            if (pile.Kind != StockpileKind.Temporary)
                continue;

            var tile = world.WorldToTile(pile.Position);
            int dist = Math.Abs(tile.tx - originX) + Math.Abs(tile.ty - originY);
            if (dist > 1)
                continue;

            int added = pile.AddResource(type, remaining);
            remaining -= added;

            if (remaining <= 0)
                return;
        }

        // daarna nieuwe tijdelijke stockpiles maken
        var spots = new (int x, int y)[]
        {
            (originX, originY),
            (originX + 1, originY),
            (originX - 1, originY),
            (originX, originY + 1),
            (originX, originY - 1),
            (originX + 1, originY + 1),
            (originX - 1, originY + 1),
            (originX + 1, originY - 1),
            (originX - 1, originY - 1),
        };

        foreach (var spot in spots)
        {
            if (remaining <= 0)
                break;

            if (!world.InBounds(spot.x, spot.y))
                continue;

            var tile = world.GetTile(spot.x, spot.y);
            if (!tile.IsWalkable)
                continue;

            bool hasStockpile = world.GetTileEntitiesOnTile(spot.x, spot.y).Any(e => e is Stockpile);
            if (hasStockpile)
                continue;

            var pile = new Stockpile
            {
                Kind = StockpileKind.Temporary,
                MaxInventory = tempPileCapacity,
                Inventory = new List<InventoryStack>()
            };

            if (!world.TryPlaceTileEntity(pile, spot.x, spot.y))
                continue;

            int added = pile.AddResource(type, remaining);
            remaining -= added;
        }

        // eventueel: als er nog remaining is, log/debug
    }
}
