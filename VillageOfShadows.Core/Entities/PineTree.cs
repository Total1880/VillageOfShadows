using System;
using System.Collections.Generic;
using System.Text;

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

        public override Entity Create()
        {
            return new PineTree();
        }
    }
}
