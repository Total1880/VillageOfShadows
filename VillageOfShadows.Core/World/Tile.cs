using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.World
{
    public sealed class Tile
    {
        public TileType Type;
        public bool HasTree;
        public TreeStage TreeStage;
        public float TreeGrowth; // 0..1 progress within current stage
    }
}
