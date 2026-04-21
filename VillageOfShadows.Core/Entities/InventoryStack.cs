using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public sealed class InventoryStack
    {
        public required ResourceType ResourceType { get; init; }
        public int Amount { get; set; }
        public int ReservedAmount { get; set; }
        public float UnitSize { get; init; } = 1f;

        public int AvailableAmount => Amount - ReservedAmount;
        public float TotalSize => Amount * UnitSize;
        public bool IsEdible
        {
            get
            {
                if (ResourceType == ResourceType.Apples) return true;
                return false;
            }
        }
        public float FoodValuePerUnit { get; init; } = 0f;
    }
}
