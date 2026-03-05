using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.Entities
{
    public sealed class OakTree : Tree
    {
        public OakTree()
        {
            SaplingGrowthPerSec = 0.035f;
            YoungGrowthPerSec = 0.025f;
            MatureSpreadChancePerSec = 0.0030f;
        }

        public override Entity Create()
        {
            return new OakTree();
        }
    }
}
