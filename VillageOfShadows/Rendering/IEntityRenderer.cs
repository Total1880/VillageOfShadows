using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    public interface IEntityRenderer
    {
        void Draw(SpriteBatch sb, World world, Entity e, int tileX, int tileY);
        void DrawEntities(SpriteBatch sb, World world, IEnumerable<Entity> entities);
    }
}
