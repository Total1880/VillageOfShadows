using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public sealed class AppleTree : Tree
    {
        public AppleTree()
        {
            SaplingGrowthPerSec = 0.033f;
            YoungGrowthPerSec = 0.022f;
            MatureSpreadChancePerSec = 0.0028f;
            FoodGrowthPerSec = 0.050f;
            HasFood = true;
            MaxFoodValue = 6;
        }

        public override Tree CreateSapling()
        {
            return new AppleTree
            {
                Stage = TreeStage.Sapling,
                FoodValue = 0
            };
        }
    }
}
