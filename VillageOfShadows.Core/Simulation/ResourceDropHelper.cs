using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Simulation;
public static class ResourceDropHelper
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

        var candidateTiles = new (int x, int y)[]
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

        foreach (var tile in candidateTiles)
        {
            if (remaining <= 0)
                break;

            if (!world.InBounds(tile.x, tile.y))
                continue;

            var entities = world.GetTileEntitiesOnTile(tile.x, tile.y);
            var pile = entities
                .OfType<Stockpile>()
                .FirstOrDefault(x => x.Kind == StockpileKind.Temporary);

            if (pile is null)
            {
                pile = new Stockpile
                {
                    Kind = StockpileKind.Temporary,
                    MaxInventory = tempPileCapacity,
                    Inventory = new List<InventoryStack>()
                };

                if (!world.TryPlaceTileEntity(pile, tile.x, tile.y))
                    continue;
            }

            int added = pile.AddResource(type, remaining);
            remaining -= added;
        }

        if (remaining > 0)
        {
            // eventueel log/debug
        }
    }
}