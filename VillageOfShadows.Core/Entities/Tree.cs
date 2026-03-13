using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public abstract class Tree : TileEntity
    {
        private float _foodValue;
        public float SaplingGrowthPerSec { get; init; } = 0.030f;
        public float YoungGrowthPerSec { get; init; } = 0.020f;
        public float MatureSpreadChancePerSec { get; init; } = 0.0025f;
        public float FoodGrowthPerSec { get; init; } = 0.0050f;
        public TreeStage Stage { get; set; } = TreeStage.Sapling;
        public float Growth; // 0..1 progress within current stage
        public bool HasFood { get; init; } = false;
        public float FoodValue { get { return _foodValue > MaxFoodValue ? MaxFoodValue : _foodValue; } set { _foodValue = value; } }
        public float MaxFoodValue { get; set; }
        public bool MarkedForChop { get; set; }
        public float ChopWorkRequired { get; set; } = 5f;
        public int WoodYield { get; set; } = 3;
        protected Tree()
        {
            Stage = TreeStage.Sapling;
        }
        public abstract Tree CreateSapling();
    }
}
