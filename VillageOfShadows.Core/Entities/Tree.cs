using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Core.Entities
{
    public abstract class Tree : Entity
    {
        public float SaplingGrowthPerSec { get; init; } = 0.030f;
        public float YoungGrowthPerSec { get; init; } = 0.020f;
        public float MatureSpreadChancePerSec { get; init; } = 0.0025f;
        public TreeStage Stage { get; set; }
        public float Growth; // 0..1 progress within current stage
        protected Tree()
        {
            Stage = TreeStage.Sapling;
        }
    }
}
