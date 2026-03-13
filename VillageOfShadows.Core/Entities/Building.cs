using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public abstract class Building : TileEntity
    {
        public float MaxInventory { get; init; } = 0;
        public required IList<InventoryStack> Inventory { get; set; } = new List<InventoryStack>();

        public float InventorySize => Inventory.Sum(x => x.TotalSize);
        public float FreeInventory => MaxInventory - InventorySize;

        public int GetAmount(ResourceType type)
        {
            return Inventory
                .Where(x => x.ResourceType == type)
                .Sum(x => x.Amount);
        }

        public bool CanStore(ResourceType type, int amount, float unitSize = 1f)
        {
            return FreeInventory >= amount * unitSize;
        }

        public int AddResource(ResourceType type, int amount, float unitSize = 1f)
        {
            if (amount <= 0)
                return 0;

            int maxAddable = (int)(FreeInventory / unitSize);
            int toAdd = Math.Min(amount, maxAddable);

            if (toAdd <= 0)
                return 0;

            var existing = Inventory.FirstOrDefault(x => x.ResourceType == type && x.UnitSize == unitSize);
            if (existing is null)
            {
                Inventory.Add(new InventoryStack
                {
                    ResourceType = type,
                    Amount = toAdd,
                    UnitSize = unitSize
                });
            }
            else
            {
                existing.Amount += toAdd;
            }

            return toAdd;
        }

        public int RemoveResource(ResourceType type, int amount)
        {
            if (amount <= 0)
                return 0;

            int remaining = amount;
            int removed = 0;

            for (int i = Inventory.Count - 1; i >= 0; i--)
            {
                var stack = Inventory[i];
                if (stack.ResourceType != type)
                    continue;

                int take = Math.Min(stack.Amount, remaining);
                stack.Amount -= take;
                remaining -= take;
                removed += take;

                if (stack.Amount <= 0)
                    Inventory.RemoveAt(i);

                if (remaining <= 0)
                    break;
            }

            return removed;
        }
    }
}
