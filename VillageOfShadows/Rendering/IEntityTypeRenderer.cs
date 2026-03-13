using Microsoft.Xna.Framework.Graphics;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    public interface IEntityTypeRenderer
    {
        bool CanDraw(Entity e);
        void Draw(SpriteBatch sb, World world, Entity entity, int tileX, int tileY);
    }
}
