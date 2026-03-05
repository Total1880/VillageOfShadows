using System.Numerics;

namespace VillageOfShadows.Core.Entities
{
    public abstract class Entity
    {
        public EntityId EntityId;
        public Guid Id { get => EntityId.Value; }
        public Vector2 Position;
        public abstract Entity Create();
    }
}
