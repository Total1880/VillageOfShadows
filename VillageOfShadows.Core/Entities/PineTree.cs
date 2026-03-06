using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Core.Entities
{
    public sealed class PineTree : Tree
    {
        public PineTree()
        {
            SaplingGrowthPerSec = 0.030f;
            YoungGrowthPerSec = 0.020f;
            MatureSpreadChancePerSec = 0.0025f;
        }

        public override Tree CreateSapling()
        {
            return new PineTree
            {
                Stage = TreeStage.Sapling
            };
        }
    }
}
