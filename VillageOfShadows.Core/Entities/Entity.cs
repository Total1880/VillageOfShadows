using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Vector2 Position;
        public abstract Entity Create();
    }
}
