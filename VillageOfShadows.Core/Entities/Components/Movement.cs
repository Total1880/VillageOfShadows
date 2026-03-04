using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.Entities.Components
{
    public struct Movement
    {
        public float Speed;              // tiles/sec of world units/sec
        public Vector2 Target;           // where to walk
        public bool HasTarget;
    }
}
