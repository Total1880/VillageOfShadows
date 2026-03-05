using System.Numerics;

namespace VillageOfShadows.Core.Entities
{
    public abstract class Entity
    {
        public EntityId EntityId = EntityId.New();
        public Guid Id { get => EntityId.Value; }
        public Vector2 Position;
        public abstract Entity Create();
    }
}
